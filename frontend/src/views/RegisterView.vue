<script setup lang="ts">
import { onMounted } from 'vue'
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/yup'
import * as yup from 'yup'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useI18n } from 'vue-i18n'
import { BFormInput, BButton, BAlert } from 'bootstrap-vue-next'

const router = useRouter()
const auth = useAuthStore()
const { t } = useI18n()

onMounted(() => {
  auth.clearError()
})

const schema = yup.object({
  name: yup.string().required(() => t('validation.nameRequired')).min(2, () => t('validation.nameMin')),
  email: yup.string().required(() => t('validation.emailRequired')).email(() => t('validation.emailInvalid')),
  password: yup.string().required(() => t('validation.passwordRequired')).min(6, () => t('validation.passwordMin')),
  passwordConfirm: yup
    .string()
    .required(() => t('validation.confirmPasswordRequired'))
    .oneOf([yup.ref('password')], () => t('validation.passwordsMatch')),
})

const { defineField, handleSubmit, errors } = useForm({
  validationSchema: toTypedSchema(schema),
  initialValues: { name: '', email: '', password: '', passwordConfirm: '' },
})

const [name, nameAttrs] = defineField('name')
const [email, emailAttrs] = defineField('email')
const [password, passwordAttrs] = defineField('password')
const [passwordConfirm, passwordConfirmAttrs] = defineField('passwordConfirm')

const onSubmit = handleSubmit(async (values) => {
  const ok = await auth.register(values.email, values.password, values.name)
  if (ok) {
    router.push(auth.isAdmin ? '/admin' : '/')
  }
})
</script>

<template>
  <div class="container py-4">
    <div class="row justify-content-center">
      <div class="col-md-5">
        <h1 class="mb-4">{{ t('auth.registerTitle') }}</h1>

        <BAlert v-if="auth.error" variant="danger" dismissible class="mb-3">
          {{ auth.error }}
        </BAlert>

        <form @submit.prevent="onSubmit">
          <div class="mb-3">
            <label class="form-label" for="reg-name">{{ t('auth.name') }}</label>
            <BFormInput
              id="reg-name"
              v-model="name"
              v-bind="nameAttrs"
              type="text"
              :placeholder="t('auth.namePlaceholder')"
              :state="errors.name ? false : undefined"
              aria-describedby="reg-name-error"
            />
            <div id="reg-name-error" class="form-text text-danger">
              {{ errors.name }}
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label" for="reg-email">{{ t('auth.email') }}</label>
            <BFormInput
              id="reg-email"
              v-model="email"
              v-bind="emailAttrs"
              type="email"
              :placeholder="t('auth.emailPlaceholder')"
              :state="errors.email ? false : undefined"
              aria-describedby="reg-email-error"
            />
            <div id="reg-email-error" class="form-text text-danger">
              {{ errors.email }}
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label" for="reg-password">{{ t('auth.password') }}</label>
            <BFormInput
              id="reg-password"
              v-model="password"
              v-bind="passwordAttrs"
              type="password"
              placeholder="••••••••"
              :state="errors.password ? false : undefined"
              aria-describedby="reg-password-error"
            />
            <div id="reg-password-error" class="form-text text-danger">
              {{ errors.password }}
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label" for="reg-password-confirm">{{ t('auth.repeatPassword') }}</label>
            <BFormInput
              id="reg-password-confirm"
              v-model="passwordConfirm"
              v-bind="passwordConfirmAttrs"
              type="password"
              placeholder="••••••••"
              :state="errors.passwordConfirm ? false : undefined"
              aria-describedby="reg-password-confirm-error"
            />
            <div id="reg-password-confirm-error" class="form-text text-danger">
              {{ errors.passwordConfirm }}
            </div>
          </div>

          <BButton type="submit" variant="primary" class="w-100 mb-2" @click.prevent="onSubmit">
            {{ t('auth.submitRegister') }}
          </BButton>
          <p class="text-center text-muted small mb-0">
            {{ t('auth.hasAccount') }}
            <router-link to="/login">{{ t('auth.signIn') }}</router-link>
          </p>
        </form>
      </div>
    </div>
  </div>
</template>
