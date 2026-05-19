# Docs Belajar DotnetCRUD

## 1. Struktur Folder
- `Controllers`
- `Data`
- `DTOs`
- `Models`
- `Repositories`
- `Services`
- `Properties`
- `bin` (generated)
- `obj` (generated)

## 2. Install Package
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Swashbuckle.AspNetCore
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
```

## 3. Konfigurasi `appsettings.json`
- `ConnectionStrings:DefaultConnection` untuk PostgreSQL.
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience` untuk token JWT.

Contoh ringkas:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=dotnetcrud_db;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-long-secret-key-minimal-32-char",
    "Issuer": "DotnetCRUD",
    "Audience": "DotnetCRUDClient"
  }
}
```

## 4. Buat Komponen Inti
- Model: `User`, `Product`, dll.
- DTO Auth: `RegisterDto`, `LoginDto`, `AuthResponseDto`.
- Repository: `IUserRepository`, `UserRepository`.
- Service: `IAuthService`, `AuthService`.
- Controller: `AuthController`.

## 5. Setup `Program.cs`
- Register DbContext `UseNpgsql(...)`.
- Register DI:
  - `IUserRepository -> UserRepository`
  - `IAuthService -> AuthService`
- Aktifkan:
  - `AddControllers()`
  - `AddCors(...)`
  - `AddAuthentication().AddJwtBearer(...)`
  - `AddAuthorization()`
  - `AddEndpointsApiExplorer()`
  - `AddSwaggerGen()`

## 6. Migration dan Update Database (ORM EF Core)
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Jika `dotnet ef` belum tersedia:
```bash
dotnet tool install --global dotnet-ef
```

## 7. Menjalankan Aplikasi
```bash
dotnet run
```

## 8. Troubleshooting Umum
- Jika `dotnet build` gagal dengan error file terkunci (`MSB3021/MSB3027`):
  - Hentikan proses app yang masih berjalan (`Ctrl + C`).
  - Atau kill process:
    ```powershell
    taskkill /PID <PID> /F
    ```
  - Build ulang lalu update database lagi.
