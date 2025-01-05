-- CHANGE THESE VALUES!
DECLARE @database NVARCHAR(MAX) = 'BookPricesJob';
DECLARE @user NVARCHAR(MAX) = '';
DECLARE @password NVARCHAR(MAX) = '';

-- create database
CREATE DATABASE IF NOT EXISTS @database;

-- create migration user with password and grant access to database
GRANT SELECT, INSERT, UPDATE, DELETE, CREATE, ALTER, DROP
ON @database.*
TO @user IDENTIFIED BY @password;
