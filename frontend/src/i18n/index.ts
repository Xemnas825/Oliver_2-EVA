import { createI18n } from 'vue-i18n'
import es from '@/locales/es.json'
import en from '@/locales/en.json'

const savedLocale = typeof localStorage !== 'undefined' ? localStorage.getItem('locale') : null
const defaultLocale = savedLocale === 'en' || savedLocale === 'es' ? savedLocale : 'es'

export const i18n = createI18n({
  legacy: false,
  locale: defaultLocale,
  fallbackLocale: 'es',
  messages: { es, en },
})

export function setLocale(locale: 'es' | 'en') {
  i18n.global.locale.value = locale
  try {
    localStorage.setItem('locale', locale)
  } catch (_) {}
}
