<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { BCard, BButton } from 'bootstrap-vue-next'

const { t } = useI18n()
</script>

<template>
  <div class="container py-5">
    <h1 class="mb-4">{{ t('admin.title') }}</h1>
    <div class="row g-3">
      <div class="col-md-6">
        <BCard :title="t('nav.games')" class="h-100">
          <p class="card-text">{{ t('admin.manageGames') }}</p>
          <BButton variant="primary" :to="{ name: 'admin-games' }">{{ t('admin.goToGames') }}</BButton>
        </BCard>
      </div>
      <div class="col-md-6">
        <BCard :title="t('nav.characters')" class="h-100">
          <p class="card-text">{{ t('admin.manageCharacters') }}</p>
          <BButton variant="primary" :to="{ name: 'admin-characters' }">{{ t('admin.goToCharacters') }}</BButton>
        </BCard>
      </div>
    </div>
  </div>
</template>
