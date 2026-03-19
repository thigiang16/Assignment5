<template>
  <v-container class="overview-page">
    <div class="mb-6 heading-wrap">
      <h1 class="text-h4">Dashboard</h1>
      <p class="text-body-2 text-medium-emphasis">
        Quick overview of your system
      </p>
    </div>

    <v-row class="mb-6">
      <v-col cols="12" md="6">
        <v-card class="pa-4 d-flex align-center stat-card" rounded="xl" elevation="3">

          <v-avatar color="blue-lighten-4" class="mr-4">
            <v-icon color="blue">mdi-file-document</v-icon>
          </v-avatar>

          <div>
            <div class="text-caption">Total Templates</div>
            <div class="text-h5 font-weight-bold">
              {{ templates.length }}
            </div>
          </div>

        </v-card>
      </v-col>

      <v-col cols="12" md="6">
        <v-card class="pa-4 d-flex align-center stat-card" rounded="xl" elevation="3">

          <v-avatar color="teal-lighten-4" class="mr-4">
            <v-icon color="teal">mdi-shape</v-icon>
          </v-avatar>

          <div>
            <div class="text-caption">Categories</div>
            <div class="text-h5 font-weight-bold">
              {{ categories.length }}
            </div>
          </div>

        </v-card>
      </v-col>

    </v-row>

    <v-card class="mb-6 pa-4 section-card" rounded="xl" elevation="3">

      <div class="text-subtitle-1 mb-3">
        Categories
      </div>

      <v-chip
        v-for="category in categories"
        :key="category"
        class="mr-2 mb-2"
        color="teal"
        variant="outlined"
        @click="filterCategory(category)"
      >
        {{ category }}
      </v-chip>

    </v-card>

    <v-card class="pa-4 section-card" rounded="xl" elevation="3">

      <div class="text-subtitle-1 mb-3">
        Recent Templates
      </div>

      <v-list density="compact" class="list-surface">

        <v-list-item
          v-for="template in recentTemplates"
          :key="template.id"
        >
          <template #prepend>
            <v-icon color="teal">mdi-file</v-icon>
          </template>

          <v-list-item-title>
            {{ template.title }}
          </v-list-item-title>

        </v-list-item>

      </v-list>

    </v-card>

  </v-container>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from "vue"
import api from "@/services/api"

interface Template {
  id: number
  title: string
  category: string
}

const templates = ref<Template[]>([])

async function loadTemplates() {
  const res = await api.get("/api/eventtemplates")
  templates.value = res.data
}

onMounted(loadTemplates)

const categories = computed(() => {
  const set = new Set(templates.value.map(t => t.category))
  return Array.from(set)
})

const recentTemplates = computed(() => {
  return templates.value.slice(0, 5)
})

function filterCategory(category: string) {
  alert(`Filter templates by: ${category}`)
}
</script>

<style scoped>
.overview-page {
  max-width: 1000px;
}

.heading-wrap {
  border-left: 5px solid #14b8a6;
  padding-left: 12px;
}

.stat-card {
  border: 1px solid rgba(20, 184, 166, 0.14);
  background: linear-gradient(180deg, #f8fafc 0%, #ffffff 100%);
}

.section-card {
  border: 1px solid rgba(20, 184, 166, 0.14);
  background: linear-gradient(180deg, #f0fdfa 0%, #ffffff 100%);
}

.list-surface {
  background: rgba(255, 255, 255, 0.7);
  border-radius: 12px;
}

@media (max-width: 640px) {
  .overview-page {
    padding-top: 8px;
  }
}
</style>