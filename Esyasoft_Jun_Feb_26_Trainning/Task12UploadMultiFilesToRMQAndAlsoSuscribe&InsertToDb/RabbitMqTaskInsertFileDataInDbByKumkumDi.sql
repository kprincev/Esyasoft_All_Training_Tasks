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



alter PROCEDURE usp_InsertMeterReadingJson
    @json NVARCHAR(MAX)
AS
BEGIN

    INSERT INTO MeterDataByRabbitMq
    (
        MeterId,
        ReadingDate,
        Voltage
    )
    SELECT
        MeterId,
        ReadingDate,
        Voltage
    FROM OPENJSON(@json)
    WITH
    (
        MeterId VARCHAR(50),
        ReadingDate DATETIME,
        Voltage DECIMAL(10,2)
    )

END