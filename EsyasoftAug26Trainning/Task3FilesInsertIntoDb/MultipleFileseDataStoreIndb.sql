use AugEsyasoftTranningTask

CREATE TABLE Employees
(
    EmployeeId INT PRIMARY KEY,
    Name VARCHAR(50) NOT NULL,
    Department VARCHAR(30) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Salary INT NOT NULL
);
select * from EMPLOYEES
ncate table employees

CREATE SP_InsertEmployees
    @Json NVARCHAR(MAX)
AS
BEGIN

    INSERT INTO Employees
    (
        EmployeeId,
        Name,
        Department,
        Email,
        Salary
    )
    SELECT
        EmployeeId,
        Name,
        Department,
        Email,
        Salary
    FROM OPENJSON(@Json, '$.Employees')
    WITH
    (
        EmployeeId INT '$.EmployeeId',
        Name VARCHAR(50) '$.Name',
        Department VARCHAR(30) '$.Department',
        Email VARCHAR(100) '$.Email',
        Salary INT '$.Salary'
    );

END



drop table employees