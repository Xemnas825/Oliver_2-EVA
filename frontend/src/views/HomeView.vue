<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useGamesStore } from '@/stores/games'
import { useCharactersStore } from '@/stores/characters'
import GameCard from '@/components/GameCard.vue'
import CharacterCard from '@/components/CharacterCard.vue'
import { BButton, BAlert } from 'bootstrap-vue-next'

const { t } = useI18n()
const gamesStore = useGamesStore()
const charactersStore = useCharactersStore()

const previewGames = computed(() => gamesStore.games.slice(0, 3))
const previewCharacters = computed(() => charactersStore.characters.slice(0, 3))

onMounted(() => {
  gamesStore.fetchGames()
  charactersStore.fetchCharacters()
})
</script>

<template>
  <div class="home-view">
    <section class="hero py-5 mb-4">
      <div class="container text-center">
        <h1 class="display-5 fw-bold mb-3">{{ t('home.title') }}</h1>
        <p class="lead text-muted mx-auto" style="max-width: 36rem;">
          {{ t('home.subtitle') }}
        </p>
      </div>
    </section>

    <div class="container pb-5">
      <BAlert v-if="gamesStore.error || charactersStore.error" variant="warning" dismissible class="mb-3">
        {{ gamesStore.error || charactersStore.error }}
      </BAlert>

      <section class="mb-5">
        <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
          <h2 class="h4 mb-0">{{ t('nav.games') }}</h2>
          <BButton variant="outline-primary" size="sm" :to="{ name: 'games' }">
            {{ t('home.seeAllGames') }}
          </BButton>
        </div>
        <div v-if="gamesStore.loading && previewGames.length === 0" class="text-muted py-4 text-center">
          {{ t('home.loadingGames') }}
        </div>
        <div v-else-if="previewGames.length === 0" class="text-muted py-4 text-center">
          {{ t('home.noGames') }}
        </div>
        <div v-else class="row g-4">
          <div
            v-for="game in previewGames"
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
                  {{ t('common.viewMore') }}
                </BButton>
              </template>
            </GameCard>
          </div>
        </div>
      </section>

      <section>
        <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
          <h2 class="h4 mb-0">{{ t('nav.characters') }}</h2>
          <BButton variant="outline-primary" size="sm" :to="{ name: 'characters' }">
            {{ t('home.seeAllCharacters') }}
          </BButton>
        </div>
        <div v-if="charactersStore.loading && previewCharacters.length === 0" class="text-muted py-4 text-center">
          {{ t('home.loadingCharacters') }}
        </div>
        <div v-else-if="previewCharacters.length === 0" class="text-muted py-4 text-center">
          {{ t('home.noCharacters') }}
        </div>
        <div v-else class="row g-4">
          <div
            v-for="character in previewCharacters"
            :key="character.id"
            class="col-sm-6 col-lg-4"
          >
            <CharacterCard :character="character">
              <template #actions>
                <BButton
                  variant="outline-primary"
                  size="sm"
                  :to="{ name: 'character-detail', params: { id: character.id } }"
                >
                  {{ t('common.viewMore') }}
                </BButton>
              </template>
            </CharacterCard>
          </div>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
.hero {
  background: linear-gradient(to bottom, var(--bs-secondary-bg), transparent);
  border-bottom: 1px solid var(--bs-border-color);
}
</style>
