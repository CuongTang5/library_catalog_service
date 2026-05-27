import { createRouter, createWebHistory } from 'vue-router';
import BooksView from '@/views/BooksView.vue';

const routes = [
  {
    path: '/',
    redirect: '/books'
  },
  {
    path: '/books',
    name: 'Books',
    component: BooksView
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
