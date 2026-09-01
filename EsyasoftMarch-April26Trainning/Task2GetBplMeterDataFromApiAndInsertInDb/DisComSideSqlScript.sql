
use distibutionData

select * from ThreePhaseMeterLSData
select * from SinglePhaseMeterLSData
select * from MeterDataSyncCounter
truncate table MeterDataSyncCounter
truncate table ThreePhaseMeterLSData
truncate table SinglePhaseMeterLSData


INSERT INTO MeterDataSyncCounter
(MeterType, BLPYear, Lastid, FetchCount,totalcount, entry_ts,CounterUpdateTime)
VALUES
('sp','2025',1,10,0,GETDATE(),getdate()),
('sp','2026',1,10,0,GETDATE(),getdate()),
('tp','2025',1,10,0,GETDATE(),getdate()),
('tp','2026',1,10,0,GETDATE(),getdate());


drop table ThreePhaseMeterLSData

drop table SinglePhaseMeterLSData


CREATE TABLE ThreePhaseMeterLSData (
    Id INT ,
    MSN VARCHAR(50),
    TS DATETIME,
    WH_Imp DECIMAL(18,2),
    Vah_Imp DECIMAL(18,2),
    WH_Exp DECIMAL(18,2),
    Vah_Exp DECIMAL(18,2),
    VR DECIMAL(18,2),
    VY DECIMAL(18,2),
    VB DECIMAL(18,2),
    IR DECIMAL(18,2),
    IY DECIMAL(18,2),
    IB DECIMAL(18,2)
);

CREATE TABLE SinglePhaseMeterLSData (
    Id INT ,
    MSN VARCHAR(50),
    TS DATETIME,
    WH_Imp DECIMAL(18,2),
    Vah_Imp DECIMAL(18,2),
    WH_Exp DECIMAL(18,2),
    Vah_Exp DECIMAL(18,2),
    V DECIMAL(18,2),
    C DECIMAL(18,2)
);


drop table MeterDataSyncCounter

CREATE TABLE MeterDataSyncCounter (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MeterType VARCHAR(10),      -- sp / tp
    BLPYear VARCHAR(10),        -- 2025 / 2026
    Lastid INT,
    FetchCount INT,
    TotalCount int,
    entry_ts datetime,
    CounterUpdateTime DATETIME
);

INSERT INTO MeterDataSyncCounter
(MeterType, BLPYear, Lastid, FetchCount,totalcount, entry_ts,CounterUpdateTime)
VALUES
('sp','2025',1,10,0,GETDATE(),getdate()),
('sp','2026',1,10,0,GETDATE(),getdate()),
('tp','2025',1,10,0,GETDATE(),getdate()),
('tp','2026',1,10,0,GETDATE(),getdate());

select * from MeterDataSyncCounter
truncate table MeterDataSyncCounter


drop procedure getmetercounter


CREATE PROCEDURE GetMeterCounter
(
    @MeterType VARCHAR(10),
    @BLPYear VARCHAR(10)
)
AS
BEGIN

    SELECT 
        Lastid,
        FetchCount
    FROM MeterDataSyncCounter
    WHERE MeterType = @MeterType
      AND BLPYear = @BLPYear
END




ALTER PROCEDURE InsertApiResponse
    @JsonData NVARCHAR(MAX),
    @MeterType VARCHAR(10),
    @BLPYear VARCHAR(10)
AS
BEGIN
    DECLARE @MaxId INT;

    BEGIN TRANSACTION;

    BEGIN TRY

        IF @MeterType = 'sp'
        BEGIN
            INSERT INTO SinglePhaseMeterLSData
            (
                Id, MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp, V, C
            )
            SELECT
                Id, MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp, V, C
            FROM OPENJSON(@JsonData)
            WITH
            (
                Id INT,
                MSN VARCHAR(50),
                TS DATETIME,
                WH_Imp DECIMAL(18,2),
                Vah_Imp DECIMAL(18,2),
                WH_Exp DECIMAL(18,2),
                Vah_Exp DECIMAL(18,2),
                V DECIMAL(18,2),
                C DECIMAL(18,2)
            );

            SELECT @MaxId = MAX(Id) FROM SinglePhaseMeterLSData;
        END
        ELSE
        BEGIN
            INSERT INTO ThreePhaseMeterLSData
            (
                Id, MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp,
                VR, VY, VB, IR, IY, IB
            )
            SELECT
                Id, MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp,
                VR, VY, VB, IR, IY, IB
            FROM OPENJSON(@JsonData)
            WITH
            (
                Id INT,
                MSN VARCHAR(50),
                TS DATETIME,
                WH_Imp DECIMAL(18,2),
                Vah_Imp DECIMAL(18,2),
                WH_Exp DECIMAL(18,2),
                Vah_Exp DECIMAL(18,2),
                VR DECIMAL(18,2),
                VY DECIMAL(18,2),
                VB DECIMAL(18,2),
                IR DECIMAL(18,2),
                IY DECIMAL(18,2),
                IB DECIMAL(18,2)
            );

            SELECT @MaxId = MAX(Id) FROM ThreePhaseMeterLSData;
        END

        UPDATE MeterDataSyncCounter
        SET
            Lastid = @MaxId + 1,
            TotalCount = @MaxId,
            CounterUpdateTime = GETDATE()
        WHERE MeterType = @MeterType
        AND BLPYear = @BLPYear;

        SELECT @MaxId;

        COMMIT TRANSACTION;

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
