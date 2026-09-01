use simpletaskdb
CREATE TABLE Ls_Xml_Data
(
    MeterSerialNumber VARCHAR(50),
    IntervalTime DATETIME,

    RT_1_0_12_27_0_255_1800 DECIMAL(18,3),
    RT_1_0_1_7_0_255_1800 DECIMAL(18,3),
    RT_1_0_9_29_0_255_1800 DECIMAL(18,3),
    RT_1_0_2_29_0_255_1800 DECIMAL(18,3),
    RT_1_0_10_29_0_255_1800 DECIMAL(18,3),
    RT_1_0_11_27_0_255_1800 DECIMAL(18,3),
    RT_1_0_91_27_0_255_1800 DECIMAL(18,3),
    RT_0_0_96_10_1_255_1800 DECIMAL(18,3)
);

CREATE TABLE Ls_Xml_Data
(
    MeterSerialNumber VARCHAR(50),
    IntervalTime DATETIME,

    a DECIMAL(18,3),
    b DECIMAL(18,3),
    c DECIMAL(18,3),
    d DECIMAL(18,3),
    e DECIMAL(18,3),
    f DECIMAL(18,3),
    g DECIMAL(18,3),
    h DECIMAL(18,3)
);
ALTER TABLE Ls_Xml_Data
ADD CONSTRAINT UQ_LS_MeterSerial_IntervalTime
UNIQUE (MeterSerialNumber, IntervalTime);


drop table Ls_Xml_Data
drop procedure sp_insert_ls_fromjson
select * from ls_xml_data
truncate table ls_xml_data

CREATE PROCEDURE SP_Insert_LS_FromJson
(
    @JsonData NVARCHAR(MAX)
)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Ls_Xml_Data
    (
        MeterSerialNumber,
        IntervalTime,
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h
    )
    SELECT
        MeterSerialNumber,
        IntervalTime,
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h
        
    FROM OPENJSON(@JsonData)
    WITH
    (
        MeterSerialNumber VARCHAR(50),
        IntervalTime DATETIME,

        a DECIMAL(18,3),
        b DECIMAL(18,3),
        c DECIMAL(18,3),
        d DECIMAL(18,3),
        e DECIMAL(18,3),
        f DECIMAL(18,3),
        g DECIMAL(18,3),
        h DECIMAL(18,3)
    );
END;

