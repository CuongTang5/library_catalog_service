# 📧 Email Mẫu Gửi cho Nhóm N1

## Template 1: Email ngắn gọn

---

**Tiêu đề:** Backend API Info - Catalog Service

**Nội dung:**

```
Xin chào nhóm N1,

Dưới đây là thông tin Backend API của chúng tôi (Catalog Service) mà nhóm bạn có thể kết nối:

📍 Backend URL:
   http://26.30.78.80:5185

📊 Cấu trúc dữ liệu sách (JSON):
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

🔌 Các endpoint API:
- GET    /api/books           → Danh sách tất cả sách
- GET    /api/books/:id       → Chi tiết sách
- POST   /api/books           → Thêm sách mới
- PUT    /api/books/:id       → Cập nhật sách
- DELETE /api/books/:id       → Xóa sách

✅ Đặc điểm:
- CORS: Enabled (Accept all origins)
- Response: JSON format
- Backend: .NET 8 + Entity Framework Core
- Seed data: 3 sách mẫu sẵn có

📝 Test nhanh:
curl http://26.30.78.80:5185/api/books

Xin vui lòng xác nhận khi nhóm bạn có thể kết nối được.
Nếu có câu hỏi, liên hệ bất cứ lúc nào.

Cảm ơn!
```

---

## Template 2: Message ngắn (Zalo/Messenger)

```
Hi N1,

Gửi URL backend cho em:
🔗 http://26.30.78.80:5185

API endpoint: /api/books
CORS: Enabled

JSON format sách:
{id, tenSach, tacGia, nhaSanXuat, soLuong, soBanDaMuon, soBanConLai, trangThai}

Endpoint:
- GET /api/books
- GET /api/books/:id
- POST /api/books
- PUT /api/books/:id
- DELETE /api/books/:id

Có 3 sách mẫu sẵn rồi. Các anh test thử nhé!

Test: curl http://26.30.78.80:5185/api/books
```

---

## Template 3: Chi tiết đầy đủ

Gửi file **BACKEND_API_SEND_TO_N1.md** kèm theo email hoặc file này:

```
Xin chào nhóm N1,

Dưới đây là thông tin API Backend của nhóm chúng tôi mà nhóm bạn cần để kết nối frontend:

📎 **File đính kèm:** BACKEND_API_SEND_TO_N1.md (Chi tiết đầy đủ)

🔑 **Thông tin tóm tắt:**

URL Backend:        http://26.30.78.80:5185
API Base:           /api/books
Kiểu dữ liệu:       JSON (application/json)
CORS:               ✅ Enabled
HTTP Methods:       GET, POST, PUT, DELETE
Response Codes:     200, 201, 204, 404, 500

🧪 Test nhanh:
```bash
# Lấy danh sách sách
curl -X GET http://26.30.78.80:5185/api/books

# Lấy sách theo ID
curl -X GET http://26.30.78.80:5185/api/books/1

# Thêm sách mới
curl -X POST http://26.30.78.80:5185/api/books \
  -H "Content-Type: application/json" \
  -d '{"tenSach":"Sách mới","tacGia":"Tác giả","nhaSanXuat":"NXB","soLuong":10,"soBanDaMuon":0}'
```

Vui lòng xem file BACKEND_API_SEND_TO_N1.md để biết chi tiết về:
- Cấu trúc JSON từng trường
- Các endpoint và ví dụ response
- Status codes
- Seed data mẫu

Khi nhóm bạn kết nối được, vui lòng xác nhận.

Cảm ơn!
```

---

## Nơi tìm file:

File **BACKEND_API_SEND_TO_N1.md** nằm ở:
```
d:\fullstack\btl\BACKEND_API_SEND_TO_N1.md
```

---

## Lưu ý:

1. ✅ Chắc chắn backend đang chạy (dotnet run)
2. ✅ Firewall cho phép port 5185
3. ✅ IP server đúng (26.30.78.80 hoặc localhost nếu test local)
4. ✅ CORS đã bật trong Program.cs

---

## Sau khi gửi:

Chờ nhóm N1 test và xác nhận khi họ:
- [ ] Kết nối được tới http://26.30.78.80:5185
- [ ] Lấy được danh sách sách từ GET /api/books
- [ ] Có thể test POST/PUT/DELETE (nếu cần)

---

**Ghi chú thêm:**

Nếu nhóm N1 báo lỗi CORS, hãy kiểm tra Program.cs có:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors();
```

✅ Phần này đã setup trong backend hiện tại.
