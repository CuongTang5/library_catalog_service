<template>
  <div class="library-page">
    <aside class="left-bar">
      <div class="admin-top">
        <div class="admin-avatar">C</div>
        <div class="admin-meta">
          <strong>CuongTang</strong>
          <span>cuongtang@smartlib.net</span>
        </div>
      </div>
      <div class="admin-badge">ADMIN PORTAL</div>
      <div class="admin-title">HỆ THỐNG ADMIN</div>
      <nav class="admin-menu">
        <RouterLink to="/" class="menu-item" active-class="active" exact>
          <span class="menu-icon">🏠</span>
          <span>Overview Dashboard</span>
        </RouterLink>
        <RouterLink to="/books" class="menu-item" active-class="active">
          <span class="menu-icon">📚</span>
          <span>Danh mục Sách (NT)</span>
        </RouterLink>
        <button class="menu-item">
          <span class="menu-icon">📜</span>
          <span>Quy tắc mượn trả</span>
        </button>
        <button class="menu-item">
          <span class="menu-icon">👥</span>
          <span>Quản lý Độc giả</span>
        </button>
        <button class="menu-item">
          <span class="menu-icon">💳</span>
          <span>Thẻ Thư viện</span>
        </button>
      </nav>
      <button class="logout-btn">Logout</button>
    </aside>

    <main class="content">
      <div class="top-search">
        <div class="search-box">
          🔍
          <input v-model="search" placeholder="Tìm kiếm sách, tác giả, nhà xuất bản..." />
        </div>
        <button class="search-btn">Tìm kiếm</button>
        <div class="cart">🛒<b>3</b></div>
      </div>

      <div class="categories">
        <div class="cat" v-for="c in categories" :key="c.name">
          <div class="cat-icon">{{ c.icon }}</div>
          <span>{{ c.name }}</span>
        </div>
      </div>

      <div class="section-head">
        <h2>Phổ biến</h2>
        <RouterLink to="/books" class="view-all-btn">Xem kho sách đầy đủ</RouterLink>
      </div>

      <div class="book-grid">
        <div class="book-card" v-for="book in filteredBooks.slice(0, 10)" :key="book.id">
          <div class="cover-wrap">
            <img :src="getCover(book.id)" />
            <div class="bookmark"></div>
          </div>
          <h3>{{ book.tenSach }}</h3>
          <p>{{ book.tacGia }}</p>
          <button class="detail-btn" @click="openModal(book)">Xem chi tiết</button>
        </div>
      </div>

      <section class="banner">
        <div class="book-stack">📚</div>
        <div>
          <h2>Catalog Service - Thư viện số</h2>
          <p>Quản lý đầu sách, số bản sao, tìm kiếm và trạng thái có thể mượn.</p>
          <RouterLink to="/books" class="banner-btn">👁 Xem kho sách</RouterLink>
        </div>
        <div class="stats">
          <div>
            <b>{{ books.length }}</b>
            <span>Tổng đầu sách</span>
          </div>
          <div>
            <b>{{ totalCopies }}</b>
            <span>Tổng bản sao</span>
</div>
          <div>
            <b>{{ totalAvailable }}</b>
            <span>Có thể mượn</span>
          </div>
        </div>
      </section>
    </main>

    <!-- MODAL BACKDROP -->
    <div v-if="selectedBookModal" class="modal-backdrop" @click="closeModal"></div>

    <!-- MODAL DETAIL -->
    <div v-if="selectedBookModal" class="modal-detail">
      <div class="modal-content">
        <button class="modal-close" @click="closeModal">✕</button>
        <img :src="getCover(selectedBookModal.id)" class="modal-cover" />
        <h2>{{ selectedBookModal.tenSach }}</h2>
        <p class="modal-author">{{ selectedBookModal.tacGia }}</p>
        <div class="modal-info">
          <div class="info-row">
            <span>Nhà xuất bản:</span>
            <b>{{ selectedBookModal.nhaSanXuat }}</b>
          </div>
          <div class="info-row">
            <span>Số lượng:</span>
            <b>{{ selectedBookModal.soLuong }}</b>
          </div>
          <div class="info-row">
            <span>Số bản đã mượn:</span>
            <b>{{ selectedBookModal.soBanDaMuon ?? 0 }}</b>
          </div>
          <div class="info-row">
            <span>Số bản còn lại:</span>
            <b>{{ getAvailable(selectedBookModal) }}</b>
          </div>
          <div class="info-row">
            <span>Trạng thái:</span>
            <b :class="getAvailable(selectedBookModal) > 0 ? 'status-ok' : 'status-bad'">
              {{ getAvailable(selectedBookModal) > 0 ? 'Có thể mượn' : 'Hết sách' }}
            </b>
          </div>
        </div>
        <div class="modal-buttons">
          <button class="btn-close" @click="closeModal">Đóng</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const API_URL = `http://${window.location.hostname}:5185/api/books`

const books = ref([])
const search = ref('')
const selectedBookModal = ref(null)

const categories = [
  { name: 'Tất cả', icon: '📚' },
  { name: 'Sách điện tử', icon: '📱' },
  { name: 'Mới', icon: '🆕' },
  { name: 'Bán chạy', icon: '⭐' },
  { name: 'Sách nói', icon: '▶️' },
  { name: 'Tiểu thuyết', icon: '🪐' },
  { name: 'Lãng mạn', icon: '💛' },
  { name: 'Kỳ ảo', icon: '🔷' },
  { name: 'Trinh thám', icon: '🖌' }
]

const covers = [
  'https://covers.openlibrary.org/b/isbn/9780143127550-L.jpg',
  'https://covers.openlibrary.org/b/isbn/9780439708180-L.jpg',
  'https://covers.openlibrary.org/b/isbn/9780061120084-L.jpg',
  'https://covers.openlibrary.org/b/isbn/9780743273565-L.jpg',
  'https://covers.openlibrary.org/b/isbn/9780451524935-L.jpg',
  'https://covers.openlibrary.org/b/isbn/9780547928227-L.jpg',
  'https://covers.openlibrary.org/b/isbn/9780132350884-L.jpg',
  'https://covers.openlibrary.org/b/isbn/9780201633610-L.jpg'
]

const getCover = (id) => covers[id % covers.length]

const loadBooks = async () => {
  const res = await fetch(API_URL)
  books.value = await res.json()
}
const getAvailable = (book) =>
  book.soBanConLai ?? book.soLuong - (book.soBanDaMuon ?? 0)

const filteredBooks = computed(() => {
  const q = search.value.toLowerCase()
  return books.value.filter(b =>
    b.tenSach?.toLowerCase().includes(q) ||
    b.tacGia?.toLowerCase().includes(q) ||
    b.nhaSanXuat?.toLowerCase().includes(q)
  )
})

const totalCopies = computed(() =>
  books.value.reduce((s, b) => s + Number(b.soLuong || 0), 0)
)

const totalAvailable = computed(() =>
  books.value.reduce((s, b) => s + getAvailable(b), 0)
)

const openModal = (book) => { selectedBookModal.value = book }
const closeModal = () => { selectedBookModal.value = null }

onMounted(loadBooks)
</script>

<style scoped>
.library-page {
  min-height: 100vh;
  display: grid;
  grid-template-columns: 300px 1fr;
  background: #d4c2b2;
  font-family: 'Segoe UI', sans-serif;
  color: #58443a;
}

.left-bar {
  background: #0d4a42;
  padding: 32px 24px;
  display: flex;
  flex-direction: column;
  gap: 24px;
  border-radius: 0 30px 30px 0;
}

.admin-top { display: flex; align-items: center; gap: 16px; }

.admin-avatar {
  width: 60px; height: 60px; border-radius: 50%;
  background: #ebf6f2; color: #0d4a42;
  display: grid; place-items: center;
  font-weight: bold; font-size: 20px;
}

.admin-meta strong { display: block; color: #fff; font-size: 16px; margin-bottom: 4px; }
.admin-meta span { color: #b9d9d1; font-size: 13px; }

.admin-badge {
  display: inline-flex; align-items: center; justify-content: center;
  padding: 10px 14px; border-radius: 999px; background: #176f63;
  color: #e6fbf4; font-size: 12px; letter-spacing: .4px; width: fit-content;
}

.admin-title { color: #9bd0c7; font-size: 12px; letter-spacing: 1px; text-transform: uppercase; margin-top: 8px; }

.admin-menu { display: grid; gap: 10px; margin-top: 10px; }

.menu-item {
  display: flex; align-items: center; gap: 14px; width: 100%;
  border: none; border-radius: 18px; padding: 16px 18px;
  background: transparent; color: #c3dad5;
  text-align: left; font-size: 15px; cursor: pointer;
  transition: background .2s, color .2s;
  text-decoration: none;
}

.menu-item:hover { background: rgba(255,255,255,.08); }
.menu-item.active { background: #ffffff; color: #0d4a42; font-weight: 700; }

.menu-icon { width: 28px; height: 28px; display: grid; place-items: center; font-size: 16px; }

.logout-btn {
  margin-top: auto; width: 100%;
  border: 1px solid rgba(255,255,255,.3); background: transparent;
  color: #fff; border-radius: 20px; padding: 14px 18px; font-weight: 700; cursor: pointer;
}

.content { background: #fffaf3; padding: 40px; overflow-y: auto; }

.top-search { display: grid; grid-template-columns: 1fr 180px 70px; gap: 20px; align-items: center; }

.search-box {
  height: 70px; border-radius: 35px; background: #f5eee5;
  display: flex; align-items: center; gap: 15px; padding: 0 25px;
  box-shadow: inset 0 2px 8px rgba(0,0,0,.05);
}
.search-box input { border: none; outline: none; background: transparent; font-size: 18px; width: 100%; }

.search-btn {
  height: 70px; border: none; border-radius: 35px;
  color: white; font-weight: bold; font-size: 18px;
  background: #0d4a42; cursor: pointer;
}

.cart { position: relative; cursor: pointer; }
.cart b {
  position: absolute; top: 0; right: 0;
  background: #ff4d4d; color: white;
  border-radius: 50%; padding: 3px 7px; font-size: 12px;
}

.categories {
  display: grid; grid-template-columns: repeat(9, 1fr);
  gap: 20px; margin: 35px 0; text-align: center;
}

.cat-icon { font-size: 38px; margin-bottom: 8px; }

.section-head { display: flex; justify-content: space-between; align-items: center; }

.view-all-btn {
  border: none; padding: 15px 28px; border-radius: 25px;
  background: #f1e8dd; cursor: pointer; font-weight: bold;
  text-decoration: none; color: #58443a;
}

.book-grid {
  display: grid; grid-template-columns: repeat(5, 1fr);
  gap: 35px; margin-top: 25px;
}

.book-card { cursor: pointer; transition: .2s; }
.book-card:hover { transform: translateY(-8px); }

.cover-wrap { position: relative; height: 260px; }
.cover-wrap img {
  width: 100%; height: 240px; object-fit: cover;
  border-radius: 12px; box-shadow: 0 12px 25px rgba(0,0,0,.18);
}

.bookmark {
  position: absolute; right: 22px; bottom: 0;
  width: 24px; height: 65px; background: #f6a632;
  clip-path: polygon(0 0,100% 0,100% 100%,50% 80%,0 100%);
}

.book-card h3 { margin: 12px 0 5px; font-size: 16px; }
.book-card p { color: #e54747; margin: 0 0 10px; }

.detail-btn {
  width: 100%; border: none; padding: 10px 14px;
  border-radius: 16px; background: #0d4a42; color: white;
  font-weight: bold; cursor: pointer;
}

.banner {
  margin-top: 35px; background: #efe4d7; border-radius: 30px;
  padding: 35px; display: grid;
  grid-template-columns: 220px 1fr 300px;
  align-items: center; gap: 30px;
}

.book-stack { font-size: 130px; }

.banner-btn {
  display: inline-block; border: none; padding: 14px 30px;
  border-radius: 25px; background: #0d4a42; color: white;
  font-weight: bold; cursor: pointer; text-decoration: none;
  margin-top: 12px;
}

.stats { display: grid; gap: 15px; }
.stats div { background: white; border-radius: 18px; padding: 16px; }
.stats b { font-size: 28px; display: block; }

/* MODAL */
.modal-backdrop {
  position: fixed; top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5); z-index: 999;
}

.modal-detail {
  position: fixed; top: 50%; left: 50%;
  transform: translate(-50%, -50%);
  background: white; border-radius: 30px; padding: 40px;
  max-width: 500px; width: 90%; max-height: 80vh; overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0,0,0,.3); z-index: 1000;
}

.modal-close {
  position: absolute; top: 15px; right: 15px;
  width: 40px; height: 40px; border: none;
  background: #f1e8dd; border-radius: 50%; cursor: pointer; font-size: 24px;
}
.modal-cover { width: 100%; height: 300px; object-fit: cover; border-radius: 20px; margin-bottom: 20px; }
.modal-detail h2 { margin: 15px 0 5px; font-size: 24px; }
.modal-author { color: #e54747; font-weight: bold; margin-bottom: 20px; }

.modal-info { background: #f5eee5; border-radius: 20px; padding: 20px; margin-bottom: 25px; }
.info-row { display: flex; justify-content: space-between; padding: 10px 0; border-bottom: 1px solid #eadccc; }
.info-row:last-child { border-bottom: none; }

.status-ok { color: #16843b; background: #dff5e7; padding: 4px 10px; border-radius: 12px; }
.status-bad { color: #d32222; background: #ffe0e0; padding: 4px 10px; border-radius: 12px; }

.modal-buttons { display: flex; gap: 10px; }
.btn-close { flex: 1; border: none; padding: 12px; border-radius: 20px; cursor: pointer; font-weight: bold; background: #f1e8dd; }

@media (max-width: 1200px) {
  .book-grid { grid-template-columns: repeat(2, 1fr); }
  .categories { grid-template-columns: repeat(3, 1fr); }
  .banner { grid-template-columns: 1fr; }
}
</style>