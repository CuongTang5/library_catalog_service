# Yêu cầu kết nối Backend từ Nhóm N1

## Danh sách yêu cầu cần nhóm N1 cung cấp

### 1. **Backend API URL**
Nhóm N1 cần cung cấp URL của backend API dưới dạng:
```
http://<domain-hoặc-IP>:<port>/api/books
```

**Ví dụ:**
- `http://26.30.78.80:3000/api/books` (IP tĩnh)
- `https://api.n1.local:8080/api/books` (domain)
- `http://n1-server:5000/api/books` (hostname)

**Lưu ý:** 
- Backend phải cho phép CORS (Cross-Origin Resource Sharing) hoặc frontend/backend phải ở cùng origin
- URL phải có thể truy cập được từ máy chạy frontend

---

### 2. **Cấu trúc JSON dữ liệu sách**

Nhóm N1 phải trả về JSON với cấu trúc **tối thiểu** như sau:

```json
{
  "id": 1,
  "tenSach": "Lập trình C#",
  "tacGia": "Nguyễn Văn A",
  "nhaSanXuat": "NXB BKHN",
  "soLuong": 10,
  "soBanDaMuon": 3,
  "soBanConLai": 7
}
```

**Chi tiết các trường:**

| Trường | Kiểu | Bắt buộc | Mô tả |
|--------|------|----------|-------|
| `id` | number | ✅ | ID duy nhất của sách |
| `tenSach` | string | ✅ | Tên sách |
| `tacGia` | string | ✅ | Tác giả |
| `nhaSanXuat` | string | ✅ | Nhà xuất bản |
| `soLuong` | number | ✅ | Tổng số bản |
| `soBanDaMuon` | number | ❌ | Số bản đã mượn (mặc định = 0) |
| `soBanConLai` | number | ❌ | Số bản còn lại (tính = soLuong - soBanDaMuon) |

---

### 3. **Danh sách các Endpoint API cần cung cấp**

#### **GET /api/books** (Lấy danh sách tất cả sách)
```http
GET /api/books
Content-Type: application/json

Response 200:
[
  { "id": 1, "tenSach": "...", "tacGia": "...", ... },
  { "id": 2, "tenSach": "...", "tacGia": "...", ... }
]
```

#### **GET /api/books/:id** (Lấy thông tin chi tiết sách)
```http
GET /api/books/1
Content-Type: application/json

Response 200:
{ "id": 1, "tenSach": "...", "tacGia": "...", ... }

Response 404:
{ "error": "Book not found" }
```

#### **POST /api/books** (Thêm sách mới - tuỳ chọn)
```http
POST /api/books
Content-Type: application/json

Request Body:
{
  "tenSach": "Sách mới",
  "tacGia": "Tác giả",
  "nhaSanXuat": "NXB",
  "soLuong": 5
}

Response 201:
{ "id": 3, "tenSach": "...", ... }
```

#### **PUT /api/books/:id** (Cập nhật thông tin sách - tuỳ chọn)
```http
PUT /api/books/1
Content-Type: application/json

Request Body:
{
  "tenSach": "Tên mới",
  "soLuong": 15,
  "soBanDaMuon": 5
}

Response 200 / 204:
{ "id": 1, ... } hoặc No Content
```

#### **DELETE /api/books/:id** (Xóa sách - tuỳ chọn)
```http
DELETE /api/books/1

Response 200 / 204:
No Content hoặc { "success": true }
```

---

### 4. **Yêu cầu kỹ thuật**

- ✅ Backend phải hỗ trợ **CORS** (nếu frontend chạy trên domain/port khác)
  ```http
  Access-Control-Allow-Origin: *
  ```
  
- ✅ Response phải trả về **JSON** với header:
  ```http
  Content-Type: application/json; charset=utf-8
  ```

- ✅ HTTP Status codes:
  - `200 OK` - Thành công (GET, PUT, DELETE)
  - `201 Created` - Tạo mới thành công (POST)
  - `204 No Content` - Thành công, không có nội dung (PUT, DELETE)
  - `404 Not Found` - Không tìm thấy
  - `400 Bad Request` - Lỗi dữ liệu đầu vào
  - `500 Internal Server Error` - Lỗi server

---

### 5. **Seed Data (dữ liệu mẫu)**

Để dễ kiểm tra, nhóm N1 nên seed ít nhất 3-5 sách mẫu vào database, ví dụ:

```json
[
  {
    "id": 1,
    "tenSach": "Lão Hạc",
    "tacGia": "Nam Cao",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 20,
    "soBanDaMuon": 0,
    "soBanConLai": 20
  },
  {
    "id": 2,
    "tenSach": "Số Đỏ",
    "tacGia": "Vũ Trọng Phụng",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 15,
    "soBanDaMuon": 2,
    "soBanConLai": 13
  },
  {
    "id": 3,
    "tenSach": "Truyện Kiều",
    "tacGia": "Nguyễn Du",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 25,
    "soBanDaMuon": 5,
    "soBanConLai": 20
  }
]
```

---

## Tóm tắt: Gửi cho nhóm N1

**Email/Message mẫu:**

```
Xin chào nhóm N1,

Nhóm tôi cần kết nối với backend API của bạn. Vui lòng cung cấp:

1. URL của Backend API (ví dụ: http://26.30.78.80:3000/api/books)
2. Danh sách các endpoint GET /api/books, GET /api/books/:id, POST, PUT, DELETE (nếu có)
3. Xác nhận cấu trúc JSON sách trả về gồm các trường:
   - id (number)
   - tenSach (string)
   - tacGia (string)
   - nhaSanXuat (string)
   - soLuong (number)
   - soBanDaMuon (number, tuỳ chọn)
   - soBanConLai (number, tuỳ chọn)
4. Xác nhận CORS được bật để frontend có thể gọi API
5. Seed ít nhất 3-5 sách mẫu để kiểm tra

Cảm ơn!
```

---

## Cách cập nhật URL Backend trong Frontend

Sau khi nhóm N1 cung cấp URL, vui lòng:

1. Mở file `frontend/.env.local` (hoặc `.env.production`)
2. Thêm dòng:
   ```
   VITE_API_URL=http://26.30.78.80:3000/api/books
   ```
3. Frontend sẽ tự động dùng URL từ nhóm N1

(Xem hướng dẫn ở file `API_CONFIG.md`)
