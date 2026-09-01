
use MarchTraining

create table WorkBookData (id int primary key identity(1,1),workbook_name varchar(30),entry_time datetime, jsondata nvarchar(max))

select * from workbookdata
truncate table workbookdata
alter procedure InsertWorkBookData
(
    @json nvarchar(max),
    @filename varchar(30)
)
as begin
  --    WAITFOR DELAY '00:00:05'; -- 5 seconds 
    insert into workbookdata(workbook_name,entry_time,jsondata)values(@filename,getdate(),@json);
end
