import { createRouter, createWebHistory } from 'vue-router'
import BooksView from '../views/BooksView.vue'
import DashBoardView from '../views/DashBoardView.vue'
import StockImportView from '../views/StockImportView.vue'

const routes = [
  { path: '/', component: DashBoardView },
  { path: '/dashboard', redirect: '/' },
  { path: '/books', component: BooksView },
  { path: '/stock-imports', component: StockImportView }
]

export default createRouter({
  history: createWebHistory(),
  routes
})