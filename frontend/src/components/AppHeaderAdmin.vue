<script setup lang="ts">
import { useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { setLocale } from '@/i18n'
import { BNavbar, BNavbarBrand, BNavbarToggle, BCollapse, BNavbarNav, BNavItem, BNavItemDropdown, BDropdownItem } from 'bootstrap-vue-next'

const route = useRoute()
const { t } = useI18n()

const navItems = [
  { path: '/admin', name: 'admin', labelKey: 'nav.panel' },
  { path: '/admin/juegos', name: 'admin-games', labelKey: 'nav.games' },
  { path: '/admin/personajes', name: 'admin-characters', labelKey: 'nav.characters' },
]

const isActive = (path: string) => route.path === path || (path !== '/admin' && route.path.startsWith(path))

function changeLocale(locale: 'es' | 'en') {
  setLocale(locale)
}
</script>

<template>
  <BNavbar v-b-color-mode="'dark'" toggleable="lg" variant="secondary" class="shadow-sm">
    <div class="container-fluid">
      <BNavbarBrand to="/admin">{{ t('nav.appNameAdmin') }}</BNavbarBrand>
      <BNavbarToggle target="admin-nav-collapse" />
      <BCollapse id="admin-nav-collapse" is-nav>
        <BNavbarNav class="ms-auto">
          <BNavItemDropdown :text="t('nav.language')" right class="me-2">
            <BDropdownItem href="#" @click.prevent="changeLocale('es')">Español</BDropdownItem>
            <BDropdownItem href="#" @click.prevent="changeLocale('en')">English</BDropdownItem>
          </BNavItemDropdown>
          <BNavItem
            v-for="item in navItems"
            :key="item.path"
            :to="item.path"
            :active="isActive(item.path)"
          >
            {{ t(item.labelKey) }}
          </BNavItem>
          <BNavItem to="/">{{ t('nav.goToSite') }}</BNavItem>
        </BNavbarNav>
      </BCollapse>
    </div>
  </BNavbar>
</template>
