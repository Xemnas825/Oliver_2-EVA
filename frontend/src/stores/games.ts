import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api } from '@/services/api'
import type { Game } from '@/types'

export interface GameForm {
  name: string
  imageUrl: string | null
  year: number | null
  description: string | null
}

export const useGamesStore = defineStore('games', () => {
  const games = ref<Game[]>([])
  const currentGame = ref<Game | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchGames() {
    loading.value = true
    error.value = null
    try {
      const { data } = await api.get<Game[]>('/api/games')
      games.value = data
    } catch (e) {
      error.value = 'Error al cargar los juegos.'
    } finally {
      loading.value = false
    }
  }

  async function fetchGame(id: number) {
    loading.value = true
    error.value = null
    currentGame.value = null
    try {
      const { data } = await api.get<Game>(`/api/games/${id}`)
      currentGame.value = data
      return data
    } catch {
      error.value = 'Juego no encontrado.'
      return null
    } finally {
      loading.value = false
    }
  }

  async function createGame(form: GameForm) {
    error.value = null
    try {
      const { data } = await api.post<Game>('/api/games', {
        name: form.name,
        imageUrl: form.imageUrl || null,
        year: form.year ?? null,
        description: form.description || null,
      })
      games.value = [...games.value, data]
      return data
    } catch (e) {
      error.value = 'Error al crear el juego.'
      return null
    }
  }

  async function updateGame(id: number, form: GameForm) {
    error.value = null
    try {
      const { data } = await api.put<Game>(`/api/games/${id}`, {
        name: form.name,
        imageUrl: form.imageUrl || null,
        year: form.year ?? null,
        description: form.description || null,
      })
      const idx = games.value.findIndex((g) => g.id === id)
      if (idx !== -1) games.value[idx] = data
      if (currentGame.value?.id === id) currentGame.value = data
      return data
    } catch {
      error.value = 'Error al actualizar el juego.'
      return null
    }
  }

  async function deleteGame(id: number) {
    error.value = null
    try {
      await api.delete(`/api/games/${id}`)
      games.value = games.value.filter((g) => g.id !== id)
      if (currentGame.value?.id === id) currentGame.value = null
      return true
    } catch {
      error.value = 'Error al eliminar el juego.'
      return false
    }
  }

  return {
    games,
    currentGame,
    loading,
    error,
    fetchGames,
    fetchGame,
    createGame,
    updateGame,
    deleteGame,
  }
})
