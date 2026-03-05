<template>
  <div class="landing">

    <!-- NAVBAR -->
    <v-app-bar flat color="transparent" class="navbar">
      <v-container class="d-flex align-center justify-space-between">

        <div class="logo">
          PlanWise <span class="ai-badge">AI</span>
        </div>

        <div class="nav-links">
          <a href="#">Home</a>
          <a href="#">How It Works</a>
          <a href="#">About</a>
        </div>

        <div class="auth-buttons">
          <v-btn variant="text" to="/login">Login</v-btn>

          <v-btn color="teal" rounded to="/register">
            Sign Up
          </v-btn>
        </div>

      </v-container>
    </v-app-bar>

    <!-- HERO SECTION -->
    <section class="hero">

      <div class="overlay"></div>

      <v-container class="hero-content">

        <h1>Plan Smarter. Celebrate Better.</h1>

        <p>
          Describe your event and let us create personalized ideas for you.
        </p>

        <!-- INPUT PREVIEW -->
        <div class="search-box">
          <v-text-field
            placeholder="E.g. 'Fun summer hangout with friends'"
            variant="solo"
            hide-details
          ></v-text-field>

          <v-btn color="teal" size="large">
            Get Ideas
          </v-btn>
        </div>

        <!-- FEATURES -->
        <div class="features">

          <div class="feature">
            <v-icon size="36">mdi-lightbulb-outline</v-icon>
            <h3>Creative Ideas</h3>
            <p>Generate fun and unique event suggestions.</p>
          </div>

          <div class="feature">
            <v-icon size="36">mdi-calendar-check-outline</v-icon>
            <h3>Easy Planning</h3>
            <p>Get actionable plans tailored to your group.</p>
          </div>

          <div class="feature">
            <v-icon size="36">mdi-heart-outline</v-icon>
            <h3>Save & Expand</h3>
            <p>Save favorites and expand plans with AI.</p>
          </div>

        </div>

        <!-- CTA BUTTONS -->
        <div class="cta">
          <v-btn color="primary" size="large" to="/login">
            Login
          </v-btn>

          <v-btn color="teal" size="large" to="/register">
            Sign Up Free
          </v-btn>
        </div>

      </v-container>

    </section>

  </div>
</template>

<script setup lang="ts">
</script>

<style scoped>

.landing {
  font-family: "Inter", sans-serif;
}

/* NAVBAR */

.navbar {
  position: absolute;
  width: 100%;
  z-index: 10;
  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);

  border-bottom: 1px solid rgba(255,255,255,0.15);
}

.logo {
  font-size: 20px;
  font-weight: 700;
  color: white;
  letter-spacing: 0.5px;
}

.ai-badge {
  background: teal;
  color: white;
  padding: 3px 8px;
  border-radius: 12px;
  font-size: 12px;
}

.nav-links a {
  margin: 0 12px;
  color: white;
  text-decoration: none;
  font-weight: 500;
  opacity: 0.9;
  transition: opacity 0.2s;
}

/* HERO */

.hero {
  position: relative;
  height: 100vh;
  background-image: url('/event-bg.jpg');
  background-size: cover;
  background-position: center;
  display: flex;
  align-items: center;
  justify-content: center;
}

.overlay {
  position: absolute;
  inset: 0;
  background: rgba(0,0,0,0.55);
}

.hero-content {
  position: relative;
  text-align: center;
  color: white;
  max-width: 900px;
}

.hero-content h1 {
  font-size: 52px;
  font-weight: 700;
  margin-bottom: 16px;
}

.hero-content p {
  font-size: 18px;
  margin-bottom: 32px;
}

/* SEARCH */

.search-box {
  display: flex;
  gap: 10px;
  max-width: 600px;
  margin: auto;
  margin-bottom: 40px;
}

/* FEATURES */

.features {
  display: flex;
  justify-content: center;
  gap: 50px;
  margin-top: 40px;
}

.feature {
  text-align: center;
  max-width: 200px;
}

.feature h3 {
  margin-top: 10px;
}

/* CTA */

.cta {
  margin-top: 40px;
  display: flex;
  justify-content: center;
  gap: 16px;
}

</style>