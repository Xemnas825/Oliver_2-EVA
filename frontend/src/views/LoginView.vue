<script setup lang="ts">
import { onMounted } from 'vue'
import { useForm } from 'vee-validate'
import { toTypedSchema } from '@vee-validate/yup'
import * as yup from 'yup'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { BFormInput, BButton, BAlert } from 'bootstrap-vue-next'

const router = useRouter()
const auth = useAuthStore()

onMounted(() => {
  auth.clearError()
})

const schema = yup.object({
  email: yup.string().required('El email es obligatorio').email('Email no válido'),
  password: yup.string().required('La contraseña es obligatoria'),
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
        <h1 class="mb-4">Iniciar sesión</h1>

        <BAlert v-if="auth.error" variant="danger" dismissible class="mb-3">
          {{ auth.error }}
        </BAlert>

        <form @submit.prevent="onSubmit">
          <div class="mb-3">
            <label class="form-label" for="login-email">Email</label>
            <BFormInput
              id="login-email"
              v-model="email"
              v-bind="emailAttrs"
              type="email"
              placeholder="correo@ejemplo.com"
              :state="errors.email ? false : undefined"
              aria-describedby="login-email-error"
            />
            <div id="login-email-error" class="form-text text-danger">
              {{ errors.email }}
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label" for="login-password">Contraseña</label>
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
            Entrar
          </BButton>
          <p class="text-center text-muted small mb-0">
            ¿No tienes cuenta?
            <router-link to="/registro">Regístrate</router-link>
          </p>
        </form>
      </div>
    </div>
  </div>
</template>
