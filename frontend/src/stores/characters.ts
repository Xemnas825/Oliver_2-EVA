import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api } from '@/services/api'
import type { Character } from '@/types'

export interface CharacterForm {
  name: string
  gameId: number
  imageUrl: string | null
  description: string | null
}

export const useCharactersStore = defineStore('characters', () => {
  const characters = ref<Character[]>([])
  const currentCharacter = ref<Character | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchCharacters(gameId?: number) {
    loading.value = true
    error.value = null
    try {
      const params = gameId != null ? { gameId } : {}
      const { data } = await api.get<Character[]>('/api/characters', { params })
      characters.value = data
    } catch (e) {
      error.value = 'Error al cargar los personajes.'
    } finally {
      loading.value = false
    }
  }

  async function fetchCharacter(id: number) {
    loading.value = true
    error.value = null
    currentCharacter.value = null
    try {
      const { data } = await api.get<Character>(`/api/characters/${id}`)
      currentCharacter.value = data
      return data
    } catch {
      error.value = 'Personaje no encontrado.'
      return null
    } finally {
      loading.value = false
    }
  }

  async function createCharacter(form: CharacterForm) {
    error.value = null
    try {
      const { data } = await api.post<Character>('/api/characters', {
        name: form.name,
        gameId: form.gameId,
        imageUrl: form.imageUrl || null,
        description: form.description || null,
      })
      characters.value = [...characters.value, data]
      return data
    } catch (e) {
      error.value = 'Error al crear el personaje.'
      return null
    }
  }

  async function updateCharacter(id: number, form: CharacterForm) {
    error.value = null
    try {
      const { data } = await api.put<Character>(`/api/characters/${id}`, {
        name: form.name,
        gameId: form.gameId,
        imageUrl: form.imageUrl || null,
        description: form.description || null,
      })
      const idx = characters.value.findIndex((c) => c.id === id)
      if (idx !== -1) characters.value[idx] = data
      if (currentCharacter.value?.id === id) currentCharacter.value = data
      return data
    } catch {
      error.value = 'Error al actualizar el personaje.'
      return null
    }
  }

  async function deleteCharacter(id: number) {
    error.value = null
    try {
      await api.delete(`/api/characters/${id}`)
      characters.value = characters.value.filter((c) => c.id !== id)
      if (currentCharacter.value?.id === id) currentCharacter.value = null
      return true
    } catch {
      error.value = 'Error al eliminar el personaje.'
      return false
    }
  }

  return {
    characters,
    currentCharacter,
    loading,
    error,
    fetchCharacters,
    fetchCharacter,
    createCharacter,
    updateCharacter,
    deleteCharacter,
  }
})
