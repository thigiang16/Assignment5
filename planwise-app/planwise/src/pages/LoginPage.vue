<template>
  <v-container class="fill-height d-flex align-center justify-center">

    <v-card class="auth-card pa-6" width="420">

      <h2 class="text-center mb-2">Welcome Back</h2>
      <p class="text-center subtitle">
        Sign in to generate personalized event ideas.
      </p>

      <v-text-field
        v-model="email"
        label="Email"
        placeholder="your@email.com"
        variant="outlined"
        class="mt-4"
      />

      <v-text-field
        v-model="password"
        label="Password"
        placeholder="Enter your password"
        variant="outlined"
        type="password"
      />

      <v-btn
        block
        color="teal"
        size="large"
        class="mt-4"
        @click="submit"
      >
        Login
      </v-btn>

      <p class="text-center mt-4">
        Don't have an account?
        <router-link to="/register">Register</router-link>
      </p>

      <p class="text-center mt-4">
        ← <router-link to="/">Back to home</router-link>
      </p>

    </v-card>

  </v-container>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { useRouter } from 'vue-router'

const router = useRouter()
const auth = useAuth()

const email = ref('')
const password = ref('')

const submit = async () => {
  try {
    await auth.login(email.value, password.value)
    router.push(auth.isAdmin.value ? '/admin' : '/dashboard')
  } catch {
    alert('Invalid login')
  }
}
</script>

<style scoped>

.auth-card{
  border-radius:16px;
  box-shadow:0 10px 30px rgba(0,0,0,0.1);
  width:100%;
  max-width:420px;
}

.subtitle{
  color:#666;
}

@media (max-width:640px){
.auth-card{
padding:20px !important;
}
}

</style>