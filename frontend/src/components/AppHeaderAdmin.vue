<script setup lang="ts">
import { useRoute } from 'vue-router'
import { BNavbar, BNavbarBrand, BNavbarToggle, BCollapse, BNavbarNav, BNavItem } from 'bootstrap-vue-next'

const route = useRoute()

const navItems = [
  { path: '/admin', name: 'admin', label: 'Panel' },
  { path: '/admin/juegos', name: 'admin-games', label: 'Juegos' },
  { path: '/admin/personajes', name: 'admin-characters', label: 'Personajes' },
]

const isActive = (path: string) => route.path === path || (path !== '/admin' && route.path.startsWith(path))
</script>

<template>
  <BNavbar v-b-color-mode="'dark'" toggleable="lg" variant="secondary" class="shadow-sm">
    <div class="container-fluid">
      <BNavbarBrand to="/admin">Wiki Videojuegos — Admin</BNavbarBrand>
      <BNavbarToggle target="admin-nav-collapse" />
      <BCollapse id="admin-nav-collapse" is-nav>
        <BNavbarNav class="ms-auto">
          <BNavItem
            v-for="item in navItems"
            :key="item.path"
            :to="item.path"
            :active="isActive(item.path)"
          >
            {{ item.label }}
          </BNavItem>
          <BNavItem to="/">Salir al sitio</BNavItem>
        </BNavbarNav>
      </BCollapse>
    </div>
  </BNavbar>
</template>
