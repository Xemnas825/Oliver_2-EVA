<script setup lang="ts">
import { onMounted } from 'vue'
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/yup'
import * as yup from 'yup'
import type { Character } from '@/types'
import type { CharacterForm } from '@/stores/characters'
import { useGamesStore } from '@/stores/games'
import { BFormInput, BButton, BFormSelect } from 'bootstrap-vue-next'

const props = defineProps<{
  character?: Character | null
}>()

const emit = defineEmits<{
  submit: [form: CharacterForm]
}>()

const gamesStore = useGamesStore()

const schema = yup.object({
  name: yup.string().required('El nombre es obligatorio').min(1, 'El nombre es obligatorio'),
  gameId: yup.number().required('Selecciona un juego').integer().min(1, 'Selecciona un juego'),
  imageUrl: yup.string().url('Debe ser una URL válida').nullable(),
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
      <label class="form-label">Nombre</label>
      <BFormInput
        v-model="name"
        v-bind="nameAttrs"
        type="text"
        placeholder="Nombre del personaje"
        :state="errors.name ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.name }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">Juego</label>
      <BFormSelect
        v-model="gameId"
        v-bind="gameIdAttrs"
        :options="[
          { value: 0, text: '-- Selecciona un juego --' },
          ...gamesStore.games.map((g) => ({ value: g.id, text: g.name })),
        ]"
        :state="errors.gameId ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.gameId }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">URL de imagen</label>
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
      <label class="form-label">Descripción</label>
      <textarea
        v-model="description"
        v-bind="descriptionAttrs"
        class="form-control"
        :class="{ 'is-invalid': errors.description }"
        rows="3"
        placeholder="Descripción del personaje"
      />
      <div class="form-text text-danger">{{ errors.description }}</div>
    </div>
    <div class="d-flex gap-2 justify-content-end">
      <slot name="cancel" />
      <BButton type="submit" variant="primary">Guardar</BButton>
    </div>
  </form>
</template>
