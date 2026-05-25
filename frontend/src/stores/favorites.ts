import { defineStore } from 'pinia'
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'
import type { FavoriteItem, FavoriteType } from '@/types'

function storageKey(userId: number) {
  return `favorites_${userId}`
}

export const useFavoritesStore = defineStore('favorites', () => {
  const favorites = ref<FavoriteItem[]>([])

  function loadFavorites() {
    const userId = useAuthStore().user?.id
    if (!userId) {
      favorites.value = []
      return
    }
    const raw = localStorage.getItem(storageKey(userId))
    if (!raw) {
      favorites.value = []
      return
    }
    try {
      favorites.value = JSON.parse(raw) as FavoriteItem[]
    } catch {
      favorites.value = []
    }
  }

  function saveFavorites() {
    const userId = useAuthStore().user?.id
    if (!userId) return
    localStorage.setItem(storageKey(userId), JSON.stringify(favorites.value))
  }

  function isFavorite(type: FavoriteType, id: number) {
    return favorites.value.some((f: FavoriteItem) => f.type === type && f.id === id)
  }

  function toggleFavorite(type: FavoriteType, id: number) {
    if (isFavorite(type, id)) {
      removeFavorite(type, id)
    } else {
      favorites.value.push({ type, id })
      saveFavorites()
    }
  }

  function removeFavorite(type: FavoriteType, id: number) {
    favorites.value = favorites.value.filter(
      (f: FavoriteItem) => !(f.type === type && f.id === id)
    )
    saveFavorites()
  }

  function resetFavorites() {
    favorites.value = []
  }

  return {
    favorites,
    loadFavorites,
    isFavorite,
    toggleFavorite,
    removeFavorite,
    resetFavorites,
  }
})
