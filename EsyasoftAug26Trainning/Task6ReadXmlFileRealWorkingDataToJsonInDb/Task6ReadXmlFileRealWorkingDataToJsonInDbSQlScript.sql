use AugEsyasoftTranningTask
drop table IP_MeterData
Create Table IP_MeterData(sn int primary key identity(1,1),
jobn varchar(60),
entry_ts datetime default getdate(),
hes_id varchar(40),
mtr_typ varchar(40),
sample_id int ,
msn varchar(40),
ts datetime,
v decimal(10,3),
i_p int,
i_n int,
pf int,
freq decimal(10,3) ,
w_imp int ,
va_imp int,
wh_imp decimal(10,3),
vah_imp decimal(10,3),
md_w_imp decimal(10,3),
md_w_imp_ts datetime,
md_va_imp decimal(10,3),
md_va_imp_ts datetime,
pwr_on_dur int,
tamp_cnt int,
bill_cnt int,
prgm_cnt int,
wh_exp int,
vah_exp int ,
load_Imt_stat bit,
load_Imt_val int)

select * from IP_MeterData
truncate table IP_MeterData

alter procedure SP_InsertJsonIPData
@json nvarchar(max)
as begin
insert into IP_MeterData(jobn,msn,ts,v,i_p,i_n,pf,freq,w_imp,va_imp,wh_imp,vah_imp,md_w_imp,md_w_imp_ts,
md_va_imp,md_va_imp_ts,pwr_on_dur,tamp_cnt,bill_cnt,prgm_cnt,wh_exp,vah_exp,load_Imt_stat,load_Imt_val)
select jobn,msn,ts,v,i_p,i_n,pf,freq,w_imp,va_imp,wh_imp,vah_imp,md_w_imp,md_w_imp_ts,md_va_imp,md_va_imp_ts,pwr_on_dur,tamp_cnt,bill_cnt,prgm_cnt,wh_exp,vah_exp,load_Imt_stat,load_Imt_val from openjson(@json)
with 
(
	jobn varchar(60) '$.jobname',
	msn varchar(40) '$.meterserialno',
	ts datetime '$.timeStamp',
	v decimal(10,3) '$.Data_1_0_12_7_0_255_A2',
	i_p int '$.Data_1_0_11_7_0_255_A2',
	i_n int '$.Data_1_0_91_7_0_255_A2',
	pf int '$.Data_1_0_13_7_0_255_A2',
	freq decimal(10,3) '$.Data_1_0_14_7_0_255_A2',
	w_imp int '$.Data_1_0_1_7_0_255_A2',
	va_imp int '$.Data_1_0_9_7_0_255_A2',
	wh_imp decimal(10,3) '$.Data_1_0_1_8_0_255_A2',
	vah_imp decimal(10,3) '$.Data_1_0_9_8_0_255_A2',
	md_w_imp decimal(10,3) '$.Data_1_0_1_6_0_255_A2',
	md_w_imp_ts datetime '$.Data_1_0_1_6_0_255_A5',
	md_va_imp decimal(10,3) '$.Data_1_0_9_6_0_255_A2',
	md_va_imp_ts datetime '$.Data_1_0_9_6_0_255_A5',
	pwr_on_dur int '$.Data_0_0_94_91_14_255_A2',
	tamp_cnt int '$.Data_0_0_94_91_0_255_A2',
	bill_cnt int '$.Data_0_0_0_1_0_255_A2',
	prgm_cnt int '$.Data_0_0_96_2_0_255_A2',
	wh_exp int '$.Data_1_0_2_8_0_255_A2',
	vah_exp int '$.Data_1_0_10_8_0_255_A2',
	load_Imt_stat bit '$.Data_0_0_96_3_10_255_A2',
	load_Imt_val int '$.Data_0_0_17_0_0_255_A2'
)
end