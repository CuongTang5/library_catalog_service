# 📧 Backend API Info - Gửi cho Nhóm N1

## 🔗 Backend URL

```
http://<server-IP>:5185
```

**Ví dụ:**
- Local: `http://localhost:5185`
- LAN Server: `http://26.30.78.80:5185`
- Domain: `http://library-api.local:5185`

---

## 📊 Cấu trúc Dữ Liệu API

### Đối tượng Book (Sách)

**JSON Response Example:**

```json
{
  "id": 1,
  "tenSach": "Lập trình C#",
  "tacGia": "Nguyễn Văn A",
  "nhaSanXuat": "NXB BKHN",
  "soLuong": 10,
  "soBanDaMuon": 3,
  "soBanConLai": 7,
  "trangThai": "Có thể mượn"
}
```

**Chi tiết các trường:**

| Trường | Kiểu | Mô tả |
|--------|------|-------|
| `id` | number | ID duy nhất của sách |
| `tenSach` | string | Tên sách |
| `tacGia` | string | Tác giả |
| `nhaSanXuat` | string | Nhà xuất bản |
| `soLuong` | number | Tổng số bản |
| `soBanDaMuon` | number | Số bản đã mượn |
| `soBanConLai` | number | Số bản còn lại (= soLuong - soBanDaMuon) |
| `trangThai` | string | Trạng thái ("Có thể mượn" hoặc "Hết sách") |

---

## 🔌 Danh sách Endpoint API

### 1️⃣ GET /api/books
**Lấy danh sách tất cả sách**

```http
GET http://26.30.78.80:5185/api/books
```

**Response:**
```json
[
  {
    "id": 1,
    "tenSach": "Lão Hạc",
    "tacGia": "Nam Cao",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 20,
    "soBanDaMuon": 0,
    "soBanConLai": 20,
    "trangThai": "Có thể mượn"
  },
  {
    "id": 2,
    "tenSach": "Số Đỏ",
    "tacGia": "Vũ Trọng Phụng",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 15,
    "soBanDaMuon": 2,
    "soBanConLai": 13,
    "trangThai": "Có thể mượn"
  }
]
```

---

### 2️⃣ GET /api/books/:id
**Lấy thông tin chi tiết một sách**

```http
GET http://26.30.78.80:5185/api/books/1
```

**Response (200 OK):**
```json
{
  "id": 1,
  "tenSach": "Lão Hạc",
  "tacGia": "Nam Cao",
  "nhaSanXuat": "NXB Văn Học",
  "soLuong": 20,
  "soBanDaMuon": 0,
  "soBanConLai": 20,
  "trangThai": "Có thể mượn"
}
```

**Response (404 Not Found):**
```json
{
  "error": "Book not found"
}
```

---

### 3️⃣ POST /api/books
**Thêm sách mới**

```http
POST http://26.30.78.80:5185/api/books
Content-Type: application/json

{
  "tenSach": "Truyện Kiều",
  "tacGia": "Nguyễn Du",
  "nhaSanXuat": "NXB Văn Học",
  "soLuong": 25,
  "soBanDaMuon": 5
}
```

**Response (201 Created):**
```json
{
  "id": 4,
  "tenSach": "Truyện Kiều",
  "tacGia": "Nguyễn Du",
  "nhaSanXuat": "NXB Văn Học",
  "soLuong": 25,
  "soBanDaMuon": 5,
  "soBanConLai": 20,
  "trangThai": "Có thể mượn"
}
```

---

### 4️⃣ PUT /api/books/:id
**Cập nhật thông tin sách**

```http
PUT http://26.30.78.80:5185/api/books/1
Content-Type: application/json

{
  "id": 1,
  "tenSach": "Lão Hạc (Bản sửa)",
  "tacGia": "Nam Cao",
  "nhaSanXuat": "NXB Văn Học",
  "soLuong": 30,
  "soBanDaMuon": 10
}
```

**Response (204 No Content):**
```
(Không có body, chỉ status code)
```

---

### 5️⃣ POST /api/books/:id/borrow
**Tăng số lượng đã mượn khi độc giả mượn sách**

```http
POST http://26.30.78.80:5185/api/books/3/borrow
Content-Type: application/json

{
  "quantity": 1
}
```

**Response (200 OK):**
```json
{
  "id": 3,
  "tenSach": "Lão Hạc",
  "tacGia": "Nam Cao",
  "nhaSanXuat": "NXB Văn Học",
  "soLuong": 20,
  "soBanDaMuon": 1,
  "soBanConLai": 19,
  "trangThai": "Có thể mượn"
}
```

**Lưu ý:**
- `quantity` phải là số nguyên dương
- Không thể mượn quá số lượng còn lại

---

### 6️⃣ POST /api/books/:id/return
**Giảm số lượng đã mượn khi trả sách**

```http
POST http://26.30.78.80:5185/api/books/3/return
Content-Type: application/json

{
  "quantity": 1
}
```

**Response (200 OK):**
```json
{
  "id": 3,
  "tenSach": "Lão Hạc",
  "tacGia": "Nam Cao",
  "nhaSanXuat": "NXB Văn Học",
  "soLuong": 20,
  "soBanDaMuon": 0,
  "soBanConLai": 20,
  "trangThai": "Có thể mượn"
}
```

**Lưu ý:**
- `quantity` phải là số nguyên dương
- Không thể trả quá số lượng đã mượn

---

### 7️⃣ DELETE /api/books/:id
**Xóa sách**

```http
DELETE http://26.30.78.80:5185/api/books/1
```

**Response (204 No Content):**
```
(Không có body, chỉ status code)
```

---

### ⭐ Bonus: GET /api/books/products
**Lấy danh sách sách với tên trường khác (tùy chọn)**

```http
GET http://26.30.78.80:5185/api/books/products
```

**Response:**
```json
[
  {
    "ma": "1",
    "tenSanPham": "Lão Hạc",
    "tacGia": "Nam Cao",
    "nhaSanXuat": "NXB Văn Học",
    "sl": 20
  }
]
```

---

## 🧪 Kiểm Tra API bằng Curl

### 1. Lấy danh sách sách

```bash
curl -X GET "http://26.30.78.80:5185/api/books" \
  -H "Content-Type: application/json"
```

### 2. Lấy sách theo ID

```bash
curl -X GET "http://26.30.78.80:5185/api/books/1" \
  -H "Content-Type: application/json"
```

### 3. Thêm sách mới

```bash
curl -X POST "http://26.30.78.80:5185/api/books" \
  -H "Content-Type: application/json" \
  -d '{
    "tenSach": "Sách mới",
    "tacGia": "Tác giả",
    "nhaSanXuat": "NXB",
    "soLuong": 10,
    "soBanDaMuon": 0
  }'
```

### 4. Cập nhật sách

```bash
curl -X PUT "http://26.30.78.80:5185/api/books/1" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "tenSach": "Tên mới",
    "tacGia": "Tác giả",
    "nhaSanXuat": "NXB",
    "soLuong": 15,
    "soBanDaMuon": 2
  }'
```

### 5. Xóa sách

```bash
curl -X DELETE "http://26.30.78.80:5185/api/books/1"
```

---

## 📋 HTTP Status Codes

| Code | Mô tả |
|------|-------|
| `200 OK` | Thành công (GET, PUT, DELETE) |
| `201 Created` | Tạo mới thành công (POST) |
| `204 No Content` | Thành công, không có nội dung (PUT, DELETE) |
| `400 Bad Request` | Lỗi dữ liệu đầu vào |
| `404 Not Found` | Không tìm thấy |
| `500 Internal Server Error` | Lỗi server |

---

## 🛡️ CORS & Headers

**CORS Policy:**
```
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, POST, PUT, DELETE
Access-Control-Allow-Headers: Content-Type
```

**Response Headers:**
```
Content-Type: application/json; charset=utf-8
```

---

## 📝 Seed Data (Dữ liệu mẫu hiện có)

Backend đã có sẵn 3 sách mẫu:

```json
[
  {
    "id": 3,
    "tenSach": "Lão Hạc",
    "tacGia": "Nam Cao",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 20,
    "soBanDaMuon": 0,
    "soBanConLai": 20,
    "trangThai": "Có thể mượn"
  },
  {
    "id": 4,
    "tenSach": "Số Đỏ",
    "tacGia": "Vũ Trọng Phụng",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 15,
    "soBanDaMuon": 2,
    "soBanConLai": 13,
    "trangThai": "Có thể mượn"
  },
  {
    "id": 5,
    "tenSach": "Truyện Kiều",
    "tacGia": "Nguyễn Du",
    "nhaSanXuat": "NXB Văn Học",
    "soLuong": 25,
    "soBanDaMuon": 5,
    "soBanConLai": 20,
    "trangThai": "Có thể mượn"
  }
]
```

---

## 🚀 Cách Kết Nối Frontend với Backend này

### Frontend (.env.local)

```env
VITE_API_URL=http://26.30.78.80:5185/api/books
```

hoặc (nếu chạy local)

```env
VITE_API_URL=http://localhost:5185/api/books
```

### Chạy Frontend

```bash
cd frontend
npm run dev
```

Frontend sẽ tự động kết nối tới backend và hiển thị danh sách sách.

---

## 📞 Liên Hệ

- **Backend Tech:** .NET 8 (C#), Entity Framework Core, SQL Server
- **API Framework:** ASP.NET Core Web API
- **Port:** 5185
- **CORS:** Đã enabled

---

## ✅ Checklist

- [x] Backend API URL: `http://26.30.78.80:5185`
- [x] Endpoint GET /api/books: ✅ Hoạt động
- [x] Endpoint GET /api/books/:id: ✅ Hoạt động
- [x] Endpoint POST /api/books: ✅ Hoạt động
- [x] Endpoint PUT /api/books/:id: ✅ Hoạt động
- [x] Endpoint DELETE /api/books/:id: ✅ Hoạt động
- [x] CORS enabled: ✅ Có
- [x] Seed data: ✅ 3 sách mẫu
- [x] JSON format: ✅ Chuẩn

---

**Ghi chú:** Backend này là phiên bản demo. Nếu cần tùy chỉnh cấu trúc API hoặc thêm endpoint khác, vui lòng liên hệ.
