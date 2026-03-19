<template>
  <v-app>
    <Navbar v-if="showNavbar" />
    <router-view />
  </v-app>
</template>

<script setup lang="ts">
import { computed } from "vue"
import { useRoute } from "vue-router"
import Navbar from "@/components/Navbar.vue"

const route = useRoute()
const showNavbar = computed(() => !route.path.startsWith("/admin"))
</script>