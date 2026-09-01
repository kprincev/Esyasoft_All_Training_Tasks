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