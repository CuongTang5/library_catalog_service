<template>
  <div class="stock-import-view">
    <a-row :gutter="[16, 16]" class="mb-4">
      <a-col :span="24">
        <a-button type="primary" size="large" @click="showCreateModal = true">
          📦 Tạo phiếu nhập
        </a-button>
      </a-col>
    </a-row>

    <!-- List of Receipts -->
    <a-table
      :columns="columns"
      :data-source="receipts"
      :loading="loading"
      rowKey="id"
      :pagination="{ pageSize: 10, showSizeChanger: true }"
      :scroll="{ x: 1000 }"
    >
      <template #bodyCell="{ column, record }">
        <template v-if="column.key === 'status'">
          <a-tag :color="statusColor(record.status)">{{ statusText(record.status) }}</a-tag>
        </template>

        <template v-else-if="column.key === 'action'">
          <a-space>
            <a-button type="link" size="small" @click="openDetailModal(record)">Chi tiết</a-button>
            <a-button
              v-if="record.status === 'Pending'"
              type="link"
              size="small"
              status="success"
              @click="confirmApproveReceipt(record.id)"
            >
              Duyệt
            </a-button>
            <a-button
              v-if="record.status === 'Pending'"
              type="link"
              size="small"
              status="danger"
              @click="confirmCancelReceipt(record.id)"
            >
              Hủy
            </a-button>
          </a-space>
        </template>
      </template>
    </a-table>

    <!-- Create Receipt Modal -->
    <a-modal
      v-model:visible="showCreateModal"
      title="Tạo phiếu nhập kho"
      :width="700"
      @ok="submitCreateReceipt"
      okText="Lưu"
      cancelText="Hủy"
    >
      <a-form :model="createForm" layout="vertical">
        <a-form-item label="Người tạo" required>
          <a-input v-model:value="createForm.createdBy" placeholder="Nhập tên người tạo" />
        </a-form-item>

        <a-form-item label="Ghi chú">
          <a-textarea v-model:value="createForm.note" placeholder="Ghi chú về phiếu nhập" rows="3" />
        </a-form-item>

        <a-divider>Chi tiết hàng nhập</a-divider>

        <div v-if="createForm.items.length === 0" class="text-center mb-4">
          <p>Không có mục nào. Nhấn "Thêm mục" để bắt đầu.</p>
        </div>

        <div v-for="(item, index) in createForm.items" :key="index" class="mb-4 p-4 border rounded">
          <a-row :gutter="[16, 16]">
            <a-col :span="24">
              <a-form-item label="Sách" required>
                <a-select
                  v-model:value="item.bookId"
                  placeholder="Chọn sách"
                  :options="bookOptions"
                  :filterOption="filterBookOption"
                  :notFoundContent="bookLoading ? 'Đang tải...' : 'Không tìm thấy'"
                  optionLabelProp="label"
                  show-search
                >
                  <template #option="{ value, label }">
                    <div>{{ label }}</div>
                  </template>
                </a-select>
              </a-form-item>
            </a-col>

            <a-col :span="12">
              <a-form-item label="Số lượng" required>
                <a-input-number
                  v-model:value="item.quantity"
                  :min="1"
                  :precision="0"
                  placeholder="Số lượng"
                />
              </a-form-item>
            </a-col>

            <a-col :span="12">
              <a-form-item label="Tình trạng" required>
                <a-select
                  v-model:value="item.condition"
                  placeholder="Chọn tình trạng"
                  :options="conditionOptions"
                />
              </a-form-item>
            </a-col>

            <a-col :span="24">
              <a-form-item label="Ghi chú">
                <a-input v-model:value="item.note" placeholder="Ghi chú cho mục này" />
              </a-form-item>
            </a-col>

            <a-col :span="24">
              <a-button danger @click="removeItem(index)">Xóa mục này</a-button>
            </a-col>
          </a-row>
        </div>

        <a-button type="dashed" class="w-100" @click="addItem">+ Thêm mục</a-button>
      </a-form>
    </a-modal>

    <!-- Detail Modal -->
    <a-modal
      v-model:visible="showDetailModal"
      :title="`Chi tiết phiếu: ${detailReceipt?.code}`"
      :width="800"
      okText="Đóng"
      :cancelButtonProps="{ style: { display: 'none' } }"
    >
      <a-descriptions :column="2" v-if="detailReceipt">
        <a-descriptions-item label="Mã phiếu">{{ detailReceipt.code }}</a-descriptions-item>
        <a-descriptions-item label="Trạng thái">
          <a-tag :color="statusColor(detailReceipt.status)">{{ statusText(detailReceipt.status) }}</a-tag>
        </a-descriptions-item>
        <a-descriptions-item label="Ngày nhập">{{ formatDate(detailReceipt.importDate) }}</a-descriptions-item>
        <a-descriptions-item label="Người tạo">{{ detailReceipt.createdBy }}</a-descriptions-item>
        <a-descriptions-item label="Tạo lúc">{{ formatDateTime(detailReceipt.createdAt) }}</a-descriptions-item>
        <a-descriptions-item label="Duyệt lúc" v-if="detailReceipt.approvedAt">
          {{ formatDateTime(detailReceipt.approvedAt) }}
        </a-descriptions-item>
        <a-descriptions-item label="Ghi chú" :span="2">{{ detailReceipt.note || '(không có)' }}</a-descriptions-item>
      </a-descriptions>

      <a-divider>Danh sách hàng</a-divider>

      <a-table
        :columns="itemColumns"
        :data-source="detailReceipt?.items || []"
        rowKey="id"
        :pagination="false"
        :bordered="true"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'condition'">
            <a-tag :color="conditionColor(record.condition)">{{ record.condition }}</a-tag>
          </template>
        </template>
      </a-table>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { message, Modal } from 'ant-design-vue'
import { API_CONFIG } from '@/config/api.config'
import { getAuthToken } from '../utils/auth'

const loading = ref(false)
const bookLoading = ref(false)
const receipts = ref([])
const books = ref([])
const showCreateModal = ref(false)
const showDetailModal = ref(false)
const detailReceipt = ref(null)

const createForm = ref({
  createdBy: '',
  note: '',
  items: []
})

const conditionOptions = [
  { label: 'Tốt', value: 'Good' },
  { label: 'Hỏng', value: 'Damaged' },
  { label: 'Cháy', value: 'Burned' },
  { label: 'Mất', value: 'Lost' }
]

const getAuthHeaders = () => {
  const token = getAuthToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
}

const apiFetch = async (endpoint, options = {}) => {
  const url = `${API_CONFIG.BASE_URL}${endpoint}`
  const response = await fetch(url, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...getAuthHeaders(),
      ...(options.headers || {})
    }
  })

  if (!response.ok) {
    const text = await response.text()
    throw new Error(text || `HTTP ${response.status}`)
  }

  return response.status === 204 ? null : await response.json()
}

const columns = [
  {
    title: 'Mã phiếu',
    dataIndex: 'code',
    key: 'code',
    width: 120
  },
  {
    title: 'Ngày nhập',
    dataIndex: 'importDate',
    key: 'importDate',
    width: 120,
    customRender: ({ text }) => formatDate(text)
  },
  {
    title: 'Người tạo',
    dataIndex: 'createdBy',
    key: 'createdBy',
    width: 100
  },
  {
    title: 'Tổng SL',
    dataIndex: 'totalQuantity',
    key: 'totalQuantity',
    width: 80
  },
  {
    title: 'SL tốt',
    dataIndex: 'goodQuantity',
    key: 'goodQuantity',
    width: 80
  },
  {
    title: 'SL lỗi',
    dataIndex: 'defectiveQuantity',
    key: 'defectiveQuantity',
    width: 80
  },
  {
    title: 'Trạng thái',
    dataIndex: 'status',
    key: 'status',
    width: 100
  },
  {
    title: 'Thao tác',
    key: 'action',
    width: 150,
    fixed: 'right'
  }
]

const itemColumns = [
  {
    title: 'Tên sách',
    dataIndex: 'bookName',
    key: 'bookName'
  },
  {
    title: 'Số lượng',
    dataIndex: 'quantity',
    key: 'quantity',
    width: 80
  },
  {
    title: 'Tình trạng',
    dataIndex: 'condition',
    key: 'condition',
    width: 100
  },
  {
    title: 'Ghi chú',
    dataIndex: 'note',
    key: 'note'
  }
]

const bookOptions = ref([])

const filterBookOption = (input, option) => {
  return option.label.toLowerCase().includes(input.toLowerCase())
}

const statusColor = (status) => {
  switch (status) {
    case 'Pending':
      return 'processing'
    case 'Approved':
      return 'success'
    case 'Cancelled':
      return 'error'
    default:
      return 'default'
  }
}

const statusText = (status) => {
  switch (status) {
    case 'Pending':
      return 'Chờ duyệt'
    case 'Approved':
      return 'Đã duyệt'
    case 'Cancelled':
      return 'Đã hủy'
    default:
      return status
  }
}

const conditionColor = (condition) => {
  switch (condition) {
    case 'Good':
      return 'green'
    case 'Damaged':
      return 'orange'
    case 'Burned':
      return 'red'
    case 'Lost':
      return 'default'
    default:
      return 'default'
  }
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return date.toLocaleDateString('vi-VN')
}

const formatDateTime = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return date.toLocaleString('vi-VN')
}

const fetchReceipts = async () => {
  loading.value = true
  try {
    const response = await apiFetch('/api/stock-imports')
    receipts.value = response
  } catch (error) {
    message.error('Lỗi khi tải danh sách phiếu nhập: ' + (error.message || error))
  } finally {
    loading.value = false
  }
}

const fetchBooks = async () => {
  bookLoading.value = true
  try {
    const response = await apiFetch('/api/books')
    books.value = response
    bookOptions.value = response.map((book) => ({
      label: `${book.tenSach} (${book.tacGia})`,
      value: book.id
    }))
  } catch (error) {
    message.error('Lỗi khi tải danh sách sách: ' + (error.response?.data?.message || error.message))
  } finally {
    bookLoading.value = false
  }
}

const addItem = () => {
  createForm.value.items.push({
    bookId: undefined,
    quantity: 1,
    condition: 'Good',
    note: ''
  })
}

const removeItem = (index) => {
  createForm.value.items.splice(index, 1)
}

const submitCreateReceipt = async () => {
  if (!createForm.value.createdBy.trim()) {
    message.error('Vui lòng nhập tên người tạo')
    return
  }

  if (createForm.value.items.length === 0) {
    message.error('Vui lòng thêm ít nhất một mục')
    return
  }

  for (const item of createForm.value.items) {
    if (!item.bookId) {
      message.error('Vui lòng chọn sách cho tất cả các mục')
      return
    }
    if (!item.quantity || item.quantity < 1) {
      message.error('Vui lòng nhập số lượng hợp lệ')
      return
    }
  }

  try {
    await apiFetch('/api/stock-imports', {
      method: 'POST',
      body: JSON.stringify({
        createdBy: createForm.value.createdBy.trim(),
        note: createForm.value.note?.trim() || null,
        items: createForm.value.items.map((item) => ({
          bookId: item.bookId,
          quantity: item.quantity,
          condition: item.condition || 'Good',
          note: item.note?.trim() || null
        }))
      })
    })

    message.success('Tạo phiếu nhập thành công')
    showCreateModal.value = false
    createForm.value = {
      createdBy: '',
      note: '',
      items: []
    }
    await fetchReceipts()
    window.dispatchEvent(new Event('stock-imports-updated'))
  } catch (error) {
    message.error('Lỗi khi tạo phiếu nhập: ' + (error.message || error))
  }
}

const openDetailModal = async (record) => {
  try {
    const response = await apiFetch(`/api/stock-imports/${record.id}`)
    detailReceipt.value = response
    showDetailModal.value = true
  } catch (error) {
    message.error('Lỗi khi tải chi tiết phiếu nhập: ' + (error.response?.data?.message || error.message))
  }
}

const confirmApproveReceipt = (id) => {
  Modal.confirm({
    title: 'Bạn có chắc muốn duyệt phiếu nhập này không?',
    okText: 'Duyệt',
    cancelText: 'Hủy',
    okType: 'primary',
    onOk: async () => await approveReceipt(id)
  })
}

const confirmCancelReceipt = (id) => {
  Modal.confirm({
    title: 'Bạn có chắc muốn hủy phiếu nhập này không?',
    okText: 'Hủy',
    cancelText: 'Đóng',
    okType: 'danger',
    onOk: async () => await cancelReceipt(id)
  })
}

const approveReceipt = async (id) => {
  try {
    await apiFetch(`/api/stock-imports/${id}/approve`, { method: 'POST' })
    message.success('Duyệt phiếu nhập thành công')
    await fetchReceipts()
    window.dispatchEvent(new Event('stock-imports-updated'))
  } catch (error) {
    message.error('Lỗi khi duyệt phiếu nhập: ' + (error.message || error))
  }
}

const cancelReceipt = async (id) => {
  try {
    await apiFetch(`/api/stock-imports/${id}/cancel`, { method: 'POST' })
    message.success('Hủy phiếu nhập thành công')
    await fetchReceipts()
    window.dispatchEvent(new Event('stock-imports-updated'))
  } catch (error) {
    message.error('Lỗi khi hủy phiếu nhập: ' + (error.message || error))
  }
}

onMounted(async () => {
  await Promise.all([fetchReceipts(), fetchBooks()])
})
</script>

<style scoped>
.stock-import-view {
  padding: 20px;
}

.mb-4 {
  margin-bottom: 16px;
}

.p-4 {
  padding: 16px;
}

.border {
  border: 1px solid #d9d9d9;
}

.rounded {
  border-radius: 4px;
}

.w-100 {
  width: 100%;
}

.text-center {
  text-align: center;
}
</style>
