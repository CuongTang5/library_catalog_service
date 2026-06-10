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
            <template #icon><span>📚</span></template>
            Danh mục Sách (NT)
          </a-menu-item>
          <a-menu-item key="stock-imports" @click="$router.push('/stock-imports')">
            <template #icon><span>📦</span></template>
            Nhập kho
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
              <a-button :loading="isImporting" @click="triggerImportExcel" style="background: #2196F3; border-color: #2196F3; color: white">
                📤 Nhập Excel
              </a-button>
              <a-button @click="$router.push('/stock-imports')" style="background: #ff9800; border-color: #ff9800; color: white">
                📦 Nhập kho
              </a-button>
              <input
                ref="excelFileInput"
                type="file"
                accept=".xlsx,.xls"
                style="display: none"
                @change="handleExcelFileChange"
              />
              <a-button
  @click="() => { manageCategoriesOpen = true; fetchCategories() }"
>
  🗂️ Quản lý thể loại
</a-button>
              <a-button type="primary" style="background: #0d4a42; border-color: #0d4a42" @click="startAdd">
                + Thêm sách
              </a-button>
            </a-space>
          </a-col>
        </a-row>

        <!-- BULK ACTION BAR -->
        <div v-if="selectedRowKeys.length > 0" class="selected-toolbar">
          <div class="selected-info">
            <CheckCircleOutlined />
            <span>✓ {{ selectedRowKeys.length }} sách được chọn</span>
          </div>

          <a-space>
            <a-button danger type="primary" @click="deleteSelectedBooks">
              <template #icon>
                <DeleteOutlined />
              </template>
              Xóa đã chọn
            </a-button>

            <a-button @click="clearSelection">
              <template #icon>
                <CloseOutlined />
              </template>
              Bỏ chọn
            </a-button>
          </a-space>
        </div>

        <!-- TABLE -->
        <div class="table-wrapper">
          <a-table
            :columns="columns"
            :data-source="filteredBooks"
            :row-key="r => r.id"
            :row-selection="rowSelection"
            :pagination="paginationConfig"
            @change="handleTableChange"
            size="middle"
            style="background: white; border-radius: 16px; width:100%"
          >
            <template #bodyCell="{ column, record, index }">
              <template v-if="column.key === 'stt'">
                {{ calculateStt(index) }}
              </template>

              <template v-if="column.key === 'displayId'">
                {{ 1000 + calculateStt(index) }}
              </template>

              <template v-if="column.key === 'tenSach'">
                <div style="max-width:140px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap">{{ record.tenSach }}</div>
              </template>

              <template v-if="column.key === 'tacGia'">
                <div style="max-width:130px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap">{{ record.tacGia }}</div>
              </template>

              <template v-if="column.key === 'nhaSanXuat'">
                <div style="max-width:150px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap">{{ record.nhaSanXuat }}</div>
              </template>

              <template v-if="column.key === 'theLoai'">
                <a-tooltip :title="record.theLoai">
                  <div class="cell-theloai">{{ record.theLoai || '' }}</div>
                </a-tooltip>
              </template>

              <template v-if="column.key === 'soLuong'">
                {{ record.soLuong }}
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
                <div class="rating-compact">
                  <div class="stars">⭐ {{ formatRating(record) }} / 5</div>
                  <div class="count">{{ record.soLuotDanhGia ?? 0 }} lượt</div>
                </div>
              </template>

              <template v-if="column.key === 'action'">
                <div class="action-buttons">
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
                </div>
              </template>
            </template>
          </a-table>
        </div>

    <!-- MODAL CHI TIẾT -->
    <a-modal
      v-model:open="detailOpen"
      :title="selectedBook?.tenSach"
      :footer="null"
      centered
      class="book-detail-modal"
      :width="880"
      :body-style="{ maxHeight: '70vh', overflowY: 'auto' }"
    >
      <template v-if="selectedBook">
        <a-row gutter="16">
          <a-col :span="8">
            <div class="detail-image-wrapper">
              <img :src="selectedBook.imageUrl || 'https://picsum.photos/400/600'" class="detail-image" />
            </div>
          </a-col>
          <a-col :span="16">
            <div class="detail-info">
              <a-descriptions :column="1" size="small" bordered>
                <a-descriptions-item label="Mã">{{ getSelectedBookDisplayId() || '-' }}</a-descriptions-item>
                <a-descriptions-item label="Tác giả">{{ selectedBook.tacGia }}</a-descriptions-item>
                <a-descriptions-item label="Nhà xuất bản">{{ selectedBook.nhaSanXuat }}</a-descriptions-item>
                <a-descriptions-item label="Thể loại">{{ selectedBook.theLoai || 'Chưa phân loại' }}</a-descriptions-item>
                <a-descriptions-item label="Số lượng">{{ selectedBook.soLuong }}</a-descriptions-item>
                <a-descriptions-item label="Đã mượn">{{ selectedBook.soBanDaMuon ?? 0 }}</a-descriptions-item>
                <a-descriptions-item label="Còn lại">{{ getAvailable(selectedBook) }}</a-descriptions-item>
                <a-descriptions-item label="Trạng thái">
                  <a-tag :color="getAvailable(selectedBook) > 0 ? 'success' : 'error'">
                    {{ getAvailable(selectedBook) > 0 ? 'Có thể mượn' : 'Hết sách' }}
                  </a-tag>
                </a-descriptions-item>
                <a-descriptions-item label="Đánh giá">⭐ {{ formatRating(selectedBook) }} / 5</a-descriptions-item>
                <a-descriptions-item label="ISBN">{{ selectedBook.isbn }}</a-descriptions-item>
              </a-descriptions>

                <div class="detail-description">
                  {{ selectedBook.moTa || 'Chưa có mô tả' }}
                </div>
              </div>

              <div style="margin-top: 16px; display:flex; justify-content:flex-end; gap:8px">
              <a-button type="primary" style="background:#0d4a42; border-color:#0d4a42" @click="startEditFromModal(selectedBook)">Sửa</a-button>
              <a-popconfirm title="Xóa sách này?" ok-text="Xóa" cancel-text="Hủy" ok-type="danger" @confirm="deleteBookFromModal(selectedBook.id)">
                <a-button danger>Xóa</a-button>
              </a-popconfirm>
              <a-button @click="detailOpen = false">Đóng</a-button>
            </div>
            </a-col>
        </a-row>
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
      centered
      class="book-form-modal"
      :width="900"
      :z-index="1000"
      :body-style="{ maxHeight: '70vh', overflowY: 'auto' }"
    >
      <a-form :model="form" layout="vertical" class="book-form" style="margin-top: 8px">
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="Tên sách" required>
              <a-input v-model:value="form.tenSach" placeholder="Nhập tên sách" />
            </a-form-item>
            <a-form-item label="Tác giả" required>
              <a-input v-model:value="form.tacGia" placeholder="Nhập tác giả" />
            </a-form-item>
            <a-form-item label="Nhà xuất bản" required>
              <a-input v-model:value="form.nhaSanXuat" placeholder="Nhập nhà xuất bản" />
            </a-form-item>
            <a-form-item label="ISBN">
              <a-input v-model:value="form.isbn" placeholder="Nhập ISBN" />
            </a-form-item>
          </a-col>

          <a-col :span="12">
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
            <a-form-item label="Link ảnh bìa">
              <a-input v-model:value="form.imageUrl" placeholder="Nhập URL ảnh bìa" />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row>
          <a-col :span="24">
            <a-form-item label="Mô tả sách">
              <a-textarea v-model:value="form.moTa" :rows="4" placeholder="Nhập mô tả sách" />
            </a-form-item>
          </a-col>
        </a-row>

        <a-row>
          <a-col :span="24">
            <a-form-item label="Số bản còn lại">
              <a-input-number :value="formAvailable" disabled style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-modal>

    <a-modal
      v-model:open="isOtherCategoryModalOpen"
      title="Nhập thể loại khác"
      ok-text="Xác nhận"
      cancel-text="Hủy"
      centered
      class="category-modal"
      :width="480"
      :z-index="3000"
      get-container="body"
      @ok="handleConfirmOtherCategory"
      @cancel="handleCancelOtherCategory"
    >
      <a-form layout="vertical">
        <a-form-item label="Thể loại mới" required>
          <a-input
            v-model:value="newCategoryName"
            placeholder="Nhập thể loại mới"
            @keydown.enter.prevent="handleConfirmOtherCategory"
            autofocus
          />
        </a-form-item>
      </a-form>
</a-modal>

    <a-modal
      v-model:open="categoryDetailOpen"
      :title="selectedCategory ? `Chi tiết thể loại: ${selectedCategory.name}` : 'Chi tiết thể loại'"
      :confirm-loading="categoryDetailLoading"
      ok-text="Đóng"
      cancel-text="Đóng"
      @ok="categoryDetailOpen = false"
      @cancel="categoryDetailOpen = false"
      centered
      class="category-detail-modal"
      :width="560"
      :z-index="1200"
      :body-style="{ maxHeight: '60vh', overflowY: 'auto' }"
    >
      <template v-if="selectedCategory">
        <div v-if="categoryDetailLoading" style="min-height: 120px; display:flex; align-items:center; justify-content:center">
          Đang tải...
        </div>
        <div v-else>
          <div v-if="categoryBooks.length > 0">
            <p>Đang được sử dụng bởi {{ categoryBooks.length }} sách</p>
            <ol style="padding-left: 18px; margin: 0">
              <li v-for="(bookName, index) in categoryBooks" :key="index" style="margin-bottom: 8px">
                {{ bookName }}
              </li>
            </ol>
          </div>
          <div v-else>
            Hiện chưa có sách nào sử dụng thể loại này.
          </div>
        </div>
      </template>
    </a-modal>

    <!-- MODAL QUẢN LÝ THỂ LOẠI -->
    <a-modal
      v-model:open="manageCategoriesOpen"
      centered
      :width="720"
      :z-index="1100"
      @openChange="val => { if (val) { fetchCategories() } }"
      @cancel="() => { manageCategoriesOpen = false; editCategoryId = null; editCategoryName = '' }"
      :footer="null"
    >
      <template #title>
        <div style="display:flex; justify-content:space-between; align-items:center; padding-right:32px">
          <span>Quản lý thể loại</span>
          <a-button
            size="small"
            type="primary"
            style="background:#0d4a42; border-color:#0d4a42"
            @click="() => { addingCategoryFromManager = true; newCategoryName = ''; isOtherCategoryModalOpen = true }"
          >+ Thêm thể loại</a-button>
        </div>
      </template>
      <div>
        <a-list :data-source="categoryObjects">
          <template #renderItem="{ item }">
            <a-list-item>
              <a-row style="width:100%; align-items:center">
                <a-col :span="16">
                  <div v-if="editCategoryId !== item.id">{{ item.name }}</div>
                  <div v-else>
                    <a-input v-model:value="editCategoryName" />
                  </div>
                </a-col>
                <a-col :span="8" style="text-align:right">
                  <template v-if="editCategoryId === item.id">
                    <a-button size="small" type="primary" @click="updateCategory(item.id, editCategoryName)">Lưu</a-button>
                    <a-button size="small" @click="cancelEditCategory">Hủy</a-button>
                  </template>
                  <template v-else>
                    <a-space size="small">
                      <a-button size="small" @click="showCategoryDetail(item)">Chi tiết</a-button>
                      <a-button size="small" @click="startEditCategory(item)">Sửa</a-button>
                      <a-button size="small" danger @click="deleteCategory(item.id)">Xóa</a-button>
                    </a-space>
                  </template>
                </a-col>
              </a-row>
            </a-list-item>
          </template>
        </a-list>
      </div>
    </a-modal>

      </a-layout-content>
    </a-layout>
  </a-layout>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted, watch, h } from 'vue'
import { message, Modal, Input, Button, Space, Popover } from 'ant-design-vue'
import {
  SearchOutlined,
  DeleteOutlined,
  CloseOutlined,
  CheckCircleOutlined
} from '@ant-design/icons-vue'
import * as XLSX from 'xlsx'
import { redeemAuthHandoffCode } from '../utils/authHandoff'
import { saveAuthSession, getAuthToken } from '../utils/auth'

const isEmbedded = (() => {
  if (new URLSearchParams(window.location.search).get('embed') === 'true') return true
  try { return window.self !== window.top } catch { return true }
})()

const isVpsHost = window.location.hostname === '163.223.210.87'
const GATEWAY_BASE_URL = 'http://163.223.210.87:5000'
const DIRECT_CATALOG_BASE_URL = `http://${window.location.hostname}:5185/api`

const BOOKS_API_URL = isVpsHost
  ? `${GATEWAY_BASE_URL}/api/catalog/books`
  : 'http://localhost:5185/api/books'

const CATEGORIES_API_URL = isVpsHost
  ? `http://${window.location.hostname}:5185/api/categories`
  : 'http://localhost:5185/api/categories'

const books = ref([])
const search = ref('')
const formOpen = ref(false)
const detailOpen = ref(false)
const editingId = ref(null)
const selectedBook = ref(null)
const saving = ref(false)
const collapsed = ref(false)
const pagination = ref({ current: 1, pageSize: 10 })
const selectedRowKeys = ref([])
const excelFileInput = ref(null)
const isImporting = ref(false)

const selectedBooks = computed(() =>
  books.value.filter(book => selectedRowKeys.value.includes(book.id))
)

const triggerImportExcel = () => {
  if (isImporting.value) {
    return
  }
  excelFileInput.value?.click()
}

const importBooks = async (items) => {
  const res = await fetch(`${BOOKS_API_URL}/import`, {
    method: 'POST',
    headers: getAuthHeaders(),
    body: JSON.stringify(items)
  })

  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || `HTTP ${res.status}`)
  }

  return await res.json()
}

const getExcelCell = (row, keys) => {
  for (const key of keys) {
    const value = row[key]
    if (value !== undefined && value !== null && String(value).trim() !== '') {
      return String(value).trim()
    }
  }
  return ''
}

const handleExcelFileChange = async (event) => {
  const input = event.target
  if (!input?.files?.length) {
    return
  }

  const file = input.files[0]
  input.value = ''
  isImporting.value = true

  try {
    const arrayBuffer = await file.arrayBuffer()
    const workbook = XLSX.read(arrayBuffer, { type: 'array' })
    const sheetName = workbook.SheetNames[0]
    const worksheet = workbook.Sheets[sheetName]

    if (!worksheet) {
      throw new Error('Không tìm thấy sheet đầu tiên trong file Excel.')
    }

    const rows = XLSX.utils.sheet_to_json(worksheet, { defval: '' })
    const itemsToImport = []

    for (const row of rows) {
      const tenSach = getExcelCell(row, ['Tên sách', 'Ten sach', 'TenSach'])
      if (!tenSach) {
        continue
      }

      const tacGia = getExcelCell(row, ['Tác giả', 'Tac gia', 'TacGia']) || 'Chưa rõ'
      const nhaSanXuat = getExcelCell(row, ['NXB', 'Nha san xuat', 'NhaSanXuat']) || 'Chưa rõ'
      const theLoai = getExcelCell(row, ['Thể loại', 'The loai', 'TheLoai']) || 'Chưa phân loại'
      const soLuongRaw = getExcelCell(row, ['Số lượng', 'So luong', 'SoLuong'])
      const soBanDaMuonRaw = getExcelCell(row, ['Đã mượn', 'Da muon', 'DaMuon', 'So ban da muon'])
      const isbn = getExcelCell(row, ['ISBN', 'Isbn'])
      const moTa = getExcelCell(row, ['Mô tả', 'Mo ta', 'MoTa'])
      const imageUrl = getExcelCell(row, ['Link ảnh', 'Link ảnh', 'Link ảnh'])

      const soLuong = Number.isFinite(Number(soLuongRaw)) && Number(soLuongRaw) > 0
        ? Math.trunc(Number(soLuongRaw))
        : 1
      let soBanDaMuon = Number.isFinite(Number(soBanDaMuonRaw)) && Number(soBanDaMuonRaw) >= 0
        ? Math.trunc(Number(soBanDaMuonRaw))
        : 0

      if (soBanDaMuon > soLuong) {
        soBanDaMuon = 0
      }

      itemsToImport.push({
        tenSach,
        tacGia,
        nhaSanXuat,
        theLoai,
        soLuong,
        soBanDaMuon,
        isbn,
        moTa,
        imageUrl
      })
    }

    if (itemsToImport.length === 0) {
      message.warning('Không tìm thấy dòng sách hợp lệ trong file Excel.')
      return
    }

    const result = await importBooks(itemsToImport)
    message.success(`Đã nhập thành công ${result.imported ?? itemsToImport.length} sách. Bỏ qua ${result.skipped ?? 0} dòng.`)
    await loadBooks()
  } catch (err) {
    console.error('Import Excel failed', err)
    message.error(err?.message || 'Lỗi khi nhập Excel')
  } finally {
    isImporting.value = false
  }
}

const rowSelection = computed(() => ({
  selectedRowKeys: selectedRowKeys.value,
  onChange: keys => {
    selectedRowKeys.value = keys
  }
}))

const clearSelection = () => {
  selectedRowKeys.value = []
}


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
const addingCategoryFromManager = ref(false)
const manageCategoriesOpen = ref(false)
const categoryDetailOpen = ref(false)
const categoryDetailLoading = ref(false)
const categoryBooks = ref([])
const selectedCategory = ref(null)
const categoryObjects = ref([]) // { id, name }
watch(manageCategoriesOpen, (val) => { if (val) fetchCategories() })
const editCategoryId = ref(null)
const editCategoryName = ref('')

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

const categories = ref([])

const loadCategories = async () => {
  try {
    const res = await fetch(CATEGORIES_API_URL, {
      headers: getAuthHeaders()
    })

    if (res.status === 404) {
      console.warn('Categories API not available; using default category list')
      categories.value = defaultTheLoaiOptions.map(o => o.value)
      return
    }

    if (!res.ok) {
      const errorText = await res.text()
      console.warn('Failed to load categories', res.status, errorText)
      categories.value = defaultTheLoaiOptions.map(o => o.value)
      return
    }

    const data = await res.json()
    console.log('categories loaded', data)
    categories.value = data.map(c => c.name)
  } catch (e) {
    console.warn('Failed to load categories', e)
    categories.value = defaultTheLoaiOptions.map(o => o.value)
  }
}

const fetchCategories = async () => {
  try {
    const res = await fetch(CATEGORIES_API_URL, {
      headers: getAuthHeaders()
    })
    if (res.status === 404) {
      console.warn('Categories API not available for fetchCategories')
      categoryObjects.value = []
      return
    }
    if (!res.ok) {
      const err = await res.text()
      console.warn('fetchCategories failed', res.status, err)
      categoryObjects.value = []
      return
    }
    const data = await res.json()
    console.log('categories loaded', data)
    categoryObjects.value = data.map(c => ({ id: c.id, name: c.name }))
  } catch (e) {
    console.warn('fetchCategories error', e)
    categoryObjects.value = []
  }
}

const updateCategory = async (id, newName) => {
  const name = (newName || '').trim()
  if (!name) { message.warning('Tên thể loại không được rỗng'); return }
  if (name.toLowerCase() === 'khác') { message.warning('Tên không hợp lệ'); return }
  try {
    const res = await fetch(`${CATEGORIES_API_URL}/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ name })
    })
    if (!res.ok) {
      const txt = await res.text()
      message.error(txt || 'Lỗi cập nhật thể loại')
      return
    }
    await fetchCategories()
    await loadCategories()
    message.success('Cập nhật thể loại thành công')
    editCategoryId.value = null
    editCategoryName.value = ''
  } catch (e) {
    console.error(e)
    message.error('Lỗi cập nhật thể loại')
  }
}

const deleteCategory = async (id) => {
  try {
    const token = localStorage.getItem('token') || localStorage.getItem('accessToken')
    const headers = { 'Content-Type': 'application/json', ...(token ? { Authorization: `Bearer ${token}` } : {}) }

    const usageRes = await fetch(`${CATEGORIES_API_URL}/${id}/usage`, { headers })
    if (!usageRes.ok) { message.error('Không thể kiểm tra thể loại'); return }
    const usage = await usageRes.json()

    const doDelete = async (force) => {
      const url = `${CATEGORIES_API_URL}/${id}${force ? '?force=true' : ''}`
      const res = await fetch(url, { method: 'DELETE', headers })
      if (!res.ok) { const txt = await res.text(); message.warning(txt || 'Không thể xóa thể loại'); return }
      await fetchCategories()
      await loadCategories()
      message.success('Đã xóa thể loại')
    }

    if (!usage.isUsed) {
      Modal.confirm({
        title: 'Xác nhận xóa',
        content: 'Bạn có chắc muốn xóa thể loại này không?',
        okText: 'Xóa',
        cancelText: 'Hủy',
        okType: 'danger',
        zIndex: 4000,
        onOk: () => doDelete(false)
      })
    } else {
      const names = usage.bookNames.join(', ')
      Modal.confirm({
        title: 'Thể loại đang được sử dụng',
        content: `Sách '${names}' đang được sử dụng thể loại này. Bạn có muốn xóa không?`,
        okText: 'Xóa',
        cancelText: 'Hủy',
        okType: 'danger',
        zIndex: 4000,
        onOk: () => doDelete(true)
      })
    }
  } catch (e) {
    console.error(e)
    message.error('Lỗi xóa thể loại')
  }
}

const startEditCategory = (item) => {
  editCategoryId.value = item.id
  editCategoryName.value = item.name
}

const cancelEditCategory = () => {
  editCategoryId.value = null
  editCategoryName.value = ''
}

const showCategoryDetail = async (category) => {
  selectedCategory.value = category
  categoryDetailOpen.value = true
  categoryDetailLoading.value = true
  categoryBooks.value = []

  try {
    const token = localStorage.getItem('token') || localStorage.getItem('accessToken')
    const headers = {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    }
    const res = await fetch(`${CATEGORIES_API_URL}/${category.id}/usage`, { headers })
    if (!res.ok) {
      const text = await res.text()
      throw new Error(text || `HTTP ${res.status}`)
    }
    const data = await res.json()
    categoryBooks.value = Array.isArray(data.bookNames) ? data.bookNames : []
  } catch (err) {
    console.error('Category usage load failed', err)
    message.error('Không thể tải chi tiết thể loại')
    categoryBooks.value = []
  } finally {
    categoryDetailLoading.value = false
  }
}

// categories are derived from defaultTheLoaiOptions + books[].theLoai

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
  const source = (categories.value && categories.value.length > 0) ? categories.value : defaultTheLoaiOptions.map(o => o.value)
  const optionMap = new Map()
  source.forEach(v => {
    const normalized = normalizeTheLoai(v)
    if (normalized) optionMap.set(normalized.toLowerCase(), { label: normalized, value: normalized })
  })

  // include any currently selected values not yet present (to avoid losing selections)
  form.value.theLoaiValues.forEach(value => {
    const normalized = normalizeTheLoai(value)
    if (normalized && normalized.toLowerCase() !== 'khác' && !optionMap.has(normalized.toLowerCase())) {
      optionMap.set(normalized.toLowerCase(), { label: normalized, value: normalized })
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

const createCategory = async (name) => {
  const normalizedName = (name || '').trim()
  if (!normalizedName) throw new Error('Tên thể loại rỗng')

  const res = await fetch(CATEGORIES_API_URL, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ name: normalizedName })
  })

  if (!res.ok) {
    const errorText = await res.text()
    console.warn('Create category failed', res.status, errorText)
    if (res.status === 404) {
      return { name: normalizedName, fallback: true }
    }
    const err = new Error(errorText || `HTTP ${res.status}`)
    err.status = res.status
    throw err
  }

  const data = await res.json()
  return { ...data, fallback: false }
}

const handleConfirmOtherCategory = async () => {
  const normalized = normalizeTheLoai(newCategoryName.value)

  if (!normalized) {
    message.warning('Vui lòng nhập thể loại')
    return
  }

  try {
    if (addingCategoryFromManager.value) {
      await createCategory(normalized)
      await fetchCategories()
      await loadCategories()
      newCategoryName.value = ''
      isOtherCategoryModalOpen.value = false
      addingCategoryFromManager.value = false
      message.success('Đã thêm thể loại mới')
    } else {
      const data = await createCategory(normalized)
      const returnedName = (data.name || data.Name || normalized).toString()

      if (!data.fallback) {
        await loadCategories()
      } else {
        message.warning('API thể loại chưa sẵn sàng, thể loại sẽ lưu cùng sách')
      }

      form.value.theLoaiValues = removeDuplicateTheLoai([
        ...form.value.theLoaiValues,
        returnedName
      ])

      message.success('Đã thêm thể loại mới')
      newCategoryName.value = ''
      isOtherCategoryModalOpen.value = false
    }
  } catch (err) {
    console.error(err)
    const msg = (err && err.message) ? err.message : 'Lỗi khi gọi API thể loại'
    message.error(msg)
  }
}

const handleCancelOtherCategory = () => {
  newCategoryName.value = ''
  isOtherCategoryModalOpen.value = false
  addingCategoryFromManager.value = false
  form.value.theLoaiValues = form.value.theLoaiValues.filter(item => item !== 'Khác')
}

const columnFilters = reactive({
  tenSach: '',
  tacGia: '',
  nhaSanXuat: '',
  theLoai: '',
  status: ''
})

const resetColumnFilter = (key) => {
  columnFilters[key] = ''
}

const getColumnTitle = (title, key, placeholder) => {
  const isActive = !!columnFilters[key]
  return h('span', { class: 'column-title' }, [
    h('span', { class: 'column-title-text' }, title),
    h(Popover, {
      trigger: 'click',
      placement: 'bottomRight',
      overlayClassName: 'column-search-popover'
    }, {
      content: () => getSearchDropdown(key, placeholder),
      default: () => h('span', { class: 'column-search-wrap', onClick: e => e.stopPropagation() }, [
        h(SearchOutlined, {
          class: ['column-search-icon', { active: isActive }]
        })
      ])
    })
  ])
}

const getSearchDropdown = (key, placeholder) => {
  const titleText = placeholder.replace(/^Tìm theo\s*/i, '')
  return h('div', { class: 'search-dropdown-box' }, [
    h('div', { class: 'search-dropdown-title' }, `Tìm kiếm theo ${titleText}`),
    h(Input, {
      value: columnFilters[key],
      placeholder,
      allowClear: true,
      autofocus: true,
      style: { width: '100%' },
      onChange: e => { columnFilters[key] = e.target.value },
      onPressEnter: () => {}
    }),
    h(Space, { style: { marginTop: '12px', display: 'flex', justifyContent: 'flex-end' } }, () => [
      h(Button, {
        size: 'small',
        onClick: () => resetColumnFilter(key)
      }, () => 'Đặt lại'),
      h(Button, {
        type: 'primary',
        size: 'small'
      }, () => 'Tìm kiếm')
    ])
  ])
}

const columns = [
  { title: 'STT', key: 'stt', width: 50, align: 'center' },
  { title: 'Mã', key: 'displayId', width: 70, align: 'center' },
  {
    title: () => getColumnTitle('Tên sách', 'tenSach', 'Tìm theo tên sách'),
    dataIndex: 'tenSach',
    key: 'tenSach',
    width: 190,
    sorter: (a, b) =>
      (a.tenSach || '').localeCompare(b.tenSach || ''),
    onHeaderCell: () => ({ style: { width: '100%' } })
  },
  {
    title: () => getColumnTitle('Tác giả', 'tacGia', 'Tìm theo tác giả'),
    dataIndex: 'tacGia',
    key: 'tacGia',
    width: 170,
    sorter: (a, b) =>
      (a.tacGia || '').localeCompare(b.tacGia || ''),
    onHeaderCell: () => ({ style: { width: '100%' } })
  },
  {
    title: () => getColumnTitle('NXB', 'nhaSanXuat', 'Tìm theo nhà xuất bản'),
    dataIndex: 'nhaSanXuat',
    key: 'nhaSanXuat',
    width: 170,
    sorter: (a, b) =>
      (a.nhaSanXuat || '').localeCompare(b.nhaSanXuat || ''),
    onHeaderCell: () => ({ style: { width: '100%' } })
  },
  {
    title: () => getColumnTitle('Thể loại', 'theLoai', 'Tìm theo thể loại'),
    dataIndex: 'theLoai',
    key: 'theLoai',
    width: 190,
    sorter: (a, b) =>
      (a.theLoai || '').localeCompare(b.theLoai || ''),
    onHeaderCell: () => ({ style: { width: '100%' } })
  },
  { title: 'SL', dataIndex: 'soLuong', key: 'soLuong', width: 60, align: 'center', sorter: (a, b) => a.soLuong - b.soLuong },
  { title: 'Còn', key: 'available', width: 60, align: 'center', sorter: (a, b) => getAvailable(a) - getAvailable(b) },
  {
  title: 'Trạng thái',
  key: 'status',
  width: 110,
  filters: [
    { text: 'Có thể mượn', value: 'available' },
    { text: 'Hết sách', value: 'out' }
  ],
  onFilter: (value, record) => {
    const available = getAvailable(record) > 0
    return value === 'available' ? available : !available
  }
},
  { title: 'Đánh giá', key: 'rating', width: 100, align: 'center' },
  { title: 'Thao tác', key: 'action', width: 180 }
]

const getAuthHeaders = () => {
  const token = getAuthToken()
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {})
  }
}

const sendReportEvent = async (eventType, title) => {
  try {
    const token =
      localStorage.getItem('token') ||
      localStorage.getItem('accessToken')
    await fetch('http://163.223.210.87:5000/api/identity/Report/events', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      },
      body: JSON.stringify({
        eventType,
        data: { title, username: 'admin' },
        sourceService: 'CatalogService'
      })
    })
  } catch (err) {
    console.warn('sendReportEvent failed', err)
  }
}

const handleAuthCode = async () => {
  const code = new URLSearchParams(window.location.search).get('code')
  if (!code) return

  try {
    const session = await redeemAuthHandoffCode(code)
    saveAuthSession(session)
    window.history.replaceState({}, '', window.location.pathname)
  } catch (error) {
    console.error('Redeem code failed:', error)
    message.error('Lỗi xác thực: ' + (error.message || 'Không xác định'))
  }
}

const loadBooks = async () => {
  try {
    const token = localStorage.getItem('accessToken')
    const response = await fetch(`${BOOKS_API_URL}`, {
      headers: {
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      }
    })

    if (response.status === 401) {
      message.error('Phiên đăng nhập không hợp lệ hoặc đã hết hạn')
      books.value = []
      return
    }

    if (!response.ok) {
      const text = await response.text()
      console.error('loadBooks failed', response.status, text)
      message.error('Lỗi khi tải danh sách sách')
      books.value = []
      return
    }

    books.value = await response.json()
  } catch (error) {
    console.error('loadBooks error', error)
    message.error('Lỗi khi tải danh sách sách')
    books.value = []
  }
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
  const matchText = (value, keyword) => {
    if (!keyword) return true
    return String(value ?? '').toLowerCase().includes(keyword.toLowerCase())
  }

  return books.value.filter(book => {
    const statusText = getAvailable(book) > 0 ? 'Có thể mượn' : 'Hết sách'

    return (
      matchText(book.tenSach, columnFilters.tenSach) &&
      matchText(book.tacGia, columnFilters.tacGia) &&
      matchText(book.nhaSanXuat, columnFilters.nhaSanXuat) &&
      matchText(book.theLoai, columnFilters.theLoai) &&
      matchText(statusText, columnFilters.status)
    )
  })
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
      const res = await fetch(`${BOOKS_API_URL}/${editingId.value}`, {
        method: 'PUT',
        headers: getAuthHeaders(),
        body: JSON.stringify(payload)
      })
      if (!res.ok) {
        const err = await res.text()
        console.error('PUT failed:', res.status, err)
        return
      }
      await sendReportEvent('book.updated', payload.tenSach || payload.title)
    } else {
      const res = await fetch(BOOKS_API_URL, {
        method: 'POST',
        headers: getAuthHeaders(),
        body: JSON.stringify(payload)
      })
      if (!res.ok) {
        const err = await res.text()
        console.error('POST failed:', res.status, err)
        return
      }
      await sendReportEvent('book.added', payload.tenSach || payload.title)
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
  await fetch(`${BOOKS_API_URL}/${id}`, {
    method: 'DELETE',
    headers: getAuthHeaders()
  })
  await sendReportEvent(
    'book.deleted',
    selectedBook.value?.tenSach || selectedBook.value?.title || `ID ${id}`
  )
  await loadBooks()
}

const deleteBookFromModal = async (id) => {
  await deleteBook(id)
  detailOpen.value = false
}

const deleteSelectedBooks = () => {
  if (selectedRowKeys.value.length === 0) return

  Modal.confirm({
    title: 'Xóa các sách đã chọn?',
    content: `Bạn có chắc muốn xóa ${selectedRowKeys.value.length} sách đã chọn không?`,
    okText: 'Xóa',
    cancelText: 'Hủy',
    okType: 'danger',
    zIndex: 4000,
    async onOk() {
      for (const book of selectedBooks.value) {
        await fetch(`${BOOKS_API_URL}/${book.id}`, {
          method: 'DELETE',
          headers: getAuthHeaders()
        })
        await sendReportEvent('book.deleted', book.tenSach || book.title || `ID ${book.id}`)
      }
      clearSelection()
      await loadBooks()
      message.success('Đã xóa các sách đã chọn')
    }
  })
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

onMounted(async () => {
  await handleAuthCode()
  await loadBooks()
  await loadCategories()
  window.addEventListener('stock-imports-updated', loadBooks)
})

onUnmounted(() => {
  window.removeEventListener('stock-imports-updated', loadBooks)
})
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

/* Compact form styling for book modal */
.book-form .ant-form-item {
  margin-bottom: 8px;
}

/* Modal specific styling */
.book-form-modal .ant-modal-content,
.book-detail-modal .ant-modal-content,
.category-modal .ant-modal-content {
  border-radius: 12px;
}

.book-form-modal .ant-form-item,
.book-form-modal .book-form .ant-form-item {
  margin-bottom: 14px;
}

.book-form-modal .ant-modal-body {
  padding: 16px 24px;
}

.book-detail-modal .detail-image-wrapper { width: 100%; display:flex; align-items:center; justify-content:center }
.book-detail-modal .detail-image { width: 300px; height: 420px; object-fit: cover; border-radius: 14px }
.book-detail-modal .detail-info { padding-left: 12px }
.book-detail-modal .detail-description {
  margin-top: 12px;
  max-height: 3.6em; /* ~3 lines */
  overflow: hidden;
  text-overflow: ellipsis;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  line-clamp: 3;
  -webkit-box-orient: vertical;
}

.category-modal .ant-modal-body { padding: 12px 20px }
.category-modal .ant-input { width: 100%; }

.ant-modal-footer { display:flex; justify-content:flex-end }

.detail-image-wrapper { width: 100%; display:flex; align-items:center; justify-content:center }
.detail-image { width: 100%; height: 320px; object-fit: cover; border-radius: 8px }

/* Table wrapper to avoid page horizontal scroll */
.table-wrapper {
  width: 100%;
  max-width: 100%;
  overflow-x: auto; /* allow table internal scroll if needed */
}

.cell-theloai {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
}

.action-buttons {
  display: flex;
  gap: 6px;
  white-space: nowrap;
}

.rating-compact {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.rating-compact .stars { font-weight: 600 }
.rating-compact .count { font-size: 12px; color: #666 }
.selected-toolbar {
  margin-bottom: 16px;
  padding: 16px 20px;
  background: #f6ffed;
  border: 1px solid #d9f7be;
  border-radius: 12px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}

.selected-info {
  display: flex;
  align-items: center;
  gap: 8px;
  color: #135200;
  font-weight: 700;
  font-size: 14px;
}

:deep(.ant-table-thead > tr > th) {
  white-space: nowrap;
}

:deep(.ant-table-filter-trigger) {
  margin-left: 8px;
  color: #bfbfbf;
}

:deep(.ant-table-filter-trigger:hover) {
  color: #1677ff;
  background: #e6f4ff;
}

.ant-pro-search-icon {
  font-size: 14px;
  color: #bfbfbf;
}

:deep(.ant-table-filter-trigger:hover .ant-pro-search-icon) {
  color: #1677ff;
}

.ant-pro-search-icon.active {
  color: #1677ff;
}

.search-dropdown-box {
  padding: 10px;
  width: 240px;
}

.column-title {
  display: inline-flex;
  align-items: center;
  gap: 14px;
  white-space: nowrap;
}

.column-title-text {
  font-weight: 600;
  white-space: nowrap;
}

.column-search-icon {
  color: #bfbfbf;
  font-size: 14px;
  cursor: pointer;
  margin-left: 8px;
  margin-right: 8px;
  padding: 3px;
  border-radius: 6px;
  transition: all .2s ease;
}

.column-search-icon:hover,
.column-search-icon.active {
  color: #1677ff;
  background: #e6f4ff;
}

</style>
