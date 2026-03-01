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
  email: yup.string().required(() => t('validation.emailRequired')).email(() => t('validation.emailInvalid')),
  password: yup.string().required(() => t('validation.passwordRequired')),
})

const { defineField, handleSubmit, errors } = useForm({
  validationSchema: toTypedSchema(schema),
  initialValues: { email: '', password: '' },
})

const [email, emailAttrs] = defineField('email')
const [password, passwordAttrs] = defineField('password')

const onSubmit = handleSubmit(async (values) => {
  const ok = await auth.login(values.email, values.password)
  if (ok) {
    const redirect = (router.currentRoute.value.query.redirect as string) || (auth.isAdmin ? '/admin' : '/')
    router.push(redirect)
  }
})
</script>

<template>
  <div class="container py-4">
    <div class="row justify-content-center">
      <div class="col-md-5">
        <h1 class="mb-4">{{ t('auth.loginTitle') }}</h1>

        <BAlert v-if="auth.error" variant="danger" dismissible class="mb-3">
          {{ auth.error }}
        </BAlert>

        <form @submit.prevent="onSubmit">
          <div class="mb-3">
            <label class="form-label" for="login-email">{{ t('auth.email') }}</label>
            <BFormInput
              id="login-email"
              v-model="email"
              v-bind="emailAttrs"
              type="email"
              :placeholder="t('auth.emailPlaceholder')"
              :state="errors.email ? false : undefined"
              aria-describedby="login-email-error"
            />
            <div id="login-email-error" class="form-text text-danger">
              {{ errors.email }}
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label" for="login-password">{{ t('auth.password') }}</label>
            <BFormInput
              id="login-password"
              v-model="password"
              v-bind="passwordAttrs"
              type="password"
              placeholder="••••••••"
              :state="errors.password ? false : undefined"
              aria-describedby="login-password-error"
            />
            <div id="login-password-error" class="form-text text-danger">
              {{ errors.password }}
            </div>
          </div>

          <BButton type="submit" variant="primary" class="w-100 mb-2" @click.prevent="onSubmit">
            {{ t('auth.submitLogin') }}
          </BButton>
          <p class="text-center text-muted small mb-0">
            {{ t('auth.noAccount') }}
            <router-link to="/registro">{{ t('auth.signUp') }}</router-link>
          </p>
        </form>
      </div>
    </div>
  </div>
</template>
