<template>
  <a-layout :class="{ embedded: isEmbedded }" style="min-height: 100vh">

    <!-- SIDEBAR -->
    <a-layout-sider
      v-if="!isEmbedded"
      width="240"
      style="background: #0d4a42; border-radius: 0 20px 20px 0"
      :collapsed="collapsed"
      collapsible
      :trigger="null"
    >
      <div class="sider-inner">
        <div class="admin-top">
          <a-avatar style="background: #ebf6f2; color: #0d4a42; font-weight: bold; flex-shrink: 0">C</a-avatar>
          <div v-if="!collapsed" class="admin-meta">
            <div class="admin-name">CuongTang</div>
            <div class="admin-email">cuongtang@smartlib.net</div>
          </div>
        </div>

        <a-tag v-if="!collapsed" color="#176f63" style="margin: 0; width: fit-content">ADMIN PORTAL</a-tag>
        <div v-if="!collapsed" class="admin-title">HỆ THỐNG ADMIN</div>

        <a-menu theme="dark" mode="inline" :selected-keys="['books']"
          style="background: transparent; border: none; margin-top: 8px">
          <a-menu-item key="dashboard" @click="$router.push('/')">
            <template #icon><span>🏠</span></template>
            Overview Dashboard
          </a-menu-item>
          <a-menu-item key="books">
            <template #icon><span>�</span></template>
            Danh mục Sách (NT)
          </a-menu-item>
          <a-menu-item key="rules" disabled>
            <template #icon><span>📜</span></template>
            Quy tắc mượn trả
          </a-menu-item>
          <a-menu-item key="readers" disabled>
            <template #icon><span>👥</span></template>
            Quản lý Độc giả
          </a-menu-item>
          <a-menu-item key="card" disabled>
            <template #icon><span>💳</span></template>
            Thẻ Thư viện
          </a-menu-item>
        </a-menu>

        <div style="margin-top: auto; padding-top: 16px">
          <a-button block ghost @click="collapsed = !collapsed" style="border-color: rgba(255,255,255,.3)">
            {{ collapsed ? '→' : '← Thu gọn' }}
          </a-button>
        </div>
      </div>
    </a-layout-sider>

    <!-- MAIN -->
    <a-layout style="background: #fffaf3">
      <a-layout-content style="padding: 28px 24px; min-width: 0">

        <!-- HEADER ROW -->
        <a-row justify="space-between" align="middle" style="margin-bottom: 20px">
          <a-col>
            <a-space>
              <a-button v-if="!isEmbedded" @click="$router.push('/')">← Quay lại</a-button>
              <a-typography-title :level="3" style="margin: 0">Kho sách</a-typography-title>
            </a-space>
          </a-col>
          <a-col>
            <a-button type="primary" style="background: #0d4a42; border-color: #0d4a42" @click="startAdd">
              + Thêm sách
            </a-button>
          </a-col>
        </a-row>

        <!-- SEARCH -->
        <a-input-search
          v-model:value="search"
placeholder="Tìm kiếm sách, tác giả, nhà xuất bản..."
          style="margin-bottom: 20px; max-width: 500px"
          size="large"
          allow-clear
        />

        <!-- TABLE -->
        <a-table
          :columns="columns"
          :data-source="filteredBooks"
          :row-key="r => r.id"
          :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: total => `Tổng ${total} sách` }"
          size="middle"
          :scroll="{ x: 700 }"
          style="background: white; border-radius: 16px"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'available'">
              {{ getAvailable(record) }}
            </template>
            <template v-if="column.key === 'status'">
              <a-tag :color="getAvailable(record) > 0 ? 'success' : 'error'">
                {{ getAvailable(record) > 0 ? 'Có thể mượn' : 'Hết sách' }}
              </a-tag>
            </template>
            <template v-if="column.key === 'action'">
              <a-space>
                <a-button size="small" @click="openModal(record)">Chi tiết</a-button>
                <a-button size="small" type="primary" ghost @click="startEdit(record)">Sửa</a-button>
                <a-popconfirm
                  title="Bạn có chắc muốn xóa sách này?"
                  ok-text="Xóa"
                  cancel-text="Hủy"
                  ok-type="danger"
                  @confirm="deleteBook(record.id)"
                >
                  <a-button size="small" danger>Xóa</a-button>
                </a-popconfirm>
              </a-space>
            </template>
          </template>
        </a-table>

      </a-layout-content>
    </a-layout>

    <!-- MODAL CHI TIẾT -->
    <a-modal
      v-model:open="detailOpen"
      :title="selectedBook?.tenSach"
      :footer="null"
      width="480px"
    >
      <template v-if="selectedBook">
        <img :src="selectedBook.imageUrl || 'https://picsum.photos/300/450'" style="width:100%; height:220px; object-fit:cover; border-radius:12px; margin-bottom:16px" />
        <a-descriptions :column="1" bordered size="small">
          <a-descriptions-item label="Tác giả">{{ selectedBook.tacGia }}</a-descriptions-item>
          <a-descriptions-item label="Nhà xuất bản">{{ selectedBook.nhaSanXuat }}</a-descriptions-item>
          <a-descriptions-item label="Số lượng">{{ selectedBook.soLuong }}</a-descriptions-item>
          <a-descriptions-item label="Đã mượn">{{ selectedBook.soBanDaMuon ?? 0 }}</a-descriptions-item>
          <a-descriptions-item label="Còn lại">{{ getAvailable(selectedBook) }}</a-descriptions-item>
          <a-descriptions-item label="Trạng thái">
            <a-tag :color="getAvailable(selectedBook) > 0 ? 'success' : 'error'">
              {{ getAvailable(selectedBook) > 0 ? 'Có thể mượn' : 'Hết sách' }}
            </a-tag>
          </a-descriptions-item>
          <a-descriptions-item label="Thể loại">{{ selectedBook.theLoai || 'Chưa phân loại' }}</a-descriptions-item>
          <a-descriptions-item label="ISBN">{{ selectedBook.isbn }}</a-descriptions-item>
          <a-descriptions-item label="Mô tả">{{ selectedBook.moTa || 'Chưa có mô tả' }}</a-descriptions-item>
        </a-descriptions>
<a-space style="margin-top: 16px; width: 100%; justify-content: flex-end">
          <a-button type="primary" style="background:#0d4a42; border-color:#0d4a42" @click="startEditFromModal(selectedBook)">Sửa</a-button>
          <a-popconfirm title="Xóa sách này?" ok-text="Xóa" cancel-text="Hủy" ok-type="danger" @confirm="deleteBookFromModal(selectedBook.id)">
            <a-button danger>Xóa</a-button>
          </a-popconfirm>
          <a-button @click="detailOpen = false">Đóng</a-button>
        </a-space>
      </template>
    </a-modal>

    <!-- MODAL FORM THÊM/SỬA -->
    <a-modal
      v-model:open="formOpen"
      :title="editingId ? 'Sửa sách' : 'Thêm sách'"
      :confirm-loading="saving"
      ok-text="Lưu"
      cancel-text="Hủy"
      @ok="saveBook"
      @cancel="cancelForm"
    >
      <a-form :model="form" layout="vertical" style="margin-top: 8px">
        <a-form-item label="Tên sách" required>
          <a-input v-model:value="form.tenSach" placeholder="Nhập tên sách" />
        </a-form-item>
        <a-form-item label="Tác giả" required>
          <a-input v-model:value="form.tacGia" placeholder="Nhập tác giả" />
        </a-form-item>
        <a-form-item label="Nhà xuất bản" required>
          <a-input v-model:value="form.nhaSanXuat" placeholder="Nhập nhà xuất bản" />
        </a-form-item>
        <a-form-item label="Thể loại">
          <a-select
            mode="multiple"
            v-model:value="form.theLoaiValues"
            :options="theLoaiOptions"
            placeholder="Chọn thể loại"
            allow-clear
          />
        </a-form-item>
        <a-form-item v-if="form.theLoaiValues.includes('Khác')" label="Nhập thể loại khác">
          <a-input v-model:value="form.theLoaiKhac" placeholder="Nhập thể loại khác" />
        </a-form-item>
        <a-form-item label="Số lượng" required>
          <a-input-number v-model:value="form.soLuong" :min="0" style="width: 100%" />
        </a-form-item>
        <a-form-item label="Số bản đã mượn">
          <a-input-number v-model:value="form.soBanDaMuon" :min="0" style="width: 100%" />
        </a-form-item>
        <a-form-item label="ISBN">
          <a-input v-model:value="form.isbn" placeholder="Nhập ISBN" />
        </a-form-item>
        <a-form-item label="Link ảnh bìa">
          <a-input v-model:value="form.imageUrl" placeholder="Nhập URL ảnh bìa" />
        </a-form-item>
        <a-form-item label="Mô tả sách">
          <a-textarea v-model:value="form.moTa" rows="4" placeholder="Nhập mô tả sách" />
        </a-form-item>
        <a-form-item label="Số bản còn lại">
          <a-input-number :value="formAvailable" disabled style="width: 100%" />
        </a-form-item>
      </a-form>
    </a-modal>

  </a-layout>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

const isEmbedded = (() => {
  if (new URLSearchParams(window.location.search).get('embed') === 'true') return true
  try { return window.self !== window.top } catch { return true }
})()

// Use the current page host so clients on LAN call the backend on the same server
const API_URL = `http://${window.location.hostname}:5185/api/books`

const books = ref([])
const search = ref('')
const formOpen = ref(false)
const detailOpen = ref(false)
const editingId = ref(null)
const selectedBook = ref(null)
const saving = ref(false)
const collapsed = ref(false)

const theLoaiOptions = [
  { label: 'Văn học Việt Nam', value: 'Văn học Việt Nam' },
  { label: 'Văn học nước ngoài', value: 'Văn học nước ngoài' },
  { label: 'Thiếu nhi', value: 'Thiếu nhi' },
  { label: 'Truyện ngắn', value: 'Truyện ngắn' },
  { label: 'Tiểu thuyết', value: 'Tiểu thuyết' },
  { label: 'Kỹ năng sống', value: 'Kỹ năng sống' },
  { label: 'Công nghệ thông tin', value: 'Công nghệ thông tin' },
  { label: 'Khoa học', value: 'Khoa học' },
  { label: 'Kinh tế', value: 'Kinh tế' },
  { label: 'Giáo trình', value: 'Giáo trình' },
  { label: 'Khác', value: 'Khác' }
]

const form = ref({
  tenSach: '',
  tacGia: '',
  nhaSanXuat: '',
  soLuong: 0,
  soBanDaMuon: 0,
  imageUrl: '',
  moTa: '',
  isbn: '',
  theLoaiValues: [],
  theLoaiKhac: ''
})

const buildTheLoaiPayload = () => {
  const selected = Array.isArray(form.value.theLoaiValues) ? form.value.theLoaiValues : []
  const mainValues = selected.filter(item => item !== 'Khác')
  const otherValue = form.value.theLoaiKhac?.trim() || ''
  const list = [...mainValues]
  if (selected.includes('Khác') && otherValue) {
    list.push(otherValue)
  }
  return list.filter(Boolean).join(', ')
}

const parseTheLoaiString = (value) => {
  const raw = (value || '').split(',').map(item => item.trim()).filter(Boolean)
  const known = []
  const unknown = []
  const optionValues = theLoaiOptions.map(opt => opt.value).filter(val => val !== 'Khác')

  raw.forEach(item => {
    if (optionValues.includes(item)) {
      known.push(item)
    } else {
      unknown.push(item)
    }
  })

  const theLoaiValues = [...new Set(known)]
  const theLoaiKhac = unknown.join(', ')
  if (theLoaiKhac && !theLoaiValues.includes('Khác')) {
    theLoaiValues.push('Khác')
  }

  return { theLoaiValues, theLoaiKhac }
}

const columns = [
  { title: 'Mã', dataIndex: 'id', key: 'id', width: 60, align: 'center', sorter: (a, b) => a.id - b.id },
  { title: 'Tên sách', dataIndex: 'tenSach', key: 'tenSach', sorter: (a, b) => a.tenSach.localeCompare(b.tenSach) },
  { title: 'Tác giả', dataIndex: 'tacGia', key: 'tacGia', sorter: (a, b) => a.tacGia.localeCompare(b.tacGia) },
  { title: 'NXB', dataIndex: 'nhaSanXuat', key: 'nhaSanXuat', sorter: (a, b) => a.nhaSanXuat.localeCompare(b.nhaSanXuat) },
  { title: 'Thể loại', dataIndex: 'theLoai', key: 'theLoai', sorter: (a, b) => (a.theLoai || '').localeCompare(b.theLoai || ''), width: 220 },
  { title: 'SL', dataIndex: 'soLuong', key: 'soLuong', width: 60, align: 'center', sorter: (a, b) => a.soLuong - b.soLuong },
  { title: 'Còn', key: 'available', width: 60, align: 'center', sorter: (a, b) => getAvailable(a) - getAvailable(b) },
  { title: 'Trạng thái', key: 'status', width: 140, filters: [{ text: 'Có thể mượn', value: true }, { text: 'Hết sách', value: false }], onFilter: (value, record) => (getAvailable(record) > 0) === value },
  { title: 'Thao tác', key: 'action', width: 200, fixed: 'right' }
]

const loadBooks = async () => {
  const res = await fetch(API_URL)
  books.value = await res.json()
}

const getAvailable = (book) => book.soLuong - (book.soBanDaMuon ?? 0)

const formAvailable = computed(() => (form.value.soLuong ?? 0) - (form.value.soBanDaMuon ?? 0))

const filteredBooks = computed(() => {
  const q = search.value.toLowerCase()
  return books.value.filter(b =>
    b.tenSach?.toLowerCase().includes(q) ||
    b.tacGia?.toLowerCase().includes(q) ||
    b.nhaSanXuat?.toLowerCase().includes(q) ||
    (b.theLoai || '').toLowerCase().includes(q)
  )
})

const openModal = (book) => { selectedBook.value = book; detailOpen.value = true }

const resetForm = () => {
  form.value = {
    tenSach: '',
    tacGia: '',
    nhaSanXuat: '',
    soLuong: 0,
    soBanDaMuon: 0,
    imageUrl: '',
    moTa: '',
    isbn: '',
    theLoaiValues: [],
    theLoaiKhac: ''
  }
}

const startAdd = () => {
  editingId.value = null
  resetForm()
  formOpen.value = true
}

const startEdit = (book) => {
  editingId.value = book.id
  const parsed = parseTheLoaiString(book.theLoai)
  form.value = {
    tenSach: book.tenSach || '',
    tacGia: book.tacGia || '',
    nhaSanXuat: book.nhaSanXuat || '',
    soLuong: book.soLuong ?? 0,
    soBanDaMuon: book.soBanDaMuon ?? 0,
    imageUrl: book.imageUrl || '',
    moTa: book.moTa || '',
    isbn: book.isbn || '',
    theLoaiValues: parsed.theLoaiValues,
    theLoaiKhac: parsed.theLoaiKhac
  }
  formOpen.value = true
}

const startEditFromModal = (book) => {
  detailOpen.value = false
  startEdit(book)
}

const cancelForm = () => { formOpen.value = false; editingId.value = null }

const saveBook = async () => {
  saving.value = true
  try {
    const payload = {
      tenSach: form.value.tenSach,
      tacGia: form.value.tacGia,
      nhaSanXuat: form.value.nhaSanXuat,
      soLuong: form.value.soLuong,
      soBanDaMuon: form.value.soBanDaMuon,
      imageUrl: form.value.imageUrl,
      moTa: form.value.moTa,
      isbn: form.value.isbn,
      theLoai: buildTheLoaiPayload()
    }

    if (editingId.value) {
      payload.id = editingId.value
      const res = await fetch(`${API_URL}/${editingId.value}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!res.ok) {
        const err = await res.text()
        console.error('PUT failed:', res.status, err)
        return
      }
    } else {
      const res = await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
      })
      if (!res.ok) {
        const err = await res.text()
        console.error('POST failed:', res.status, err)
        return
      }
    }

    formOpen.value = false
    const savedId = editingId.value
    editingId.value = null
    await loadBooks()
    // Cập nhật lại selectedBook nếu modal chi tiết đang mở
    if (savedId && detailOpen.value) {
      selectedBook.value = books.value.find(b => b.id === savedId) ?? null
    }
  } finally {
    saving.value = false
  }
}

const deleteBook = async (id) => {
  await fetch(`${API_URL}/${id}`, { method: 'DELETE' })
  await loadBooks()
}

const deleteBookFromModal = async (id) => {
  await deleteBook(id)
  detailOpen.value = false
}

onMounted(loadBooks)
</script>

<style scoped>
.sider-inner {
  display: flex;
  flex-direction: column;
  height: 100%;
  padding: 24px 16px;
  gap: 14px;
}

.admin-top {
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 0;
}

.admin-meta { min-width: 0; }

.admin-name {
  color: #fff;
  font-weight: 600;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.admin-email {
  color: #b9d9d1;
  font-size: 11px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.admin-title {
  color: #9bd0c7;
  font-size: 11px;
  letter-spacing: 1px;
  text-transform: uppercase;
}

:deep(.ant-menu-item-selected) {
  background-color: #ffffff !important;
  color: #0d4a42 !important;
  font-weight: 700;
}

:deep(.ant-menu-item) {
  color: #c3dad5;
}

:deep(.ant-layout-sider-trigger) {
  background: #0a3830;
}
</style>
