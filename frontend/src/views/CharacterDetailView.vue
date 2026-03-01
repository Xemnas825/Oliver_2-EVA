<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useCharactersStore } from '@/stores/characters'
import { BButton, BAlert } from 'bootstrap-vue-next'

const route = useRoute()
const charactersStore = useCharactersStore()
const { t } = useI18n()

const id = computed(() => Number(route.params.id))

onMounted(() => {
  charactersStore.fetchCharacter(id.value)
})
</script>

<template>
  <div class="container py-4">
    <BButton variant="outline-secondary" class="mb-3" :to="{ name: 'characters' }">
      {{ t('common.backToList') }}
    </BButton>

    <BAlert v-if="charactersStore.error" variant="danger">
      {{ charactersStore.error }}
    </BAlert>

    <div v-else-if="charactersStore.loading" class="text-center py-5">
      {{ t('common.loading') }}
    </div>

    <div v-else-if="charactersStore.currentCharacter" class="row">
      <div class="col-lg-8">
        <h1 class="mb-2">{{ charactersStore.currentCharacter.name }}</h1>
        <p v-if="charactersStore.currentCharacter.gameName" class="text-muted mb-3">
          {{ t('common.game') }}: {{ charactersStore.currentCharacter.gameName }}
        </p>
        <div v-if="charactersStore.currentCharacter.imageUrl" class="detail-img-wrap">
          <img
            :src="charactersStore.currentCharacter.imageUrl"
            :alt="charactersStore.currentCharacter.name"
            loading="lazy"
          />
        </div>
        <p v-if="charactersStore.currentCharacter.description" class="white-space-pre-wrap">
          {{ charactersStore.currentCharacter.description }}
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
