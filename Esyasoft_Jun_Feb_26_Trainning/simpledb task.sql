use db
select * from test

====================wether task =====================================
use SimpleTaskdb

CREATE TABLE WeatherData
(
    Id INT IDENTITY PRIMARY KEY,
    City NVARCHAR(100),
    Temperature FLOAT,
    WeatherDate DATETIME,
    InsertinDate    DATETIME
);
DROP TABLE WEATHERDATA


SELECT * FROM WEATHERDATA;

create procedure insertweatherdata
(
    @City NVARCHAR(100),
    @Tem FLOAT,
    @WD DATETIME
)
as begin
insert into weatherdata(city,Temperature,WeatherDate,InsertinDate )values (@City,@Tem,@WD,getdate());
end;

exec insertweatherdata 'bhpal',3.4,'11-11-2026'

select * from weatherdata


=============================
use SimpleTaskdb

CREATE TABLE MeterReadingg
(
    MeterId       VARCHAR(50),
    ReadingDate   DATE,
    TotalAmount   DECIMAL(10,2),
    Avg_Voltage   DECIMAL(10,2),
    Avg_Current   DECIMAL(10,2),
    Energy_kWh    DECIMAL(10,2),
    CreatedOn     DATETIME DEFAULT GETDATE()
);

CREATE PROCEDURE InsertMeterReadingg
    @json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    -- ✅ Validate JSON
    IF ISJSON(@json) = 0
    BEGIN
        THROW 50001, 'Invalid JSON', 1;
    END

    -- ✅ Direct insert (single pass)
    INSERT INTO MeterReadingg
    (
        MeterId,
        ReadingDate,
        TotalAmount,
        Avg_Voltage,
        Avg_Current,
        Energy_kWh
    )
    SELECT
        JSON_VALUE(@json, '$.meterId'),
        JSON_VALUE(@json, '$.readingDate'),
        JSON_VALUE(@json, '$.totalAmount'),

        MAX(CASE WHEN b.parameter = 'Avg_Voltage' THEN b.value END),
        MAX(CASE WHEN b.parameter = 'Avg_Current' THEN b.value END),
        MAX(CASE WHEN b.parameter = 'Energy_kWh' THEN b.value END)

    FROM OPENJSON(@json, '$.breakdown')
    WITH
    (
        parameter VARCHAR(50) '$.parameter',
        value     DECIMAL(10,2) '$.value'
    ) b;
END

select * from MeterReadingg
truncate table meterreadingg




===============================================================================================

create table BPLData(Meter_Id int ,FirstIntervalDateTime Datetime ,[Avg_Voltage_(V)] DECIMAL(10,5),
[BlkEngy_I/F_(kWh)] decimal(10,5),[BlkEngy_I/F_(kVAh)] decimal(10,5),[Avg_Current_(A)] decimal(10,5));

SELECT * FROM BLpDATA
sp_rename bpldata,BlpData

CREATE PROCEDURE InsertBlpData
(
    @Meter_Id int,
    @FirstIntervalDateTime Datetime,
    @V decimal(10,5),
    @kWh decimal(10,5),
    @kVah decimal(10,5),
    @A decimal(10,5)
)
as begin
insert into blpdata values(@Meter_Id,@FirstIntervalDateTime,@V,@kWh,@kVah,@A);
end;

exec InsertBlpData 1,' 09-01-2024 06:30 ',23.3,232,32,34

SELECT * FROM BLpDATA
truncate table blpdata
select count(meter_id) from blpdata where meter_id=802614\



===============================================
create table MeterIntervalData
(
  Meter_Id        VARCHAR(50),
  IntervalDateTime DATETIME,
  Avg_Voltage_V   DECIMAL(18,5),
  BlkEngy_kWh     DECIMAL(18,5),
  BlkEngy_kVAh    DECIMAL(18,5),
  Avg_Current_A   DECIMAL(18,5)
)
;

select * from MeterIntervalData
truncate table meterintervaldata


CREATE TYPE MeterIntervalType AS TABLE
(
    Meter_Id           VARCHAR(50),
    IntervalDateTime   DATETIME,
    Avg_Voltage_V      DECIMAL(18,5),
    BlkEngy_kWh        DECIMAL(18,5),
    BlkEngy_kVAh       DECIMAL(18,5),
    Avg_Current_A      DECIMAL(18,5)
);
select * from meterintervaltype

select * from MeterReading
drop table MeterReading 
==========================task first rebitmq =========================================
use SimpleTaskdb


CREATE TABLE MeterDataByRabbitMq
(
  Id INT IDENTITY(1,1) PRIMARY KEY,

    MeterId     VARCHAR(50)    NOT NULL,
    ReadingDate DATETIME       NOT NULL,
    Voltage     DECIMAL(10,2)  NULL,
 

    CreatedOn   DATETIME       NOT NULL DEFAULT GETDATE()
);


CREATE PROCEDURE usp_InsertMeterReading
(
    @MeterId     VARCHAR(50),
    @ReadingDate DATETIME,
    @Voltage     DECIMAL(10,2)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO MeterDataByRabbitMq
    (
        MeterId,
        ReadingDate,
        Voltage
    )
    VALUES
    (
        @MeterId,
        @ReadingDate,
        @Voltage
    );
END;
GO

drop procedure usp_InsertMeterReading
SELECT * FROM sys.procedures WHERE name = 'usp_InsertMeterReading';

select * from MeterDataByRabbitMq
truncate table MeterDataByRabbitMq

===========================================TwoVirtualTableCreateServiceToPerfromOperation=========================================


use SimpleTaskdb
CREATE TABLE Employee_Staging
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

select * from Employee_Staging
truncate table srcemployee

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


CREATE PROCEDURE sp_UpdateEmployeeStatus
    @emp_id INT,
    @push_status VARCHAR(20),
    @is_processed INT,
    @remark VARCHAR(500)
AS
BEGIN
    UPDATE Employee_Staging
    SET
        push_status = @push_status,
        is_processed = @is_processed,
        process_datetime = GETDATE(),
        remark = @remark
    WHERE emp_id = @emp_id;
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


CREATE TABLE Employee_Master
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
drop procedure sp_InsertEmployee_JSON
/*insert emmployee jeson procedure */
CREATE PROCEDURE sp_InsertEmployee_JSON
    @empJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @emp_id INT,
        @name VARCHAR(100),
        @dob DATE,
        @number VARCHAR(20),
        @salary DECIMAL(10,2),
        @address VARCHAR(200),
        @email VARCHAR(100),
        @dept VARCHAR(50);

    -- ✅ SAFE JSON parsing
    SELECT
        @emp_id = JSON_VALUE(@empJson, '$.emp_id'),
        @name = JSON_VALUE(@empJson, '$.emp_name'),
        @dob = JSON_VALUE(@empJson, '$.emp_dob'),
        @number = JSON_VALUE(@empJson, '$.emp_number'),
        @salary = JSON_VALUE(@empJson, '$.emp_salary'),
        @address = JSON_VALUE(@empJson, '$.emp_address'),
        @email = JSON_VALUE(@empJson, '$.emp_email'),
        @dept = JSON_VALUE(@empJson, '$.emp_department');

    SET @dept = LTRIM(RTRIM(@dept));

    -- 🔴 VALIDATION
    IF DATEDIFF(YEAR, @dob, GETDATE()) < 18
        THROW 50001, 'Age < 18', 1;

    IF @salary <= 0
        THROW 50002, 'Salary invalid', 1;

    IF LEN(@number) <> 10
        THROW 50003, 'Mobile invalid', 1;

    IF @email NOT LIKE '%@%.%'
        THROW 50004, 'Email invalid', 1;

    IF NOT EXISTS (
        SELECT 1 FROM Department
        WHERE department_name = @dept
    )
        THROW 50005, 'Department not found', 1;


    -- ✅ INSERT
    INSERT INTO Employee_Master
    VALUES
    (@emp_id,@name,@dob,@number,@salary,@address,@email,@dept);
END
============================================




CREATE PROCEDURE sp_InsertEmployee
    @emp_id INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @name VARCHAR(100),
        @dob DATE,
        @number VARCHAR(20),
        @salary DECIMAL(10,2),
        @address VARCHAR(200),
        @email VARCHAR(100),
        @dept VARCHAR(50);

    -- Get data from staging
    SELECT
        @name = emp_name,
        @dob = emp_dob,
        @number = emp_number,
        @salary = emp_salary,
        @address = emp_address,
        @email = emp_email,
        @dept = emp_department
    FROM SimpleTaskdb.dbo.Employee_Staging
    WHERE emp_id = @emp_id;

    -- 🔴 VALIDATIONS
    IF DATEDIFF(YEAR, @dob, GETDATE()) < 18
        THROW 50001, 'Age less than 18', 1;

    IF @salary <= 0
        THROW 50002, 'Salary cannot be zero', 1;

    IF LEN(@number) <> 10
        THROW 50003, 'Invalid mobile number', 1;

    IF @email NOT LIKE '%@%.%'
        THROW 50004, 'Invalid email format', 1;

    IF NOT EXISTS (
        SELECT 1 FROM Department WHERE department_name = @dept
    )
        THROW 50005, 'Department not found', 1;

    -- ✅ INSERT DESTINATION
    INSERT INTO Employee_Master
    (emp_id, emp_name, emp_dob, emp_number,
     emp_salary, emp_address, emp_email, emp_department)
    VALUES
    (@emp_id, @name, @dob, @number,
     @salary, @address, @email, @dept);
END




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


======================================================================================
use simpletaskdb
CREATE TABLE Ls_Xml_Data
(
    MeterSerialNumber VARCHAR(50),
    IntervalTime DATETIME,

    RT_1_0_12_27_0_255_1800 DECIMAL(18,3),
    RT_1_0_1_7_0_255_1800 DECIMAL(18,3),
    RT_1_0_9_29_0_255_1800 DECIMAL(18,3),
    RT_1_0_2_29_0_255_1800 DECIMAL(18,3),
    RT_1_0_10_29_0_255_1800 DECIMAL(18,3),
    RT_1_0_11_27_0_255_1800 DECIMAL(18,3),
    RT_1_0_91_27_0_255_1800 DECIMAL(18,3),
    RT_0_0_96_10_1_255_1800 DECIMAL(18,3)
);

CREATE TABLE Ls_Xml_Data
(
    MeterSerialNumber VARCHAR(50),
    IntervalTime DATETIME,

    a DECIMAL(18,3),
    b DECIMAL(18,3),
    c DECIMAL(18,3),
    d DECIMAL(18,3),
    e DECIMAL(18,3),
    f DECIMAL(18,3),
    g DECIMAL(18,3),
    h DECIMAL(18,3)
);
ALTER TABLE Ls_Xml_Data
ADD CONSTRAINT UQ_LS_MeterSerial_IntervalTime
UNIQUE (MeterSerialNumber, IntervalTime);


drop table Ls_Xml_Data
drop procedure sp_insert_ls_fromjson
select * from ls_xml_data
truncate table ls_xml_data

CREATE PROCEDURE SP_Insert_LS_FromJson
(
    @JsonData NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Ls_Xml_Data
    (
        MeterSerialNumber,
        IntervalTime,
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h
    )
    SELECT
        MeterSerialNumber,
        IntervalTime,
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h
        
    FROM OPENJSON(@JsonData)
    WITH
    (
        MeterSerialNumber VARCHAR(50),
        IntervalTime DATETIME,

        a DECIMAL(18,3),
        b DECIMAL(18,3),
        c DECIMAL(18,3),
        d DECIMAL(18,3),
        e DECIMAL(18,3),
        f DECIMAL(18,3),
        g DECIMAL(18,3),
        h DECIMAL(18,3)
    );
END;




==============================================================================================
use simpletaskdb
CREATE TABLE SinglePhaseMeterData
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    MeterSerialNumber VARCHAR(50) NOT NULL,

    TS DATETIME2(0) NOT NULL,   -- Date + Time (interval timestamp)

    IntervalMinutes INT NOT NULL, 
    -- 15 ya 30 (isse pata chalega ye kis type ka meter hai)

    Current_A DECIMAL(10,3) NULL,   -- Ampere
    Voltage_V DECIMAL(10,3) NULL,   -- Volt

    KW DECIMAL(12,4) NULL,
    KVA DECIMAL(12,4) NULL,
    KVARh DECIMAL(12,4) NULL,

    CreatedOn DATETIME2(0) DEFAULT GETDATE()
);


CREATE TABLE ConsumerMaster
(
    ConsumerId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ConsumerNumber VARCHAR(50) NOT NULL,
    ConsumerName VARCHAR(150) NOT NULL,
    MobileNumber VARCHAR(15),
    Email VARCHAR(150),
    AddressLine1 VARCHAR(250),
    City VARCHAR(100),
    State VARCHAR(100),
    Pincode VARCHAR(10),
    MeterSerialNumber VARCHAR(50) NOT NULL,
    MeterType VARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME2(0) DEFAULT GETDATE()
);




INSERT INTO ConsumerMaster
(ConsumerNumber, ConsumerName, MobileNumber, City, State, MeterSerialNumber, MeterType)
VALUES
-- 15 MIN INTERVAL
('CNS-001','Ramesh Kumar','9876543210','Delhi','Delhi','MTR-15-1','SINGLE_PHASE'),
('CNS-002','Suresh Verma','9876543211','Noida','UP','MTR-15-2','SINGLE_PHASE'),
('CNS-003','Amitttt Sharma','9876543212','Ghaziabad','UP','MTR-15-3','SINGLE_PHASE'),
('CNS-004','Amgsfdgit Sharma','9876543212','Ghaziabad','UP','MTR-15-4','SINGLE_PHASE'),
('CNS-005','Amidadst Sharma','9876543212','Ghaziabad','UP','MTR-15-5','SINGLE_PHASE'),

('CNS-006','Rohiddt Singh','9876543220','Faridabad','Haryana','MTR-30-1','SINGLE_PHASE'),
('CNS-007','Anil Gupta','9876543221','Gurgaon','Haryana','MTR-30-2','SINGLE_PHASE'),
('CNS-008','Rohit Singh','9876543220','Faridabad','Haryana','MTR-30-3','SINGLE_PHASE'),
('CNS-009','Rohdadfit Singh','9876543220','Faridabad','Haryana','MTR-30-4','SINGLE_PHASE'),
('CNS-010','Rohigfdgt Singh','9876543220','Faridabad','Haryana','MTR-30-5','SINGLE_PHASE'),



('CNS-011','Vikas Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-1','THREE_PHASE'),
('CNS-012','Vikdadfas Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-2','THREE_PHASE'),
('CNS-013','Vikcsafdas Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-3','THREE_PHASE'),



('CNS-014','Vikaseafa Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-4','THREE_PHASE'),
('CNS-015','Neeraj Jain','9876543214','Kota','Rajasthan','TP-MTR-15-5','THREE_PHASE'),

-- 30 MIN INTERVAL


('CNS-016','Pankadasgj Agarwal','9876543222','Indore','MP','TP-MTR-30-1','THREE_PHASE'),
('CNS-017','Pankcsafaj Agarwal','9876543222','Indore','MP','TP-MTR-30-2','THREE_PHASE'),
('CNS-018','Pankaj Agarwal','9876543222','Indore','MP','TP-MTR-30-3','THREE_PHASE'),
('CNS-019','Sanjay Patel','9876543223','Ahmedabad','Gujarat','TP-MTR-30-4','THREE_PHASE'),
('CNS-020','Mahesh Shah','9876543224','Surat','Gujarat','TP-MTR-30-5','THREE_PHASE');





CREATE TABLE MeterHourlyTarget
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

     -- CNS-001 etc.

    MSN VARCHAR(50) NOT NULL,              -- Meter Serial Number
     ConsumerNumber VARCHAR(50) NOT NULL,
    MeterType VARCHAR(20) NOT NULL,        -- SINGLE_PHASE / THREE_PHASE

    TS DATETIME2(0) NOT NULL,              -- Hour start (Date + Hour)

    -- AVERAGE VALUES (hourly)
    Avg_V      DECIMAL(12,4) NULL,
    Avg_I      DECIMAL(12,4) NULL,
    Avg_KW     DECIMAL(12,4) NULL,
    Avg_KVA    DECIMAL(12,4) NULL,
    Avg_KVARh  DECIMAL(12,4) NULL,

    -- SUM VALUES (hourly)
    Sum_V      DECIMAL(14,4) NULL,
    Sum_I      DECIMAL(14,4) NULL,
    Sum_KW     DECIMAL(14,4) NULL,
    Sum_KVA    DECIMAL(14,4) NULL,
    Sum_KVARh  DECIMAL(14,4) NULL,

    CreatedOnDate DATETIME2(0) DEFAULT GETDATE()
);

drop table meterprocessingcounter
CREATE TABLE MeterProcessingCounter
(
    MSN VARCHAR(50) PRIMARY KEY,
    LastProcessedTS DATETIME2(0) NOT NULL
);



drop procedure sp_insertmeterhourlytarget
CREATE PROCEDURE SP_InsertMeterHourlyTarget
(
    @ConsumerNumber VARCHAR(50),
    @MSN VARCHAR(50),
    @MeterType VARCHAR(20),
    @TS DATETIME2,

    @Avg_V DECIMAL(12,4),
    @Avg_I DECIMAL(12,4),
    @Avg_KW DECIMAL(12,4),
    @Avg_KVA DECIMAL(12,4),
    @Avg_KVARh DECIMAL(12,4),

    @Sum_V DECIMAL(14,4),
    @Sum_I DECIMAL(14,4),
    @Sum_KW DECIMAL(14,4),
    @Sum_KVA DECIMAL(14,4),
    @Sum_KVARh DECIMAL(14,4)
)
AS
BEGIN
    IF NOT EXISTS
    (
        SELECT 1 FROM MeterHourlyTarget
        WHERE MSN=@MSN AND TS=@TS
    )
    BEGIN
        INSERT INTO MeterHourlyTarget
        (ConsumerNumber,MSN,MeterType,TS,
         Avg_V,Avg_I,Avg_KW,Avg_KVA,Avg_KVARh,
         Sum_V,Sum_I,Sum_KW,Sum_KVA,Sum_KVARh)
        VALUES
        (@ConsumerNumber,@MSN,@MeterType,@TS,
         @Avg_V,@Avg_I,@Avg_KW,@Avg_KVA,@Avg_KVARh,
         @Sum_V,@Sum_I,@Sum_KW,@Sum_KVA,@Sum_KVARh);
    END
END

use simpletaskdb
truncate table MeterHourlyTarget
select * from ConsumerMaster
select * from SinglePhaseMeterData
select * from ThreePhaseMeterData
select * from MeterHourlyTarget
select * from MeterProcessingCounter
truncate table MeterProcessingCounter
select * from C
SELECT MIN(TS), MAX(TS)
FROM SinglePhaseMeterData
WHERE MeterSerialNumber = 'MTR-15-1';


INSERT INTO MeterProcessingCounter (MSN, LastProcessedTS)
SELECT MeterSerialNumber, '1900-01-01'
FROM ConsumerMaster;
UPDATE MeterProcessingCounter
SET LastProcessedTS = '2026-02-09 00:00:00';

SELECT COUNT(*) FROM MeterHourlyTarget WHERE MSN='MTR-15-1'


update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-1' WHERE meterserialnumber='TP-MTR-1'

update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-2' WHERE meterserialnumber='TP-MTR-2'
update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-3' WHERE meterserialnumber='TP-MTR-3'
update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-4' WHERE meterserialnumber='TP-MTR-4'
update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-5' WHERE meterserialnumber='TP-MTR-5'


SELECT COUNT(*) FROM MeterHourlyTarget WHERE MSN='MTR-15-2'


SELECT DISTINCT CAST(TS AS DATE)
FROM SinglePhaseMeterData
WHERE MeterSerialNumber='MTR-15-1'
ORDER BY 1;




SELECT MSN, LastProcessedTS FROM MeterProcessingCounter

CREATE PROCEDURE SP_GetMeterProcessingCounter
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MSN, LastProcessedTS
    FROM MeterProcessingCounter
    ORDER BY MSN;
END


CREATE PROCEDURE SP_GetSinglePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MeterSerialNumber,
        TS,
        Voltage_V,
        Current_A,
        KW,
        KVA,
        KVARh
    FROM SinglePhaseMeterData
    WHERE MeterSerialNumber = @MSN
      AND TS >= @StartDate
      AND TS < @EndDate
    ORDER BY TS;
END



CREATE PROCEDURE SP_GetThreePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MeterSerialNumber,
        TS,
        (V_R + V_Y + V_B) / 3.0 AS AvgVoltage,
        (I_R + I_Y + I_B) / 3.0 AS AvgCurrent,
        KW,
        KVA,
        KVARh
    FROM ThreePhaseMeterData
    WHERE MeterSerialNumber = @MSN
      AND TS >= @StartDate
      AND TS < @EndDate
    ORDER BY TS;
END


CREATE PROCEDURE SP_UpdateMeterProcessingCounter
(
    @MSN VARCHAR(50),
    @TS  DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE MeterProcessingCounter
    SET LastProcessedTS = @TS
    WHERE MSN = @MSN;
END


ALTER TABLE table_name
DROP COLUMN consumer_number;



drop procedure SP_GetSinglePhaseRawData
drop procedure SP_GetThreePhaseRawData



ALTER PROCEDURE SP_GetThreePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ConsumerNumber,   -- 🔥 Added
        t.MeterSerialNumber,
        t.TS,
        (V_R + V_Y + V_B) / 3.0 AS AvgVoltage,
        (I_R + I_Y + I_B) / 3.0 AS AvgCurrent,
        t.KW,
        t.KVA,
        t.KVARh
    FROM ThreePhaseMeterData t
    INNER JOIN ConsumerMaster c
        ON c.MeterSerialNumber = t.MeterSerialNumber
    WHERE t.MeterSerialNumber = @MSN
      AND t.TS >= @StartDate
      AND t.TS < @EndDate
    ORDER BY t.TS;
END

ALTER PROCEDURE SP_GetSinglePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ConsumerNumber,   -- 🔥 Added
        s.MeterSerialNumber,
        s.TS,
        s.Voltage_V,
        s.Current_A,
        s.KW,
        s.KVA,
        s.KVARh
    FROM SinglePhaseMeterData s
    INNER JOIN ConsumerMaster c
        ON c.MeterSerialNumber = s.MeterSerialNumber
    WHERE s.MeterSerialNumber = @MSN
      AND s.TS >= @StartDate
      AND s.TS < @EndDate
    ORDER BY s.TS;
END






===========================================================Api Task====================================================
use simpletaskdb

CREATE TABLE Consumer
(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100),
    Email NVARCHAR(100),
    Mobile NVARCHAR(15),
    Address NVARCHAR(250)
)
exec sp_rename consumer,consumers
select * from consumers
insert into consumer values ('prince','princevermaji@gmail.com',3234242344,'akjdkafdfakdjfajf')
truncate table consumers