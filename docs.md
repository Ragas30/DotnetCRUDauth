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

Urutan command yang benar saat update schema:
```bash
dotnet ef migrations add NamaMigration
dotnet ef database update
```

Contoh:
```bash
dotnet ef migrations add UpdateUserRoleToEnum
dotnet ef database update
```

Catatan penting:
- Jangan pakai `dotnet ef migrations database update` (salah command).
- `database update` adalah command level `ef`, bukan subcommand `migrations`.

Jika `dotnet ef` belum tersedia:
```bash
dotnet tool install --global dotnet-ef
```

## 7. Kapan Harus Buat Migration?
Buat migration jika ada perubahan pada struktur yang dipetakan EF Core, misalnya:
- tambah/hapus properti di model (`User`, `Product`, dll)
- ubah tipe data properti (contoh `string` -> `enum`)
- ubah relasi, foreign key, index, constraint
- ubah konfigurasi di `OnModelCreating` (converter, max length, dsb)

Tidak perlu migration jika hanya:
- ubah logic di Controller/Service/Repository
- ubah validasi request
- ubah proses generate JWT, response DTO, atau business flow tanpa ubah schema DB

Checklist aman tiap ubah schema:
1. Ubah model / konfigurasi EF (`Models`, `AppDbContext`).
2. `dotnet build` pastikan compile.
3. `dotnet ef migrations add <NamaMigration>`.
4. Cek file migration yang dihasilkan.
5. `dotnet ef database update`.
6. Jalankan app dan test endpoint terkait.

## 8. Menjalankan Aplikasi
```bash
dotnet run
```

## 9. Role & JWT (Quick Checklist)
- Simpan role di model sebagai enum (`USER`, `ADMIN`) agar nilai role terkontrol.
- Saat generate token, pastikan claim role diisi:
  - `new Claim(ClaimTypes.Role, user.Role.ToString())`
- Di endpoint:
  - `[Authorize]` untuk semua user login
  - `[Authorize(Roles = "ADMIN")]` untuk admin-only
- Jika status `401/403`, cek token/role.
- Jika status `500` dengan pesan `Unable to resolve service`, cek registrasi DI di `Program.cs`.

## 10. Proteksi Endpoint Dengan Bearer
1. Pastikan middleware aktif dan urutannya benar:
```csharp
app.UseAuthentication();
app.UseAuthorization();
```

2. Pasang atribut di controller/endpoint:
- Semua user yang login:
  - `[Authorize]`
- Hanya role tertentu:
  - `[Authorize(Roles = "ADMIN")]`
  - `[Authorize(Roles = "ADMIN,USER")]`

3. Header request harus berisi token:
```http
Authorization: Bearer <token_jwt>
```

4. Arti status code:
- `401 Unauthorized`: token tidak ada, format salah, expired, atau signature tidak valid.
- `403 Forbidden`: token valid, tapi role tidak punya akses endpoint.

5. Testing cepat di Swagger:
- Klik `Authorize`
- Isi: `Bearer <token_jwt>`
- Coba endpoint `[Authorize]` dan endpoint `[Authorize(Roles = "...")]`

## 11. Troubleshooting Umum
- Jika `dotnet build` gagal dengan error file terkunci (`MSB3021/MSB3027`):
  - Hentikan proses app yang masih berjalan (`Ctrl + C`).
  - Atau kill process:
    ```powershell
    taskkill /PID <PID> /F
    ```
  - Build ulang lalu update database lagi.
