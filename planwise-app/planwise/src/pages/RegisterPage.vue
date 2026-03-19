<template>
  <v-container class="fill-height d-flex align-center justify-center">

    <v-card class="auth-card pa-6" width="420">

      <h2 class="text-center mb-2">Create Your Account</h2>

      <p class="text-center subtitle">
        Join us and start planning amazing events.
      </p>

      <v-text-field
        v-model="name"
        label="Name"
        placeholder="Your full name"
        variant="outlined"
        class="mt-4"
      />

      <v-text-field
        v-model="email"
        label="Email"
        placeholder="your@email.com"
        variant="outlined"
      />

      <v-text-field
        v-model="password"
        label="Password"
        placeholder="Create a password"
        type="password"
        variant="outlined"
      />

      <v-text-field
        v-model="confirmPassword"
        label="Confirm Password"
        placeholder="Confirm your password"
        type="password"
        variant="outlined"
      />

      <v-btn
        block
        color="teal"
        size="large"
        class="mt-4"
        @click="submit"
      >
        Create Account
      </v-btn>

      <p class="text-center mt-4">
        Already have an account?
        <router-link to="/login">Login</router-link>
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

const name = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')

const submit = async () => {
  if (password.value !== confirmPassword.value) {
    alert('Passwords do not match')
    return
  }

  try {
    await auth.register(name.value, email.value, password.value)
    router.push('/login')
  } catch {
    alert('Register failed')
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