<script setup lang="ts">
import { BCard, BButton } from 'bootstrap-vue-next'
</script>

<template>
  <div class="container py-5">
    <h1 class="mb-4">Panel de administración</h1>
    <div class="row g-3">
      <div class="col-md-6">
        <BCard title="Juegos" class="h-100">
          <p class="card-text">Gestionar listado y CRUD de juegos.</p>
          <BButton variant="primary" :to="{ name: 'admin-games' }">Ir a Juegos</BButton>
        </BCard>
      </div>
      <div class="col-md-6">
        <BCard title="Personajes" class="h-100">
          <p class="card-text">Gestionar listado y CRUD de personajes.</p>
          <BButton variant="primary" :to="{ name: 'admin-characters' }">Ir a Personajes</BButton>
        </BCard>
      </div>
    </div>
  </div>
</template>
