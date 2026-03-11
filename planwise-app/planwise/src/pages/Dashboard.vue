<template>
  <v-app>

    <!-- NAVBAR -->
    <v-app-bar flat elevation="1">

      <v-container class="d-flex align-center">

        <div class="logo">
          PlanWise <span class="ai-badge">AI</span>
        </div>

        <v-spacer></v-spacer>

        <v-btn variant="text" to="/">
          Home
        </v-btn>

        <v-btn variant="text" to="/dashboard">
          Dashboard
        </v-btn>

        <v-btn variant="text" prepend-icon="mdi-account-outline">
          Profile
        </v-btn>

        <v-btn v-if="auth.isAdmin.value" variant="text" to="/admin">
          Admin
        </v-btn>

        <v-btn variant="text" prepend-icon="mdi-logout" @click="handleLogout">
          Logout
        </v-btn>

      </v-container>

    </v-app-bar>

    <!-- MAIN -->
    <v-main>

      <v-container class="dashboard">

        <!-- PROMPT AREA -->

        <div class="prompt-area">

          <h1>What kind of event are you planning?</h1>

          <p class="subtitle">
            Describe your event and let our AI find the perfect plan for you.
          </p>

          <v-textarea
            v-model="prompt"
            placeholder="Small indoor party with 5 people..."
            rows="3"
            variant="outlined"
          />

          <v-btn
            color="teal"
            size="large"
            class="generate-btn"
            @click="generateIdeas"
          >
            ✨ Generate Ideas
          </v-btn>

        </div>


        <!-- RESULTS -->

        <div v-if="results.length" class="results">

          <div class="results-header">

            <h2>Recommended Event Plans</h2>

            <span class="result-count">
              {{ results.length }} results found
            </span>

          </div>

          <!-- RESULT CARD -->

          <v-card
            v-for="plan in results"
            :key="plan.title"
            class="result-card"
          >

            <div class="card-header">

              <h3>{{ plan.title }}</h3>

              <v-chip
                v-if="plan.top"
                color="green"
                size="small"
              >
                Top Match
              </v-chip>

              <v-chip
                color="teal"
                size="small"
              >
                {{ plan.match }}% Match
              </v-chip>

            </div>

            <p class="description">
              {{ plan.description }}
            </p>

            <div class="tags">

              <v-chip size="small">{{ plan.budget }}</v-chip>
              <v-chip size="small">{{ plan.group }}</v-chip>
              <v-chip size="small">{{ plan.location }}</v-chip>

            </div>

            <div class="actions">

              <v-btn color="teal" size="small">
                View Details
              </v-btn>

              <v-btn variant="outlined" size="small" @click="toggleFavorite">
                {{ saved ? '♡ Saved' : '♡ Save' }}
              </v-btn>

            </div>

          </v-card>

        </div>

      </v-container>

    </v-main>

  </v-app>
</template>

<script setup lang="ts">
import { ref } from "vue"
import { useRouter } from "vue-router"
import { useAuth } from "@/composables/useAuth"

const prompt = ref("")
const router = useRouter()
const auth = useAuth()

const results = ref<any[]>([])

const saved = ref(false)

function toggleFavorite() {
  saved.value = !saved.value
}

function generateIdeas() {

  // MOCK RESULTS (later replace with Azure API)

  results.value = [

    {
      title: "Cozy Indoor Birthday Bash",
      description:
        "A warm and intimate celebration perfect for small groups with games, snacks, and decorations.",
      match: 95,
      top: true,
      budget: "Budget: Low",
      group: "Group Size: 4-8",
      location: "Indoor"
    },

    {
      title: "Movie Marathon Party",
      description:
        "Create a cinema experience at home with a projector, popcorn bar, and cozy seating.",
      match: 88,
      top: false,
      budget: "Budget: Low",
      group: "Group Size: 6-10",
      location: "Indoor"
    },

    {
      title: "DIY Game Night Extravaganza",
      description:
        "Board games, card games, and fun group activities with themed snacks and prizes.",
      match: 85,
      top: false,
      budget: "Budget: Low",
      group: "Group Size: 4-12",
      location: "Indoor"
    }

  ]
}

function handleLogout() {
  auth.logout()
  router.push("/login")
}
</script>

<style scoped>

.dashboard{
  max-width:900px;
  margin:auto;
}

.prompt-area{
  text-align:center;
  margin-top:40px;
}

.subtitle{
  color:#777;
  margin-bottom:20px;
}

.generate-btn{
  margin-top:10px;
  width:300px;
}

.results{
  margin-top:50px;
}

.results-header{
  display:flex;
  justify-content:space-between;
  align-items:center;
  margin-bottom:20px;
}

.result-card{
  padding:20px;
  margin-bottom:18px;
}

.card-header{
  display:flex;
  align-items:center;
  gap:10px;
}

.description{
  margin:10px 0;
  color:#555;
}

.tags{
  display:flex;
  gap:8px;
  margin-bottom:10px;
}

.actions{
  display:flex;
  gap:10px;
}

.logo{
  font-weight:700;
}

.ai-badge{
  background:teal;
  color:white;
  padding:2px 8px;
  border-radius:10px;
  font-size:12px;
  margin-left:4px;
}

</style>