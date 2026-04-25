UPDATE characters SET age = 32 WHERE id = 1;

INSERT INTO characters (full_name, preferred_name, age, birthday, character_type, is_alive, created_at, updated_at)
VALUES ('Linda Marie Morrison', 'Linda', 61, '1964-04-22', 'secondary', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

INSERT INTO characters (full_name, preferred_name, age, birthday, character_type, is_alive, created_at, updated_at)
VALUES ('Priya Anjali Mehta', 'Priya', 28, '1997-01-01', 'secondary', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);
