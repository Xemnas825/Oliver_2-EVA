<script setup lang="ts">
import { onMounted } from 'vue'
import { useGamesStore } from '@/stores/games'
import GameCard from '@/components/GameCard.vue'
import { BAlert } from 'bootstrap-vue-next'

const gamesStore = useGamesStore()

onMounted(() => {
  gamesStore.fetchGames()
})
</script>

<template>
  <div class="container py-4">
    <h1 class="mb-4">Juegos</h1>

    <BAlert v-if="gamesStore.error" variant="danger" dismissible>
      {{ gamesStore.error }}
    </BAlert>

    <div v-if="gamesStore.loading" class="text-center py-5">
      Cargando…
    </div>

    <div v-else-if="gamesStore.games.length === 0" class="text-muted text-center py-5">
      No hay juegos todavía.
    </div>

    <div v-else class="row g-4">
      <div
        v-for="game in gamesStore.games"
        :key="game.id"
        class="col-sm-6 col-lg-4"
      >
        <GameCard :game="game">
          <template #actions>
            <BButton
              variant="outline-primary"
              size="sm"
              :to="{ name: 'game-detail', params: { id: game.id } }"
            >
              Ver más
            </BButton>
          </template>
        </GameCard>
      </div>
    </div>
  </div>
</template>
