# N1_DEPLOYMENT_GUIDE

## 1. Clone project

```bash
git clone https://github.com/CuongTang5/library_catalog_service.git
cd library_catalog_service
```

## 2. Chạy backend

### 2.1 Chuẩn bị

- Cài .NET SDK 8.x
- Cài SQL Server và đảm bảo có thể truy cập `localhost`
- Kiểm tra connection string trong `CatalogService/appsettings.json`:
  - `Server=localhost;Database=DigitalLibraryCatalog;Trusted_Connection=True;TrustServerCertificate=True;`

### 2.2 Cài đặt và chạy

```bash
cd CatalogService
dotnet restore
dotnet build
dotnet run --urls "http://0.0.0.0:5185"
```

- Backend sẽ mở tại `http://0.0.0.0:5185`.
- Swagger UI: `http://localhost:5185/swagger/index.html`.

## 3. Chạy frontend

### 3.1 Chuẩn bị

- Node.js và npm (hoặc yarn) đã cài.
- Vào thư mục frontend.

### 3.2 Cài đặt và chạy

```bash
cd frontend
npm install
npm run dev
```

- Frontend mặc định sẽ chạy với Vite.
- Frontend lấy backend base URL mặc định: `http://<host>:5185`.
- Nếu cần ghi đè, đặt biến môi trường `VITE_API_URL` trước khi build/run.

## 4. Push GitHub

```bash
git add .
git commit -m "Deploy catalog service update"
git push origin main
```

## 5. Pull VPS

```bash
ssh <user>@<vps-host>
cd /path/to/library_catalog_service
git pull origin main
```

## 6. PM2 restart

- Nếu VPS dùng PM2 để quản lý frontend hoặc backend, chạy:

```bash
pm install -g pm2
pm2 restart <process-name>
```

- Hoặc nếu backend chạy dưới dạng node/vite: `pm2 restart frontend`.

## 7. Các lỗi đã gặp

### 7.1 `package-lock.json conflict`

- Nguyên nhân: có thay đổi khác nhau giữa hai branch/commit trong `frontend/package-lock.json`.
- Xử lý: resolve conflict, kiểm tra `npm install` lại, commit `package-lock.json` sạch.

### 7.2 API không chạy

- Kiểm tra backend có đang lắng nghe cổng `5185`.
- Kiểm tra lệnh khởi động backend: `dotnet run --urls "http://0.0.0.0:5185"`.
- Kiểm tra có exception startup do SQL Server, migration hoặc .NET SDK.

### 7.3 thiếu .NET SDK

- Lỗi: `The SDK 'Microsoft.NET.Sdk' specified could not be found`.
- Giải pháp: cài .NET SDK 8.x tương thích với project.

### 7.4 frontend không có dữ liệu

- Lỗi thường do backend chưa chạy hoặc `VITE_API_URL` sai.
- Kiểm tra `frontend/src/config/api.config.js`.
- Kiểm tra console browser và network request tới `http://<host>:5185/api/books`.

### 7.5 connection refused 5185

- Nguyên nhân: backend chưa chạy, firewall chặn hoặc cổng không mở.
- Xử lý:
  - Kiểm tra backend đã chạy chưa bằng `dotnet run`.
  - Kiểm tra firewall cho phép cổng `5185`.
  - Kiểm tra `app.Run("http://0.0.0.0:5185")` trong `CatalogService/Program.cs`.

## 8. Lưu ý triển khai

- Backend tự động áp dụng EF Core migration khi khởi động.
- Cần đảm bảo SQL Server có database `DigitalLibraryCatalog` hoặc có quyền tạo DB.
- Frontend và backend phải cùng host/cổng hoặc sử dụng CORS (backend đã mở CORS toàn bộ).
- Nếu deploy trên VPS, cần mở port `5185` và cấu hình reverse proxy nếu cần.
- Không dùng dữ liệu nháp; chỉ dùng endpoint đã xác định trong source code.
