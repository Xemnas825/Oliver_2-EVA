<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { useGamesStore } from '@/stores/games'
import GameCard from '@/components/GameCard.vue'
import { BButton, BAlert } from 'bootstrap-vue-next'

const route = useRoute()
const gamesStore = useGamesStore()

const id = computed(() => Number(route.params.id))

onMounted(() => {
  gamesStore.fetchGame(id.value)
})
</script>

<template>
  <div class="container py-4">
    <BButton variant="outline-secondary" class="mb-3" :to="{ name: 'games' }">
      ← Volver al listado
    </BButton>

    <BAlert v-if="gamesStore.error" variant="danger">
      {{ gamesStore.error }}
    </BAlert>

    <div v-else-if="gamesStore.loading" class="text-center py-5">
      Cargando…
    </div>

    <div v-else-if="gamesStore.currentGame" class="row">
      <div class="col-lg-8">
        <h1 class="mb-2">{{ gamesStore.currentGame.name }}</h1>
        <p v-if="gamesStore.currentGame.year" class="text-muted mb-3">
          Año: {{ gamesStore.currentGame.year }}
        </p>
        <img
          v-if="gamesStore.currentGame.imageUrl"
          :src="gamesStore.currentGame.imageUrl"
          :alt="gamesStore.currentGame.name"
          class="img-fluid rounded mb-3"
          style="max-height: 400px; object-fit: cover"
        />
        <p v-if="gamesStore.currentGame.description" class="white-space-pre-wrap">
          {{ gamesStore.currentGame.description }}
        </p>
        <p v-else class="text-muted">Sin descripción.</p>
      </div>
    </div>
  </div>
</template>
