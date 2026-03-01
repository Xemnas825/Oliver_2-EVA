import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { i18n } from '@/i18n'
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
        meta: { titleKey: 'nav.home' },
      },
      {
        path: 'juegos',
        name: 'games',
        component: () => import('@/views/GamesListView.vue'),
        meta: { titleKey: 'games.title' },
      },
      {
        path: 'juegos/:id',
        name: 'game-detail',
        component: () => import('@/views/GameDetailView.vue'),
        meta: { titleKey: 'games.detailTitle' },
      },
      {
        path: 'personajes',
        name: 'characters',
        component: () => import('@/views/CharactersListView.vue'),
        meta: { titleKey: 'characters.title' },
      },
      {
        path: 'personajes/:id',
        name: 'character-detail',
        component: () => import('@/views/CharacterDetailView.vue'),
        meta: { titleKey: 'characters.detailTitle' },
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
        meta: { titleKey: 'auth.loginTitle' },
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
        meta: { titleKey: 'auth.registerTitle' },
      },
    ],
  },
  {
    path: '/admin',
    component: LayoutAdmin,
    meta: { requiresAuth: true, requiresAdmin: true },
    children: [
      {
        path: '',
        name: 'admin',
        component: () => import('@/views/AdminView.vue'),
        meta: { titleKey: 'nav.admin' },
      },
      {
        path: 'juegos',
        name: 'admin-games',
        component: () => import('@/views/AdminGamesView.vue'),
        meta: { titleKey: 'games.adminTitle' },
      },
      {
        path: 'personajes',
        name: 'admin-characters',
        component: () => import('@/views/AdminCharactersView.vue'),
        meta: { titleKey: 'characters.adminTitle' },
      },
    ],
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

router.beforeEach((to, _from, next) => {
  const t = i18n.global.t
  const titleKey = to.meta.titleKey as string | undefined
  const appName = t('nav.appName')
  document.title = titleKey ? `${t(titleKey)} | ${appName}` : appName

  const authStore = useAuthStore()
  const requiresAuth = to.matched.some((r) => r.meta.requiresAuth)
  const requiresAdmin = to.matched.some((r) => r.meta.requiresAdmin)
  const isAuthPage = to.name === 'login' || to.name === 'register'

  if (requiresAuth && !authStore.isAuthenticated) {
    next({ name: 'login', query: { redirect: to.fullPath } })
    return
  }
  if (requiresAdmin && !authStore.isAdmin) {
    next({ path: '/' })
    return
  }
  if (isAuthPage && authStore.isAuthenticated) {
    next({ path: authStore.isAdmin ? '/admin' : '/' })
    return
  }

  next()
})

export default router
