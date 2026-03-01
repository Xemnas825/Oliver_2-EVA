<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/yup'
import * as yup from 'yup'
import type { Game } from '@/types'
import type { GameForm } from '@/stores/games'
import { BFormInput, BButton } from 'bootstrap-vue-next'

const props = defineProps<{
  game?: Game | null
}>()

const emit = defineEmits<{
  submit: [form: GameForm]
}>()

const schema = yup.object({
  name: yup.string().required('El nombre es obligatorio').min(1, 'El nombre es obligatorio'),
  imageUrl: yup.string().url('Debe ser una URL válida').nullable(),
  year: yup.number().nullable().integer('Año entero').min(1900, 'Año válido').max(2100, 'Año válido'),
  description: yup.string().nullable(),
})

const { defineField, handleSubmit, errors } = useForm({
  validationSchema: toTypedSchema(schema),
  initialValues: {
    name: props.game?.name ?? '',
    imageUrl: props.game?.imageUrl ?? '',
    year: props.game?.year ?? null,
    description: props.game?.description ?? '',
  },
})

const [name, nameAttrs] = defineField('name')
const [imageUrl, imageUrlAttrs] = defineField('imageUrl')
const [year, yearAttrs] = defineField('year')
const [description, descriptionAttrs] = defineField('description')

const onSubmit = handleSubmit((values) => {
  emit('submit', {
    name: values.name,
    imageUrl: values.imageUrl && values.imageUrl.trim() ? values.imageUrl.trim() : null,
    year: values.year != null ? Number(values.year) : null,
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
        placeholder="Nombre del juego"
        :state="errors.name ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.name }}</div>
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
      <label class="form-label">Año</label>
      <BFormInput
        v-model="year"
        v-bind="yearAttrs"
        type="number"
        placeholder="2020"
        min="1900"
        max="2100"
        :state="errors.year ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.year }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">Descripción</label>
      <textarea
        v-model="description"
        v-bind="descriptionAttrs"
        class="form-control"
        :class="{ 'is-invalid': errors.description }"
        rows="3"
        placeholder="Descripción del juego"
      />
      <div class="form-text text-danger">{{ errors.description }}</div>
    </div>
    <div class="d-flex gap-2 justify-content-end">
      <slot name="cancel" />
      <BButton type="submit" variant="primary">Guardar</BButton>
    </div>
  </form>
</template>
