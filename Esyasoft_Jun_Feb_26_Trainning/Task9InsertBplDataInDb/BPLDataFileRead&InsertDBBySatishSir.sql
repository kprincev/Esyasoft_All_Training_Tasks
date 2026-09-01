use SimpleTaskdb

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



alter PROCEDURE InsertBPLJsonData
    @json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO BlpData
    (
        Meter_Id,
        FirstIntervalDateTime,
        [Avg_Voltage_(V)],
        [BlkEngy_I/F_(kWh)],
        [BlkEngy_I/F_(kVAh)],
        [Avg_Current_(A)]
    )
    SELECT
        Meter_Id,
        FirstIntervalDateTime,
        [Avg_Voltage_(V)],
        [BlkEngy_I/F_(kWh)],
        [BlkEngy_I/F_(kVAh)],
        [Avg_Current_(A)]
    FROM OPENJSON(@json)
    WITH
    (
       Meter_Id INT '$.Meter_Id',

        -- 🔥 THIS LINE IS IMPORTANT
        FirstIntervalDateTime DATETIME '$.IntervalDateTime',

        [Avg_Voltage_(V)] DECIMAL(10,5) '$.Avg_Voltage_V',
        [BlkEngy_I/F_(kWh)] DECIMAL(10,5) '$.BlkEngy_I_F_kWh',
        [BlkEngy_I/F_(kVAh)] DECIMAL(10,5) '$.BlkEngy_I_F_kVAh',
        [Avg_Current_(A)] DECIMAL(10,5) '$.Avg_Current_A'
    );
END



CREATE OR ALTER PROCEDURE InsertBPLJsonData
    @json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.BPLData
    (
        Meter_Id,
        FirstIntervalDateTime,
        [Avg_Voltage_(V)],
        [BlkEngy_I/F_(kWh)],
        [BlkEngy_I/F_(kVAh)],
        [Avg_Current_(A)]
    )
    SELECT
        Meter_Id,
        FirstIntervalDateTime,
        [Avg_Voltage_(V)],
        [BlkEngy_I/F_(kWh)],
        [BlkEngy_I/F_(kVAh)],
        [Avg_Current_(A)]
    FROM OPENJSON(@json)
    WITH
    (
        Meter_Id INT '$.Meter_Id',

        -- 🔥 THIS LINE IS IMPORTANT
        FirstIntervalDateTime DATETIME '$.IntervalDateTime',

        [Avg_Voltage_(V)] DECIMAL(10,5) '$.Avg_Voltage_V',
        [BlkEngy_I/F_(kWh)] DECIMAL(10,5) '$.BlkEngy_I_F_kWh',
        [BlkEngy_I/F_(kVAh)] DECIMAL(10,5) '$.BlkEngy_I_F_kVAh',
        [Avg_Current_(A)] DECIMAL(10,5) '$.Avg_Current_A'
    );
END