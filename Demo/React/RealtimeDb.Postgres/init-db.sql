ALTER SYSTEM SET wal_level='logical';
ALTER SYSTEM SET max_wal_senders='10';
ALTER SYSTEM SET max_replication_slots='10';

CREATE USER my_user WITH PASSWORD 'my_pwd';
ALTER ROLE my_user WITH REPLICATION;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO my_user;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO my_user;

CREATE TABLE IF NOT EXISTS businesses (
	business_id serial PRIMARY KEY,
	business_name VARCHAR ( 50 ) UNIQUE NOT NULL,
	rating integer
);

CREATE PUBLICATION dotnetify_pub FOR ALL TABLES;
SELECT * FROM pg_create_logical_replication_slot('dotnetify_slot', 'pgoutput');