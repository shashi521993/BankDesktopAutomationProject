# Bank Management System (WPF + SQL Server)

## Project Overview

Bank Management System is a desktop-based banking application developed using C#, WPF, XAML, SQL Server, and Entity Framework.

This application allows users to perform core banking operations through a clean and user-friendly interface.

---

# Features

* User Login Authentication
* Create New Bank Account
* Deposit Amount
* Withdraw Amount
* Check Account Balance
* Modify Account Details
* View All Accounts
* Delete Account
* Logout Functionality

---

# Technologies Used

* C#
* WPF (Windows Presentation Foundation)
* XAML
* SQL Server
* Entity Framework
* Visual Studio 2022

---

# Database Used

```sql
BankDB
```

---

# Default Login Credentials

Use the following credentials to login into the application:

| Username | Password |
| -------- | -------- |
| admin    | 1234     |

---

# Project Structure

* Login Window
* Dashboard Window
* Create Account Module
* Deposit Module
* Withdraw Module
* Check Balance Module
* Modify Account Module
* View Accounts Module

---

# How To Run The Project

1. Open the solution file in Visual Studio.
2. Configure SQL Server database connection.
3. Restore NuGet packages if required.
4. Build the project.
5. Run the application.

---

# Screens Included

* Login Page
* Dashboard
* Create Account
* Deposit Window
* Withdraw Window
* Check Balance Window
* Modify Account Window
* View Accounts Window

---

# Author
Harshith Kumar M


# Manual Login Credential Setup

The login credentials are stored manually in the SQL Server database.

Before running the application, insert login credentials into the `Users` table.

Follow the steps below carefully.

---

## Step 1 — Open SQL Server Management Studio (SSMS)

- Open SQL Server Management Studio
- Connect to your SQL Server instance

Example:

```text
DESKTOP-XXXXXXX\SQLEXPRESS

Step 2 — Create Database

Execute:

CREATE DATABASE BankDB;
Step 3 — Use Database

Execute:

USE BankDB;
Step 4 — Create Users Table

Execute:

CREATE TABLE Users
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50),
    Password VARCHAR(50)
);
Step 5 — Insert Login Credentials

Execute:

INSERT INTO Users (Username, Password)
VALUES ('admin', '1234');
Step 6 — Verify Credentials

Execute:

SELECT * FROM Users;

Expected Output:

Id	Username	Password
1	admin	1234

Default Login Credentials
Username	Password
admin	1234

Step 7 — Run Application
Open project in Visual Studio
Build Solution
Press F5
Login using:
Username: admin
Password: 1234