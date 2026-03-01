<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useCharactersStore } from '@/stores/characters'
import { useGamesStore } from '@/stores/games'
import type { Character } from '@/types'
import type { CharacterForm } from '@/stores/characters'
import CharacterCard from '@/components/CharacterCard.vue'
import CharacterFormComponent from '@/components/CharacterForm.vue'
import { BButton, BModal, BAlert } from 'bootstrap-vue-next'

const charactersStore = useCharactersStore()
const gamesStore = useGamesStore()
const showModal = ref(false)
const showDeleteModal = ref(false)
const editingCharacter = ref<Character | null>(null)
const characterToDelete = ref<Character | null>(null)

onMounted(() => {
  gamesStore.fetchGames()
  charactersStore.fetchCharacters()
})

watch(showModal, (visible) => {
  if (!visible) editingCharacter.value = null
})

function openCreate() {
  editingCharacter.value = null
  showModal.value = true
}

function openEdit(character: Character) {
  editingCharacter.value = character
  showModal.value = true
}

async function handleSubmit(form: CharacterForm) {
  if (editingCharacter.value) {
    const updated = await charactersStore.updateCharacter(editingCharacter.value.id, form)
    if (updated) showModal.value = false
  } else {
    const created = await charactersStore.createCharacter(form)
    if (created) showModal.value = false
  }
}

function askDelete(character: Character) {
  characterToDelete.value = character
  showDeleteModal.value = true
}

async function confirmDelete() {
  if (!characterToDelete.value) return
  const ok = await charactersStore.deleteCharacter(characterToDelete.value.id)
  if (ok) {
    showDeleteModal.value = false
    characterToDelete.value = null
  }
}

function cancelDelete() {
  showDeleteModal.value = false
  characterToDelete.value = null
}
</script>

<template>
  <div class="container py-4">
    <h1 class="mb-4">Administrar Personajes</h1>

    <BAlert v-if="charactersStore.error" variant="danger" dismissible class="mb-3">
      {{ charactersStore.error }}
    </BAlert>

    <div class="mb-3">
      <BButton variant="primary" @click="openCreate">Añadir personaje</BButton>
    </div>

    <div v-if="charactersStore.loading && charactersStore.characters.length === 0" class="text-center py-5">
      Cargando…
    </div>

    <div v-else-if="charactersStore.characters.length === 0" class="text-muted py-5">
      No hay personajes. Añade el primero.
    </div>

    <div v-else class="row g-4">
      <div
        v-for="character in charactersStore.characters"
        :key="character.id"
        class="col-sm-6 col-lg-4"
      >
        <CharacterCard :character="character">
          <template #actions>
            <div class="d-flex gap-1">
              <BButton variant="outline-secondary" size="sm" @click="openEdit(character)">
                Editar
              </BButton>
              <BButton variant="outline-danger" size="sm" @click="askDelete(character)">
                Borrar
              </BButton>
            </div>
          </template>
        </CharacterCard>
      </div>
    </div>

    <BModal v-model="showModal" :title="editingCharacter ? 'Editar personaje' : 'Nuevo personaje'" hide-footer>
      <CharacterFormComponent
        :key="editingCharacter?.id ?? 'new'"
        :character="editingCharacter"
        @submit="handleSubmit"
      >
        <template #cancel>
          <BButton variant="secondary" @click="showModal = false">Cancelar</BButton>
        </template>
      </CharacterFormComponent>
    </BModal>

    <BModal v-model="showDeleteModal" title="Eliminar personaje" hide-footer>
      <p v-if="characterToDelete">
        ¿Eliminar <strong>{{ characterToDelete.name }}</strong>?
      </p>
      <div class="d-flex gap-2 justify-content-end mt-3">
        <BButton variant="secondary" @click="cancelDelete">Cancelar</BButton>
        <BButton variant="danger" @click="confirmDelete">Eliminar</BButton>
      </div>
    </BModal>
  </div>
</template>
