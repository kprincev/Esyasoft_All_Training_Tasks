create database AugEsyasoftTranningTask
use AugEsyasoftTranningTask

CREATE TABLE Departments (
    DepartmentID INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName VARCHAR(50),
    Location VARCHAR(50)
);
select * from departments
INSERT INTO Departments (DepartmentName, Location) VALUES
('IT', 'Delhi'),
('HR', 'Noida'),
('Finance', 'Mumbai'),
('Sales', 'Pune');


CREATE TABLE Employees (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(100),
    Gender CHAR(1),
    Salary DECIMAL(10,2),
    HireDate DATE,
    DepartmentID INT,
    ManagerID INT NULL,
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);

select * from employees
DROP TABLE Employees
DROP TABLE Departments

INSERT INTO Employees (Name, Gender, Salary, HireDate, DepartmentID, ManagerID) VALUES
('Amit Sharma', 'M', 60000, '2019-01-10', 1, NULL),
('Rohit Verma', 'M', 55000, '2020-03-15', 1, 1),
('Neha Singh', 'F', 50000, '2021-06-20', 2, 1),
('Pooja Mehta', 'F', 45000, '2022-02-10', 2, 3),
('Rahul Jain', 'M', 70000, '2018-11-05', 3, NULL),
('Ankit Gupta', 'M', 65000, '2020-09-12', 4, 5);
