// frontend/src/config/api.config.js
// API Configuration - Dễ dàng thay đổi backend URL

/**
 * Backend API Base URL
 * 
 * Mặc định: sử dụng cùng host với frontend
 * 
 * Để thay đổi, chỉnh sửa một trong các cách sau:
 * 1. Thay đổi trực tiếp ở đây
 * 2. Sử dụng biến môi trường VITE_API_URL
 * 3. Thay đổi khi runtime qua window.API_CONFIG
 */

// Lấy URL từ biến môi trường (nếu có) hoặc dùng default
const getApiBaseUrl = () => {
  // Ưu tiên 1: Biến môi trường
  if (import.meta.env.VITE_API_URL) {
    return import.meta.env.VITE_API_URL
  }

  // Ưu tiên 2: Window runtime config (nếu được set bởi script khác)
  if (window.API_CONFIG?.BASE_URL) {
    return window.API_CONFIG.BASE_URL
  }

  // Ưu tiên 3: Default - sử dụng cùng host với frontend
  return `http://${window.location.hostname}:5185`
}

export const API_CONFIG = {
  // Base URL của backend
  BASE_URL: getApiBaseUrl(),

  // Endpoint cho danh sách sách
  BOOKS: {
    LIST: '/api/books',           // GET /api/books
    DETAIL: '/api/books/:id',     // GET /api/books/:id
    CREATE: '/api/books',         // POST /api/books
    UPDATE: '/api/books/:id',     // PUT /api/books/:id
    DELETE: '/api/books/:id'      // DELETE /api/books/:id
  },

  // Timeout cho request (ms)
  TIMEOUT: 30000,

  // Headers mặc định
  HEADERS: {
    'Content-Type': 'application/json'
  }
}

/**
 * Helper function: Tạo full URL từ endpoint
 * @param {string} endpoint - Endpoint API (ví dụ: '/api/books')
 * @param {object} params - Parameters để replace (ví dụ: { id: 1 })
 * @returns {string} Full URL
 */
export const getApiUrl = (endpoint, params = {}) => {
  let url = endpoint
  Object.keys(params).forEach(key => {
    url = url.replace(`:${key}`, params[key])
  })
  return `${API_CONFIG.BASE_URL}${url}`
}

/**
 * Helper function: Fetch dữ liệu từ API
 * @param {string} endpoint - Endpoint API
 * @param {object} options - Fetch options (method, body, headers, ...)
 * @returns {Promise} Response JSON
 */
export const fetchApi = async (endpoint, options = {}) => {
  const url = getApiUrl(endpoint, options.params)

  try {
    const response = await fetch(url, {
      ...options,
      headers: {
        ...API_CONFIG.HEADERS,
        ...options.headers
      },
      timeout: options.timeout || API_CONFIG.TIMEOUT
    })

    if (!response.ok) {
      throw new Error(`HTTP Error: ${response.status}`)
    }

    return await response.json()
  } catch (error) {
    console.error('API Error:', error)
    throw error
  }
}

export default API_CONFIG
