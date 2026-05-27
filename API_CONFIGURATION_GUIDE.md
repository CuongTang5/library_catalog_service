# 🔌 API Configuration & Backend Integration Guide

## Tổng Quan

Frontend được thiết kế để dễ dàng kết nối với bất kỳ backend nào. Có 3 cách để cấu hình Backend API URL:

1. **Biến môi trường** (ưu tiên cao) - dùng cho development/production
2. **Runtime config** - thay đổi động khi chạy
3. **Default** - tự động dùng hostname của frontend

---

## Cách 1: Sử dụng Biến Môi Trường (Recommended)

### Setup cho Development (Local)

1. Tạo file `.env.local` trong thư mục `frontend/`:

```bash
cd frontend
cp .env.example .env.local
```

2. Sửa `.env.local`:

```env
# Local development
VITE_API_URL=http://localhost:5185/api/books

# Hoặc với IP LAN server
VITE_API_URL=http://172.16.7.197:5185/api/books

# Hoặc với backend từ nhóm N1
VITE_API_URL=http://26.30.78.80:3000/api/books
```

3. Khởi động frontend:

```bash
npm run dev
```

### Setup cho Production

1. Tạo file `.env.production`:

```bash
cd frontend
cat > .env.production << EOF
VITE_API_URL=http://26.30.78.80:3000/api/books
EOF
```

2. Build frontend:

```bash
npm run build
```

---

## Cách 2: Runtime Configuration (Động)

### Thay đổi API URL khi chạy (không cần reload)

Thêm code vào `frontend/src/main.js` hoặc console:

```javascript
// Cách 1: Set window.API_CONFIG
window.API_CONFIG = {
  BASE_URL: 'http://26.30.78.80:3000'
}

// Sau đó reload trang hoặc component sẽ tự lấy URL mới
```

### Hoặc thêm button để switch backend (tùy chọn)

File: `frontend/src/components/ApiSwitcher.vue` (tạo mới):

```vue
<template>
  <div class="api-switcher">
    <label>Backend URL:</label>
    <input v-model="apiUrl" type="text" />
    <button @click="switchBackend">Kết nối</button>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const apiUrl = ref(window.API_CONFIG?.BASE_URL || 'http://localhost:5185')

const switchBackend = () => {
  window.API_CONFIG = { BASE_URL: apiUrl.value }
  window.location.reload()
}
</script>

<style scoped>
.api-switcher {
  display: flex;
  gap: 10px;
  padding: 10px;
  background: #f0f0f0;
}

input {
  flex: 1;
  padding: 5px;
}
</style>
```

---

## Cách 3: Default Behavior (Tự Động)

Nếu không cấu hình gì, frontend sẽ tự động dùng:

```javascript
// window.location.hostname = host của frontend
// port = 5185 (backend mặc định)
const url = `http://${window.location.hostname}:5185`
```

**Ví dụ:**
- Frontend ở `http://localhost:5174/` → Backend sẽ là `http://localhost:5185`
- Frontend ở `http://26.30.78.80:5174/` → Backend sẽ là `http://26.30.78.80:5185`

---

## Cách Sử dụng API Config trong Components

### Cách cũ (Hardcode URL)

```javascript
const API_URL = 'http://172.16.7.197:5185/api/books'

const loadBooks = async () => {
  const res = await fetch(API_URL)
  books.value = await res.json()
}
```

### Cách mới (Sử dụng API Config)

```javascript
import { getApiUrl, fetchApi, API_CONFIG } from '@/config/api.config.js'

// Cách 1: Sử dụng helper function
const loadBooks = async () => {
  try {
    const url = getApiUrl(API_CONFIG.BOOKS.LIST)
    const books = await fetch(url).then(r => r.json())
    return books
  } catch (error) {
    console.error('Failed to load books:', error)
  }
}

// Cách 2: Sử dụng fetchApi helper
const loadBooks = async () => {
  try {
    const books = await fetchApi(API_CONFIG.BOOKS.LIST)
    return books
  } catch (error) {
    console.error('Failed to load books:', error)
  }
}

// Cách 3: Trực tiếp dùng API_CONFIG.BASE_URL
const API_URL = `${API_CONFIG.BASE_URL}${API_CONFIG.BOOKS.LIST}`
```

---

## Update Frontend Views để sử dụng API Config

### BooksView.vue

Thay đổi từ:

```javascript
const API_URL = 'http://172.16.7.197:5185/api/books'
```

Thành:

```javascript
import { getApiUrl, API_CONFIG } from '@/config/api.config.js'

const API_URL = getApiUrl(API_CONFIG.BOOKS.LIST)
// hoặc: const API_URL = `${API_CONFIG.BASE_URL}${API_CONFIG.BOOKS.LIST}`
```

---

## Checklist: Kết nối Backend từ Nhóm N1

- [ ] Nhóm N1 cung cấp Backend API URL (ví dụ: `http://26.30.78.80:3000`)
- [ ] Nhóm N1 xác nhận API hỗ trợ CORS
- [ ] Nhóm N1 cung cấp seed data (ít nhất 3 sách)
- [ ] Kiểm tra endpoint `GET /api/books` trả về JSON đúng cấu trúc
- [ ] Tạo `.env.local` hoặc `.env.production` với URL từ N1:
  ```
  VITE_API_URL=http://26.30.78.80:3000
  ```
- [ ] Chạy `npm run dev` hoặc `npm run build`
- [ ] Kiểm tra danh sách sách hiển thị từ backend N1

---

## Troubleshooting

### Frontend hiển thị "No data" hoặc trống

**Nguyên nhân 1: CORS Error**
- Kiểm tra Console browser (F12)
- Nếu thấy lỗi CORS, yêu cầu nhóm N1 enable CORS headers

**Nguyên nhân 2: Backend URL sai**
- Kiểm tra `.env.local` có đúng URL không
- Kiểm tra backend có chạy không: `curl http://26.30.78.80:3000/api/books`

**Nguyên nhân 3: Firewall/Network**
- Kiểm tra ping tới server backend
- Kiểm tra port backend có open không

### Cách debug

1. Mở DevTools (F12) → Console
2. Chạy:
   ```javascript
   // Kiểm tra config hiện tại
   console.log(window.API_CONFIG)
   
   // Test API directly
   fetch('http://26.30.78.80:3000/api/books')
     .then(r => r.json())
     .then(d => console.log(d))
   ```

---

## Tối ưu: Sử dụng .env cho mỗi environment

```bash
# Development
.env.local (ignored by git)
VITE_API_URL=http://localhost:5185

# Production
.env.production
VITE_API_URL=http://26.30.78.80:3000

# Staging
.env.staging
VITE_API_URL=http://staging-api.n1.local:8080
```

Chạy:
```bash
npm run dev              # dùng .env.local
npm run build            # dùng .env.production
npm run build:staging    # dùng .env.staging (nếu cấu hình)
```

---

## Tài liệu thêm

- [API Requirements từ N1](./API_REQUIREMENTS_N1.md)
- [Frontend Router Config](./src/router/index.js)
- [Views sử dụng API](./src/views/)
