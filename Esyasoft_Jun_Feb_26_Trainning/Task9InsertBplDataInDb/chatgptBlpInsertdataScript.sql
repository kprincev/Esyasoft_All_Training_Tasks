create table MeterIntervalData
(
  Meter_Id        VARCHAR(50),
  IntervalDateTime DATETIME,
  Avg_Voltage_V   DECIMAL(18,5),
  BlkEngy_kWh     DECIMAL(18,5),
  BlkEngy_kVAh    DECIMAL(18,5),
  Avg_Current_A   DECIMAL(18,5)
)
;

select * from MeterIntervalData
truncate table meterintervaldata


CREATE TYPE MeterIntervalType AS TABLE
(
    Meter_Id           VARCHAR(50),
    IntervalDateTime   DATETIME,
    Avg_Voltage_V      DECIMAL(18,5),
    BlkEngy_kWh        DECIMAL(18,5),
    BlkEngy_kVAh       DECIMAL(18,5),
    Avg_Current_A      DECIMAL(18,5)
);
select * from meterintervaltype

select * from MeterReading
drop table MeterReading 
