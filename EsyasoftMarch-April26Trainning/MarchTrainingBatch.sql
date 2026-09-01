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






=========================================================================================================================

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
===================================================================


use distibutionData
use mdmsdataserver

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

insert into MDMS_2026_ThreePhase 

select * from ThreePhaseMeterLSData
select * from SinglePhaseMeterLSData
select * from MeterDataSyncCounter

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


select * from MDMS_2026_SinglePhase
select * from MDMS_2026_ThreePhase
select * from MDMS_2025_SinglePhase
select * from MDMS_2025_ThreePhase



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




======================


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




===========================================================task jeson =============================================================================

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




=START===================================================TASK SNAPSHOT YNO YNR =========================================================================================================================================================================================

truncate table yno
truncate table ynr
truncate table destination
select * from destination
select * from source
select * from yno
select * from ynr
truncate table source
select * from master 
select * from BatchCounter
truncate table BatchCounter 
INSERT INTO BatchCounter (LastProcessedTbid,batchsize) VALUES (0,20);
========================================================== experiment window======






===================================================================================
============================================tables ==========

create table Yno(tbid int ,event_id int,event_mdm_id int,ts datetime,V decimal(18,4),i decimal(18,4) ,msnid int);
create table Ynr(tbid int ,event_id int,event_mdm_id int,ts datetime,V decimal(18,4),i decimal(18,4) ,msnid int);


drop table destination 
create table destination(tb_ref_id int primary key identity(1,1),tbid_Occur int ,event_id_occur int,event_mdm_id int,
ts_occur datetime,v_occur decimal(18,4),i_occur decimal(18,4)  ,
tbid_Restor int ,event_id_Restor int,
ts_Restor datetime,v_Restor decimal(18,4),i_Restor decimal(18,4) ,msnid int )

create table source(tbid int primary key identity(1,1),event_id int,event_mdm_id int,ts datetime,V decimal(18,4),i decimal(18,4) ,msnid int);

create table master(eventid int, event_mdm_id int ,description varchar(30),active int,IsResterotion int );



CREATE TABLE BatchCounter (
    Id INT PRIMARY KEY IDENTITY(1,1),
    LastProcessedTbid INT DEFAULT 0,
     BatchSize int 
);
===============================================================
=============================================================tablesdata=================================

insert into source(event_id,event_mdm_id,ts,v,i,msnid)values(101,529,getdate(),42.4,43.2,111)
insert into source(event_id,event_mdm_id,ts,v,i,msnid)values(102,529,'2026-03-26 16:40:00',42.4,43.2,111)
insert into source(event_id,event_mdm_id,ts,v,i,msnid)values(101,529,'2026-03-26 16:50:00',42.4,43.2,111)
insert into source(event_id,event_mdm_id,ts,v,i,msnid)values(102,529,'2026-03-26 17:00:00',42.4,43.2,111)
insert into source(event_id,event_mdm_id,ts,v,i,msnid)values(55,520,'2026-03-26 16:42:00',42.4,43.2,111)
insert into source(event_id,event_mdm_id,ts,v,i,msnid) values
(101,529,'2026-03-26 10:00:00',40.5,41.2,101),
(102,529,'2026-03-26 10:10:00',41.0,42.1,101),
(101,529,'2026-03-26 10:20:00',39.8,40.5,102),
(102,529,'2026-03-26 10:30:00',42.2,43.0,102),


(55,520,'2026-03-26 10:50:00',45.0,44.5,104),
(101,529,'2026-03-26 11:00:00',41.5,42.8,104),
(102,529,'2026-03-26 11:10:00',40.0,41.3,105),

(55,520,'2026-03-26 11:30:00',46.2,45.8,106),

(101,529,'2026-03-26 11:40:00',42.0,43.1,106),
(102,529,'2026-03-26 11:50:00',41.7,42.6,107),

(55,520,'2026-03-26 12:10:00',47.1,46.0,108),
(101,529,'2026-03-26 12:20:00',40.8,41.9,108),

(102,529,'2026-03-26 12:30:00',42.5,43.3,109),

(55,520,'2026-03-26 12:50:00',48.0,47.2,110),
(101,529,'2026-03-26 13:00:00',41.2,42.0,110),
(102,529,'2026-03-26 13:10:00',42.9,43.7,111);


insert into master values(101,529,'power off Occurence',1,0),
(102,529,'power on restoration',1,1),(103,528,'low voltage',0,0),(55,520,'tempering',1,0);

===================================================================== procedure line by line ================


CREATE PROCEDURE GetMasterData
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        eventid,
        event_mdm_id,
        IsResterotion
    FROM master
    WHERE active = 1
END

========================
ALTER PROCEDURE GetSourceBatch
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LastId INT, @batchsize INT;

    SELECT 
        @LastId = LastProcessedTbid,
        @batchsize = BatchSize
    FROM BatchCounter;

  SELECT 
    tbid,
    event_id,
    event_mdm_id,
    ts,
    v,
    i,
    msnid
FROM (
    SELECT TOP(@batchsize)
        tbid,
        event_id,
        event_mdm_id,
        ts,
        v,
        i,
        msnid
    FROM source
    WHERE tbid > @LastId
    ORDER BY tbid
) t
ORDER BY ts, tbid;
END



===================================
ALTER PROCEDURE InsertDestinationJson
    @json NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO destination (
        tbid_Occur,event_id_occur,event_mdm_id ,ts_occur,v_occur,i_occur,
        tbid_Restor,event_id_Restor,ts_Restor,v_Restor,i_Restor,msnid
    )
    SELECT 
        o.tbid,
        o.event_id,
        o.event_mdm_id,
        o.ts,
        o.v,
        o.i,
        

        r.tbid,
        r.event_id,
        r.ts,
        r.v,
        r.i,
        r.msnid

    FROM OPENJSON(@json)
    WITH (
        occurJson NVARCHAR(MAX) '$.occur' AS JSON,
        restJson  NVARCHAR(MAX) '$.restore' AS JSON
    ) root

    CROSS APPLY OPENJSON(root.occurJson)
    WITH (
        tbid INT,
        event_id INT,
        event_mdm_id INT,
        ts DATETIME,
        v DECIMAL(18,4),
        i DECIMAL(18,4)
        
    ) o

    CROSS APPLY OPENJSON(root.restJson)
    WITH (
        tbid INT,
        event_id INT,
        ts DATETIME,
        v DECIMAL(18,4),
        i DECIMAL(18,4),
        msnid INT
    ) r
END

==========================================
CREATE PROCEDURE ProcessYnoYnrJson
    @ynoInsertJson NVARCHAR(MAX) = NULL,
    @ynrInsertJson NVARCHAR(MAX) = NULL,
    @ynoDeleteJson NVARCHAR(MAX) = NULL,
    @ynrDeleteJson NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

--
    --  INSERT YNO

    IF (@ynoInsertJson IS NOT NULL AND LEN(@ynoInsertJson) > 0)
    BEGIN
        INSERT INTO Yno (tbid, event_id, event_mdm_id, ts, v, i, msnid)
        SELECT 
            tbid,
            event_id,
            event_mdm_id,
            ts,
            v,
            i,
            msnid
        FROM OPENJSON(@ynoInsertJson)
        WITH (
            tbid INT,
            event_id INT,
            event_mdm_id INT,
            ts DATETIME,
            v DECIMAL(18,4),
            i DECIMAL(18,4),
            msnid INT
        );
    END
    -- INSERT YNR

    IF (@ynrInsertJson IS NOT NULL AND LEN(@ynrInsertJson) > 0)
    BEGIN
        INSERT INTO Ynr (tbid, event_id, event_mdm_id, ts, v, i, msnid)
        SELECT 
            tbid,
            event_id,
            event_mdm_id,
            ts,
            v,
            i,
            msnid
        FROM OPENJSON(@ynrInsertJson)
        WITH (
            tbid INT,
            event_id INT,
            event_mdm_id INT,
            ts DATETIME,
            v DECIMAL(18,4),
            i DECIMAL(18,4),
            msnid INT
        );
    END


    --  DELETE YNO
    IF (@ynoDeleteJson IS NOT NULL AND LEN(@ynoDeleteJson) > 0)
    BEGIN
        DELETE FROM Yno
        WHERE tbid IN (
            SELECT value FROM OPENJSON(@ynoDeleteJson)
        );
    END

 
    --  DELETE YNR

    IF (@ynrDeleteJson IS NOT NULL AND LEN(@ynrDeleteJson) > 0)
    BEGIN
        DELETE FROM Ynr
        WHERE tbid IN (
            SELECT value FROM OPENJSON(@ynrDeleteJson)
        );
    END

END






==========================================
create procedure updatebatchcounter
(
    @lastid int 
)
as begin
update batchcounter set LastProcessedTbid=@lastid 
end



=============================================
CREATE PROCEDURE GetYnoYnrFiltered
    @json NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    --  Yno
    SELECT 'Yno' AS SourceType, *
    FROM Yno
    WHERE EXISTS (
        SELECT 1
        FROM OPENJSON(@json)
        WITH (
            event_mdm_id INT,
            msnid INT
        ) j
        WHERE j.event_mdm_id = Yno.event_mdm_id
          AND j.msnid = Yno.msnid
    )

    UNION ALL

    --  Ynr
    SELECT 'Ynr' AS SourceType, *
    FROM Ynr
    WHERE EXISTS (
        SELECT 1
        FROM OPENJSON(@json)
        WITH (
            event_mdm_id INT,
            msnid INT
        ) j
        WHERE j.event_mdm_id = Ynr.event_mdm_id
          AND j.msnid = Ynr.msnid
    )
END




==========================================
CREATE PROCEDURE FindMatchingFromYno
    @mdm INT,
    @msn INT,
    @ts DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM Yno
    WHERE event_mdm_id = @mdm
      AND msnid = @msn
      AND ts > @ts
    ORDER BY ts;
END
========================================
CREATE PROCEDURE FindMatchingFromYnr
    @mdm INT,
    @msn INT,
    @ts DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1 *
    FROM Ynr
    WHERE event_mdm_id = @mdm
      AND msnid = @msn
      AND ts < @ts
    ORDER BY ts DESC;
END


=========================================


CREATE PROCEDURE InsertYnoJson
    @json NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Yno
    SELECT *
    FROM OPENJSON(@json)
    WITH (
        tbid INT,
        event_id INT,
        event_mdm_id INT,
        ts DATETIME,
        v FLOAT,
        i FLOAT,
        msnid INT
    )
END

===========================================

CREATE PROCEDURE InsertYnrJson
    @json NVARCHAR(MAX)
AS
BEGIN
    INSERT INTO Ynr
    SELECT *
    FROM OPENJSON(@json)
    WITH (
        tbid INT,
        event_id INT,
        event_mdm_id INT,
        ts DATETIME,
        v FLOAT,
        i FLOAT,
        msnid INT
    )
END

========================================
CREATE PROCEDURE DeleteYnoJson
    @json NVARCHAR(MAX)
AS
BEGIN
    DELETE FROM Yno
    WHERE tbid IN (
        SELECT value FROM OPENJSON(@json)
    )
END
=========================================
CREATE PROCEDURE DeleteYnrJson
    @json NVARCHAR(MAX)
AS
BEGIN
    DELETE FROM Ynr
    WHERE tbid IN (
        SELECT value FROM OPENJSON(@json)
    )
END

=========================================




===END==================================================================TASK SNAPSHOT YNO YNR================================================================================================================================================


create table hey(IdProject nvarchar(40),Name varchar(30), IdStructure nvarchar(40),
Structurename varchar(30),BaseStructure varchar(40),DatabaseSchema varchar(40),
IdProperty nvarchar(40),Propertename varchar(40),DataType int,Precision int ,Scale int ,IsNullable varchar(10),
ObjectName varchar(30),DefaultType  int ,DefaultValue nvarchar(20))

truncate table hey










=================================================part one ================================================================


declare @json nvarchar(max)
select @json=bulkcolumn from openrowset(bulk 'D:\Task\hey.json',single_clob) as j 

INSERT INTO hey (
    IdProject,
    Name,
    IdStructure,
    Structurename,
    BaseStructure,
    DatabaseSchema,
    IdProperty,
    Propertename,
    DataType,
    Precision,
    Scale,
    IsNullable,
    ObjectName,
    DefaultType,
    DefaultValue
)

SELECT 
    p.IdProject,
    p.Name,

    s.IdStructure,
    s.Name AS Structurename,
    s.BaseStructure,
    s.DatabaseSchema,

    pr.IdProperty,
    pr.Name AS Propertename,
    pr.DataType,
    pr.Precision,
    pr.Scale,

  
CASE 
    WHEN pr.IsNullable = 1 THEN 'true'
    ELSE 'false'
END AS IsNullable,

    pr.ObjectName,
    pr.DefaultType,
    pr.DefaultValue

FROM OPENJSON(@json)
WITH (
    IdProject NVARCHAR(50),
    Name NVARCHAR(100),
    structures NVARCHAR(MAX) AS JSON
) p

outer APPLY OPENJSON(p.structures)
WITH (
    IdStructure NVARCHAR(50),
    Name NVARCHAR(100),
    BaseStructure NVARCHAR(100),
    DatabaseSchema NVARCHAR(50),
    properties NVARCHAR(MAX) AS JSON
) s

outer APPLY OPENJSON(s.properties)
WITH (
    IdProperty NVARCHAR(50),
    Name NVARCHAR(100),
    DataType INT,
    Precision INT,
    Scale INT,
    IsNullable BIT,
    ObjectName NVARCHAR(100),
    DefaultType INT,
    DefaultValue NVARCHAR(100)
) pr;


select * from hey





























select 

SELECT * FROM HEY
truncate table hey

--update hey set defaultvalue=null where idproperty='618DC40B-4D04-4BF8-B1E6-12E13DDE86F4'
DECLARE @NEWJSON NVARCHAR(MAX)
SET @NEWJSON=
(SELECT 
    h.IdProject,
    h.Name,

    (
        SELECT 
            h2.IdStructure,
            h2.Structurename AS Name,
            h2.BaseStructure,
            h2.DatabaseSchema,

            (
                SELECT 
                    h3.IdProperty,
                    h3.IdStructure,
                    h3.Propertename AS Name,
                    h3.DataType,
                    h3.Precision,
                    h3.Scale,

                    
                    CASE 
                        WHEN h3.IsNullable = 'true' THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END AS IsNullable,

                    h3.ObjectName,
                    h3.DefaultType,
                    h3.DefaultValue

                FROM hey h3
                WHERE h3.IdStructure = h2.IdStructure

             FOR JSON PATH, INCLUDE_NULL_VALUES
            ) AS properties

        FROM hey h2
        WHERE h2.IdProject = h.IdProject

        GROUP BY 
            h2.IdStructure,
            h2.Structurename,
            h2.BaseStructure,
            h2.DatabaseSchema,
            h2.IdProject

       FOR JSON PATH, INCLUDE_NULL_VALUES
    ) AS structures

FROM hey h
GROUP BY h.IdProject, h.Name

FOR JSON PATH, INCLUDE_NULL_VALUES);

SELECT * FROM OPENJSON(@NEWJSON)

































