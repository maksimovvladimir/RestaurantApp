-- =========================================================
-- БД "Ресторан" — точно по диаграмме пользователя (PostgreSQL 17)
-- Клиент НЕ является пользователем системы — запись о посещении
-- (бронь) вносит администратор/официант вручную, включая "живых" гостей.
-- =========================================================

-- ---------- ТАБЛИЦЫ ----------

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    login VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('waiter','kitchen','admin')),
    full_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20)
);

CREATE TABLE tables (
    id_table SERIAL PRIMARY KEY,
    number INT UNIQUE NOT NULL,
    capacity INT NOT NULL CHECK (capacity > 0 AND capacity <= 4),
    status VARCHAR(20) NOT NULL DEFAULT 'free'
);

CREATE TABLE shifts (
    id SERIAL PRIMARY KEY,
    waiter_id INT NOT NULL REFERENCES users(id),
    shift_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME,
    status VARCHAR(20) NOT NULL DEFAULT 'open'
);

-- "Зарезервированные_столики" — какие столики закреплены за официантом на смену
CREATE TABLE reserved_tables (
    service_id SERIAL PRIMARY KEY,
    id_table INT NOT NULL REFERENCES tables(id_table),
    shift_id INT NOT NULL REFERENCES shifts(id),
    UNIQUE (id_table, shift_id)
);

-- "Брони" — запись о посещении (и предзаказ, и "живой" гость), вносит админ/официант
CREATE TABLE reservations (
    id_reserve SERIAL PRIMARY KEY,
    client_name VARCHAR(100) NOT NULL,
    client_phone VARCHAR(20),
    res_date DATE NOT NULL,
    time_start TIME NOT NULL,
    time_end TIME NOT NULL,
    guests_count INT NOT NULL CHECK (guests_count > 0),
    status VARCHAR(20) NOT NULL DEFAULT 'active'
);

-- "Бронь_столики"
CREATE TABLE reservation_tables (
    id_reserve_table SERIAL PRIMARY KEY,
    id_reserve INT NOT NULL REFERENCES reservations(id_reserve),
    id_table INT NOT NULL REFERENCES tables(id_table),
    UNIQUE (id_reserve, id_table)
);

CREATE TABLE dish_categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE dishes (
    id SERIAL PRIMARY KEY,
    category_id INT NOT NULL REFERENCES dish_categories(id),
    name VARCHAR(100) NOT NULL,
    price NUMERIC(10,2) NOT NULL CHECK (price >= 0),
    description TEXT
);

CREATE TABLE stock (
    id SERIAL PRIMARY KEY,
    dish_id INT UNIQUE NOT NULL REFERENCES dishes(id),
    quantity_avalible INT NOT NULL DEFAULT 0 CHECK (quantity_avalible >= 0)
);

-- PK = dish_id: у блюда в любой момент не больше одной активной скидки
CREATE TABLE discounts (
    dish_id INT PRIMARY KEY REFERENCES dishes(id),
    name VARCHAR(100) NOT NULL,
    discount_percent NUMERIC(5,2) NOT NULL CHECK (discount_percent BETWEEN 0 AND 100),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    CHECK (end_date >= start_date)
);

-- "Заказы" — привязан не к столику напрямую, а к конкретной паре (бронь, столик)
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    id_reserve_table INT NOT NULL REFERENCES reservation_tables(id_reserve_table),
    status VARCHAR(30) NOT NULL DEFAULT 'draft'
        CHECK (status IN ('draft','placed','cooking','ready','served','cancelled')),
    created_at TIMESTAMP NOT NULL DEFAULT now(),
    confirmed_at TIMESTAMP,
    total_amount NUMERIC(10,2) NOT NULL DEFAULT 0
);

-- "Позиции_заказа" — без снэпшота цены (по диаграмме), цена считается на момент запроса
CREATE TABLE order_items (
    id SERIAL PRIMARY KEY,
    order_id INT NOT NULL REFERENCES orders(id),
    dish_id INT NOT NULL REFERENCES dishes(id),
    quantity INT NOT NULL CHECK (quantity > 0),
    UNIQUE (order_id, dish_id)
);

CREATE TABLE receipts (
    id SERIAL PRIMARY KEY,
    order_id INT UNIQUE NOT NULL REFERENCES orders(id),
    paid_at TIMESTAMP NOT NULL DEFAULT now(),
    amount NUMERIC(10,2),
    payment_methoc VARCHAR(20) NOT NULL CHECK (payment_methoc IN ('cash','card','online'))
);

-- ---------- ИНДЕКСЫ ----------

CREATE INDEX idx_orders_reserve_table ON orders(id_reserve_table);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_created_at ON orders(created_at);
CREATE INDEX idx_order_items_order_id ON order_items(order_id);
CREATE INDEX idx_order_items_dish_id ON order_items(dish_id);
CREATE INDEX idx_reservations_date ON reservations(res_date, time_start, time_end);
CREATE INDEX idx_reservation_tables_table ON reservation_tables(id_table);
CREATE INDEX idx_shifts_waiter_date ON shifts(waiter_id, shift_date);
CREATE INDEX idx_reserved_tables_shift ON reserved_tables(shift_id);

-- ---------- ПРЕДСТАВЛЕНИЕ: текущая цена блюда с учётом скидки ----------

CREATE VIEW v_dish_current_price AS
SELECT
    d.id AS dish_id,
    d.name,
    d.price AS base_price,
    disc.discount_percent,
    CASE
        WHEN disc.discount_percent IS NOT NULL
             AND CURRENT_DATE BETWEEN disc.start_date AND disc.end_date
        THEN ROUND(d.price * (1 - disc.discount_percent / 100), 2)
        ELSE d.price
    END AS current_price
FROM dishes d
LEFT JOIN discounts disc ON disc.dish_id = d.id;

-- ---------- ФУНКЦИИ-ПРОЦЕДУРЫ ----------

CREATE OR REPLACE FUNCTION sp_add_dish_to_order(
    p_order_id INT, p_dish_id INT, p_qty INT
) RETURNS TEXT AS $$
DECLARE
    v_available INT;
    v_dish_name VARCHAR;
    v_order_status VARCHAR;
BEGIN
    SELECT status INTO v_order_status FROM orders WHERE id = p_order_id;
    IF v_order_status IS NULL THEN
        RAISE EXCEPTION 'Заказ №% не найден', p_order_id;
    END IF;
    IF v_order_status <> 'draft' THEN
        RAISE EXCEPTION 'Заказ №% нельзя редактировать — статус "%"', p_order_id, v_order_status;
    END IF;

    SELECT quantity_avalible INTO v_available FROM stock WHERE dish_id = p_dish_id FOR UPDATE;
    IF v_available IS NULL THEN
        RAISE EXCEPTION 'Блюдо № % отсутствует на складе (нет записи stock)', p_dish_id;
    END IF;

    SELECT name INTO v_dish_name FROM dishes WHERE id = p_dish_id;

    IF v_available < p_qty THEN
        RETURN format('Невозможно добавить блюдо "%s" в количестве %s порций. Сейчас доступно %s порций.',
                       v_dish_name, p_qty, v_available);
    END IF;

    UPDATE stock SET quantity_avalible = quantity_avalible - p_qty WHERE dish_id = p_dish_id;

    INSERT INTO order_items (order_id, dish_id, quantity)
    VALUES (p_order_id, p_dish_id, p_qty)
    ON CONFLICT (order_id, dish_id) DO UPDATE
        SET quantity = order_items.quantity + EXCLUDED.quantity;

    RETURN format('Блюдо "%s" в количестве %s порций было успешно добавлено в Заказ №%s', v_dish_name, p_qty, p_order_id);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_remove_dish_from_order(
    p_order_id INT, p_dish_id INT
) RETURNS TEXT AS $$
DECLARE
    v_qty INT;
    v_dish_name VARCHAR;
BEGIN
    SELECT quantity INTO v_qty FROM order_items WHERE order_id = p_order_id AND dish_id = p_dish_id;
    IF v_qty IS NULL THEN
        RAISE EXCEPTION 'Блюдо № % не найдено в Заказе №%', p_dish_id, p_order_id;
    END IF;

    SELECT name INTO v_dish_name FROM dishes WHERE id = p_dish_id;

    UPDATE stock SET quantity_avalible = quantity_avalible + v_qty WHERE dish_id = p_dish_id;
    DELETE FROM order_items WHERE order_id = p_order_id AND dish_id = p_dish_id;

    RETURN format('Блюдо "%s" в количестве %s порций было успешно удалено из Заказа №%s', v_dish_name, v_qty, p_order_id);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION sp_change_order_status(p_order_id INT, p_new_status VARCHAR) RETURNS VOID AS $$
DECLARE
    v_current VARCHAR;
BEGIN
    SELECT status INTO v_current FROM orders WHERE id = p_order_id;
    IF v_current IS NULL THEN
        RAISE EXCEPTION 'Заказ №% не найден', p_order_id;
    END IF;

    IF v_current = 'placed' AND p_new_status = 'draft' THEN
        RAISE EXCEPTION 'Оформленный заказ нельзя вернуть в составление';
    END IF;

    UPDATE orders SET status = p_new_status,
        confirmed_at = CASE WHEN p_new_status = 'placed' THEN now() ELSE confirmed_at END
    WHERE id = p_order_id;
END;
$$ LANGUAGE plpgsql;

-- ---------- ТРИГГЕРЫ ----------

CREATE OR REPLACE FUNCTION trg_check_order_editable() RETURNS TRIGGER AS $$
DECLARE v_status VARCHAR;
BEGIN
    SELECT status INTO v_status FROM orders WHERE id = COALESCE(NEW.order_id, OLD.order_id);
    IF v_status <> 'draft' THEN
        RAISE EXCEPTION 'Изменение позиций заказа №% запрещено: статус "%"', COALESCE(NEW.order_id, OLD.order_id), v_status;
    END IF;
    RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_order_items_editable
BEFORE INSERT OR UPDATE OR DELETE ON order_items
FOR EACH ROW EXECUTE FUNCTION trg_check_order_editable();

-- Сумма заказа считается по ТЕКУЩЕЙ цене (в этой версии нет снэпшота цены в order_items)
CREATE OR REPLACE FUNCTION trg_recalc_order_total() RETURNS TRIGGER AS $$
BEGIN
    UPDATE orders SET total_amount = (
        SELECT COALESCE(SUM(oi.quantity * vp.current_price), 0)
        FROM order_items oi
        JOIN v_dish_current_price vp ON vp.dish_id = oi.dish_id
        WHERE oi.order_id = COALESCE(NEW.order_id, OLD.order_id)
    ) WHERE id = COALESCE(NEW.order_id, OLD.order_id);
    RETURN COALESCE(NEW, OLD);
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_recalc_total
AFTER INSERT OR UPDATE OR DELETE ON order_items
FOR EACH ROW EXECUTE FUNCTION trg_recalc_order_total();

CREATE OR REPLACE FUNCTION trg_fix_receipt_amount() RETURNS TRIGGER AS $$
BEGIN
    IF NEW.amount IS NULL THEN
        SELECT total_amount INTO NEW.amount FROM orders WHERE id = NEW.order_id;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_receipt_amount
BEFORE INSERT ON receipts
FOR EACH ROW EXECUTE FUNCTION trg_fix_receipt_amount();

-- Контроль вместимости: сумма capacity столиков брони не может быть меньше guests_count
CREATE OR REPLACE FUNCTION trg_check_reservation_capacity() RETURNS TRIGGER AS $$
DECLARE
    v_total_capacity INT;
    v_guests INT;
BEGIN
    SELECT SUM(t.capacity) INTO v_total_capacity
    FROM reservation_tables rt JOIN tables t ON t.id_table = rt.id_table
    WHERE rt.id_reserve = NEW.id_reserve;

    SELECT guests_count INTO v_guests FROM reservations WHERE id_reserve = NEW.id_reserve;

    IF v_total_capacity IS NOT NULL AND v_total_capacity < v_guests THEN
        RAISE EXCEPTION 'Вместимость выбранных столов (% мест) меньше количества гостей (%)', v_total_capacity, v_guests;
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_reservation_capacity
AFTER INSERT ON reservation_tables
FOR EACH ROW EXECUTE FUNCTION trg_check_reservation_capacity();

-- =========================================================
-- Конец скрипта
-- =========================================================
