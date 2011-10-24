use master
go

create database SoapMessageBox
go

use SoapMessageBox
go

create table MessageBox
(
	Identifier 			int identity,
	Endpoint			nvarchar(256),
	SmallSoapMessage 	nvarchar(2048) null,
	LargeSoapMessage	ntext null,
	Owner				uniqueidentifier null,
	constraint PK_MessageBox primary key (Endpoint, identifier)
)
go

create procedure dbo.InsertSmallMessage
(	
	@Endpoint 	nvarchar(256),
	@Message 	nvarchar(2048)
)
as
	set nocount off

	insert dbo.MessageBox(Endpoint, SmallSoapMessage, LargeSoapMessage)
	values (@Endpoint, @Message, null)
go

create procedure dbo.InsertLargeMessage
(
	@Endpoint	nvarchar(256),
	@Message 	ntext
)
as
	set nocount off

	insert dbo.MessageBox(Endpoint, SmallSoapMessage, LargeSoapMessage)
	values (@Endpoint, null, @Message)
go

create procedure dbo.GetMessages
(
	@Endpoints nvarchar(4000)
)
as
	set nocount on

	declare @updateSql as nvarchar(4000)
	declare @selectSql as nvarchar(4000)
	declare @deleteSql as nvarchar(4000)
	declare @guid as uniqueidentifier

	set @guid = NEWID()

	begin tran

	set @updateSql = 'update dbo.Messagebox set owner=@g where Endpoint in (@ep) and owner is null'

	exec sp_executesql  @updateSql, N'@g uniqueidentifier, @ep nvarchar(4000)', @g=@guid, @ep=@Endpoints

	set @selectSql = 'select * from dbo.Messagebox where owner=@g and Endpoint in (@ep)'

	exec sp_executesql @selectSql, N'@g uniqueidentifier, @ep nvarchar(4000)', @g=@guid, @ep=@Endpoints

	set @deleteSql = 'delete dbo.Messagebox where owner=@g and Endpoint in (@ep)'

	exec sp_executesql @deleteSql, N'@g uniqueidentifier, @ep nvarchar(4000)', @g=@guid, @ep=@Endpoints

	commit tran