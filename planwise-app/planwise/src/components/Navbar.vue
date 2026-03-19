<template>
  <v-app-bar color="white" elevation="1" height="64">
    <v-app-bar-title>
      <router-link to="/" class="brand-link">
        <span class="brand-text">PlanWise</span><span class="ai-badge">AI</span>
      </router-link>
    </v-app-bar-title>

    <template v-if="!mobile">
      <template v-if="auth.isAuthenticated.value">
        <v-btn
          v-for="item in navItems"
          :key="item.to"
          variant="text"
          class="nav-btn"
          :prepend-icon="item.icon"
          :to="item.to"
        >
          {{ item.label }}
        </v-btn>

        <v-btn
          v-if="auth.isAdmin.value"
          variant="outlined"
          color="teal"
          size="small"
          class="nav-btn admin-btn"
          prepend-icon="mdi-shield-crown-outline"
          to="/admin"
        >
          Return to Admin Panel
        </v-btn>

        <v-btn
          variant="text"
          class="nav-btn"
          prepend-icon="mdi-logout"
          @click="handleLogout"
        >
          Logout
        </v-btn>
      </template>

      <template v-else>
        <v-btn variant="text" class="nav-btn" prepend-icon="mdi-login" to="/login">Login</v-btn>
        <v-btn color="teal" class="nav-btn" prepend-icon="mdi-account-plus-outline" to="/register">Sign Up</v-btn>
      </template>
    </template>

    <v-app-bar-nav-icon v-if="mobile" @click="drawer = !drawer" />
  </v-app-bar>

  <v-navigation-drawer v-model="drawer" temporary location="right" width="280">
    <div class="drawer-brand pa-5">
      <span class="brand-text">PlanWise</span><span class="ai-badge ml-1">AI</span>
    </div>

    <v-divider />

    <v-list nav density="comfortable" class="px-2 py-2">
      <template v-if="auth.isAuthenticated.value">
        <v-list-item
          v-for="item in navItems"
          :key="item.to"
          :prepend-icon="item.icon"
          :title="item.label"
          :to="item.to"
          rounded="lg"
          @click="drawer = false"
        />

        <v-list-item
          v-if="auth.isAdmin.value"
          prepend-icon="mdi-shield-crown-outline"
          title="Return to Admin Panel"
          to="/admin"
          rounded="lg"
          @click="drawer = false"
        />

        <v-divider class="my-2" />

        <v-list-item
          prepend-icon="mdi-logout"
          title="Logout"
          rounded="lg"
          @click="handleLogout"
        />
      </template>

      <template v-else>
        <v-list-item
          prepend-icon="mdi-login"
          title="Login"
          to="/login"
          rounded="lg"
          @click="drawer = false"
        />

        <v-list-item
          prepend-icon="mdi-account-plus-outline"
          title="Sign Up"
          to="/register"
          rounded="lg"
          @click="drawer = false"
        />
      </template>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import { ref } from "vue"
import { useDisplay } from "vuetify"
import { useAuth } from "@/composables/useAuth"

const auth = useAuth()
const { mobile } = useDisplay()
const drawer = ref(false)

const navItems = [
  { label: "Home",      to: "/",          icon: "mdi-home-outline" },
  { label: "Dashboard", to: "/dashboard", icon: "mdi-view-dashboard-outline" },
  { label: "Profile",   to: "/profile",   icon: "mdi-account-outline" }
]

async function handleLogout() {
  drawer.value = false
  await auth.logout()
}
</script>

<style scoped>
.brand-link {
  text-decoration: none;
  color: inherit;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.brand-text {
  font-size: 20px;
  font-weight: 700;
  color: #0f172a;
}

.ai-badge {
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  padding: 2px 9px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 700;
  line-height: 1.6;
  letter-spacing: 0;
}

.nav-btn {
  text-transform: none;
  font-size: 14px;
  font-weight: 600;
  letter-spacing: 0;
}

.admin-btn {
  margin-inline: 4px;
  font-size: 13px;
  text-transform: none;
}

.drawer-brand {
  display: flex;
  align-items: center;
}
</style>