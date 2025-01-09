-- create database
CREATE DATABASE IF NOT EXISTS BookPricesJob;

-- create migration user with password and grant access to database
-- REPLACE THE VALUES: <user> and <password> !!!!!
CREATE USER IF NOT EXISTS '<user>'@'%' IDENTIFIED BY '<password>';
GRANT SELECT, INSERT, UPDATE, DELETE
ON BookPricesJob.*
TO '<user>'@'%';
