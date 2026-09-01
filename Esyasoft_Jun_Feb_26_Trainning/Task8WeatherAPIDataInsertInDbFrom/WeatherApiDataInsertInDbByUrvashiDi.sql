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