<script setup lang="ts">
import { computed } from 'vue'
import type { Character } from '@/types'
import { BCard, BCardBody, BCardTitle, BCardText } from 'bootstrap-vue-next'

const props = defineProps<{
  character: Character
}>()

const truncate = (text: string | null, max: number) => {
  if (!text) return ''
  return text.length <= max ? text : text.slice(0, max) + '…'
}

const hasValidImageUrl = computed(() => {
  const url = props.character?.imageUrl
  if (!url || typeof url !== 'string') return false
  return url.startsWith('http://') || url.startsWith('https://')
})
</script>

<template>
  <BCard class="h-100 shadow-sm">
    <div v-if="hasValidImageUrl" class="card-img-aspect">
      <img
        :src="character.imageUrl!"
        :alt="character.name"
        loading="lazy"
      />
    </div>
    <BCardBody class="d-flex flex-column">
      <BCardTitle>{{ character.name }}</BCardTitle>
      <BCardText v-if="character.gameName" class="text-muted small mb-1">
        {{ character.gameName }}
      </BCardText>
      <BCardText class="flex-grow-1 small">
        {{ truncate(character.description, 120) }}
      </BCardText>
      <slot name="actions" />
    </BCardBody>
  </BCard>
</template>

<style scoped>
.card-img-aspect {
  aspect-ratio: 3 / 4;
  overflow: hidden;
  background: var(--bs-secondary-bg);
}
.card-img-aspect img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}
</style>
