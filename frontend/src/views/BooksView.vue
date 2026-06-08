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
            <a-space>
              <a-button @click="exportToExcel" style="background: #4CAF50; border-color: #4CAF50; color: white">
                📥 Xuất Excel
              </a-button>
              <a-button type="primary" style="background: #0d4a42; border-color: #0d4a42" @click="startAdd">
                + Thêm sách
              </a-button>
            </a-space>
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
          :pagination="paginationConfig"
          @change="handleTableChange"
          size="middle"
          :scroll="{ x: 700 }"
          style="background: white; border-radius: 16px"
        >
          <template #bodyCell="{ column, record, index }">
            <template v-if="column.key === 'stt'">
              {{ calculateStt(index) }}
            </template>
            <template v-if="column.key === 'displayId'">
              {{ 1000 + calculateStt(index) }}
            </template>
            <template v-if="column.key === 'available'">
              {{ getAvailable(record) }}
            </template>
            <template v-if="column.key === 'status'">
              <a-tag :color="getAvailable(record) > 0 ? 'success' : 'error'">
                {{ getAvailable(record) > 0 ? 'Có thể mượn' : 'Hết sách' }}
              </a-tag>
            </template>
            <template v-if="column.key === 'rating'">
              <div>
                <div>⭐ {{ formatRating(record) }} / 5</div>
                <div>{{ record.soLuotDanhGia ?? 0 }} lượt</div>
              </div>
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
          <a-descriptions-item label="Mã">{{ getSelectedBookDisplayId() || '-' }}</a-descriptions-item>
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
          <a-descriptions-item label="Đánh giá trung bình">⭐ {{ formatRating(selectedBook) }} / 5</a-descriptions-item>
          <a-descriptions-item label="Số lượt đánh giá">{{ selectedBook.soLuotDanhGia ?? 0 }} lượt</a-descriptions-item>
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
            @change="handleTheLoaiChange"
            @select="handleTheLoaiSelect"
            placeholder="Chọn thể loại"
            allow-clear
          />
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

    <a-modal
      v-model:open="isOtherCategoryModalOpen"
      title="Nhập thể loại khác"
      ok-text="Xác nhận"
      cancel-text="Hủy"
      @ok="handleConfirmOtherCategory"
      @cancel="handleCancelOtherCategory"
    >
      <a-form layout="vertical">
        <a-form-item label="Thể loại mới" required>
          <a-input
            v-model:value="newCategoryName"
            placeholder="Nhập thể loại mới"
            @keydown.enter.prevent="handleConfirmOtherCategory"
          />
        </a-form-item>
      </a-form>
    </a-modal>

  </a-layout>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { message } from 'ant-design-vue'
import * as XLSX from 'xlsx'

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
const pagination = ref({ current: 1, pageSize: 10 })

const paginationConfig = computed(() => ({
  current: pagination.value.current,
  pageSize: pagination.value.pageSize,
  showSizeChanger: true,
  showTotal: total => `Tổng ${total} sách`
}))

const handleTableChange = (paginationInfo) => {
  pagination.value.current = paginationInfo.current || 1
  pagination.value.pageSize = paginationInfo.pageSize || pagination.value.pageSize
}

const calculateStt = (index) => {
  return (pagination.value.current - 1) * pagination.value.pageSize + index + 1
}

const isOtherCategoryModalOpen = ref(false)
const newCategoryName = ref('')

const normalizeTheLoai = (value) => (value || '').trim()

const defaultTheLoaiOptions = [
  { label: 'Truyện ngắn', value: 'Truyện ngắn' },
  { label: 'Tiểu thuyết', value: 'Tiểu thuyết' },
  { label: 'Văn học Việt Nam', value: 'Văn học Việt Nam' },
  { label: 'Thiếu nhi', value: 'Thiếu nhi' },
  { label: 'Kỹ năng sống', value: 'Kỹ năng sống' },
  { label: 'Công nghệ thông tin', value: 'Công nghệ thông tin' },
  { label: 'Khoa học', value: 'Khoa học' },
  { label: 'Kinh tế', value: 'Kinh tế' },
  { label: 'Giáo trình', value: 'Giáo trình' }
]

const getBookTheLoaiValues = computed(() => {
  const values = new Set()
  books.value.forEach(book => {
    const items = (book.theLoai || '').split(',').map(item => normalizeTheLoai(item)).filter(Boolean)
    items.forEach(item => {
      if (item !== 'Khác') values.add(item)
    })
  })
  return Array.from(values)
})

const form = ref({
  tenSach: '',
  tacGia: '',
  nhaSanXuat: '',
  soLuong: 0,
  soBanDaMuon: 0,
  imageUrl: '',
  moTa: '',
  isbn: '',
  theLoaiValues: []
})

const categoryOptions = computed(() => {
  const optionMap = new Map()
  defaultTheLoaiOptions.forEach(opt => optionMap.set(opt.value.toLowerCase(), opt))
  getBookTheLoaiValues.value.forEach(value => {
    const normalized = normalizeTheLoai(value)
    const key = normalized.toLowerCase()
    if (normalized && !optionMap.has(key)) {
      optionMap.set(key, { label: normalized, value: normalized })
    }
  })
  form.value.theLoaiValues.forEach(value => {
    const normalized = normalizeTheLoai(value)
    const key = normalized.toLowerCase()
    if (normalized && normalized.toLowerCase() !== 'khác' && !optionMap.has(key)) {
      optionMap.set(key, { label: normalized, value: normalized })
    }
  })
  return Array.from(optionMap.values())
})

const theLoaiOptions = computed(() => [
  ...categoryOptions.value,
  { label: 'Khác', value: 'Khác' }
])

const findExistingTheLoai = (value) => {
  const normalized = normalizeTheLoai(value).toLowerCase()
  if (!normalized) return null
  return categoryOptions.value.find(item => item.value.trim().toLowerCase() === normalized)?.value || null
}

const buildTheLoaiPayload = () => {
  return removeDuplicateTheLoai(form.value.theLoaiValues || []).join(', ')
}

const parseTheLoaiString = (value) => {
  const rawItems = (value || '').split(',').map(item => normalizeTheLoai(item)).filter(Boolean)
  const uniqueValues = []
  rawItems.forEach(item => {
    const existing = findExistingTheLoai(item) || item
    if (!uniqueValues.some(v => v.toLowerCase() === existing.toLowerCase())) {
      uniqueValues.push(existing)
    }
  })
  return { theLoaiValues: uniqueValues }
}

const removeDuplicateTheLoai = (list) => {
  const map = new Map()

  list.forEach(item => {
    const normalized = normalizeTheLoai(item)
    if (!normalized) return

    const key = normalized.toLowerCase()
    if (!map.has(key)) {
      map.set(key, normalized)
    }
  })

  return Array.from(map.values())
}

const handleTheLoaiChange = (values) => {
  if (!Array.isArray(values)) {
    form.value.theLoaiValues = []
    return
  }

  form.value.theLoaiValues = removeDuplicateTheLoai(
    values
      .map(item => normalizeTheLoai(item))
      .filter(item => item && item.toLowerCase() !== 'khác')
  )
}

const handleTheLoaiSelect = (value) => {
  if (value === 'Khác') {
    form.value.theLoaiValues = form.value.theLoaiValues.filter(item => item !== 'Khác')
    newCategoryName.value = ''
    isOtherCategoryModalOpen.value = true
  }
}

const handleConfirmOtherCategory = () => {
  const normalized = normalizeTheLoai(newCategoryName.value)

  if (!normalized) {
    message.warning('Vui lòng nhập thể loại')
    return
  }

  const existed = categoryOptions.value.find(
    item => item.value.toLowerCase() === normalized.toLowerCase()
  )

  const finalValue = existed ? existed.value : normalized

  form.value.theLoaiValues = removeDuplicateTheLoai([
    ...form.value.theLoaiValues,
    finalValue
  ])

  newCategoryName.value = ''
  isOtherCategoryModalOpen.value = false
}

const handleCancelOtherCategory = () => {
  newCategoryName.value = ''
  isOtherCategoryModalOpen.value = false
  form.value.theLoaiValues = form.value.theLoaiValues.filter(item => item !== 'Khác')
}

const columns = [
  { title: 'STT', key: 'stt', width: 40, align: 'center' },
  { title: 'Mã', key: 'displayId', width: 50, align: 'center' },
  { title: 'Tên sách', dataIndex: 'tenSach', key: 'tenSach', width: 160, sorter: (a, b) => a.tenSach.localeCompare(b.tenSach) },
  { title: 'Tác giả', dataIndex: 'tacGia', key: 'tacGia', width: 120, sorter: (a, b) => a.tacGia.localeCompare(b.tacGia) },
  { title: 'NXB', dataIndex: 'nhaSanXuat', key: 'nhaSanXuat', width: 130, sorter: (a, b) => a.nhaSanXuat.localeCompare(b.nhaSanXuat) },
  { title: 'Thể loại', dataIndex: 'theLoai', key: 'theLoai', sorter: (a, b) => (a.theLoai || '').localeCompare(b.theLoai || ''), width: 120 },
  { title: 'SL', dataIndex: 'soLuong', key: 'soLuong', width: 80, align: 'center', sorter: (a, b) => a.soLuong - b.soLuong },
  { title: 'Còn', key: 'available', width: 80, align: 'center', sorter: (a, b) => getAvailable(a) - getAvailable(b) },
  { title: 'Trạng thái', key: 'status', width: 100, filters: [{ text: 'Có thể mượn', value: true }, { text: 'Hết sách', value: false }], onFilter: (value, record) => (getAvailable(record) > 0) === value },
  { title: 'Đánh giá', key: 'rating', width: 90, align: 'center' },
  { title: 'Thao tác', key: 'action', width: 130, fixed: 'right' }
]

const loadBooks = async () => {
  const res = await fetch(API_URL)
  books.value = await res.json()
}

const getAvailable = (book) => book.soLuong - (book.soBanDaMuon ?? 0)

const formatRating = (book) => {
  const avg = Number(book?.danhGiaTrungBinh ?? 0)
  return avg.toFixed(1)
}

const getSelectedBookStt = () => {
  if (!selectedBook.value) return null
  const index = filteredBooks.value.findIndex(b => b.id === selectedBook.value.id)
  return index >= 0 ? index + 1 : null
}

const getSelectedBookDisplayId = () => {
  const stt = getSelectedBookStt()
  return stt ? 1000 + stt : null
}

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
    theLoaiValues: []
  }
  handleCancelOtherCategory()
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
    theLoaiValues: parsed.theLoaiValues
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

const exportToExcel = () => {
  // Lấy toàn bộ dữ liệu từ filteredBooks (có tính đến tìm kiếm)
  const dataToExport = filteredBooks.value.map((book, index) => ({
    'STT': index + 1,
    'Mã': 1000 + index + 1,
    'Tên sách': book.tenSach || '',
    'Tác giả': book.tacGia || '',
    'NXB': book.nhaSanXuat || '',
    'Thể loại': book.theLoai || '',
    'Số lượng': book.soLuong ?? 0,
    'Còn lại': getAvailable(book),
    'Trạng thái': getAvailable(book) > 0 ? 'Có thể mượn' : 'Hết sách',
    'Đánh giá': formatRating(book)
  }))

  // Tạo workbook
  const worksheet = XLSX.utils.json_to_sheet(dataToExport)
  
  // Cấu hình độ rộng cột
  worksheet['!cols'] = [
    { wch: 8 },   // STT
    { wch: 10 },  // Mã
    { wch: 30 },  // Tên sách
    { wch: 18 },  // Tác giả
    { wch: 20 },  // NXB
    { wch: 20 },  // Thể loại
    { wch: 10 },  // Số lượng
    { wch: 10 },  // Còn lại
    { wch: 12 },  // Trạng thái
    { wch: 10 }   // Đánh giá
  ]

  const workbook = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(workbook, worksheet, 'Danh sách sách')
  
  // Xuất file
  XLSX.writeFile(workbook, 'DanhSachSach.xlsx')
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

.category-option-input-wrapper {
  padding: 8px 12px;
}

.category-option-input {
  width: 100%;
  padding: 6px 10px;
  border: 1px solid #d9d9d9;
  border-radius: 6px;
  outline: none;
  font-size: 13px;
}

.category-option-input:focus {
  border-color: #40a9ff;
  box-shadow: 0 0 0 2px rgba(24, 144, 255, 0.14);
}
</style>
