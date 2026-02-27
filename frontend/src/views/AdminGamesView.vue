<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useGamesStore } from '@/stores/games'
import type { Game } from '@/types'
import type { GameForm } from '@/stores/games'
import GameCard from '@/components/GameCard.vue'
import GameFormComponent from '@/components/GameForm.vue'
import { BButton, BModal, BAlert } from 'bootstrap-vue-next'

const gamesStore = useGamesStore()
const showModal = ref(false)
const showDeleteModal = ref(false)
const editingGame = ref<Game | null>(null)
const gameToDelete = ref<Game | null>(null)

onMounted(() => {
  gamesStore.fetchGames()
})

watch(showModal, (visible) => {
  if (!visible) editingGame.value = null
})

function openCreate() {
  editingGame.value = null
  showModal.value = true
}

function openEdit(game: Game) {
  editingGame.value = game
  showModal.value = true
}

async function handleSubmit(form: GameForm) {
  if (editingGame.value) {
    const updated = await gamesStore.updateGame(editingGame.value.id, form)
    if (updated) showModal.value = false
  } else {
    const created = await gamesStore.createGame(form)
    if (created) showModal.value = false
  }
}

function askDelete(game: Game) {
  gameToDelete.value = game
  showDeleteModal.value = true
}

async function confirmDelete() {
  if (!gameToDelete.value) return
  const ok = await gamesStore.deleteGame(gameToDelete.value.id)
  if (ok) {
    showDeleteModal.value = false
    gameToDelete.value = null
  }
}

function cancelDelete() {
  showDeleteModal.value = false
  gameToDelete.value = null
}
</script>

<template>
  <div class="container py-4">
    <h1 class="mb-4">Administrar Juegos</h1>

    <BAlert v-if="gamesStore.error" variant="danger" dismissible class="mb-3">
      {{ gamesStore.error }}
    </BAlert>

    <div class="mb-3">
      <BButton variant="primary" @click="openCreate">Añadir juego</BButton>
    </div>

    <div v-if="gamesStore.loading && gamesStore.games.length === 0" class="text-center py-5">
      Cargando…
    </div>

    <div v-else-if="gamesStore.games.length === 0" class="text-muted py-5">
      No hay juegos. Añade el primero.
    </div>

    <div v-else class="row g-4">
      <div
        v-for="game in gamesStore.games"
        :key="game.id"
        class="col-sm-6 col-lg-4"
      >
        <GameCard :game="game">
          <template #actions>
            <div class="d-flex gap-1">
              <BButton variant="outline-secondary" size="sm" @click="openEdit(game)">
                Editar
              </BButton>
              <BButton variant="outline-danger" size="sm" @click="askDelete(game)">
                Borrar
              </BButton>
            </div>
          </template>
        </GameCard>
      </div>
    </div>

    <BModal v-model="showModal" :title="editingGame ? 'Editar juego' : 'Nuevo juego'" hide-footer>
      <GameFormComponent
        :key="editingGame?.id ?? 'new'"
        :game="editingGame"
        @submit="handleSubmit"
      >
        <template #cancel>
          <BButton variant="secondary" @click="showModal = false">Cancelar</BButton>
        </template>
      </GameFormComponent>
    </BModal>

    <BModal v-model="showDeleteModal" title="Eliminar juego" hide-footer>
      <p v-if="gameToDelete">
        ¿Eliminar <strong>{{ gameToDelete.name }}</strong>?
      </p>
      <div class="d-flex gap-2 justify-content-end mt-3">
        <BButton variant="secondary" @click="cancelDelete">Cancelar</BButton>
        <BButton variant="danger" @click="confirmDelete">Eliminar</BButton>
      </div>
    </BModal>
  </div>
</template>
