import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import LayoutAuth from '@/layouts/LayoutAuth.vue'
import LayoutPublic from '@/layouts/LayoutPublic.vue'
import LayoutAdmin from '@/layouts/LayoutAdmin.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: LayoutPublic,
    children: [
      {
        path: '',
        name: 'home',
        component: () => import('@/views/HomeView.vue'),
        meta: { title: 'Inicio' },
      },
      {
        path: 'juegos',
        name: 'games',
        component: () => import('@/views/GamesListView.vue'),
        meta: { title: 'Juegos' },
      },
      {
        path: 'personajes',
        name: 'characters',
        component: () => import('@/views/CharactersListView.vue'),
        meta: { title: 'Personajes' },
      },
    ],
  },
  {
    path: '/login',
    component: LayoutAuth,
    children: [
      {
        path: '',
        name: 'login',
        component: () => import('@/views/LoginView.vue'),
        meta: { title: 'Iniciar sesión' },
      },
    ],
  },
  {
    path: '/registro',
    component: LayoutAuth,
    children: [
      {
        path: '',
        name: 'register',
        component: () => import('@/views/RegisterView.vue'),
        meta: { title: 'Registro' },
      },
    ],
  },
  {
    path: '/admin',
    component: LayoutAdmin,
    children: [
      {
        path: '',
        name: 'admin',
        component: () => import('@/views/AdminView.vue'),
        meta: { title: 'Administración' },
      },
      {
        path: 'juegos',
        name: 'admin-games',
        component: () => import('@/views/AdminGamesView.vue'),
        meta: { title: 'Admin Juegos' },
      },
      {
        path: 'personajes',
        name: 'admin-characters',
        component: () => import('@/views/AdminCharactersView.vue'),
        meta: { title: 'Admin Personajes' },
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, _from, next) => {
  document.title = to.meta.title ? `${to.meta.title} | Wiki Videojuegos` : 'Wiki Videojuegos'
  next()
})

export default router
