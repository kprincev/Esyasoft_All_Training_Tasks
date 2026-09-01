--OLE Automation Stored Procedures

DECLARE @Object INT;
DECLARE @hr INT;
DECLARE @errMsg VARCHAR(500);
DECLARE @Status INT;

DECLARE @username VARCHAR(50) = 'prince';
DECLARE @password VARCHAR(50) = '12345';
DECLARE @auth VARCHAR(200) = @username + ':' + @password;
DECLARE @base64 VARCHAR(8000);

SELECT @base64 = CAST('' AS XML).value(
    'xs:base64Binary(sql:column("bin"))', 'VARCHAR(8000)'
) FROM (SELECT CAST(@auth AS VARBINARY(MAX)) AS bin) AS t;

DECLARE @AuthHeader VARCHAR(8000) = 'Basic ' + @base64;

DECLARE @Body NVARCHAR(MAX) = N'{
  "start": 1,
  "count": 10,
  "meter_type": "sp",
  "blpyear": "2025"
}';

EXEC @hr = sp_OACreate 'MSXML2.ServerXMLHTTP', @Object OUT;
IF @hr <> 0 GOTO ErrorHandler;

EXEC @hr = sp_OAMethod @Object, 'open', NULL, 'POST', 'https://localhost:7129/api/Meter/fetchdata', false;--C++ ke virtual function jesa hota hai
IF @hr <> 0 GOTO ErrorHandler;

EXEC sp_OAMethod @Object, 'setOption', NULL, 2, 13056; -- Ignore SSL errors --- Option 2 ? SSL error handling ke liye  - Value 13056 ? sab common SSL certificate errors ignore kar do


EXEC sp_OAMethod @Object, 'setRequestHeader', NULL, 'Authorization', @AuthHeader;
EXEC sp_OAMethod @Object, 'setRequestHeader', NULL, 'Content-Type', 'application/json';
EXEC sp_OAMethod @Object, 'setRequestHeader', NULL, 'Accept', 'application/json';

EXEC @hr = sp_OAMethod @Object, 'send', NULL, @Body;
IF @hr <> 0 GOTO ErrorHandler;

EXEC sp_OAGetProperty @Object, 'status', @Status OUTPUT;

-- Hum ek temporary variable bana rahe hai usko as a table use karenge kyunki sp_OAGetProperty 
-- NVARCHAR(MAX) variables mein data kabhi-kabhi seedhe nahi bhejta.
DECLARE @ResponseTable TABLE (RawResponse NVARCHAR(MAX));

INSERT INTO @ResponseTable (RawResponse)
EXEC sp_OAGetProperty @Object, 'responseText';

SELECT 
    @Status AS StatusCode,
    RawResponse AS ApiResponse
FROM @ResponseTable;

-- Cleanup
EXEC sp_OADestroy @Object;
RETURN;

ErrorHandler:
    EXEC sp_OAGetErrorInfo @Object, NULL, @errMsg OUTPUT;
    SELECT 'Error' AS Step, @errMsg AS ErrorMessage;
    IF @Object IS NOT NULL EXEC sp_OADestroy @Object;