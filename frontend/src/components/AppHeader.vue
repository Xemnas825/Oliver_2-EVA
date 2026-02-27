<script setup lang="ts">
import { useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { BNavbar, BNavbarBrand, BNavbarToggle, BCollapse, BNavbarNav, BNavItem, BNavItemDropdown, BDropdownItem } from 'bootstrap-vue-next'

const route = useRoute()
const auth = useAuthStore()

const publicNavItems = [
  { path: '/', name: 'home', label: 'Inicio' },
  { path: '/juegos', name: 'games', label: 'Juegos' },
  { path: '/personajes', name: 'characters', label: 'Personajes' },
]

const isActive = (path: string) => {
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}

function logout() {
  auth.logout()
  window.location.href = '/'
}
</script>

<template>
  <BNavbar v-b-color-mode="'dark'" toggleable="lg" variant="primary" class="shadow-sm">
    <div class="container">
      <BNavbarBrand to="/">Wiki Videojuegos</BNavbarBrand>
      <BNavbarToggle target="nav-collapse" />
      <BCollapse id="nav-collapse" is-nav>
        <BNavbarNav>
          <BNavItem
            v-for="item in publicNavItems"
            :key="item.path"
            :to="item.path"
            :active="isActive(item.path)"
          >
            {{ item.label }}
          </BNavItem>
        </BNavbarNav>
        <BNavbarNav class="ms-auto">
          <template v-if="auth.isAuthenticated">
            <BNavItem v-if="auth.isAdmin" to="/admin" :active="route.path.startsWith('/admin')">
              Administración
            </BNavItem>
            <BNavItemDropdown right>
              <template #button-content>
                <span>{{ auth.user?.name ?? auth.user?.email }}</span>
              </template>
              <BDropdownItem href="#" @click.prevent="logout">Cerrar sesión</BDropdownItem>
            </BNavItemDropdown>
          </template>
          <template v-else>
            <BNavItem :to="{ name: 'login' }" :active="route.name === 'login'">
              Iniciar sesión
            </BNavItem>
            <BNavItem :to="{ name: 'register' }" :active="route.name === 'register'">
              Registro
            </BNavItem>
          </template>
        </BNavbarNav>
      </BCollapse>
    </div>
  </BNavbar>
</template>
