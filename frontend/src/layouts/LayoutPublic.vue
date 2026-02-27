<script setup lang="ts">
import { RouterView } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
import AppFooter from '@/components/AppFooter.vue'
</script>

<template>
  <div class="layout-public d-flex flex-column min-vh-100">
    <AppHeader />
    <main class="flex-grow-1">
      <RouterView />
    </main>
    <AppFooter />
  </div>
</template>
