<script setup lang="ts">
import { BApp } from 'bootstrap-vue-next'
import { RouterView } from 'vue-router'
</script>

<template>
  <BApp>
    <RouterView />
  </BApp>
</template>

<style>
#app {
  min-height: 100vh;
}
</style>
