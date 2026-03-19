<template>
  <v-layout class="admin-layout">
    <v-app-bar v-if="mobile" flat elevation="1">
      <v-app-bar-nav-icon @click="drawer = !drawer" />
      <v-app-bar-title class="logo">PlanWise <span class="ai-badge">AI</span></v-app-bar-title>
    </v-app-bar>

    <v-navigation-drawer
      v-model="drawer"
      :permanent="!mobile"
      :temporary="mobile"
      width="240"
      class="sidebar"
    >
      <div class="pa-4 brand-block">
        <div class="text-h6 font-weight-bold logo">PlanWise <span class="ai-badge">AI</span></div>
        <div class="text-body-2 text-medium-emphasis">Admin Panel</div>
      </div>

      <v-divider />

      <v-list nav density="comfortable" class="px-2 py-2">
        <v-list-item
          title="Dashboard"
          prepend-icon="mdi-view-dashboard-outline"
          to="/admin/dashboard"
          rounded="lg"
          @click="mobile ? drawer = false : null"
        />

        <v-list-item
          title="Templates"
          prepend-icon="mdi-file-document-edit-outline"
          to="/admin/templates"
          rounded="lg"
          @click="mobile ? drawer = false : null"
        />

        <v-divider class="my-2" />

        <v-list-item
          title="View as User"
          prepend-icon="mdi-account-eye-outline"
          rounded="lg"
          @click="viewAsUser"
        />
      </v-list>

      <v-spacer />

      <v-divider />

      <v-list density="comfortable" class="px-2 pb-4">
        <v-list-item
          title="Logout"
          prepend-icon="mdi-logout"
          rounded="lg"
          @click="logout"
        />
      </v-list>
    </v-navigation-drawer>

    <v-main>
      <v-container class="pa-6">
        <router-view />
      </v-container>
    </v-main>
  </v-layout>
</template>

<script setup lang="ts">
import { ref, watch } from "vue"
import { useRouter } from "vue-router"
import { useDisplay } from "vuetify"
import { useAuth } from "@/composables/useAuth"

const router = useRouter()
const auth = useAuth()
const { mobile } = useDisplay()
const drawer = ref(!mobile.value)

watch(mobile, (isMobile) => {
  drawer.value = !isMobile
})

async function logout() {
  await auth.logout()
}

function viewAsUser() {
  if (mobile.value) drawer.value = false
  router.push("/dashboard")
}
</script>

<style scoped>
.admin-layout {
  min-height: 100vh;
  display: flex;
  background: linear-gradient(180deg, #f8fafc 0%, #ffffff 100%);
}

.sidebar {
  border-right: 1px solid #e2e8f0;
}

.brand-block {
  border-bottom: 1px solid #e2e8f0;
}

.logo {
  font-weight: 700;
}

.ai-badge {
  background: teal;
  color: white;
  padding: 2px 8px;
  border-radius: 10px;
  font-size: 12px;
  margin-left: 4px;
}

@media (max-width: 640px) {
  .admin-layout :deep(.v-main) {
    padding-top: 56px;
  }
}
</style>
