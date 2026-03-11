<template>
  <v-app-bar color="white" elevation="1">
    <v-app-bar-title>PlanWise AI</v-app-bar-title>

    <v-spacer></v-spacer>

    <template v-if="auth.isAuthenticated.value">
      <v-btn variant="text" to="/">Home</v-btn>
      <v-btn variant="text" to="/dashboard">Dashboard</v-btn>
      <v-btn variant="text">Profile</v-btn>
      <v-btn v-if="auth.isAdmin.value" variant="text" to="/admin">Admin</v-btn>
      <v-btn variant="text" @click="handleLogout">Logout</v-btn>
    </template>

    <template v-else>
      <v-btn variant="text" to="/login">Login</v-btn>
      <v-btn color="primary" to="/register">Sign Up</v-btn>
    </template>
  </v-app-bar>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router"
import { useAuth } from "@/composables/useAuth"

const router = useRouter()
const auth = useAuth()

function handleLogout() {
  auth.logout()
  router.push("/login")
}
</script>