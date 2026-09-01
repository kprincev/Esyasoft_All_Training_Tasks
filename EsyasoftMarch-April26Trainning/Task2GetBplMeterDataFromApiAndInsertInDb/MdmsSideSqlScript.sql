use mdmsdataserver


INSERT INTO MDMS_2026_SinglePhase 
    (MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp, V, C)
VALUES 
    ('MSN12345', GETDATE(), 120.50, 80.75, 10.25, 5.50, 230.00, 5.20);
INSERT INTO MDMS_2026_SinglePhase 
    (MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp, V, C)
VALUES 
    ('MSN12345', GETDATE(), 120.50, 80.75, 10.25, 5.50, 230.00, 5.20);

INSERT INTO MDMS_2026_ThreePhase 
    (MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp, VR, VY, VB, IR, IY, IB)
VALUES 
    ('MSN12345', '2026-03-19 10:45:00', 120.50, 80.30, 15.20, 10.10, 
     230.00, 231.50, 229.80, 5.20, 5.10, 5.30);


DECLARE @i INT = 1;

WHILE @i <= 500
BEGIN
    INSERT INTO MDMS_2025_SinglePhase
    (
        MSN,
        TS,
        WH_Imp,
        Vah_Imp,
        WH_Exp,
        Vah_Exp,
        V,
        C
    )
    VALUES
    (
        'SP2' + CAST(1000 + @i AS VARCHAR),
        DATEADD(MINUTE, (@i-1)*15, '2025-01-01 00:00:00'),
        120 + (@i * 0.5),
        121 + (@i * 0.5),
        0,
        0,
        230 + (@i % 5),
        5 + ((@i % 3) * 0.1)
    );

    SET @i = @i + 1;
END


DECLARE @i INT = 1;

WHILE @i <= 500
BEGIN
    INSERT INTO MDMS_2025_ThreePhase
    (
        
        MSN,
        TS,
        WH_Imp,
        Vah_Imp,
        WH_Exp,
        Vah_Exp,
        VR,
        VY,
        VB,
        IR,
        IY,
        IB
    )
    VALUES
    (
       
        'TP2' + CAST(1000 + @i AS VARCHAR),
        DATEADD(MINUTE, (@i-1)*15, '2025-01-01 00:00:00'),
        250 + (@i * 0.7),
        251 + (@i * 0.7),
        0,
        0,
        240 + (@i % 4),
        241 + (@i % 4),
        239 + (@i % 4),
        1 + ((@i % 5) * 0.1),
        1 + ((@i % 4) * 0.1),
        1 + ((@i % 3) * 0.1)
    );

    SET @i = @i + 1;
END



select * from MDMS_2026_SinglePhase
select * from MDMS_2026_ThreePhase
select * from MDMS_2025_SinglePhase
select * from MDMS_2025_ThreePhase

create database MdmsDataServer
create database distibutionData
use distibutionData
use mdmsdataserver

CREATE TABLE MDMS_2026_SinglePhase (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MSN VARCHAR(50),
    TS DATETIME,
    WH_Imp DECIMAL(18,2),
    Vah_Imp DECIMAL(18,2),
    WH_Exp DECIMAL(18,2),
    Vah_Exp DECIMAL(18,2),
    V DECIMAL(18,2),
    C DECIMAL(18,2)
);

CREATE TABLE MDMS_2026_ThreePhase (
    Id INT IDENTITY(1,1) PRIMARY KEY,
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


CREATE TABLE MDMS_2025_SinglePhase (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    MSN VARCHAR(50),
    TS DATETIME,
    WH_Imp DECIMAL(18,2),
    Vah_Imp DECIMAL(18,2),
    WH_Exp DECIMAL(18,2),
    Vah_Exp DECIMAL(18,2),
    V DECIMAL(18,2),
    C DECIMAL(18,2)
);

CREATE TABLE MDMS_2025_ThreePhase (
    Id INT IDENTITY(1,1) PRIMARY KEY,
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
INSERT INTO MDMS_2026_ThreePhase 
    (MSN, TS, WH_Imp, Vah_Imp, WH_Exp, Vah_Exp, VR, VY, VB, IR, IY, IB)
VALUES 
    ('MSN12345', '2026-03-19 10:45:00', 120.50, 80.30, 15.20, 10.10, 
     230.00, 231.50, 229.80, 5.20, 5.10, 5.30);



     alter PROCEDURE GetMeterData
    @json nvarchar(max)
    
AS
BEGIN
 
  declare  @Start INT,
    @Count INT,
    @MeterType VARCHAR(10),
    @BLPYear VARCHAR(10)

    set @Start=(select * from openjson(@json) with (Start int '$.start'))
    set @Count=(select * from openjson(@json) with (Countt int '$.count'))
    set @MeterType=(select * from openjson(@json) with (metertype varchar(10) '$.meter_type'))
    set @BLPYear =(select * from openjson(@json) with (blpyear varchar(10) '$.blpyear'))
   

    IF @MeterType = 'sp' AND @BLPYear = '2026'
    BEGIN
        SELECT TOP (@Count) *
        FROM MDMS_2026_SinglePhase
        WHERE Id >= @Start
        ORDER BY Id
    END

    ELSE IF @MeterType = 'tp' AND @BLPYear = '2026'
    BEGIN
        SELECT TOP (@Count) *
        FROM MDMS_2026_ThreePhase
        WHERE Id >= @Start
        ORDER BY Id
    END

    ELSE IF @MeterType = 'sp' AND @BLPYear = '2025'
    BEGIN
        SELECT TOP (@Count) *
        FROM MDMS_2025_SinglePhase
        WHERE Id >= @Start
        ORDER BY Id
    END

    ELSE IF @MeterType = 'tp' AND @BLPYear = '2025'
    BEGIN
        SELECT TOP (@Count) *
        FROM MDMS_2025_ThreePhase
        WHERE Id >= @Start
        ORDER BY Id
    END

END
