step belaja DOTNEt:
1. setup program.cs
2. atur folder :
	`-bin,
	`-Controllers,
	`-Data,
	`-DTOs,
	`-Models,
	`-obj,
	`-Properties,
	`-Repositories,
	`-Services,
3. install pack:
	`-dotnet add package Microsoft.EntityFrameworkCore
	`-dotnet add package Microsoft.EntityFrameworkCore.Design
	`-dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
	`-dotnet add package Swashbuckle.AspNetCore
	`-dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
	`-dotnet add package BCrypt.Net-Next
4. setup appsettings.json
5. buat model, DTO, repository, service, controller
6. buat migration dan update database