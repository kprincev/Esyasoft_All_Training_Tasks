use AugEsyasoftTranningTask

-- Step 1: Create Customer table
CREATE TABLE Customers (
    CustomerID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100),
    City VARCHAR(50),
    Phone VARCHAR(15)
);

-- Step 2: Insert 10 sample records
INSERT INTO Customers VALUES (1, 'Amit', 'Sharma', 'amit.sharma@example.com', 'Delhi', '9876543210');
INSERT INTO Customers VALUES (2, 'Priya', 'Verma', 'priya.verma@example.com', 'Mumbai', '9123456780');
INSERT INTO Customers VALUES (3, 'Rahul', 'Mehta', 'rahul.mehta@example.com', 'Pune', '9988776655');
INSERT INTO Customers VALUES (4, 'Sneha', 'Patel', 'sneha.patel@example.com', 'Ahmedabad', '9112233445');
INSERT INTO Customers VALUES (5, 'Vikas', 'Gupta', 'vikas.gupta@example.com', 'Bhopal', '9001122334');
INSERT INTO Customers VALUES (6, 'Anjali', 'Rao', 'anjali.rao@example.com', 'Hyderabad', '9556677889');
INSERT INTO Customers VALUES (7, 'Karan', 'Singh', 'karan.singh@example.com', 'Chennai', '9445566778');
INSERT INTO Customers VALUES (8, 'Neha', 'Kapoor', 'neha.kapoor@example.com', 'Kolkata', '9334455667');
INSERT INTO Customers VALUES (9, 'Suresh', 'Yadav', 'suresh.yadav@example.com', 'Lucknow', '9223344556');
INSERT INTO Customers VALUES (10, 'Meena', 'Joshi', 'meena.joshi@example.com', 'Jaipur', '9110099887');



alter procedure Sp_GetCustomers

as begin
DECLARE @lastid INT;
    DECLARE @batch INT;
    DECLARE @maxid INT;

    SET @lastid = (SELECT lastid FROM counter);
    SET @batch  = (SELECT batch FROM counter);

    ;WITH BatchCustomers AS (
        SELECT TOP (@batch) *
        FROM Customers
        WHERE CustomerID > @lastid
        ORDER BY CustomerID
    )
    SELECT * 
    FROM BatchCustomers
    FOR JSON PATH;
 
end 
 EXEC Sp_GetCustomers


select * from Customers
select * from counter



CREATE TABLE CustomerCheck(
    CustomerID INT PRIMARY KEY,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100),
    City VARCHAR(50),
    Phone VARCHAR(15)
);

select * from CustomerCheck
truncate table customercheck

create procedure Sp_InsertCounsumer
@json nvarchar(max)
as begin
Insert into CustomerCheck 
select * from openjson(@json)
with
(
    CustomerID int,
    FirstName varchar(30),
    LastName varchar(30),
    Email varchar(30),
    City varchar(40),
    Phone varchar(30)
)
end
select * from CustomerCheck
create table counter(sn  int, lastid int,batch int)
truncate table counter
insert into counter values(1,0,2)
select * from counter

create procedure UpdateCounter
@lastid int 
 as begin
update  counter set lastid=@lastid where sn=1;
end