use simpletaskdb
CREATE TABLE SinglePhaseMeterData
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

    MeterSerialNumber VARCHAR(50) NOT NULL,

    TS DATETIME2(0) NOT NULL,   -- Date + Time (interval timestamp)

    IntervalMinutes INT NOT NULL, 
    -- 15 ya 30 (isse pata chalega ye kis type ka meter hai)

    Current_A DECIMAL(10,3) NULL,   -- Ampere
    Voltage_V DECIMAL(10,3) NULL,   -- Volt

    KW DECIMAL(12,4) NULL,
    KVA DECIMAL(12,4) NULL,
    KVARh DECIMAL(12,4) NULL,

    CreatedOn DATETIME2(0) DEFAULT GETDATE()
);


CREATE TABLE ConsumerMaster
(
    ConsumerId BIGINT IDENTITY(1,1) PRIMARY KEY,
    ConsumerNumber VARCHAR(50) NOT NULL,
    ConsumerName VARCHAR(150) NOT NULL,
    MobileNumber VARCHAR(15),
    Email VARCHAR(150),
    AddressLine1 VARCHAR(250),
    City VARCHAR(100),
    State VARCHAR(100),
    Pincode VARCHAR(10),
    MeterSerialNumber VARCHAR(50) NOT NULL,
    MeterType VARCHAR(20) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedOn DATETIME2(0) DEFAULT GETDATE()
);




INSERT INTO ConsumerMaster
(ConsumerNumber, ConsumerName, MobileNumber, City, State, MeterSerialNumber, MeterType)
VALUES
-- 15 MIN INTERVAL
('CNS-001','Ramesh Kumar','9876543210','Delhi','Delhi','MTR-15-1','SINGLE_PHASE'),
('CNS-002','Suresh Verma','9876543211','Noida','UP','MTR-15-2','SINGLE_PHASE'),
('CNS-003','Amitttt Sharma','9876543212','Ghaziabad','UP','MTR-15-3','SINGLE_PHASE'),
('CNS-004','Amgsfdgit Sharma','9876543212','Ghaziabad','UP','MTR-15-4','SINGLE_PHASE'),
('CNS-005','Amidadst Sharma','9876543212','Ghaziabad','UP','MTR-15-5','SINGLE_PHASE'),

('CNS-006','Rohiddt Singh','9876543220','Faridabad','Haryana','MTR-30-1','SINGLE_PHASE'),
('CNS-007','Anil Gupta','9876543221','Gurgaon','Haryana','MTR-30-2','SINGLE_PHASE'),
('CNS-008','Rohit Singh','9876543220','Faridabad','Haryana','MTR-30-3','SINGLE_PHASE'),
('CNS-009','Rohdadfit Singh','9876543220','Faridabad','Haryana','MTR-30-4','SINGLE_PHASE'),
('CNS-010','Rohigfdgt Singh','9876543220','Faridabad','Haryana','MTR-30-5','SINGLE_PHASE'),



('CNS-011','Vikas Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-1','THREE_PHASE'),
('CNS-012','Vikdadfas Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-2','THREE_PHASE'),
('CNS-013','Vikcsafdas Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-3','THREE_PHASE'),



('CNS-014','Vikaseafa Mehta','9876543213','Jaipur','Rajasthan','TP-MTR-15-4','THREE_PHASE'),
('CNS-015','Neeraj Jain','9876543214','Kota','Rajasthan','TP-MTR-15-5','THREE_PHASE'),

-- 30 MIN INTERVAL


('CNS-016','Pankadasgj Agarwal','9876543222','Indore','MP','TP-MTR-30-1','THREE_PHASE'),
('CNS-017','Pankcsafaj Agarwal','9876543222','Indore','MP','TP-MTR-30-2','THREE_PHASE'),
('CNS-018','Pankaj Agarwal','9876543222','Indore','MP','TP-MTR-30-3','THREE_PHASE'),
('CNS-019','Sanjay Patel','9876543223','Ahmedabad','Gujarat','TP-MTR-30-4','THREE_PHASE'),
('CNS-020','Mahesh Shah','9876543224','Surat','Gujarat','TP-MTR-30-5','THREE_PHASE');





CREATE TABLE MeterHourlyTarget
(
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,

     -- CNS-001 etc.

    MSN VARCHAR(50) NOT NULL,              -- Meter Serial Number
     ConsumerNumber VARCHAR(50) NOT NULL,
    MeterType VARCHAR(20) NOT NULL,        -- SINGLE_PHASE / THREE_PHASE

    TS DATETIME2(0) NOT NULL,              -- Hour start (Date + Hour)

    -- AVERAGE VALUES (hourly)
    Avg_V      DECIMAL(12,4) NULL,
    Avg_I      DECIMAL(12,4) NULL,
    Avg_KW     DECIMAL(12,4) NULL,
    Avg_KVA    DECIMAL(12,4) NULL,
    Avg_KVARh  DECIMAL(12,4) NULL,

    -- SUM VALUES (hourly)
    Sum_V      DECIMAL(14,4) NULL,
    Sum_I      DECIMAL(14,4) NULL,
    Sum_KW     DECIMAL(14,4) NULL,
    Sum_KVA    DECIMAL(14,4) NULL,
    Sum_KVARh  DECIMAL(14,4) NULL,

    CreatedOnDate DATETIME2(0) DEFAULT GETDATE()
);

drop table meterprocessingcounter
CREATE TABLE MeterProcessingCounter
(
    MSN VARCHAR(50) PRIMARY KEY,
    LastProcessedTS DATETIME2(0) NOT NULL
);



drop procedure sp_insertmeterhourlytarget
CREATE PROCEDURE SP_InsertMeterHourlyTarget
(
    @ConsumerNumber VARCHAR(50),
    @MSN VARCHAR(50),
    @MeterType VARCHAR(20),
    @TS DATETIME2,

    @Avg_V DECIMAL(12,4),
    @Avg_I DECIMAL(12,4),
    @Avg_KW DECIMAL(12,4),
    @Avg_KVA DECIMAL(12,4),
    @Avg_KVARh DECIMAL(12,4),

    @Sum_V DECIMAL(14,4),
    @Sum_I DECIMAL(14,4),
    @Sum_KW DECIMAL(14,4),
    @Sum_KVA DECIMAL(14,4),
    @Sum_KVARh DECIMAL(14,4)
)
AS
BEGIN
    IF NOT EXISTS
    (
        SELECT 1 FROM MeterHourlyTarget
        WHERE MSN=@MSN AND TS=@TS
    )
    BEGIN
        INSERT INTO MeterHourlyTarget
        (ConsumerNumber,MSN,MeterType,TS,
         Avg_V,Avg_I,Avg_KW,Avg_KVA,Avg_KVARh,
         Sum_V,Sum_I,Sum_KW,Sum_KVA,Sum_KVARh)
        VALUES
        (@ConsumerNumber,@MSN,@MeterType,@TS,
         @Avg_V,@Avg_I,@Avg_KW,@Avg_KVA,@Avg_KVARh,
         @Sum_V,@Sum_I,@Sum_KW,@Sum_KVA,@Sum_KVARh);
    END
END

use simpletaskdb
truncate table MeterHourlyTarget
select * from ConsumerMaster
select * from SinglePhaseMeterData
select * from ThreePhaseMeterData
select * from MeterHourlyTarget order by msn
select * from MeterProcessingCounter
truncate table MeterProcessingCounter
select * from C
SELECT MIN(TS), MAX(TS)
FROM SinglePhaseMeterData
WHERE MeterSerialNumber = 'MTR-15-1';


INSERT INTO MeterProcessingCounter (MSN, LastProcessedTS)
SELECT MeterSerialNumber, '1900-01-01'
FROM ConsumerMaster;
UPDATE MeterProcessingCounter
SET LastProcessedTS = '2026-02-09 00:00:00';

SELECT COUNT(*) FROM MeterHourlyTarget WHERE MSN='MTR-15-1'


update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-1' WHERE meterserialnumber='TP-MTR-1'

update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-2' WHERE meterserialnumber='TP-MTR-2'
update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-3' WHERE meterserialnumber='TP-MTR-3'
update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-4' WHERE meterserialnumber='TP-MTR-4'
update ThreePhaseMeterData set meterserialnumber='TP-MTR-15-5' WHERE meterserialnumber='TP-MTR-5'


SELECT COUNT(*) FROM MeterHourlyTarget WHERE MSN='MTR-15-2'


SELECT DISTINCT CAST(TS AS DATE)
FROM SinglePhaseMeterData
WHERE MeterSerialNumber='MTR-15-1'
ORDER BY 1;




SELECT MSN, LastProcessedTS FROM MeterProcessingCounter

CREATE PROCEDURE SP_GetMeterProcessingCounter
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MSN, LastProcessedTS
    FROM MeterProcessingCounter
    ORDER BY MSN;
END

select * from MeterProcessingCounter
CREATE PROCEDURE SP_GetSinglePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MeterSerialNumber,
        TS,
        Voltage_V,
        Current_A,
        KW,
        KVA,
        KVARh
    FROM SinglePhaseMeterData
    WHERE MeterSerialNumber = @MSN
      AND TS >= @StartDate
      AND TS < @EndDate
    ORDER BY TS;
END



CREATE PROCEDURE SP_GetThreePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        MeterSerialNumber,
        TS,
        (V_R + V_Y + V_B) / 3.0 AS AvgVoltage,
        (I_R + I_Y + I_B) / 3.0 AS AvgCurrent,
        KW,
        KVA,
        KVARh
    FROM ThreePhaseMeterData
    WHERE MeterSerialNumber = @MSN
      AND TS >= @StartDate
      AND TS < @EndDate
    ORDER BY TS;
END


CREATE PROCEDURE SP_UpdateMeterProcessingCounter
(
    @MSN VARCHAR(50),
    @TS  DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE MeterProcessingCounter
    SET LastProcessedTS = @TS
    WHERE MSN = @MSN;
END


ALTER TABLE table_name
DROP COLUMN consumer_number;



drop procedure SP_GetSinglePhaseRawData
drop procedure SP_GetThreePhaseRawData



ALTER PROCEDURE SP_GetThreePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ConsumerNumber,   -- 🔥 Added
        t.MeterSerialNumber,
        t.TS,
        (V_R + V_Y + V_B) / 3.0 AS AvgVoltage,
        (I_R + I_Y + I_B) / 3.0 AS AvgCurrent,
        t.KW,
        t.KVA,
        t.KVARh
    FROM ThreePhaseMeterData t
    INNER JOIN ConsumerMaster c
        ON c.MeterSerialNumber = t.MeterSerialNumber
    WHERE t.MeterSerialNumber = @MSN
      AND t.TS >= @StartDate
      AND t.TS < @EndDate
    ORDER BY t.TS;
END

ALTER PROCEDURE SP_GetSinglePhaseRawData
(
    @MSN VARCHAR(50),
    @StartDate DATETIME2(0),
    @EndDate DATETIME2(0)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ConsumerNumber,   -- 🔥 Added
        s.MeterSerialNumber,
        s.TS,
        s.Voltage_V,
        s.Current_A,
        s.KW,
        s.KVA,
        s.KVARh
    FROM SinglePhaseMeterData s
    INNER JOIN ConsumerMaster c
        ON c.MeterSerialNumber = s.MeterSerialNumber
    WHERE s.MeterSerialNumber = @MSN
      AND s.TS >= @StartDate
      AND s.TS < @EndDate
    ORDER BY s.TS;
END
