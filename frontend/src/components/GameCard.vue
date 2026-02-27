<script setup lang="ts">
import { computed } from 'vue'
import type { Game } from '@/types'
import { BCard, BCardImg, BCardBody, BCardTitle, BCardText, BButton } from 'bootstrap-vue-next'

const props = defineProps<{
  game: Game
}>()

const truncate = (text: string | null, max: number) => {
  if (!text) return ''
  return text.length <= max ? text : text.slice(0, max) + '…'
}

const hasValidImageUrl = computed(() => {
  const url = props.game?.imageUrl
  if (!url || typeof url !== 'string') return false
  return url.startsWith('http://') || url.startsWith('https://')
})
</script>

<template>
  <BCard class="h-100 shadow-sm">
    <BCardImg
      v-if="hasValidImageUrl"
      :src="game.imageUrl!"
      :alt="game.name"
      top
      height="180"
      style="object-fit: cover"
    />
    <BCardBody class="d-flex flex-column">
      <BCardTitle>{{ game.name }}</BCardTitle>
      <BCardText v-if="game.year" class="text-muted small mb-1">
        Año: {{ game.year }}
      </BCardText>
      <BCardText class="flex-grow-1 small">
        {{ truncate(game.description, 120) }}
      </BCardText>
      <slot name="actions" />
    </BCardBody>
  </BCard>
</template>
