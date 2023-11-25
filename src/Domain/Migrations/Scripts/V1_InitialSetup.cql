CREATE KEYSPACE my_keyspace
WITH replication = {'class': 'SimpleStrategy', 'replication_factor': 1};

CREATE TABLE IF NOT EXISTS my_keyspace.User
(
    login
    TEXT
    PRIMARY
    KEY,
    password
    TEXT
);

CREATE TABLE IF NOT EXISTS my_keyspace.PasswordStorage
(
    login
    TEXT,
    resourceName
    TEXT,
    resourcePassword
    TEXT,
    PRIMARY
    KEY
(
    login,
    resourceName
));