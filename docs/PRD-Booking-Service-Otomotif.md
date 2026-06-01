# Product Requirements Document (PRD)
## Booking Service Otomotif (Bengkel Mobil)

## 1. Ringkasan Produk
**Nama Produk:** AutoCare Booking API (working title)  
**Tipe Produk:** Backend Web API (ASP.NET Core)  
**Tujuan Utama:** Memudahkan customer melakukan booking servis mobil, membantu bengkel mengelola antrian servis, dan memberi visibilitas status pekerjaan secara end-to-end.

Project ini ditujukan sebagai **portfolio showcase** yang merepresentasikan kemampuan production-ready backend engineering.

## 2. Latar Belakang & Problem Statement
Proses booking bengkel sering masih manual (telepon/chat), menyebabkan:
- Bentrok jadwal servis.
- Estimasi pengerjaan tidak transparan.
- Riwayat servis kendaraan sulit dilacak.
- Koordinasi customer, admin bengkel, dan mekanik kurang efisien.

Produk ini menyelesaikan masalah tersebut melalui sistem booking terstruktur dengan status kerja terstandar.

## 3. Goals dan Non-Goals
### 3.1 Goals (MVP)
- Customer dapat membuat booking servis berdasarkan slot waktu yang tersedia.
- Admin dapat mengelola booking dan assign mekanik.
- Mekanik dapat update progres pekerjaan.
- Sistem menyimpan riwayat servis per kendaraan.
- Tersedia estimasi biaya dan ringkasan hasil servis.

### 3.2 Non-Goals (Fase setelah MVP)
- Pembayaran online live production (MVP hanya sandbox/test mode).
- Integrasi WhatsApp real API (MVP cukup mock/notifikasi simulasi).
- Mobile app native.
- Dynamic pricing berbasis AI.

## 4. Target User & Role
### 4.1 Customer
- Registrasi/login.
- Kelola data kendaraan.
- Buat booking servis.
- Lihat status progres servis.
- Lihat riwayat servis kendaraan.

### 4.2 Admin / Service Advisor
- Kelola jenis layanan & slot waktu.
- Validasi booking masuk.
- Assign mekanik.
- Update hasil inspeksi, estimasi biaya, final summary.

### 4.3 Mechanic
- Lihat job yang di-assign.
- Update status pengerjaan.
- Isi catatan teknis servis.

## 5. Use Case Utama
1. Customer memilih kendaraan + layanan + tanggal/jam.
2. Sistem cek ketersediaan slot.
3. Booking dibuat dengan status `Booked`.
4. Saat kendaraan datang, admin ubah status `Check-in`.
5. Admin assign mekanik, status menjadi `In Service`.
6. Mekanik update pekerjaan hingga selesai, status `Done`.
7. Admin menutup transaksi menjadi `Paid` (atau completed billing).
8. Riwayat servis tersimpan dan dapat dilihat customer.

## 6. Scope Fitur
### 6.1 Fitur Wajib MVP
- Authentication & Authorization (JWT, role-based: Customer/Admin/Mechanic).
- Manajemen kendaraan customer.
- Booking appointment servis.
- Slot availability check (anti double-booking).
- Status workflow servis:
  - `Booked`
  - `Check-in`
  - `In Service`
  - `Done`
  - `Paid`
- Estimasi biaya servis.
- Catatan servis dan riwayat servis.
- Search, filter, sort, pagination pada data booking.

### 6.2 Fitur Pembeda (Showcase+)
- Reminder H-1 (email/mock notifier).
- Upload foto kondisi kendaraan sebelum servis.
- Dashboard antrian harian.
- Rekomendasi servis berikutnya berbasis tanggal/km terakhir.
- Integrasi payment gateway sandbox (Midtrans).

## 7. Functional Requirements
### 7.1 Auth & User
- Register, login, refresh token (opsional MVP minimal: login JWT).
- Role-based endpoint protection.

### 7.2 Vehicle Management
- Customer dapat CRUD kendaraan miliknya.
- Field minimum: plate number, brand, model, year, current mileage.

### 7.3 Service Catalog
- Admin CRUD layanan bengkel.
- Field minimum: service name, duration, base price.

### 7.4 Booking Management
- Customer membuat booking dengan: vehicle, service type, date-time slot, keluhan.
- Sistem menolak slot yang bentrok.
- Admin dapat reschedule/cancel sesuai policy.

### 7.5 Job Progress Management
- Admin/mechanic update status sesuai urutan workflow.
- Sistem validasi transisi status agar tidak lompat state.

### 7.6 Cost Estimation & Service Notes
- Admin/mechanic input estimasi biaya.
- Mekanik mengisi catatan teknis hasil servis.
- Saat selesai, generate service summary.

### 7.7 Service History
- Customer melihat riwayat lengkap per kendaraan.
- Data mencakup tanggal servis, layanan, biaya, catatan.

### 7.8 Payment Management (Final Decision)
- Model hybrid payment:
  - `Manual Payment` oleh Admin sebagai fallback operasional.
  - `Midtrans Sandbox` sebagai gateway utama untuk demo showcase.
- Booking yang sudah `Done` dapat dibuatkan transaksi pembayaran.
- Sistem menyimpan jejak transaksi provider dan sinkronisasi status pembayaran dari webhook.
- Invoice ringkas dapat diakses customer/admin.

## 8. Non-Functional Requirements
- API response time target: p95 < 500ms untuk endpoint read umum.
- Error handling konsisten (standardized error response).
- Logging terpusat (Serilog).
- Data integrity via FK + transaction untuk proses kritikal.
- Audit fields: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`.
- Event-level audit trail untuk aksi kritikal (booking, assignment, status change, payment update).
- Soft delete untuk entity tertentu (mis. vehicle/service catalog bila diperlukan).
- API documented via Swagger/OpenAPI.

## 9. Business Rules
- Slot tidak boleh double-book pada resource yang sama (time overlap check).
- Status transition harus berurutan:
  - `Booked -> Check-in -> In Service -> Done -> Paid`
- Hanya Admin yang bisa assign mekanik.
- Customer hanya bisa akses data miliknya sendiri.
- Mechanic hanya bisa update job yang di-assign ke dirinya.
- Booking tidak boleh ditandai `Paid` sebelum status pekerjaan `Done`.
- Webhook payment harus idempotent (event sama tidak boleh menyebabkan double update).

## 10. Data Model (High-Level)
Entitas inti:
- `User` (Role: Customer/Admin/Mechanic)
- `Vehicle`
- `ServiceCatalog`
- `Booking`
- `BookingServiceItem` (jika multi-layanan per booking)
- `MechanicAssignment`
- `ServiceProgressLog`
- `CostEstimate`
- `ServiceRecord`
- `VehicleConditionPhoto`
- `PaymentTransaction`
- `AuditLog`

## 11. API Blueprint (High-Level)
### 11.1 Auth
- `POST /api/auth/register`
- `POST /api/auth/login`

### 11.2 Vehicles
- `GET /api/vehicles`
- `POST /api/vehicles`
- `PUT /api/vehicles/{id}`
- `DELETE /api/vehicles/{id}`

### 11.3 Services
- `GET /api/services`
- `POST /api/services` (Admin)
- `PUT /api/services/{id}` (Admin)
- `DELETE /api/services/{id}` (Admin)

### 11.4 Booking
- `GET /api/bookings`
- `GET /api/bookings/{id}`
- `POST /api/bookings`
- `PUT /api/bookings/{id}/reschedule`
- `PUT /api/bookings/{id}/cancel`
- `PUT /api/bookings/{id}/status`
- `PUT /api/bookings/{id}/assign-mechanic`

### 11.5 History & Notes
- `POST /api/bookings/{id}/estimate`
- `POST /api/bookings/{id}/service-notes`
- `GET /api/vehicles/{id}/service-history`

### 11.6 Upload
- `POST /api/bookings/{id}/photos`

### 11.7 Payment
- `POST /api/bookings/{id}/payments/create` (create Midtrans sandbox transaction)
- `POST /api/payments/webhook/midtrans` (callback status dari Midtrans)
- `PUT /api/bookings/{id}/payment/manual` (Admin fallback manual payment)
- `GET /api/bookings/{id}/invoice`

### 11.8 Audit
- `GET /api/audit-logs` (Admin, filter by entity/action/date/actor, paginated)
- `GET /api/audit-logs/{id}` (Admin)

## 12. Security & Access Control
- JWT Bearer Authentication.
- Role-based authorization policy.
- Input validation (FluentValidation).
- Prevent over-posting via DTO terpisah.
- Masking data sensitif jika diperlukan.
- Verifikasi signature webhook dari Midtrans sebelum update status transaksi.

## 13. Observability & Quality Plan
- Structured logging (Serilog).
- Global exception middleware.
- Health check endpoint.
- Unit test prioritas:
  - Slot booking conflict checker.
  - Status transition validator.
  - Authorization business rules.
- Integration test prioritas:
  - End-to-end booking flow.
  - Payment webhook flow + idempotency.
  - Audit log creation pada create/update/delete dan event bisnis utama.

## 14. Deployment & DevOps Plan
- Dockerfile untuk API.
- `docker-compose` untuk API + database.
- CI pipeline (GitHub Actions): restore, build, test.
- Target deploy: Railway/Render/Azure (pilih satu untuk demo live).

## 15. Success Metrics (MVP)
- 100% endpoint inti booking flow berjalan sesuai skenario demo.
- Tidak ada bug kritikal pada alur `Booked -> Paid`.
- Minimum 70% coverage untuk service layer inti (target internal, opsional).
- Demo dapat diakses online + dokumentasi Swagger aktif.

## 16. Demo Script untuk Interview
1. Login sebagai Customer.
2. Tambah kendaraan.
3. Buat booking servis.
4. Login Admin, lakukan assign mekanik.
5. Login Mechanic, update progres hingga `Done`.
6. Login Admin, finalisasi `Paid`.
7. Login Customer, lihat riwayat servis dan summary biaya.

## 17. Roadmap Implementasi
### Phase 1 - Foundation
- Setup auth, role, base entities, migration, seed data.

### Phase 2 - Core Booking
- Vehicle, service catalog, booking + slot validation + status workflow.

### Phase 3 - Operations
- Assignment mechanic, progress log, estimate, service notes, history.

### Phase 4 - Showcase Hardening
- Logging, tests, docker, CI/CD, deployment, README polish.

## 18. Risiko & Mitigasi
- Risiko: aturan booking overlap kompleks.  
  Mitigasi: dedicated domain service + unit test edge cases.

- Risiko: scope melebar (feature creep).  
  Mitigasi: kunci MVP, fitur pembeda dikerjakan setelah flow inti stabil.

- Risiko: demo gagal karena environment setup.  
  Mitigasi: docker-compose one-command run + seeded demo data.

- Risiko: status payment tidak sinkron akibat webhook retry/out-of-order.  
  Mitigasi: simpan `providerTransactionId`, `providerEventId`, dan terapkan idempotency check + status transition guard.

## 19. Definition of Done (MVP)
- Semua endpoint core tersedia dan lolos test utama.
- Flow end-to-end dari booking sampai paid berjalan.
- Dokumentasi API + README lengkap.
- Aplikasi dapat dijalankan lokal via docker-compose.
- Minimal satu environment live untuk showcase.

---

## Appendix A - Tech Stack (Disepakati)
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / PostgreSQL (pilih satu konsisten)
- FluentValidation
- Serilog
- Swagger/OpenAPI
- xUnit + integration testing stack
- Docker + GitHub Actions
- Midtrans Sandbox (payment gateway testing)

## Appendix C - Payment Detail Blueprint
### C.1 Provider Choice
- Provider utama: `Midtrans` mode `Sandbox` (gratis untuk testing/demo).

### C.2 Payment Status Internal
- `Unpaid`
- `PaymentPending`
- `Paid`
- `PaymentFailed`
- `Refunded`

### C.3 PaymentTransaction (minimum fields)
- `Id`
- `BookingId`
- `Provider` (default: `Midtrans`)
- `ProviderTransactionId`
- `ProviderOrderId`
- `GrossAmount`
- `PaymentType`
- `TransactionStatus`
- `FraudStatus` (jika ada)
- `RawNotificationPayload`
- `CreatedAt`
- `UpdatedAt`

### C.6 Audit Log Minimum Fields
- `Id`
- `EntityName`
- `EntityId`
- `Action` (`CREATE`, `UPDATE`, `DELETE`, `STATUS_CHANGE`, `PAYMENT_UPDATE`)
- `OldValues` (json)
- `NewValues` (json)
- `ActorUserId`
- `ActorRole`
- `CorrelationId`
- `OccurredAt`

### C.4 Mapping Status Midtrans -> Internal
- `settlement` atau `capture` -> `Paid`
- `pending` -> `PaymentPending`
- `deny`, `cancel`, `expire` -> `PaymentFailed`
- `refund` atau `partial_refund` -> `Refunded`

### C.5 Retrofit Strategy ke Existing System
1. Pertahankan endpoint manual payment sebagai fallback.
2. Tambahkan endpoint create gateway payment dan webhook callback.
3. Update status booking menjadi `Paid` hanya saat internal payment status `Paid`.
4. Semua event payment dicatat ke audit log.

## Appendix B - Catatan Posisi Project
Dokumen ini menjadi acuan final blueprint awal. Update versi berikutnya dilakukan jika ada perubahan scope atau keputusan teknis besar.
