-- create database
CREATE DATABASE IF NOT EXISTS BookPricesJob;

-- create app user with password and grant access to database
-- REPLACE THE VALUES: <user> and <password> !!!!!
CREATE USER IF NOT EXISTS '<user>'@'%' IDENTIFIED BY '<password>';
GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, ALTER, DROP, REFERENCES, INDEX
ON BookPricesJob.*
TO '<user>'@'%';
