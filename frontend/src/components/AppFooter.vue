<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

<template>
  <footer class="bg-dark text-light py-4 mt-auto">
    <div class="container">
      <div class="row align-items-center">
        <div class="col-md-6">
          <span class="fw-bold">{{ t('nav.appName') }}</span> — {{ t('footer.tagline') }}
        </div>
        <div class="col-md-6 text-md-end mt-2 mt-md-0">
          <small>&copy; {{ new Date().getFullYear() }}</small>
        </div>
      </div>
    </div>
  </footer>
</template>
