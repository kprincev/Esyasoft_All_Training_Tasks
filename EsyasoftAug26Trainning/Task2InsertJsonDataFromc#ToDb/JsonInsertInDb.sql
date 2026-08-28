
use AugEsyasoftTranningTask




create procedure GetEmpData
as begin 
    select * from Employees;
end

exec getempdata

select * from COMPANY
select * from DEPARTMENT
select * from employees
select * from EMPSALARY
select * from PROJECTS
select * from TASK

create table COMPANY(COMPID int primary key ,COMPNAME VARCHAR(50),COUNTRY VARCHAR(30),STATE VARCHAR(30),CITY VARCHAR(40))

CREATE TABLE DEPARTMENT(DEPTID INT PRIMARY KEY,COMPID INT ,DEPTNAME VARchar(30) , FOREIGN KEY (COMPID) referenceS COMPANY(COMPID))  

CREATE TABLE EMPLOYEES (EMPID INT PRIMARY KEY,EMPNAME VARCHAR(30),DEPTID INT ,EMAIL VARCHAR(50) ,FOREIGN KEY (DEPTID) REFERENCES DEPARTMENT(DEPTID))

CREATE table EMPSALARY(EMPID INT ,BASIC INT,BONUS INT ,CURRENCY VARCHAR(10),FOREIGN KEY (EMPID) REFERENCES EMPLOYEES(EMPID) )

CREATE TABLE PROJECTS(PROJECTID INT PRIMARY KEY ,EMPID INT ,PROJECTNAME VARCHAR(50),STATUS VARCHAR(20),FOREIGN KEY (EMPID) REFERENCES EMPLOYEES(EMPID))

CREATE TABLE TASK (TASKID INT PRIMARY KEY ,PROJECTID INT,TASKNAME VARCHAR(50),STATUS VARCHAR(20),HOURS INT ,FOREIGN KEY (PROJECTID) REFERENCES PROJECTS(PROJECTID))

CREATE PROCEDURE sp_InsertCompany
    @COMPID INT,
    @COMPNAME VARCHAR(50),
    @COUNTRY VARCHAR(30),
    @STATE VARCHAR(30),
    @CITY VARCHAR(40)
AS
BEGIN
    INSERT INTO COMPANY (COMPID, COMPNAME, COUNTRY, STATE, CITY)
    VALUES (@COMPID, @COMPNAME, @COUNTRY, @STATE, @CITY)
END
CREATE PROCEDURE sp_InsertDepartment
    @DEPTID INT,
    @COMPID INT,
    @DEPTNAME VARCHAR(30)
AS
BEGIN
    INSERT INTO DEPARTMENT (DEPTID, COMPID, DEPTNAME)
    VALUES (@DEPTID, @COMPID, @DEPTNAME)
END


CREATE PROCEDURE sp_InsertEmployee
    @EMPID INT,
    @EMPNAME VARCHAR(30),
    @DEPTID INT,
    @EMAIL VARCHAR(50)
AS
BEGIN
    INSERT INTO EMPLOYEES (EMPID, EMPNAME, DEPTID, EMAIL)
    VALUES (@EMPID, @EMPNAME, @DEPTID, @EMAIL)
END

CREATE PROCEDURE sp_InsertSalary
    @EMPID INT,
    @BASIC INT,
    @BONUS INT,
    @CURRENCY VARCHAR(10)
AS
BEGIN
    INSERT INTO EMPSALARY (EMPID, BASIC, BONUS, CURRENCY)
    VALUES (@EMPID, @BASIC, @BONUS, @CURRENCY)
END

CREATE PROCEDURE sp_InsertProject
    @PROJECTID INT,
    @EMPID INT,
    @PROJECTNAME VARCHAR(50),
    @STATUS VARCHAR(20)
AS
BEGIN
    INSERT INTO PROJECTS (PROJECTID, EMPID, PROJECTNAME, STATUS)
    VALUES (@PROJECTID, @EMPID, @PROJECTNAME, @STATUS)
END


CREATE PROCEDURE sp_InsertTask
    @TASKID INT,
    @PROJECTID INT,
    @TASKNAME VARCHAR(50),
    @STATUS VARCHAR(20),
    @HOURS INT
AS
BEGIN
    INSERT INTO TASK (TASKID, PROJECTID, TASKNAME, STATUS, HOURS)
    VALUES (@TASKID, @PROJECTID, @TASKNAME, @STATUS, @HOURS)
END


select * from COMPANY
select * from DEPARTMENT
select * from employees
select * from EMPSALARY
select * from PROJECTS
select * from TASK


drop table  COMPANY
drop table  DEPARTMENT
drop table  employees
drop table  EMPSALARY
drop table  PROJECTS
drop  table  TASK

SELECT 
    f.name AS ForeignKeyName,
    OBJECT_NAME(f.parent_object_id) AS ChildTable,
    OBJECT_NAME(f.referenced_object_id) AS ParentTable
FROM sys.foreign_keys AS f;
alter table PROJECTS drop constraint FK__PROJECTS__EMPID__04E4BC85
alter table DEPARTMENT drop constraint FK__DEPARTMEN__COMPI__71D1E811
alter table EMPLOYEES drop constraint FK__EMPLOYEES__DEPTI__74AE54BC




CREATE or alter  PROCEDURE sp_ImportCompanyJson
    @JSON NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        -- COMPANY
        INSERT INTO COMPANY
        (
            COMPID,
            COMPNAME,
            COUNTRY,
            STATE,
            CITY
        )
        SELECT
            companyId,
            companyName,
            country,
            state,
            city
        FROM OPENJSON(@JSON)
        WITH
        (
            companyId INT '$.companyId',
            companyName VARCHAR(50) '$.companyName',
            country VARCHAR(30) '$.location.country',
            state VARCHAR(30) '$.location.state',
            city VARCHAR(40) '$.location.city'
        );


        -- DEPARTMENT
        INSERT INTO DEPARTMENT
        (
            DEPTID,
            COMPID,
            DEPTNAME
        )
        SELECT
            d.departmentId,
            JSON_VALUE(@JSON, '$.companyId'),
            d.departmentName
        FROM OPENJSON(@JSON, '$.departments')
        WITH
        (
            departmentId INT '$.departmentId',
            departmentName VARCHAR(30) '$.departmentName'
        ) d;


        -- EMPLOYEES
        INSERT INTO EMPLOYEES
        (
            EMPID,
            EMPNAME,
            DEPTID,
            EMAIL
        )
        SELECT
            e.employeeId,
            e.name,
            d.departmentId,
            e.email
        FROM OPENJSON(@JSON, '$.departments') dep
        CROSS APPLY OPENJSON(dep.value, '$.employees')
        WITH
        (
            employeeId INT '$.employeeId',
            name VARCHAR(30) '$.name',
            email VARCHAR(50) '$.email'
        ) e
        CROSS APPLY OPENJSON(dep.value)
        WITH
        (
            departmentId INT '$.departmentId'
        ) d;


        -- EMPLOYEE SALARY
        INSERT INTO EMPSALARY
        (
            EMPID,
            BASIC,
            BONUS,
            CURRENCY
        )
        SELECT
            e.employeeId,
            s.basic,
            s.bonus,
            s.currency
        FROM OPENJSON(@JSON, '$.departments') dep
        CROSS APPLY OPENJSON(dep.value, '$.employees')
        WITH
        (
            employeeId INT '$.employeeId',
            salary NVARCHAR(MAX) '$.salary' AS JSON
        ) e
        CROSS APPLY OPENJSON(e.salary)
        WITH
        (
            basic INT '$.basic',
            bonus INT '$.bonus',
            currency VARCHAR(10) '$.currency'
        ) s;


        -- PROJECTS
        INSERT INTO PROJECTS
        (
            PROJECTID,
            EMPID,
            PROJECTNAME,
            STATUS
        )
        SELECT
            p.projectId,
            e.employeeId,
            p.projectName,
            p.status
        FROM OPENJSON(@JSON, '$.departments') dep
        CROSS APPLY OPENJSON(dep.value, '$.employees')
        WITH
        (
            employeeId INT '$.employeeId',
            projects NVARCHAR(MAX) '$.projects' AS JSON
        ) e
        CROSS APPLY OPENJSON(e.projects)
        WITH
        (
            projectId INT '$.projectId',
            projectName VARCHAR(50) '$.projectName',
            status VARCHAR(20) '$.status'
        ) p;


        -- TASK
        INSERT INTO TASK
        (
            TASKID,
            PROJECTID,
            TASKNAME,
            STATUS,
            HOURS
        )
        SELECT
            t.taskId,
            p.projectId,
            t.taskName,
            t.status,
            t.hours
        FROM OPENJSON(@JSON, '$.departments') dep
        CROSS APPLY OPENJSON(dep.value, '$.employees')
        WITH
        (
            projects NVARCHAR(MAX) '$.projects' AS JSON
        ) e
        CROSS APPLY OPENJSON(e.projects)
        WITH
        (
            projectId INT '$.projectId',
            tasks NVARCHAR(MAX) '$.tasks' AS JSON
        ) p
        CROSS APPLY OPENJSON(p.tasks)
        WITH
        (
            taskId INT '$.taskId',
            taskName VARCHAR(50) '$.taskName',
            status VARCHAR(20) '$.status',
            hours INT '$.hours'
        ) t;


        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH
END;

======================================================================task 4 table data json,=================================================
drop procedure ExportCompanyJson
CREATE OR ALTER PROCEDURE SP_CompanyDataToJson
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        C.COMPID AS companyId,
        C.COMPNAME AS companyName,
        JSON_QUERY((
            SELECT C.COUNTRY AS country, C.STATE AS state, C.CITY AS city
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        )) AS location,

        JSON_QUERY((
            SELECT
                D.DEPTID AS departmentId,
                D.DEPTNAME AS departmentName,
                JSON_QUERY((
                    SELECT
                        E.EMPID AS employeeId,
                        E.EMPNAME AS name,
                        E.EMAIL AS email,
                        JSON_QUERY((
                            SELECT BASIC AS basic, BONUS AS bonus, CURRENCY AS currency
                            FROM empSALARY
                            WHERE EMPID = E.EMPID
                            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                        )) AS salary,
                        JSON_QUERY((
                            SELECT
                                P.PROJECTID AS projectId,
                                P.PROJECTNAME AS projectName,
                                P.STATUS AS status,
                                JSON_QUERY((
                                    SELECT
                                        T.TASKID AS taskId,
                                        T.TASKNAME AS taskName,
                                        T.STATUS AS status,
                                        T.HOURS AS hours
                                    FROM TASK T
                                    WHERE T.PROJECTID = P.PROJECTID
                                    FOR JSON PATH
                                )) AS tasks
                            FROM PROJECTS P
                            WHERE P.EMPID = E.EMPID
                            FOR JSON PATH
                        )) AS projects
                    FROM EMPLOYEES E
                    WHERE E.DEPTID = D.DEPTID
                    FOR JSON PATH
                )) AS employees
            FROM DEPARTMENT D
            WHERE D.COMPID = C.COMPID
            FOR JSON PATH
        )) AS departments

    FROM COMPANY C
    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER;
END
GO

exec ExportCompanyJson

====================================================================================
select * from COMPANY
select * from DEPARTMENT
select * from employees
select * from EMPSALARY
select * from PROJECTS
select * from TASK

select 
    c.compid as compnayid,
    c.compname as companyname,
    c.country as 'location.Contray',
    c.state as 'location.state',
    c.city as 'location.city',
    (select
                    d.deptid as depatementid,
                    d.deptname as departmentName,
                 (select
                                    e.empid as employeeid,
                                    e.empname as employeename,
                                    e.email as email,
                                     json_query((select 
                                        s.basic as Basic,
                                        s.bonus as Bonus,
                                        s.currency as currency
                                 from empsalary s where s.empid=e.empid  for json path,without_array_wrapper
                                 
                                 )) as salary,
                                 ( select 
                                        p.projectid as Projectid,
                                        p.projectname as ProjectName,
                                        p.Status as status,
                                        (
                                            SELECT 
                                                    t.taskid as taskid,
                                                    t.taskname as taskname,
                                                    t.status as status,
                                                    t.hours as hours
                                            from task t where t.projectid=p.projectid for json path
                                        ) as tasks
                                    from projects p where p.empid=e.empid for json path 
                                  ) as Projects
                                from employees e where e.deptid=d.deptid for json path
                               
                    ) as Employees 
                from department d for json path

    ) as departments

from company c for json path, without_array_wrapper