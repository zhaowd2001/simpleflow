So, this posting is really just for fun.

I wanted to have a play at writing an additional transport for the WSE 2.0 and 
I struggled to think of a good transport that was missing that wasn't going to 
be too hard for me to write :-) 

Some things that came to mind were named pipes, message queues (already being 
done elsewhere XXXX) and SQL server (using SQL server as a transport is 
something that BizTalk Server 2004 does which is where the idea kind of came 
from).

So, I decided to go along with a simple "SQL transport" that pushes and pulls 
SOAP messages to/from a common SQL Server. I'm not really suggesting that 
there's a great use for this - it was just something to play around with.

On the face of it, Writing a transport appears to be a matter of providing an 
implementation of ISoapTransport which is responsible for dishing out 
implementations of ISoapInputChannel and ISoapOutputChannel.

I've written implementations of these things for my SQL transport which just 
works by shuffling messages in and out of a table in SQL Server. 

Note that the sample doesn't really do error handling very well and it also 
doesn't really handle endpoint references properly (see my previous posting 
here XXXX) as it just handles them as Uri's. It wouldn't be the most 
scalable thing in the world either.

Regardless - here's what's contained in this sample;

Three VS.NET projects

1) SoapSqlTransport. This is the project that contains the transport 
implementation. In order to make the sample work you would need to execute 
the script named "SetupSql.sql" contained within this project to create the 
database, table and stored procs that I use. 

2) TestClient. This is a test "client" project that continuously sends soap 
messages down the "soap.sql" transport that I introduce.

3) TestService. This is a test "server" project that acts as a SoapReceiver 
on a "soap.sql" channel, reading messages and writing them to the console.

So, in order to try and make this work do the following;

1) Open the solution in VS.NET - the solution is in the "SoapSqlTransport" 
folder.
2) Run the "SetupSql.sql" script against your database server.
3) Build the solution. Fix the error that comes up by altering the 
connection string to your database.
4) Press F5 to run the solution - this should start both the "client" and 
"server" project and illustrate messages flowing through.

Note that the application configuration file is used to introduce the new 
"soap.sql" transport. 

Note also that the reference from the "TestClient" and the "TestService" 
projects to the "SoapSqlTransport" project are not really necessary - I'm
only doing this to avoid having to manually copy the SoapSqlTransport
assembly to the bin\debug folders of these projects. There is no build-time
dependency between the code in these projects and the "SoapSqlTransport"
project.

Note that if your messages reside in the SQL Server for long enough then 
they'll expire - remember to alter your Time To Live property on a 
message that you're intending to have stay in SQL Server for a little 
while. The TestClient sets its expiry time to 10 minutes.