ALTER SYSTEM SET wal_level='logical';
ALTER SYSTEM SET max_wal_senders='10';
ALTER SYSTEM SET max_replication_slots='10';

CREATE PUBLICATION dotnetify_pub FOR ALL TABLES;
SELECT * FROM pg_create_logical_replication_slot('dotnetify_slot', 'pgoutput');

CREATE USER my_user WITH PASSWORD 'my_pwd';
ALTER ROLE my_user WITH REPLICATION;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO my_user;

CREATE TABLE IF NOT EXISTS users (
	user_id serial PRIMARY KEY,
	username VARCHAR ( 50 ) UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS accounts (
	user_id integer NOT NULL,
	account_id serial PRIMARY KEY,	
	balance decimal,
	created_on TIMESTAMP NOT NULL
);