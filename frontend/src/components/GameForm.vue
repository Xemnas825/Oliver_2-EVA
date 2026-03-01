<script setup lang="ts">
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/yup'
import * as yup from 'yup'
import type { Game } from '@/types'
import type { GameForm } from '@/stores/games'
import { useI18n } from 'vue-i18n'
import { BFormInput, BButton } from 'bootstrap-vue-next'

const props = defineProps<{
  game?: Game | null
}>()

const emit = defineEmits<{
  submit: [form: GameForm]
}>()

const { t } = useI18n()

const schema = yup.object({
  name: yup.string().required(() => t('validation.nameRequired')).min(1, () => t('validation.nameRequired')),
  imageUrl: yup.string().url(() => t('validation.urlInvalid')).nullable(),
  year: yup.number().nullable().integer(() => t('validation.yearInteger')).min(1900, () => t('validation.yearValid')).max(2100, () => t('validation.yearValid')),
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
      <label class="form-label">{{ t('gameForm.name') }}</label>
      <BFormInput
        v-model="name"
        v-bind="nameAttrs"
        type="text"
        :placeholder="t('gameForm.namePlaceholder')"
        :state="errors.name ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.name }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">{{ t('gameForm.imageUrl') }}</label>
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
      <label class="form-label">{{ t('gameForm.year') }}</label>
      <BFormInput
        v-model="year"
        v-bind="yearAttrs"
        type="number"
        :placeholder="t('gameForm.yearPlaceholder')"
        min="1900"
        max="2100"
        :state="errors.year ? false : undefined"
      />
      <div class="form-text text-danger">{{ errors.year }}</div>
    </div>
    <div class="mb-3">
      <label class="form-label">{{ t('gameForm.description') }}</label>
      <textarea
        v-model="description"
        v-bind="descriptionAttrs"
        class="form-control"
        :class="{ 'is-invalid': errors.description }"
        rows="3"
        :placeholder="t('gameForm.descriptionPlaceholder')"
      />
      <div class="form-text text-danger">{{ errors.description }}</div>
    </div>
    <div class="d-flex gap-2 justify-content-end">
      <slot name="cancel" />
      <BButton type="submit" variant="primary">{{ t('common.save') }}</BButton>
    </div>
  </form>
</template>
