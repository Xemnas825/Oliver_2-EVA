<script setup lang="ts">
import { RouterView } from 'vue-router'
import AppHeaderAdmin from '@/components/AppHeaderAdmin.vue'
import AppFooterAdmin from '@/components/AppFooterAdmin.vue'
</script>

<template>
  <div class="layout-admin d-flex flex-column min-vh-100">
    <AppHeaderAdmin />
    <main class="flex-grow-1">
      <RouterView />
    </main>
    <AppFooterAdmin />
  </div>
</template>
