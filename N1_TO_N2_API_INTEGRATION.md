# N1_TO_N2_API_INTEGRATION

## 1. Tổng quan Service

- Service: `Catalog Service` (N1)
- Backend: ASP.NET Core .NET 8
- Frontend: Vue 3 + Vite + Ant Design Vue
- Database: SQL Server
- API base: `http://<host>:5185`
- Swagger UI: `http://<host>:5185/swagger/index.html`
- CORS: `AllowAll` (backend cho phép mọi origin, phương thức và header)
- Mô tả: Catalog Service cung cấp danh sách sách, chi tiết sách, trạng thái tồn kho, thay đổi số lượng mượn/trả và đánh giá sách.

## 2. Danh sách API

### 2.1 Danh sách sách

- Method: `GET`
- Endpoint: `/api/books`
- Mục đích: Lấy danh sách tất cả sách cùng dữ liệu trạng thái hiện tại.
- Request: Không có body.
- Response: Danh sách các sách với thông tin chi tiết.

### 2.2 Tìm sách theo từ khóa

- Method: `GET`
- Endpoint: `/api/books/search`
- Query params:
  - `q` (string): từ khóa tìm kiếm trên `TenSach` hoặc `TacGia`.
- Mục đích: Tìm nhanh sách theo tên hoặc tác giả.
- Request: Không có body.
- Response: Danh sách sách khớp tìm kiếm.

### 2.3 Danh sách sản phẩm / thống kê nhanh

- Method: `GET`
- Endpoint: `/api/books/products`
- Mục đích: Lấy danh sách sách dạng sản phẩm rút gọn, có `trangThai` và số lượng còn lại.
- Request: Không có body.
- Response: Danh sách sách rút gọn.

### 2.4 Chi tiết sách

- Method: `GET`
- Endpoint: `/api/books/{id}`
- Path parameter:
  - `id` (int): ID sách.
- Mục đích: Lấy dữ liệu chi tiết một sách.
- Request: Không có body.
- Response: Một sách chi tiết.

### 2.5 Tạo sách

- Method: `POST`
- Endpoint: `/api/books`
- Mục đích: Tạo mới một bản ghi sách.
- Request: JSON body chứa dữ liệu sách.
- Response: `201 Created` với object sách vừa tạo.

### 2.6 Cập nhật sách

- Method: `PUT`
- Endpoint: `/api/books/{id}`
- Path parameter:
  - `id` (int)
- Mục đích: Cập nhật dữ liệu sách.
- Request: JSON body chứa dữ liệu sách.
- Response: `204 No Content` nếu thành công.

### 2.7 Đánh giá sách

- Method: `POST`
- Endpoint: `/api/books/{id}/rating`
- Path parameter:
  - `id` (int)
- Mục đích: Cập nhật đánh giá trung bình và số lượt đánh giá.
- Request: JSON body chứa `rating` từ 1 đến 5.
- Response: object sách đã cập nhật.

### 2.8 Mượn sách

- Method: `POST`
- Endpoint: `/api/books/{id}/borrow`
- Path parameter:
  - `id` (int)
- Mục đích: Tăng số lượng sách đã mượn (`SoBanDaMuon`).
- Request: JSON body chứa `quantity`.
- Response: object sách đã cập nhật.

### 2.9 Trả sách

- Method: `POST`
- Endpoint: `/api/books/{id}/return`
- Path parameter:
  - `id` (int)
- Mục đích: Giảm số lượng sách đã mượn (`SoBanDaMuon`).
- Request: JSON body chứa `quantity`.
- Response: object sách đã cập nhật.

### 2.10 Xóa sách

- Method: `DELETE`
- Endpoint: `/api/books/{id}`
- Path parameter:
  - `id` (int)
- Mục đích: Xóa một sách khỏi catalog.
- Request: Không có body.
- Response: `204 No Content` nếu xóa thành công.

## 3. Schema dữ liệu

### 3.1 Model `Book`

- `Id` (int)
- `TenSach` (string)
- `TacGia` (string)
- `NhaSanXuat` (string)
- `SoLuong` (int)
- `SoBanDaMuon` (int)
- `ImageUrl` (string?)
- `MoTa` (string?)
- `Isbn` (string?)
- `DanhGiaTrungBinh` (double)
- `SoLuotDanhGia` (int)
- `TheLoai` (string?)
- `SoBanConLai` (int) - tính giá trị `SoLuong - SoBanDaMuon`
- `TrangThai` (string) - `Có thể mượn` hoặc `Hết sách`

> Lưu ý: `SoBanConLai` và `TrangThai` được tính trong model/response, không phải dữ liệu lưu trực tiếp.

### 3.2 Response chính `Book`

Trường trả về từ `GET /api/books` và `GET /api/books/{id}`:

- `id`
- `tenSach`
- `tacGia`
- `nhaSanXuat`
- `soLuong`
- `soBanDaMuon`
- `soBanConLai`
- `trangThai`
- `imageUrl`
- `moTa`
- `isbn`
- `theLoai`
- `danhGiaTrungBinh`
- `soLuotDanhGia`

### 3.3 Response `products`

Trường trả về từ `/api/books/products`:

- `ma` (string): `Id` dưới dạng chuỗi.
- `tenSanPham` (string): `TenSach`.
- `tacGia` (string)
- `nhaSanXuat` (string)
- `soLuong` (int)
- `soBanDaMuon` (int)
- `soBanConLai` (int)
- `trangThai` (string)
- `theLoai` (string?)

## 4. Ví dụ Request

### 4.1 GET toàn bộ sách

```http
GET http://<host>:5185/api/books
```

### 4.2 GET chi tiết sách

```http
GET http://<host>:5185/api/books/1
```

### 4.3 POST tạo sách

```http
POST http://<host>:5185/api/books
Content-Type: application/json

{
  "tenSach": "Sách mới",
  "tacGia": "Tác giả X",
  "nhaSanXuat": "NXB Y",
  "soLuong": 20,
  "soBanDaMuon": 0,
  "imageUrl": "https://example.com/book.jpg",
  "moTa": "Mô tả sách",
  "isbn": "1234567890",
  "danhGiaTrungBinh": 0,
  "soLuotDanhGia": 0,
  "theLoai": "Khoa học"
}
```

### 4.4 PUT cập nhật sách

```http
PUT http://<host>:5185/api/books/1
Content-Type: application/json

{
  "id": 1,
  "tenSach": "Sách cập nhật",
  "tacGia": "Tác giả A",
  "nhaSanXuat": "NXB B",
  "soLuong": 15,
  "soBanDaMuon": 2,
  "imageUrl": "https://example.com/book2.jpg",
  "moTa": "Cập nhật mô tả",
  "isbn": "0987654321",
  "danhGiaTrungBinh": 4.2,
  "soLuotDanhGia": 5,
  "theLoai": "Văn học"
}
```

### 4.5 POST đánh giá sách

```http
POST http://<host>:5185/api/books/1/rating
Content-Type: application/json

{
  "rating": 5
}
```

### 4.6 POST mượn sách

```http
POST http://<host>:5185/api/books/1/borrow
Content-Type: application/json

{
  "quantity": 2
}
```

### 4.7 POST trả sách

```http
POST http://<host>:5185/api/books/1/return
Content-Type: application/json

{
  "quantity": 1
}
```

## 5. Ví dụ Response

### 5.1 Response `GET /api/books`

```json
[
  {
    "id": 1,
    "tenSach": "Lập trình C#",
    "tacGia": "Nguyễn Văn A",
    "nhaSanXuat": "NXB BKHN",
    "soLuong": 10,
    "soBanDaMuon": 0,
    "soBanConLai": 10,
    "trangThai": "Có thể mượn",
    "imageUrl": "https://picsum.photos/seed/book-1/300/450",
    "moTa": "Tác phẩm Lập trình C# của tác giả Nguyễn Văn A.",
    "isbn": null,
    "theLoai": null,
    "danhGiaTrungBinh": 0.0,
    "soLuotDanhGia": 0
  }
]
```

### 5.2 Response `GET /api/books/1`

```json
{
  "id": 1,
  "tenSach": "Lập trình C#",
  "tacGia": "Nguyễn Văn A",
  "nhaSanXuat": "NXB BKHN",
  "soLuong": 10,
  "soBanDaMuon": 0,
  "soBanConLai": 10,
  "trangThai": "Có thể mượn",
  "imageUrl": "https://picsum.photos/seed/book-1/300/450",
  "moTa": "Tác phẩm Lập trình C# của tác giả Nguyễn Văn A.",
  "isbn": null,
  "theLoai": null,
  "danhGiaTrungBinh": 0.0,
  "soLuotDanhGia": 0
}
```

### 5.3 Response `POST /api/books/1/borrow`

```json
{
  "id": 1,
  "tenSach": "Lập trình C#",
  "tacGia": "Nguyễn Văn A",
  "nhaSanXuat": "NXB BKHN",
  "soLuong": 10,
  "soBanDaMuon": 2,
  "imageUrl": null,
  "moTa": null,
  "isbn": null,
  "danhGiaTrungBinh": 0.0,
  "soLuotDanhGia": 0,
  "theLoai": null,
  "soBanConLai": 8,
  "trangThai": "Có thể mượn"
}
```

### 5.4 Response `POST /api/books/1/return`

```json
{
  "id": 1,
  "tenSach": "Lập trình C#",
  "tacGia": "Nguyễn Văn A",
  "nhaSanXuat": "NXB BKHN",
  "soLuong": 10,
  "soBanDaMuon": 1,
  "imageUrl": null,
  "moTa": null,
  "isbn": null,
  "danhGiaTrungBinh": 0.0,
  "soLuotDanhGia": 0,
  "theLoai": null,
  "soBanConLai": 9,
  "trangThai": "Có thể mượn"
}
```

## 6. Quy trình mượn sách

1. N2 gọi `POST /api/books/{id}/borrow` với body `{ "quantity": X }`.
2. Backend kiểm tra `quantity > 0`.
3. Backend tìm sách theo `id`.
4. Nếu `SoBanDaMuon + quantity > SoLuong`, trả `400 Bad Request`.
5. Nếu hợp lệ, tăng `SoBanDaMuon` và lưu vào database.
6. Response trả về object sách đã cập nhật và `soBanConLai` mới.

## 7. Quy trình trả sách

1. N2 gọi `POST /api/books/{id}/return` với body `{ "quantity": X }`.
2. Backend kiểm tra `quantity > 0`.
3. Backend tìm sách theo `id`.
4. Nếu `SoBanDaMuon - quantity < 0`, trả `400 Bad Request`.
5. Nếu hợp lệ, giảm `SoBanDaMuon` và lưu.
6. Response trả về object sách đã cập nhật.

## 8. Quy trình đồng bộ số lượng

- Đồng bộ số lượng `SoBanDaMuon` và `SoLuong` được thực hiện qua các endpoint:
  - `POST /api/books/{id}/borrow`
  - `POST /api/books/{id}/return`
  - `PUT /api/books/{id}` để cập nhật tổng `SoLuong` hoặc `SoBanDaMuon` nếu cần đồng bộ thủ công.
- `SoBanConLai` luôn tính toán tự động: `SoLuong - SoBanDaMuon`.
- Nếu cần cập nhật tổng số lượng tồn kho, N2 có thể dùng `PUT /api/books/{id}`.

## 9. Điều kiện kiểm tra còn sách

- Dùng `GET /api/books/{id}` hoặc `/api/books/products`.
- Kiểm tra:
  - `soBanConLai > 0` => còn sách
  - `trangThai == "Có thể mượn"`
- Khi mượn: endpoint `POST /api/books/{id}/borrow` sẽ trả lỗi nếu số lượng mượn vượt khả năng còn lại.

## 10. Các endpoint N2 cần gọi

- `GET /api/books`
- `GET /api/books/search?q={keyword}`
- `GET /api/books/products`
- `GET /api/books/{id}`
- `POST /api/books/{id}/borrow`
- `POST /api/books/{id}/return`
- `POST /api/books/{id}/rating`
- `POST /api/books`
- `PUT /api/books/{id}`
- `DELETE /api/books/{id}`

## 11. Các endpoint N1 có thể gọi N2

- Dựa trên source code hiện tại, `Catalog Service` (N1) không chứa bất kỳ cuộc gọi API tới N2.
- Tại thời điểm này, N1 chỉ cung cấp API cho N2 và không có logic gọi service khác trong mã nguồn.

## 12. Hướng dẫn test bằng Postman

1. Khởi động backend: `dotnet run --project CatalogService/CatalogService.csproj --urls "http://0.0.0.0:5185"`
2. Tạo collection mới.
3. Thêm request `GET http://<host>:5185/api/books`.
4. Thêm request `GET http://<host>:5185/api/books/{id}`.
5. Thêm request `POST http://<host>:5185/api/books/{id}/borrow` với body JSON.
6. Thêm request `POST http://<host>:5185/api/books/{id}/return` với body JSON.
7. Kiểm tra response status và dữ liệu:
   - `200 OK` cho các truy vấn hợp lệ.
   - `400 Bad Request` khi mượn/trả không hợp lệ.
   - `404 Not Found` khi `id` không tồn tại.

## 13. Hướng dẫn test bằng curl

```bash
curl -X GET "http://<host>:5185/api/books"

curl -X GET "http://<host>:5185/api/books/1"

curl -X GET "http://<host>:5185/api/books/search?q=C#"

curl -X POST "http://<host>:5185/api/books/1/borrow" \
  -H "Content-Type: application/json" \
  -d '{"quantity": 1}'

curl -X POST "http://<host>:5185/api/books/1/return" \
  -H "Content-Type: application/json" \
  -d '{"quantity": 1}'

curl -X POST "http://<host>:5185/api/books/1/rating" \
  -H "Content-Type: application/json" \
  -d '{"rating": 5}'
```

## 14. Swagger URL

- `http://<host>:5185/swagger/index.html`

## 15. Cấu hình VPS

- Backend lắng nghe trên `5185`.
- Cần SQL Server với database `DigitalLibraryCatalog`.
- `appsettings.json` chứa connection string mặc định:
  - `Server=localhost;Database=DigitalLibraryCatalog;Trusted_Connection=True;TrustServerCertificate=True;`
- Backend tự động chạy `Database.Migrate()` mỗi lần khởi động.
- Phải đảm bảo firewall và mạng cho phép truy cập tới cổng `5185`.

## 16. Port sử dụng

- Backend API: `5185`
- Frontend mặc định: `5174` (Vite)

## 17. Các lưu ý khi tích hợp

- N1 không yêu cầu auth.
- N1 trả về JSON với camelCase.
- `SoBanConLai` và `TrangThai` là trường tính toán trong response.
- `GET /api/books/products` trả về tài nguyên dạng sản phẩm (`ma`, `tenSanPham`, `trangThai`).
- `POST /api/books/{id}/borrow` và `POST /api/books/{id}/return` chỉ chấp nhận `quantity > 0`.
- `POST /api/books/{id}/rating` chỉ chấp nhận `rating` từ `1` đến `5`.
- N1 không gọi lại N2 trong source code hiện tại.
- Nếu cần đồng bộ số lượng tồn kho tổng thể, nên sử dụng `PUT /api/books/{id}` hoặc endpoint borrow/return thay vì sửa trực tiếp `SoBanConLai`.

## 18. Sơ đồ kiến trúc

```
Frontend (Vue)
          ↓
Catalog API (.NET)
          ↓
      SQL Server
```

## 19. Sơ đồ tích hợp

```
N2 Circulation
     ↔
  N1 Catalog
```
