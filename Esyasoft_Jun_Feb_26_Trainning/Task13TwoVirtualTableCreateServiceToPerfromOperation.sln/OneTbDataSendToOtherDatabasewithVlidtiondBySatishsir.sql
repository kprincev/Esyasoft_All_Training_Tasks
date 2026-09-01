
use SimpleTaskdb

DECLARE @i INT = 1;

WHILE @i <= 500
BEGIN
    INSERT INTO srcemployee
    (
        emp_id,
        emp_name,
        emp_dob,
        emp_number,
        emp_salary,
        emp_address,
        emp_email,
        emp_department,
        push_status,
        is_processed,
        process_datetime,
        remark
    )
    VALUES
    (
        @i,
        'Employee_' + CAST(@i AS VARCHAR),

        -- some age <18
        CASE WHEN @i % 10 = 0
             THEN DATEADD(YEAR, -15, GETDATE())
             ELSE DATEADD(YEAR, -25, GETDATE())
        END,

        -- some invalid mobile
        CASE WHEN @i % 8 = 0
             THEN '12345'
             ELSE RIGHT('9000000000' + CAST(@i AS VARCHAR), 10)
        END,

        -- some salary zero
        CASE WHEN @i % 7 = 0
             THEN 0
             ELSE 20000 + @i
        END,

        -- some address with special symbol
        CASE WHEN @i % 9 = 0
             THEN 'Addr@#' + CAST(@i AS VARCHAR)
             ELSE 'Address_' + CAST(@i AS VARCHAR)
        END,

        -- some invalid email
        CASE WHEN @i % 6 = 0
             THEN 'emp' + CAST(@i AS VARCHAR)
             ELSE 'emp' + CAST(@i AS VARCHAR) + '@gmail.com'
        END,

        -- some invalid department
        CASE WHEN @i % 5 = 0
             THEN 'UnknownDept'
             WHEN @i % 3 = 0
             THEN 'IT'
             WHEN @i % 3 = 1
             THEN 'HR'
             ELSE 'Sales'
        END,

        NULL,
        0,
        NULL,
        NULL
    );

    SET @i = @i + 1;
END
create database SecoundDb
use secounddb
CREATE TABLE Department
(
    department_id INT PRIMARY KEY,
    department_name VARCHAR(50)
)
INSERT INTO Department (department_id, department_name) VALUES
(1, 'IT'),
(2, 'HR'),
(3, 'Sales'),
(4, 'Finance'),
(5, 'Admin');





CREATE TABLE srcemployee
(
    emp_id INT,
    emp_name VARCHAR(100),
    emp_dob DATE,
    emp_number VARCHAR(20),
    emp_salary DECIMAL(10,2),
    emp_address VARCHAR(200),
    emp_email VARCHAR(100),
    emp_department VARCHAR(50),
    push_status VARCHAR(20),
    is_processed INT DEFAULT 0,
    process_datetime DATETIME,
    remark VARCHAR(500)
)

CREATE TABLE StgEmployee
(
    emp_id INT,
    emp_name VARCHAR(100),
    emp_dob DATE,
    emp_number VARCHAR(20),
    emp_salary DECIMAL(10,2),
    emp_address VARCHAR(200),
    emp_email VARCHAR(100),
    emp_department VARCHAR(50)
)
select * from StgEmployee
truncate table stgemployee

CREATE TABLE ValidEmployee
(
    emp_id INT,
    emp_name VARCHAR(100),
    emp_dob DATE,
    emp_number VARCHAR(20),
    emp_salary DECIMAL(10,2),
    emp_address VARCHAR(200),
    emp_email VARCHAR(100),
    emp_department VARCHAR(50)
)

CREATE TABLE InvalidEmployee
(
    emp_id INT,
    emp_name VARCHAR(100),
    emp_dob DATE,
    emp_number VARCHAR(20),
    emp_salary DECIMAL(10,2),
    emp_address VARCHAR(200),
    emp_email VARCHAR(100),
    emp_department VARCHAR(50),
    error varchar(500),
    create_date datetime default getdate()
)
drop table invalidEmployee

CREATE TABLE Department
(
    department_id INT PRIMARY KEY,
    department_name VARCHAR(50)
)
INSERT INTO Department (department_id, department_name) VALUES
(1, 'IT'),
(2, 'HR'),
(3, 'Sales'),
(4, 'Finance'),
(5, 'Admin');






=====================================first prodeuder=============
create procedure DataSelect
as begin 
select top 1000 emp_id,emp_name,emp_dob,emp_number,emp_salary,emp_address,
emp_email,emp_department from SrcEmployee where is_processed=0 order by emp_id;
end 
====================================secound procedure =====================
drop procedure processEmpbulk
CREATE PROCEDURE ProcessEmpBulk
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ValidationErrors AS
    (
        SELECT emp_Id, 'Phone is NULL' ErrorMsg
        FROM StgEmployee WHERE emp_number IS NULL

        UNION ALL
        SELECT emp_Id, 'Phone length is not 10'
        FROM StgEmployee WHERE emp_number IS NOT NULL AND LEN(emp_number) <> 10

        UNION ALL
        SELECT emp_Id, 'Phone contains non-numeric characters'
        FROM StgEmployee WHERE emp_number LIKE '%[^0-9]%'

        UNION ALL
        SELECT emp_Id, 'Age less than 18'
        FROM StgEmployee
            WHERE DATEDIFF(YEAR, emp_dob, GETDATE()) < 18


            union all
        select emp_Id,'Salary is Zero' 
        from StgEmployee where not emp_salary>0

        UNION ALL
        SELECT emp_Id, 'Email is NULL'
        FROM StgEmployee WHERE emp_email IS NULL

            UNION ALL
        SELECT emp_id, 'Invalid email format'
        FROM StgEmployee
        WHERE emp_email IS NOT NULL AND emp_email NOT LIKE '%_@_%._%'

        UNION ALL
        SELECT emp_Id, 'Addresh Contain Special Simbol'
        FROM StgEmployee
        WHERE emp_address IS NOT NULL AND emp_address  LIKE '%@%#%*%~%$%'

        
        UNION ALL
        SELECT emp_Id, 'Department not found'
        FROM StgEmployee
        WHERE emp_department not in(select department_name from Department)
       

    ),
    FinalValidation AS
    (
        SELECT emp_Id,
                STRING_AGG(ErrorMsg, ' | ') AS AllErrors
        FROM ValidationErrors
        GROUP BY emp_Id
    )

    SELECT * INTO #FinalValidation FROM FinalValidation;
    -- VALID
    INSERT INTO ValidEmployee (emp_id, emp_name, emp_dob, emp_number,emp_salary,emp_address,
    emp_email,emp_department)
    SELECT s.emp_id, s.emp_name, s.emp_dob, s.emp_number,s.emp_salary,s.emp_address,
    emp_email,emp_department
    FROM StgEmployee s
    LEFT JOIN #FinalValidation v ON s.emp_id = v.emp_id
    WHERE v.emp_id IS NULL;

    -- INVALID
    INSERT INTO InvalidEmployee
(
    emp_id, emp_name, emp_dob, emp_number,
    emp_salary, emp_address, emp_email,
    emp_department, error
)
SELECT
    s.emp_id, s.emp_name, s.emp_dob, s.emp_number,
    s.emp_salary, s.emp_address, s.emp_email,
    s.emp_department, v.AllErrors
FROM StgEmployee s
JOIN #FinalValidation v      -- 🔥 INNER JOIN (MANDATORY)
    ON s.emp_id = v.emp_id;

  

  SELECT 
        s.emp_id,
        CASE 
            WHEN v.emp_id IS NOT NULL THEN 'Fail'
            ELSE 'Success'
        END AS push_status,
        v.AllErrors AS remark
    FROM StgEmployee s
    LEFT JOIN FinalValidation v ON s.emp_id = v.emp_id;

    drop table #FinalValidation
    TRUNCATE TABLE StgEmployee;
END


select * from stgemployee
truncate table stgemployee
select * from invalidemployee
truncate table invalidemployee
exec ProcessEmpBulk
use secounddb
select * from ValidEmployee
exec sp_rename employee_master,ValidEmployee
truncate table ValidEmployee


use SimpleTaskdb
select * from srcemployee
exec sp_rename employee_staging ,srcemployee

update srcemployee set is_processed=0,push_status=null,process_datetime=null,remark=null ;


drop procedure sp_InsertEmployee



CREATE PROCEDURE UpdateEmpStatus
(
    @EmpStatus EmpStatusType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE src
    SET
        src.push_status = s.push_status,
        src.remark = s.remark,
        src.is_processed = CASE
            WHEN s.push_status = 'Fail' THEN 2
            ELSE 1
        END
    FROM srcemployee src
    JOIN @EmpStatus s
        ON src.emp_id = s.emp_id
    WHERE src.is_processed = 0;
END


CREATE TYPE EmpStatusType AS TABLE
(
    em_sp_id INT,
    pushtatus VARCHAR(20),
    remark VARCHAR(500)
);


SELECT * 
FROM sys.types 
WHERE is_table_type = 1;

EXEC sp_help 'EmpStatusType';
