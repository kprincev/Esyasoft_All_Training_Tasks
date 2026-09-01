=START===================================================TASK SNAPSHOT YNO YNR =========================================================================================================================================================================================
use MarchTraining

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