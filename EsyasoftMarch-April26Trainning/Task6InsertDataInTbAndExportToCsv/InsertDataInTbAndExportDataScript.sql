create database MarchTraining
use MarchTraining

DROP TABLE EMP
create table emp(id int PRIMARY KEY IDENTITY(1,1) ,name varchar(30),email nvarchar(30),phone varchar(10) ,salary int ,entry_ts datetime );

drop procedure InsertDataInEmp
CREATE PROCEDURE InsertDataInEmp(
@name varchar(30),@email nvarchar(30),@phone varchar(10),@salary int )
as begin
insert into emp(name,email,phone,salary,entry_ts)values(@name,@email,@phone,@salary,getdate());
end


create procedure extrectdata
as begin
select * from emp
end
select * from emp

alter table emp  alter column phone varchar(10)
