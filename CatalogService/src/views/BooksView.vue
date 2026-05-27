<template>
  <div class="page-shell">
    <div class="page-header">
      <h1>Catalog Service - Quản lý đầu sách</h1>
    </div>

    <div class="controls-row">
      <input
        v-model="query"
        @input="applyFilter"
        class="search"
        placeholder="Tìm theo tên sách hoặc tác giả..."
      />
      <div class="header-actions">
        <button class="btn btn-primary" @click="startCreate">Thêm sách</button>
      </div>
    </div>

    <div class="content-grid">
      <transition name="fade">
        <section v-if="showForm" class="panel panel-form">
          <h2>{{ editMode ? 'Chỉnh sửa sách' : 'Thêm sách mới' }}</h2>
          <form @submit.prevent="saveBook">
            <div class="form-row">
              <div class="form-group">
                <label for="tenSach">Tên sách</label>
                <input id="tenSach" v-model="form.tenSach" type="text" placeholder="Tên sách" required />
              </div>
              <div class="form-group">
                <label for="tacGia">Tác giả</label>
                <input id="tacGia" v-model="form.tacGia" type="text" placeholder="Tác giả" required />
              </div>
            </div>

            <div class="form-row">
              <div class="form-group">
                <label for="nhaSanXuat">Nhà xuất bản</label>
                <input id="nhaSanXuat" v-model="form.nhaSanXuat" type="text" placeholder="Nhà xuất bản" />
              </div>
              <div class="form-group">
                <label for="soLuong">Số lượng</label>
                <input id="soLuong" v-model.number="form.soLuong" type="number" min="0" required />
              </div>
            </div>

            <div class="form-row small">
              <div class="form-group">
                <label for="soBanDaMuon">Số bản đã mượn</label>
                <input id="soBanDaMuon" v-model.number="form.soBanDaMuon" type="number" min="0" />
              </div>
              <div class="form-group">
                <label for="trangThai">Trạng thái</label>
                <input id="trangThai" v-model="form.trangThai" type="text" readonly />
              </div>
            </div>

            <div class="form-actions">
              <button type="submit" class="btn btn-primary">Lưu</button>
              <button type="button" class="btn btn-ghost" @click="cancelEdit">Hủy</button>
            </div>
          </form>
        </section>
      </transition>

      <section class="panel panel-table">
        <div class="panel-header">
          <h2>Danh sách sách</h2>
          <div class="stats">
            <div class="stat-item">Tổng: <strong>{{ filteredBooks.length }}</strong></div>
            <div class="stat-item">Đang tải: <strong>{{ loading ? 'Đang...' : 'Sẵn sàng' }}</strong></div>
          </div>
        </div>

        <div class="alert error" v-if="error">
          <span>{{ error }}</span>
          <button class="btn-error" @click="error = ''">Đóng</button>
        </div>

        <div class="table-wrap">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Tên sách</th>
                <th>Tác giả</th>
                <th>Nhà xuất bản</th>
                <th>Số lượng</th>
                <th>Đã mượn</th>
                <th>Còn lại</th>
                <th>Trạng thái</th>
                <th>Hành động</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="loading">
                <td colspan="9" class="loading-row">Đang tải dữ liệu...</td>
              </tr>
              <tr v-else-if="filteredBooks.length === 0">
                <td colspan="9" class="empty-row">Không tìm thấy kết quả.</td>
              </tr>
              <tr v-else v-for="book in filteredBooks" :key="book.id">
                <td>{{ book.id }}</td>
                <td>{{ book.tenSach }}</td>
                <td>{{ book.tacGia }}</td>
                <td>{{ book.nhaSanXuat }}</td>
                <td>{{ book.soLuong }}</td>
                <td>{{ book.soBanDaMuon }}</td>
                <td>{{ book.soBanConLai }}</td>
                <td>
                  <span class="badge" :class="book.soBanConLai > 0 ? 'available' : 'unavailable'">{{ book.trangThai }}</span>
                </td>
                <td class="actions-col">
                  <button class="btn btn-warning" @click="startEdit(book)">Sửa</button>
                  <button class="btn btn-danger" @click="confirmDelete(book.id)">Xóa</button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue';

interface Book {
  id?: number;
  tenSach: string;
  tacGia: string;
  nhaSanXuat?: string;
  soLuong: number;
  soBanDaMuon?: number;
  soBanConLai?: number;
  trangThai?: string;
}

const apiUrl = 'http://192.168.29.16:5185/api/books';

const books = ref<Book[]>([]);
const loading = ref(false);
const error = ref('');
const query = ref('');
const showForm = ref(false);
const editMode = ref(false);
const editingId = ref<number | null>(null);

const emptyForm = (): Book => ({ tenSach: '', tacGia: '', nhaSanXuat: '', soLuong: 1, soBanDaMuon: 0, soBanConLai: 1, trangThai: 'Có thể mượn' });
const form = reactive<Book>(emptyForm());

function normalizeBook(b: any): Book {
  const soBanDaMuon = typeof b.soBanDaMuon === 'number' ? b.soBanDaMuon : 0;
  const soBanConLai = typeof b.soBanConLai === 'number' ? b.soBanConLai : ( (typeof b.soLuong === 'number' ? b.soLuong : 0) - soBanDaMuon );
  const trangThai = typeof b.trangThai === 'string' ? b.trangThai : (soBanConLai > 0 ? 'Có thể mượn' : 'Hết sách');
  return {
    id: b.id,
    tenSach: b.tenSach ?? '',
    tacGia: b.tacGia ?? '',
    nhaSanXuat: b.nhaSanXuat ?? '',
    soLuong: typeof b.soLuong === 'number' ? b.soLuong : 0,
    soBanDaMuon,
    soBanConLai,
    trangThai
  };
}

const fetchBooks = async () => {
  loading.value = true;
  error.value = '';
  try {
    const res = await fetch(apiUrl);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    const data = await res.json();
    books.value = Array.isArray(data) ? data.map(normalizeBook) : [];
  } catch (err) {
    error.value = 'Không thể kết nối đến API.';
  } finally {
    loading.value = false;
  }
};

const filteredBooks = computed(() => {
  const q = query.value.trim().toLowerCase();
  if (!q) return books.value;
  return books.value.filter(b => (b.tenSach || '').toLowerCase().includes(q) || (b.tacGia || '').toLowerCase().includes(q));
});

function applyFilter() {
  // computed does the filtering; this function exists for future debouncing
}

function startCreate() {
  Object.assign(form, emptyForm());
  editMode.value = false;
  editingId.value = null;
  showForm.value = true;
}

function startEdit(book: Book) {
  Object.assign(form, {
    id: book.id,
    tenSach: book.tenSach,
    tacGia: book.tacGia,
    nhaSanXuat: book.nhaSanXuat,
    soLuong: book.soLuong,
    soBanDaMuon: book.soBanDaMuon ?? 0,
    soBanConLai: book.soBanConLai ?? Math.max(0, (book.soLuong ?? 0) - (book.soBanDaMuon ?? 0)),
    trangThai: book.trangThai
  });
  editMode.value = true;
  editingId.value = book.id ?? null;
  showForm.value = true;
}

function cancelEdit() {
  showForm.value = false;
  editMode.value = false;
  editingId.value = null;
}

const saveBook = async () => {
  loading.value = true;
  error.value = '';
  try {
    const payload: any = {
      tenSach: form.tenSach,
      tacGia: form.tacGia,
      nhaSanXuat: form.nhaSanXuat,
      soLuong: Number(form.soLuong) || 0,
      soBanDaMuon: Number(form.soBanDaMuon) || 0
    };
    payload.soBanConLai = (payload.soLuong - payload.soBanDaMuon);
    payload.trangThai = payload.soBanConLai > 0 ? 'Có thể mượn' : 'Hết sách';

    let res;
    if (editMode.value && editingId.value != null) {
      res = await fetch(`${apiUrl}/${editingId.value}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
    } else {
      res = await fetch(apiUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      });
    }

    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    await fetchBooks();
    showForm.value = false;
  } catch (err) {
    error.value = 'Lưu thất bại. Kiểm tra kết nối đến API.';
  } finally {
    loading.value = false;
  }
};

const confirmDelete = (id?: number) => {
  if (!id) return;
  if (!window.confirm('Bạn có chắc muốn xóa sách này?')) return;
  deleteBook(id);
};

const deleteBook = async (id: number) => {
  loading.value = true;
  error.value = '';
  try {
    const res = await fetch(`${apiUrl}/${id}`, { method: 'DELETE' });
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    await fetchBooks();
  } catch (err) {
    error.value = 'Xóa thất bại. Kiểm tra kết nối đến API.';
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchBooks();
});
</script>

<style scoped>
.page-shell {
  min-height: 100vh;
  padding: 28px;
  background: linear-gradient(135deg, #0f172a 0%, #0ea5e9 60%);
  color: #0f172a;
}

.page-header {
  text-align: center;
  margin-bottom: 18px;
}

.page-header h1 {
  color: #ffffff;
  margin: 0;
  font-size: clamp(1.6rem, 2.6vw, 2.4rem);
}

.controls-row {
  display: flex;
  gap: 12px;
  align-items: center;
  margin-bottom: 20px;
}

.search {
  flex: 1;
  padding: 12px 14px;
  border-radius: 12px;
  border: none;
  font-size: 1rem;
  box-shadow: 0 6px 18px rgba(2,6,23,0.2);
}

.header-actions { display:flex; gap:12px; }

.content-grid {
  display: grid;
  gap: 20px;
  grid-template-columns: 1fr 2fr;
}

.panel { background: #ffffff; border-radius: 16px; padding: 18px; box-shadow: 0 12px 40px rgba(2,6,23,0.12); }

.panel-form { max-width: 720px; }

.panel-header { display:flex; justify-content:space-between; align-items:center; }

.form-row { display:flex; gap:12px; }
.form-row.small { gap:12px; }

.form-group { flex:1; display:flex; flex-direction:column; margin-bottom:12px; }
.form-group label { font-weight:600; margin-bottom:6px; }
.form-group input { padding:10px 12px; border-radius:10px; border:1px solid #e6eef8; }

.form-actions { display:flex; gap:12px; margin-top:12px; }

.btn { padding:10px 14px; border-radius:10px; cursor:pointer; border:none; font-weight:700; }
.btn-primary { background:linear-gradient(90deg,#2563eb,#06b6d4); color:#fff }
.btn-warning { background:#f59e0b; color:#fff }
.btn-danger { background:#ef4444; color:#fff }
.btn-ghost { background:transparent; border:1px solid #cbd5e1; }

.alert.error { background:#fee2e2; color:#991b1b; padding:10px 12px; border-radius:10px; display:flex; justify-content:space-between; align-items:center; }
.btn-error { background:#ef4444; color:#fff; border:none; padding:8px 10px; border-radius:8px; }

.table-wrap { overflow:auto; }
table { width:100%; border-collapse:collapse; min-width:760px; }
thead { background:#f1f8ff; }
th, td { padding:12px 14px; text-align:left; }
tbody tr { border-bottom:1px solid #f1f5f9; }
.loading-row, .empty-row { text-align:center; padding:20px 0; }

.actions-col { display:flex; gap:8px; }
.badge { padding:6px 10px; border-radius:999px; color:#fff; font-weight:700; }
.badge.available { background:#10b981; }
.badge.unavailable { background:#ef4444; }

@media (max-width: 900px) {
  .content-grid { grid-template-columns: 1fr; }
  table { min-width: 600px; }
}

@media (max-width: 480px) {
  .search { font-size:0.95rem }
  .form-row { flex-direction:column }
}

.fade-enter-active, .fade-leave-active { transition: all .25s ease; }
.fade-enter-from, .fade-leave-to { opacity:0; transform: translateY(-6px); }
</style>
  padding: 32px;
  background: radial-gradient(circle at top left, #4f8fed 0%, #1b3b7a 100%);
  color: #f8fafc;
}

.page-header {
  margin-bottom: 28px;
  text-align: center;
}

.page-header h1 {
  margin: 0;
  font-size: clamp(2rem, 2.5vw, 3rem);
  letter-spacing: 0.02em;
}

.stats-row {
  display: grid;
  gap: 20px;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  margin-bottom: 28px;
}

.stat-card {
  background: rgba(255, 255, 255, 0.12);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius: 22px;
  padding: 24px;
  backdrop-filter: blur(12px);
}

.stat-card-secondary {
  background: rgba(255, 255, 255, 0.08);
}

.stat-label {
  margin: 0 0 12px;
  font-size: 0.95rem;
  color: #dbeafe;
}

.stat-value {
  margin: 0;
  font-size: 2.4rem;
  font-weight: 700;
  color: #ffffff;
}

.content-grid {
  display: grid;
  gap: 24px;
  grid-template-columns: 1.1fr 1.4fr;
}

.panel {
  background: #ffffff;
  color: #0f172a;
  border-radius: 28px;
  padding: 28px;
  box-shadow: 0 24px 64px rgba(15, 23, 42, 0.12);
}

.panel h2 {
  margin-top: 0;
  margin-bottom: 18px;
  color: #0f172a;
}

.form-group {
  display: grid;
  gap: 8px;
  margin-bottom: 18px;
}

.form-group label {
  font-weight: 600;
  color: #334155;
}

.form-group input {
  width: 100%;
  padding: 14px 16px;
  border-radius: 16px;
  border: 1px solid #cbd5e1;
  background: #f8fafc;
  color: #0f172a;
  font-size: 1rem;
}

.btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  padding: 12px 20px;
  border-radius: 16px;
  border: none;
  cursor: pointer;
  font-weight: 700;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.btn:hover {
  transform: translateY(-1px);
}

.btn-primary {
  background: linear-gradient(135deg, #2563eb, #0ea5e9);
  color: #ffffff;
}

.btn-danger {
  background: #ef4444;
  color: #ffffff;
}

.btn-error {
  margin-left: 16px;
  padding: 10px 14px;
  background: #ef4444;
  color: #ffffff;
  border-radius: 14px;
}

.alert {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 16px;
  margin-bottom: 20px;
  padding: 16px 18px;
  border-radius: 18px;
  background: #fee2e2;
  color: #991b1b;
}

table {
  width: 100%;
  border-collapse: collapse;
  min-width: 320px;
}

thead {
  background: #eff6ff;
}

th,
td {
  padding: 16px 14px;
  text-align: left;
}

th {
  color: #0f172a;
  font-weight: 700;
  font-size: 0.95rem;
}

tbody tr {
  border-bottom: 1px solid #e2e8f0;
}

tbody tr:last-child {
  border-bottom: none;
}

td {
  color: #334155;
}

.loading-row,
.empty-row {
  text-align: center;
  padding: 24px 0;
  color: #475569;
}

@media (max-width: 960px) {
  .content-grid {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .page-shell {
    padding: 20px;
  }

  .stats-row {
    grid-template-columns: 1fr;
  }

  .panel {
    padding: 22px;
  }

  .form-group input,
  .btn,
  .btn-error {
    width: 100%;
  }
}
</style>