<template>
  <v-main>
  <v-container class="profile-page py-8">

        <div class="heading-wrap mb-6">
          <h1 class="text-h4">My Profile</h1>
          <p class="subtitle">Your account info and activity in one place.</p>
        </div>

        <v-card class="pa-6 mb-6 section-card account-card" rounded="xl" elevation="3">
          <h2 class="text-h6 mb-4">Account Information</h2>

          <div class="info-row">
            <span class="label">Email</span>
            <strong>{{ auth.userEmail.value || "-" }}</strong>
          </div>
        </v-card>

        <v-card class="pa-6 mb-6 section-card saved-card" rounded="xl" elevation="3">
          <h2 class="text-h6 mb-4">Saved Plans</h2>

          <div v-if="savedPlans.length" class="saved-plan-grid">
            <router-link
              v-for="plan in savedPlans"
              :key="plan.id"
              class="saved-plan-link"
              :to="{ name: 'plan-details', params: { id: plan.id } }"
            >
              <v-card class="saved-plan-item" rounded="lg" elevation="1">
                <div class="saved-plan-row">
                  <v-icon color="teal" class="saved-plan-icon">mdi-heart</v-icon>
                  <span class="saved-plan-title">{{ plan.title }}</span>
                  <v-icon color="#94a3b8" size="18">mdi-chevron-right</v-icon>
                </div>
              </v-card>
            </router-link>
          </div>

          <p v-else class="text-medium-emphasis">
            You have no saved plans yet.
          </p>
        </v-card>

        <v-card class="pa-6 section-card search-card" rounded="xl" elevation="3">
          <h2 class="text-h6 mb-4">Recent Searches</h2>

          <v-list v-if="recentSearches.length" class="list-surface">
            <v-list-item
              v-for="search in recentSearches"
              :key="search"
              prepend-icon="mdi-magnify"
            >
              {{ search }}
            </v-list-item>
          </v-list>

          <p v-else class="text-medium-emphasis">
            No recent searches.
          </p>
        </v-card>

    </v-container>
  </v-main>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue"
import { useAuth } from "@/composables/useAuth"
import {
  getRecentSearches,
  getSavedPlans,
  type SavedPlanListItem
} from "../services/profileService"

const auth = useAuth()

const savedPlans = ref<SavedPlanListItem[]>([])
const recentSearches = ref<string[]>([])

onMounted(async () => {
  savedPlans.value = await getSavedPlans()
  recentSearches.value = await getRecentSearches()
})
</script>

<style scoped>
.profile-page {
  max-width: 900px;
  margin: auto;
}

.heading-wrap {
  border-left: 5px solid #14b8a6;
  padding-left: 12px;
}

.subtitle {
  color: #5f6b7a;
  margin-top: 6px;
}

.section-card {
  border: 1px solid rgba(20, 184, 166, 0.14);
}

.account-card {
  background: linear-gradient(180deg, #f0fdfa 0%, #ffffff 100%);
}

.saved-card {
  background: linear-gradient(180deg, #ecfeff 0%, #ffffff 100%);
}

.search-card {
  background: linear-gradient(180deg, #f8fafc 0%, #ffffff 100%);
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #e2e8f0;
}

.info-row:last-child {
  border-bottom: 0;
}

.label {
  color: #64748b;
}

.list-surface {
  background: rgba(255, 255, 255, 0.65);
  border-radius: 12px;
}

.saved-plan-grid {
  display: grid;
  grid-template-columns: 1fr;
  gap: 10px;
}

.saved-plan-link {
  text-decoration: none;
}

.saved-plan-item {
  border: 1px solid #dbeafe;
  background: #ffffff;
  transition: transform 0.18s ease, box-shadow 0.18s ease, border-color 0.18s ease;
}

.saved-plan-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(15, 23, 42, 0.08);
  border-color: #5eead4;
}

.saved-plan-row {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 14px;
}

.saved-plan-icon {
  flex-shrink: 0;
}

.saved-plan-title {
  flex: 1;
  color: #0f172a;
  font-weight: 600;
}

@media (max-width: 640px) {
  .profile-page {
    padding-top: 12px;
  }

  .info-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 6px;
  }
}
</style>