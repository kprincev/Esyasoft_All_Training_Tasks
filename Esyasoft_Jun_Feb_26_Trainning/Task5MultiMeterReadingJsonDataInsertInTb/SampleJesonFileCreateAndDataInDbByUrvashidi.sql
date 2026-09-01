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