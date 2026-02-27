<script setup lang="ts">
import { useRoute } from 'vue-router'
import { BNavbar, BNavbarBrand, BNavbarToggle, BCollapse, BNavbarNav, BNavItem } from 'bootstrap-vue-next'

const route = useRoute()

const navItems = [
  { path: '/', name: 'home', label: 'Inicio' },
  { path: '/juegos', name: 'games', label: 'Juegos' },
  { path: '/personajes', name: 'characters', label: 'Personajes' },
  { path: '/login', name: 'login', label: 'Iniciar sesión' },
  { path: '/registro', name: 'register', label: 'Registro' },
]

const isActive = (path: string) => {
  if (path === '/') return route.path === '/'
  return route.path.startsWith(path)
}
</script>

<template>
  <BNavbar v-b-color-mode="'dark'" toggleable="lg" variant="primary" class="shadow-sm">
    <div class="container">
      <BNavbarBrand to="/">Wiki Videojuegos</BNavbarBrand>
      <BNavbarToggle target="nav-collapse" />
      <BCollapse id="nav-collapse" is-nav>
        <BNavbarNav class="ms-auto">
          <BNavItem
            v-for="item in navItems"
            :key="item.path"
            :to="item.path"
            :active="isActive(item.path)"
          >
            {{ item.label }}
          </BNavItem>
        </BNavbarNav>
      </BCollapse>
    </div>
  </BNavbar>
</template>
