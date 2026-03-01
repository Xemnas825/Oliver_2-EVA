<script setup lang="ts">
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useI18n } from 'vue-i18n'
import { setLocale } from '@/i18n'
import { BNavbar, BNavbarBrand, BNavbarToggle, BCollapse, BNavbarNav, BNavItem, BNavItemDropdown, BDropdownItem } from 'bootstrap-vue-next'

const route = useRoute()
const auth = useAuthStore()
const { t } = useI18n()

const publicNavItems = [
  { path: '/', name: 'home', labelKey: 'nav.home' },
  { path: '/juegos', name: 'games', labelKey: 'nav.games' },
  { path: '/personajes', name: 'characters', labelKey: 'nav.characters' },
]

const isActive = (path: string) => {
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}

function logout() {
  auth.logout()
  window.location.href = '/'
}

function changeLocale(locale: 'es' | 'en') {
  setLocale(locale)
}
</script>

<template>
  <BNavbar v-b-color-mode="'dark'" toggleable="lg" variant="primary" class="shadow-sm">
    <div class="container">
      <BNavbarBrand to="/">{{ t('nav.appName') }}</BNavbarBrand>
      <BNavbarToggle target="nav-collapse" />
      <BCollapse id="nav-collapse" is-nav>
        <BNavbarNav>
          <BNavItem
            v-for="item in publicNavItems"
            :key="item.path"
            :to="item.path"
            :active="isActive(item.path)"
          >
            {{ t(item.labelKey) }}
          </BNavItem>
        </BNavbarNav>
        <BNavbarNav class="ms-auto">
          <BNavItemDropdown :text="t('nav.language')" right class="me-2">
            <BDropdownItem href="#" @click.prevent="changeLocale('es')">Español</BDropdownItem>
            <BDropdownItem href="#" @click.prevent="changeLocale('en')">English</BDropdownItem>
          </BNavItemDropdown>
          <template v-if="auth.isAuthenticated">
            <BNavItem v-if="auth.isAdmin" to="/admin" :active="route.path.startsWith('/admin')">
              {{ t('nav.admin') }}
            </BNavItem>
            <BNavItemDropdown right>
              <template #button-content>
                <span>{{ auth.user?.name ?? auth.user?.email }}</span>
              </template>
              <BDropdownItem href="#" @click.prevent="logout">{{ t('nav.logout') }}</BDropdownItem>
            </BNavItemDropdown>
          </template>
          <template v-else>
            <BNavItem :to="{ name: 'login' }" :active="route.name === 'login'">
              {{ t('nav.login') }}
            </BNavItem>
            <BNavItem :to="{ name: 'register' }" :active="route.name === 'register'">
              {{ t('nav.register') }}
            </BNavItem>
          </template>
        </BNavbarNav>
      </BCollapse>
    </div>
  </BNavbar>
</template>
