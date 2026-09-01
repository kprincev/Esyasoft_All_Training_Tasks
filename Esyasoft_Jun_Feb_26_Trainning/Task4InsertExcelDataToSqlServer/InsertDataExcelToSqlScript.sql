create database SimpleTaskDb;
use Simpletaskdb;
create table Student(Stu_id int primary key identity(1,1) ,Stu_Name varchar(30),Gender varchar(20),
Age int ,Email varchar(50),Addresh varchar(50));

select * from student
create procedure InStuData
(
	@Stu_Name varchar(30),
	@Gender varchar(20),
	@Age int,
	@Email varchar(50),
	@Addresh varchar(50)
)
as begin
insert into Student values(@Stu_Name,@Gender,@Age,@Email,@Addresh);
end;

exec InStuData "Prince","Male",20,"princevermaji@gmail.com","Chand";

drop procedure Insertbook
drop table book
create table  book(book_id int primary key identity (1,1) ,book_name varchar(50))
create procedure InsertBook
(
@book_name varchar(50)

)
as begin 
insert into book values(@book_name);
end;

select * from book 
exec InsertBook "maths"
truncate table book;

CREATE TABLE MeterReading
(
    ReadingId INT IDENTITY(1,1) PRIMARY KEY,
    MeterId VARCHAR(50),
    ReadingDate DATE,
    TotalAmount DECIMAL(10,2),
    BreakdownJson NVARCHAR(MAX)
);




DECLARE @json NVARCHAR(MAX) = '
{
  "meterId": "MTR001",
  "readingDate": "2026-01-26",
  "totalAmount": 350.75,
  "breakdown": [
    { "parameter": "Avg_Voltage", "value": 230.5 },
    { "parameter": "Avg_Current", "value": 5.2 },
    { "parameter": "Energy_kWh", "value": 145.0 }
  ]
}';

INSERT INTO MeterReading
(
    MeterId,
    ReadingDate,
    TotalAmount,
    BreakdownJson
)
VALUES
(
    JSON_VALUE(@json, '$.meterId'),
    JSON_VALUE(@json, '$.readingDate'),
    JSON_VALUE(@json, '$.totalAmount'),
    JSON_QUERY(@json, '$.breakdown')
);

drop table MeterReading
select * from MeterReading
SELECT
    MeterId,
    JSON_VALUE(value,'$.parameter') AS Parameter,
    JSON_VALUE(value,'$.value') AS Value
FROM MeterReading
CROSS APPLY OPENJSON(BreakdownJson);