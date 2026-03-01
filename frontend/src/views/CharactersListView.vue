<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useCharactersStore } from '@/stores/characters'
import { useGamesStore } from '@/stores/games'
import CharacterCard from '@/components/CharacterCard.vue'
import { BAlert, BButton, BFormSelect } from 'bootstrap-vue-next'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const charactersStore = useCharactersStore()
const gamesStore = useGamesStore()

const gameIdFilter = computed(() => {
  const q = route.query.gameId
  if (q === undefined || q === '') return undefined
  const n = Number(q)
  return Number.isNaN(n) ? undefined : n
})

const filterOptions = computed(() => [
  { value: '', text: t('characters.all') },
  ...gamesStore.games.map((g) => ({ value: g.id, text: g.name })),
])

onMounted(() => {
  gamesStore.fetchGames()
  charactersStore.fetchCharacters(gameIdFilter.value)
})

function onFilterChange(value: string | number | (string | number)[] | null) {
  if (value == null || Array.isArray(value)) return
  const id = value === '' || value === 0 ? undefined : Number(value)
  router.push({ query: id != null ? { gameId: id } : {} })
  charactersStore.fetchCharacters(id)
}
</script>

<template>
  <div class="container py-4">
    <h1 class="mb-4">{{ t('characters.title') }}</h1>

    <BAlert v-if="charactersStore.error" variant="danger" dismissible class="mb-3">
      {{ charactersStore.error }}
    </BAlert>

    <div class="mb-3 d-flex flex-wrap align-items-center gap-2">
      <label class="text-nowrap mb-0">{{ t('characters.filterByGame') }}</label>
      <BFormSelect
        :model-value="gameIdFilter ?? ''"
        :options="filterOptions"
        class="w-auto"
        @update:model-value="onFilterChange"
      />
    </div>

    <div v-if="charactersStore.loading" class="text-center py-5">
      {{ t('common.loading') }}
    </div>

    <div v-else-if="charactersStore.characters.length === 0" class="text-muted text-center py-5">
      {{ t('characters.noCharacters') }}
    </div>

    <div v-else class="row g-4">
      <div
        v-for="character in charactersStore.characters"
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
  </div>
</template>
