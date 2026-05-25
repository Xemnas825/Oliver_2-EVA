<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'
import { useFavoritesStore } from '@/stores/favorites'
import type { FavoriteType } from '@/types'
import { BButton } from 'bootstrap-vue-next'

const props = withDefaults(
  defineProps<{
    type: FavoriteType
    id: number
    size?: 'sm' | 'md' | 'lg'
  }>(),
  { size: 'sm' }
)

const auth = useAuthStore()
const favoritesStore = useFavoritesStore()
const { t } = useI18n()

const isFav = computed(() => favoritesStore.isFavorite(props.type, props.id))

onMounted(() => {
  if (auth.isAuthenticated) {
    favoritesStore.loadFavorites()
  }
})
</script>

<template>
  <BButton
    v-if="auth.isAuthenticated"
    :size="size"
    :variant="isFav ? 'warning' : 'outline-warning'"
    class="favorite-btn"
    @click="favoritesStore.toggleFavorite(type, id)"
  >
    <span class="favorite-btn__icon" aria-hidden="true">{{ isFav ? '★' : '☆' }}</span>
    {{ isFav ? t('favorites.inFavorites') : t('favorites.add') }}
  </BButton>
</template>

<style scoped>
.favorite-btn__icon {
  margin-right: 0.35rem;
  font-size: 1.1em;
}
</style>
