<template>
  <v-container class="fill-height d-flex align-center justify-center">

    <v-card class="auth-card pa-6" width="420">

      <h2 class="text-center mb-2">Create Your Account</h2>

      <p class="text-center subtitle">
        Join us and start planning amazing events.
      </p>

      <v-text-field
        label="Name"
        placeholder="Your full name"
        variant="outlined"
        class="mt-4"
      />

      <v-text-field
        label="Email"
        placeholder="your@email.com"
        variant="outlined"
      />

      <v-text-field
        label="Password"
        placeholder="Create a password"
        type="password"
        variant="outlined"
      />

      <v-text-field
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
</script>

<style scoped>

.auth-card{
  border-radius:16px;
  box-shadow:0 10px 30px rgba(0,0,0,0.1);
}

.subtitle{
  color:#666;
}

</style>