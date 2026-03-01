<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useGamesStore } from '@/stores/games'
import { BButton, BAlert } from 'bootstrap-vue-next'

const route = useRoute()
const gamesStore = useGamesStore()
const { t } = useI18n()

const id = computed(() => Number(route.params.id))

onMounted(() => {
  gamesStore.fetchGame(id.value)
})
</script>

<template>
  <div class="container py-4">
    <BButton variant="outline-secondary" class="mb-3" :to="{ name: 'games' }">
      {{ t('common.backToList') }}
    </BButton>

    <BAlert v-if="gamesStore.error" variant="danger">
      {{ gamesStore.error }}
    </BAlert>

    <div v-else-if="gamesStore.loading" class="text-center py-5">
      {{ t('common.loading') }}
    </div>

    <div v-else-if="gamesStore.currentGame" class="row">
      <div class="col-lg-8">
        <h1 class="mb-2">{{ gamesStore.currentGame.name }}</h1>
        <p v-if="gamesStore.currentGame.year" class="text-muted mb-3">
          {{ t('common.year') }}: {{ gamesStore.currentGame.year }}
        </p>
        <div v-if="gamesStore.currentGame.imageUrl" class="detail-img-wrap">
          <img
            :src="gamesStore.currentGame.imageUrl"
            :alt="gamesStore.currentGame.name"
            loading="lazy"
          />
        </div>
        <p v-if="gamesStore.currentGame.description" class="white-space-pre-wrap">
          {{ gamesStore.currentGame.description }}
        </p>
        <p v-else class="text-muted">{{ t('common.noDescription') }}</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.detail-img-wrap {
  max-width: 500px;
  aspect-ratio: 3 / 4;
  overflow: hidden;
  border-radius: 0.375rem;
  background: var(--bs-secondary-bg);
  margin-bottom: 1rem;
}
.detail-img-wrap img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}
</style>
