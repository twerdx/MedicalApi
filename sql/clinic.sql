--создаем новую таблицу пациенты с id, именями до 100 знаков, дату рождения и номерои до 20 знаков (имя и др все они не могут быть пустыми)
CREATE TABLE patients (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    birth_date DATE NOT NULL,
    phone VARCHAR(20)
);
--создаем новую таблицу типы справок с айди, названием до 100 знаков, ценой до 10 знков до , и 2 после нее(только положительные значения), сроком годности и все трое не могут быть пустыми 
CREATE TABLE spravka_types (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    price NUMERIC(10, 2) NOT NULL CHECK (price > 0),
    valid_days INT NOT NULL
);
--создаем новую таблицу справок с айди, айди пациентов со существующим пациентом, ценой  ценой до 10 знков до , и 2 после нее, датой(по дефолту с текущей датой) и все они не могут быть пустыми
CREATE TABLE spravki (
    id SERIAL PRIMARY KEY,
    patient_id INT NOT NULL REFERENCES patients(id),
    spravka_type_id INT NOT NULL REFERENCES spravka_types(id),
    price NUMERIC(10, 2) NOT NULL,
    issued_at DATE NOT NULL DEFAULT CURRENT_DATE
);
--вставляем в таблицу пациенты по заданному порядку данные
INSERT INTO patients (name, birth_date, phone) VALUES
    ('Айбек Асанов', '1998-03-12', '0555123456'),
    ('Нургуль Токтосунова', '2003-07-25', '0700987654'),
    ('Бакыт Жумабаев', '1985-11-02', NULL),
    ('Айпери Садыкова', '2001-01-30', '0772345678'),
    ('Тимур Ким', '2006-09-14', '0509111222');

--вставляем в таблицу типы справок по заданному порядку данные
INSERT INTO spravka_types (name, price, valid_days) VALUES
    ('Справка 083', 100, 365),
    ('Санитарная книжка', 750, 365),
    ('Справка 086', 300, 180),
    ('Справка в бассейн', 200, 90);

--вставляем в таблицу справок по заданному порядку данные
INSERT INTO spravki (patient_id, spravka_type_id, price, issued_at) VALUES
    (1, 1, 100, '2026-10-01'),
    (1, 4, 200, '2026-10-15'),
    (2, 2, 750, '2026-10-03'),
    (2, 3, 300, '2026-11-02'),
    (4, 3, 300, '2026-11-05'),
    (5, 4, 200, '2026-11-06'),
    (5, 1, 100, '2026-11-10');
	
--выборочно выводим из справок соеденияя данные из разных таблиц в таблице пациенты
SELECT spravki.id, patients.name AS patient, spravka_types.name AS spravka, spravki.price, spravki.issued_at
FROM spravki
JOIN patients ON spravki.patient_id = patients.id
JOIN spravka_types ON spravki.spravka_type_id = spravka_types.id
ORDER BY spravki.issued_at;

SELECT COUNT(*) AS spravki_count, SUM(price) AS total FROM spravki;


--вывод по имени
SELECT * FROM patients ORDER BY name;
--после 2000
SELECT * FROM patients where birth_date>= '2000-01-01' 
--где номер не заполнен
SELECT * FROM patients where phone is NULL;
--с регистром 
SELECT * FROM patients WHERE name LIKE '%ай%';
--без регистра 
SELECT * FROM patients WHERE name ILIKE '%ай%';
--выбор выводимых данных из справок седеняя с другими таблицами где айди 1 по порядку 
SELECT spravki.id, spravka_types.name AS spravka, spravki.price, spravki.issued_at
FROM spravki
JOIN spravka_types ON spravki.spravka_type_id = spravka_types.id
WHERE spravki.patient_id = 1
ORDER BY spravki.issued_at;
--
SELECT * FROM spravki
WHERE issued_at >= '2026-11-01' AND issued_at < '2026-12-01'
ORDER BY issued_at;
--
SELECT * FROM spravka_types ORDER BY price DESC LIMIT 1;
--
SELECT COUNT(*) AS spravki_count, SUM(price) AS total
FROM spravki
WHERE issued_at >= '2026-11-01' AND issued_at < '2026-12-01';
UPDATE spravka_types SET price = 120 WHERE name = 'Справка 083';
SELECT * FROM spravka_types ORDER BY id;
SELECT * FROM spravki ORDER BY id;
DELETE FROM spravki WHERE id = 7;
SELECT patients.name, COUNT(spravki.id) AS spravki_count
FROM patients
LEFT JOIN spravki ON spravki.patient_id = patients.id
GROUP BY patients.id, patients.name
ORDER BY spravki_count DESC;
INSERT INTO spravki (patient_id, spravka_type_id, price) VALUES (999, 1, 100);
INSERT INTO spravka_types (name, price, valid_days) VALUES ('Справка в лагерь', -500, 30);

INSERT INTO patients (birth_date) VALUES ('2000-01-01');
DELETE FROM patients WHERE id = 1;

INSERT INTO spravki (patient_id, spravka_type_id, price) VALUES (3, 2, 750);
SELECT * FROM spravki WHERE patient_id = 3;









