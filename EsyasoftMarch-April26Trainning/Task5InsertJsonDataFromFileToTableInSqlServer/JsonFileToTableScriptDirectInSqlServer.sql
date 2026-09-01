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





