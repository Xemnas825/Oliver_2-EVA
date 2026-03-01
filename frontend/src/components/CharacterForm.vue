<script setup lang="ts">
import { onMounted, computed } from 'vue'
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/yup'
import * as yup from 'yup'
import type { Character } from '@/types'
import type { CharacterForm } from '@/stores/characters'
import { useGamesStore } from '@/stores/games'
import { useI18n } from 'vue-i18n'
import { BFormInput, BButton, BFormSelect } from 'bootstrap-vue-next'

const props = defineProps<{
  character?: Character | null
}>()

const emit = defineEmits<{
  submit: [form: CharacterForm]
}>()

const gamesStore = useGamesStore()
const { t } = useI18n()

const schema = yup.object({
  name: yup.string().required(() => t('validation.nameRequired')).min(1, () => t('validation.nameRequired')),
  gameId: yup.number().required(() => t('validation.selectGame')).integer().min(1, () => t('validation.selectGame')),
  imageUrl: yup.string().url(() => t('validation.urlInvalid')).nullable(),
  description: yup.string().nullable(),
})

const { defineField, handleSubmit, errors } = useForm({
  validationSchema: toTypedSchema(schema),
  initialValues: {
    name: props.character?.name ?? '',
    gameId: props.character?.gameId ?? 0,
    imageUrl: props.character?.imageUrl ?? '',
    description: props.character?.description ?? '',
  },
})

const [name, nameAttrs] = defineField('name')
const [gameId, gameIdAttrs] = defineField('gameId')
const [imageUrl, imageUrlAttrs] = defineField('imageUrl')
const [description, descriptionAttrs] = defineField('description')

const gameOptions = computed(() => [
  { value: 0, text: t('characterForm.selectGamePlaceholder') },
  ...gamesStore.games.map((g) => ({ value: g.id, text: g.name })),
])

onMounted(() => {
  gamesStore.fetchGames()
})

const onSubmit = handleSubmit((values) => {
  emit('submit', {
    name: values.name,
    gameId: Number(values.gameId),
    imageUrl: values.imageUrl && values.imageUrl.trim() ? values.imageUrl.trim() : null,
    description: values.description && values.description.trim() ? values.description.trim() : null,
  })
})
</script>

<template>
  <form @submit.prevent="onSubmit">
    <div class="mb-3">
      <label class="form-label">{{ t('characterForm.name') }}</label>
      <BFormInput
        v-model="name"
        v-bind="nameAttrs"
        type="text"
        :placeholder="t('characterForm.namePlaceholder')"
        :state="errors.name ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.name }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">{{ t('characterForm.game') }}</label>
      <BFormSelect
        v-model="gameId"
        v-bind="gameIdAttrs"
        :options="gameOptions"
        :state="errors.gameId ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.gameId }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">{{ t('characterForm.imageUrl') }}</label>
      <BFormInput
        v-model="imageUrl"
        v-bind="imageUrlAttrs"
        type="url"
        placeholder="https://…"
        :state="errors.imageUrl ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.imageUrl }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">{{ t('characterForm.description') }}</label>
      <textarea
        v-model="description"
        v-bind="descriptionAttrs"
        class="form-control"
        :class="{ 'is-invalid': errors.description }"
        rows="3"
        :placeholder="t('characterForm.descriptionPlaceholder')"
      />
      <div class="form-text text-danger">{{ errors.description }}</div>
    </div>
    <div class="d-flex gap-2 justify-content-end">
      <slot name="cancel" />
      <BButton type="submit" variant="primary">{{ t('common.save') }}</BButton>
    </div>
  </form>
</template>
