# Galaxy.Auth

## Description
In this project we will take care of user creation and login. Also here we will do a permission management. With the help of gRPC we will comunicate with the rest of the ecosystem on what permissions each user has. The architecture type is clean architecture so the solution is devided in 3 projects: Presentation, Infrastructure and Core. More about clean architecture you can find here https://medium.com/vinarah/clean-architecture-example-c-5990bd4ac8. 

## Used Technologies
- .Net Core Console App 3.0
- JSON Web Token (JWT)
- Pomelo EF
- MySql Server
- NLog
- gRPC

## Entity Framework Migrations
Run the following command from Galaxy.Auth.Presentation folder:
dotnet ef migrations add {{name}} -c AuthDbContext

## Needed in order to run locally
- you need to trust https certificates from .net. Please run this: **dotnet dev-certs https --trust** if you haven't done this before
- install .net core 3.0 https://dotnet.microsoft.com/download/dotnet-core/3.0
- install mysql (https://www.mysql.com/downloads/) or run it from docker. For ease of use you should also get workbanch, or some IDE to visual connect to MySQL server.

## Api testing
You can have a visual representation of the api on the /swagger page, also the page that the app starts.

## On deploy 
Please make sure you overwrite the following settings:
- ConnectionStrings__AuthDb
- AppSettings__Jwt__Key
- AppSettings__EncryptionKey
- AppSettings__SmtpServer__Host
- AppSettings__SmtpServer__Port
- AppSettings__SmtpServer__Username
- AppSettings__SmtpServer__Password
- AppSettings__FromEmail