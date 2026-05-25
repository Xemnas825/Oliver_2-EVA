<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useFavoritesStore } from '@/stores/favorites'
import { useGamesStore } from '@/stores/games'
import { useCharactersStore } from '@/stores/characters'
import GameCard from '@/components/GameCard.vue'
import CharacterCard from '@/components/CharacterCard.vue'
import { BButton, BAlert } from 'bootstrap-vue-next'

const { t } = useI18n()
const favoritesStore = useFavoritesStore()
const gamesStore = useGamesStore()
const charactersStore = useCharactersStore()

const favoriteGames = computed(() => {
  const ids = favoritesStore.favorites
    .filter((f) => f.type === 'game')
    .map((f) => f.id)
  return gamesStore.games.filter((g) => ids.includes(g.id))
})

const favoriteCharacters = computed(() => {
  const ids = favoritesStore.favorites
    .filter((f) => f.type === 'character')
    .map((f) => f.id)
  return charactersStore.characters.filter((c) => ids.includes(c.id))
})

const isEmpty = computed(
  () => favoriteGames.value.length === 0 && favoriteCharacters.value.length === 0
)

const loading = computed(
  () => gamesStore.loading || charactersStore.loading
)

onMounted(() => {
  favoritesStore.loadFavorites()
  gamesStore.fetchGames()
  charactersStore.fetchCharacters()
})
</script>

<template>
  <div class="container py-4">
    <h1 class="mb-4">{{ t('favorites.title') }}</h1>

    <BAlert v-if="gamesStore.error || charactersStore.error" variant="warning" dismissible class="mb-3">
      {{ gamesStore.error || charactersStore.error }}
    </BAlert>

    <div v-if="loading && isEmpty" class="text-center py-5 text-muted">
      {{ t('common.loading') }}
    </div>

    <p v-else-if="isEmpty" class="text-muted text-center py-5">
      {{ t('favorites.empty') }}
    </p>

    <template v-else>
      <section v-if="favoriteGames.length > 0" class="mb-5">
        <h2 class="h4 mb-3">{{ t('favorites.gamesSection') }}</h2>
        <div class="row g-4">
          <div
            v-for="game in favoriteGames"
            :key="game.id"
            class="col-sm-6 col-lg-4"
          >
            <GameCard :game="game" :show-favorite="false">
              <template #actions>
                <div class="d-flex flex-wrap gap-2">
                  <BButton
                    variant="outline-primary"
                    size="sm"
                    :to="{ name: 'game-detail', params: { id: game.id } }"
                  >
                    {{ t('common.viewMore') }}
                  </BButton>
                  <BButton
                    variant="outline-warning"
                    size="sm"
                    @click="favoritesStore.removeFavorite('game', game.id)"
                  >
                    {{ t('favorites.remove') }}
                  </BButton>
                </div>
              </template>
            </GameCard>
          </div>
        </div>
      </section>

      <section v-if="favoriteCharacters.length > 0">
        <h2 class="h4 mb-3">{{ t('favorites.charactersSection') }}</h2>
        <div class="row g-4">
          <div
            v-for="character in favoriteCharacters"
            :key="character.id"
            class="col-sm-6 col-lg-4"
          >
            <CharacterCard :character="character" :show-favorite="false">
              <template #actions>
                <div class="d-flex flex-wrap gap-2">
                  <BButton
                    variant="outline-primary"
                    size="sm"
                    :to="{ name: 'character-detail', params: { id: character.id } }"
                  >
                    {{ t('common.viewMore') }}
                  </BButton>
                  <BButton
                    variant="outline-warning"
                    size="sm"
                    @click="favoritesStore.removeFavorite('character', character.id)"
                  >
                    {{ t('favorites.remove') }}
                  </BButton>
                </div>
              </template>
            </CharacterCard>
          </div>
        </div>
      </section>
    </template>
  </div>
</template>
