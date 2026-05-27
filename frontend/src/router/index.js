import { createRouter, createWebHistory } from 'vue-router'
import BooksView from '../views/BooksView.vue'
import DashBoardView from '../views/DashBoardView.vue'

const routes = [
  { path: '/', component: DashBoardView },
  { path: '/dashboard', redirect: '/' },
  { path: '/books', component: BooksView }
]

export default createRouter({
  history: createWebHistory(),
  routes
})