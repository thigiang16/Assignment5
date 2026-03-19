<template>
  <div class="landing">
    <section class="hero">
      <div class="blob blob1"></div>
      <div class="blob blob2"></div>
      <div class="blob blob3"></div>

      <v-container class="hero-content">
        <div class="hero-badge">✨ AI-Powered Event Planning</div>

        <h1>Plan Smarter.<br />Celebrate Better.</h1>

        <p class="hero-sub">
          Describe your event and let our AI generate personalized ideas —<br class="desktop-break" />
          themes, food, activities, decorations, and more.
        </p>

        <div class="prompt-bar" :class="{ 'prompt-bar--focused': promptFocused }">
          <v-icon class="prompt-icon" color="rgba(255,255,255,0.5)">mdi-magnify</v-icon>
          <input
            v-model="prompt"
            class="prompt-input"
            placeholder="E.g. 'Outdoor birthday for 20 people with games and BBQ'"
            @focus="promptFocused = true"
            @blur="promptFocused = false"
            @keydown.enter="generateIdeas"
          />
          <button class="prompt-btn" @click="generateIdeas">
            Get Ideas <span class="prompt-btn-arrow">→</span>
          </button>
        </div>

        <div class="hero-chips">
          <span class="chips-label">Try:</span>
          <button
            v-for="eg in examples"
            :key="eg.label"
            class="hero-chip"
            @click="fillPrompt(eg.prompt)"
          >
            {{ eg.label }}
          </button>
        </div>
      </v-container>
    </section>

    <transition name="fade-slide">
      <div v-if="showPreview" class="preview-area">
        <div class="preview-backdrop" @click="showPreview = false"></div>
        <div class="preview-shell">
          <v-skeleton-loader type="card, article, actions" class="preview-card" />
          <div class="preview-overlay">
            <v-btn
              icon="mdi-close"
              variant="text"
              class="close-btn"
              @click="showPreview = false"
            />
            <h2>Your plan is ready 🎉</h2>
            <p>Create a free account to unlock your full event plan.</p>
            <div class="preview-buttons">
              <v-btn color="teal" to="/register">Sign Up Free</v-btn>
              <v-btn
                variant="outlined"
                :to="auth.isAuthenticated.value ? '/dashboard' : '/login'"
              >
                {{ auth.isAuthenticated.value ? 'Dashboard' : 'Login' }}
              </v-btn>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <section class="features-section">
      <v-container>
        <div class="section-eyebrow">Why PlanWise AI</div>
        <h2 class="section-title">Everything you need to plan the perfect event</h2>

        <div class="features-grid">
          <div v-for="f in features" :key="f.title" class="feature-card">
            <div class="feature-icon-wrap" :style="{ background: f.bg }">
              <v-icon :color="f.color" size="26">{{ f.icon }}</v-icon>
            </div>
            <h3 class="feature-title">{{ f.title }}</h3>
            <p class="feature-desc">{{ f.desc }}</p>
          </div>
        </div>
      </v-container>
    </section>

    <section class="how-section">
      <v-container>
        <div class="section-eyebrow">How It Works</div>
        <h2 class="section-title">Three steps to your perfect event</h2>

        <div class="steps-grid">
          <div v-for="(step, i) in steps" :key="step.title" class="step-card">
            <div class="step-number">{{ String(i + 1).padStart(2, '0') }}</div>
            <div class="step-icon-wrap">
              <v-icon color="teal" size="24">{{ step.icon }}</v-icon>
            </div>
            <h3 class="step-title">{{ step.title }}</h3>
            <p class="step-desc">{{ step.desc }}</p>
          </div>
        </div>
      </v-container>
    </section>

    <section class="inspiration-section">
      <v-container>
        <div class="section-eyebrow">Need inspiration?</div>
        <h2 class="section-title">Browse popular event types</h2>

        <div class="inspiration-grid">
          <button
            v-for="eg in examples"
            :key="eg.label"
            class="inspiration-card"
            @click="goFillPrompt(eg.prompt)"
          >
            <v-icon size="36" :color="eg.color" class="insp-icon">{{ eg.icon }}</v-icon>
            <span class="insp-label">{{ eg.label }}</span>
            <span class="insp-hint">Click to try →</span>
          </button>
        </div>
      </v-container>
    </section>

    <section class="cta-section">
      <div class="cta-blob cta-blob1"></div>
      <div class="cta-blob cta-blob2"></div>
      <v-container class="cta-inner">
        <h2>Start planning your next event today</h2>
        <p>Join thousands of users creating memorable celebrations with AI-powered ideas.</p>
        <div class="cta-buttons">
          <v-btn
            color="teal"
            size="x-large"
            rounded="pill"
            class="cta-primary"
            :to="auth.isAuthenticated.value ? '/dashboard' : '/register'"
          >
            {{ auth.isAuthenticated.value ? 'Go to Dashboard' : 'Get Started Free' }}
            <v-icon end>mdi-arrow-right</v-icon>
          </v-btn>
          <v-btn
            v-if="!auth.isAuthenticated.value"
            variant="outlined"
            size="x-large"
            rounded="pill"
            to="/login"
            class="cta-secondary"
          >
            Sign In
          </v-btn>
        </div>
      </v-container>
    </section>

  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAuth } from '@/composables/useAuth'

const auth = useAuth()
const prompt = ref('')
const promptFocused = ref(false)
const showPreview = ref(false)

const examples = [
  {
    label: 'Birthday Party',
    icon: 'mdi-cake-variant-outline',
    color: 'teal',
    prompt: 'A fun birthday party for 20 adults with dinner, games, and a dance floor'
  },
  {
    label: 'Baby Shower',
    icon: 'mdi-baby-carriage',
    color: '#8b5cf6',
    prompt: 'An elegant baby shower for 30 guests with a garden theme and light refreshments'
  },
  {
    label: 'Graduation Party',
    icon: 'mdi-school-outline',
    color: '#f59e0b',
    prompt: 'A graduation celebration for 50 people with outdoor activities and food trucks'
  },
  {
    label: 'Backyard BBQ',
    icon: 'mdi-grill-outline',
    color: '#ef4444',
    prompt: 'A casual backyard BBQ for 15 friends with lawn games and live music'
  }
]

const features = [
  {
    icon: 'mdi-lightbulb-on-outline',
    color: 'teal',
    bg: 'rgba(20,184,166,0.12)',
    title: 'Creative Ideas',
    desc: 'Let AI generate unique themes, food suggestions, activities and decorations tailored to your event.'
  },
  {
    icon: 'mdi-calendar-check-outline',
    color: '#3b82f6',
    bg: 'rgba(59,130,246,0.12)',
    title: 'Easy Planning',
    desc: 'Get fully structured, actionable event plans in seconds — no spreadsheets or research required.'
  },
  {
    icon: 'mdi-heart-outline',
    color: '#ec4899',
    bg: 'rgba(236,72,153,0.12)',
    title: 'Save & Expand',
    desc: 'Save your favourite plans and revisit them any time from your personal dashboard.'
  }
]

const steps = [
  {
    icon: 'mdi-pencil-outline',
    title: 'Describe your event',
    desc: 'Type a short description — party type, group size, vibe, location, or anything you have in mind.'
  },
  {
    icon: 'mdi-robot-outline',
    title: 'AI generates ideas',
    desc: 'Our AI instantly creates personalised event plans with themes, food, activities, and more.'
  },
  {
    icon: 'mdi-content-save-outline',
    title: 'Save or customise',
    desc: 'Browse the results, save the ones you love, and view full details on your dashboard.'
  }
]

function generateIdeas() {
  if (!prompt.value.trim()) return
  showPreview.value = true
}

function fillPrompt(text: string) {
  prompt.value = text
}

function goFillPrompt(text: string) {
  prompt.value = text
  window.scrollTo({ top: 0, behavior: 'smooth' })
}
</script>

<style scoped>
.landing {
  font-family: 'Inter', sans-serif;
  overflow-x: hidden;
  background: #ffffff;
}

.hero {
  position: relative;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  background: linear-gradient(135deg, #0f172a 0%, #020617 100%);
  padding-top: 80px; 
}

.blob {
  position: absolute;
  border-radius: 50%;
  filter: blur(100px);
  opacity: 0.35;
  animation: drift 20s infinite alternate ease-in-out;
}
.blob1 {
  width: 520px; height: 520px;
  background: #14b8a6;
  top: -80px; left: -100px;
  animation-duration: 22s;
}
.blob2 {
  width: 480px; height: 480px;
  background: #3b82f6;
  bottom: -60px; right: -80px;
  animation-duration: 18s;
  animation-direction: alternate-reverse;
}
.blob3 {
  width: 320px; height: 320px;
  background: #8b5cf6;
  top: 40%; left: 55%;
  animation-duration: 26s;
  opacity: 0.2;
}
@keyframes drift {
  from { transform: translate(0, 0) scale(1); }
  to   { transform: translate(30px, -30px) scale(1.06); }
}

.hero-content {
  position: relative;
  z-index: 2;
  text-align: center;
  color: white;
  max-width: 820px;
  padding-block: 60px;
}

.hero-badge {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: rgba(20, 184, 166, 0.15);
  border: 1px solid rgba(20, 184, 166, 0.35);
  color: #5eead4;
  font-size: 13px;
  font-weight: 600;
  padding: 6px 16px;
  border-radius: 999px;
  margin-bottom: 28px;
  letter-spacing: 0.02em;
}

.hero-content h1 {
  font-size: clamp(38px, 6vw, 68px);
  font-weight: 800;
  letter-spacing: -0.03em;
  line-height: 1.1;
  margin-bottom: 20px;
  background: linear-gradient(100deg, #ffffff 30%, #99f6e4);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

.hero-sub {
  font-size: clamp(16px, 2.2vw, 20px);
  color: rgba(255, 255, 255, 0.75);
  margin-bottom: 40px;
  line-height: 1.6;
}

.desktop-break { display: inline; }

/* AI PROMPT BAR */
.prompt-bar {
  display: flex;
  align-items: center;
  gap: 0;
  max-width: 680px;
  margin: 0 auto 20px;
  background: rgba(255, 255, 255, 0.07);
  border: 1.5px solid rgba(255, 255, 255, 0.15);
  border-radius: 16px;
  padding: 10px 10px 10px 18px;
  backdrop-filter: blur(14px);
  transition: border-color 0.25s, box-shadow 0.25s, transform 0.2s;
}
.prompt-bar--focused {
  border-color: rgba(20, 184, 166, 0.7);
  box-shadow: 0 0 0 4px rgba(20, 184, 166, 0.12), 0 16px 40px rgba(0, 0, 0, 0.3);
  transform: translateY(-2px);
}

.prompt-icon {
  flex-shrink: 0;
  margin-right: 10px;
}

.prompt-input {
  flex: 1;
  background: transparent;
  border: none;
  outline: none;
  color: white;
  font-size: 16px;
  font-family: inherit;
  min-width: 0;
}
.prompt-input::placeholder {
  color: rgba(255, 255, 255, 0.4);
}

.prompt-btn {
  flex-shrink: 0;
  background: linear-gradient(135deg, #14b8a6, #0d9488);
  color: white;
  border: none;
  border-radius: 10px;
  padding: 10px 22px;
  font-size: 15px;
  font-weight: 700;
  font-family: inherit;
  cursor: pointer;
  transition: opacity 0.2s, transform 0.2s;
  white-space: nowrap;
}
.prompt-btn:hover {
  opacity: 0.9;
  transform: scale(1.02);
}
.prompt-btn-arrow {
  margin-left: 6px;
}

/* HERO CHIPS */
.hero-chips {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-wrap: wrap;
  gap: 8px;
  margin-top: 4px;
}
.chips-label {
  font-size: 13px;
  color: rgba(255, 255, 255, 0.45);
  font-weight: 500;
}
.hero-chip {
  background: rgba(255, 255, 255, 0.07);
  border: 1px solid rgba(255, 255, 255, 0.15);
  color: rgba(255, 255, 255, 0.75);
  font-size: 13px;
  font-family: inherit;
  padding: 5px 14px;
  border-radius: 999px;
  cursor: pointer;
  transition: background 0.2s, border-color 0.2s, color 0.2s;
}
.hero-chip:hover {
  background: rgba(20, 184, 166, 0.2);
  border-color: rgba(20, 184, 166, 0.5);
  color: #99f6e4;
}

.preview-area {
  position: fixed;
  inset: 0;
  z-index: 200;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 24px;
}
.preview-backdrop {
  position: absolute;
  inset: 0;
  background: rgba(2, 6, 23, 0.8);
  backdrop-filter: blur(8px);
}
.preview-shell {
  position: relative;
  width: min(700px, 100%);
  animation: preview-pop 0.3s ease forwards;
}
@keyframes preview-pop {
  from { transform: scale(0.95); opacity: 0; }
  to   { transform: scale(1); opacity: 1; }
}
.preview-card {
  filter: blur(5px);
  opacity: 0.5;
  border-radius: 16px;
  overflow: hidden;
}
.preview-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.65);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 32px 24px;
  text-align: center;
  color: white;
}
.close-btn {
  position: absolute;
  top: 10px;
  right: 10px;
  color: rgba(255,255,255,0.6) !important;
}
.preview-overlay h2 { margin-bottom: 10px; }
.preview-overlay p  { margin-bottom: 0; color: rgba(255,255,255,0.75); }
.preview-buttons {
  display: flex;
  gap: 12px;
  margin-top: 20px;
  flex-wrap: wrap;
  justify-content: center;
}

.section-eyebrow {
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #14b8a6;
  margin-bottom: 10px;
}
.section-title {
  font-size: clamp(24px, 3.5vw, 36px);
  font-weight: 800;
  letter-spacing: -0.02em;
  color: #0f172a;
  margin-bottom: 48px;
  line-height: 1.2;
}

.features-section {
  padding: 96px 0;
  background: #ffffff;
  text-align: center;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
}

.feature-card {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 20px;
  padding: 32px 28px;
  text-align: left;
  transition: transform 0.25s ease, box-shadow 0.25s ease, border-color 0.25s;
}
.feature-card:hover {
  transform: translateY(-6px);
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.07);
  border-color: rgba(20, 184, 166, 0.35);
}

.feature-icon-wrap {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 52px;
  height: 52px;
  border-radius: 14px;
  margin-bottom: 18px;
}
.feature-title {
  font-size: 18px;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 8px;
}
.feature-desc {
  font-size: 14px;
  color: #64748b;
  line-height: 1.65;
  margin: 0;
}

.how-section {
  padding: 96px 0;
  background: #f8fafc;
  text-align: center;
}

.steps-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
  position: relative;
}

.step-card {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 20px;
  padding: 36px 28px;
  text-align: left;
  position: relative;
  overflow: hidden;
  transition: transform 0.25s ease, box-shadow 0.25s ease;
}
.step-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.06);
}
.step-number {
  position: absolute;
  top: 20px;
  right: 24px;
  font-size: 48px;
  font-weight: 900;
  color: rgba(20, 184, 166, 0.1);
  line-height: 1;
  pointer-events: none;
}
.step-icon-wrap {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 46px;
  height: 46px;
  background: rgba(20, 184, 166, 0.1);
  border-radius: 12px;
  margin-bottom: 16px;
}
.step-title {
  font-size: 17px;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 8px;
}
.step-desc {
  font-size: 14px;
  color: #64748b;
  line-height: 1.65;
  margin: 0;
}

.inspiration-section {
  padding: 96px 0;
  background: #ffffff;
  text-align: center;
}

.inspiration-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
}

.inspiration-card {
  background: #f8fafc;
  border: 1.5px solid #e2e8f0;
  border-radius: 18px;
  padding: 28px 20px 24px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  transition: transform 0.25s ease, box-shadow 0.25s ease, border-color 0.25s, background 0.25s;
  text-align: center;
}
.inspiration-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 16px 36px rgba(0, 0, 0, 0.07);
  border-color: rgba(20, 184, 166, 0.4);
  background: #f0fdfa;
}
.insp-icon { transition: transform 0.25s; }
.inspiration-card:hover .insp-icon { transform: scale(1.12); }
.insp-label {
  font-size: 15px;
  font-weight: 700;
  color: #0f172a;
}
.insp-hint {
  font-size: 12px;
  color: #94a3b8;
  opacity: 0;
  transition: opacity 0.2s;
}
.inspiration-card:hover .insp-hint { opacity: 1; }

.cta-section {
  position: relative;
  padding: 100px 0;
  overflow: hidden;
  background: linear-gradient(135deg, #0f172a 0%, #020617 100%);
  text-align: center;
  color: white;
}
.cta-blob {
  position: absolute;
  border-radius: 50%;
  filter: blur(100px);
  opacity: 0.25;
}
.cta-blob1 {
  width: 400px; height: 400px;
  background: #14b8a6;
  top: -100px; left: -80px;
}
.cta-blob2 {
  width: 360px; height: 360px;
  background: #3b82f6;
  bottom: -80px; right: -60px;
}
.cta-inner {
  position: relative;
  z-index: 2;
  max-width: 680px;
}
.cta-inner h2 {
  font-size: clamp(26px, 4vw, 40px);
  font-weight: 800;
  letter-spacing: -0.02em;
  margin-bottom: 14px;
}
.cta-inner p {
  font-size: 17px;
  color: rgba(255, 255, 255, 0.7);
  margin-bottom: 36px;
  line-height: 1.6;
}
.cta-buttons {
  display: flex;
  justify-content: center;
  gap: 14px;
  flex-wrap: wrap;
}
.cta-primary { font-size: 15px !important; font-weight: 700 !important; padding-inline: 32px !important; }
.cta-secondary { color: white !important; border-color: rgba(255,255,255,0.3) !important; padding-inline: 32px !important; }
.cta-secondary:hover { background: rgba(255,255,255,0.08) !important; }

.fade-slide-enter-active,
.fade-slide-leave-active {
  transition: opacity 0.3s ease, transform 0.3s ease;
}
.fade-slide-enter-from,
.fade-slide-leave-to {
  opacity: 0;
  transform: translateY(10px);
}

@media (max-width: 960px) {
  .features-grid,
  .steps-grid {
    grid-template-columns: 1fr;
    max-width: 520px;
    margin: 0 auto;
  }

  .inspiration-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}

@media (max-width: 640px) {
  .hero { padding-top: 72px; }

  .desktop-break { display: none; }

  .prompt-bar {
    flex-direction: column;
    align-items: stretch;
    gap: 10px;
    padding: 14px;
  }
  .prompt-icon { display: none; }
  .prompt-btn {
    width: 100%;
    padding: 13px;
    text-align: center;
    border-radius: 10px;
    font-size: 16px;
  }

  .hero-chips { justify-content: flex-start; }

  .features-section,
  .how-section,
  .inspiration-section {
    padding: 64px 0;
  }

  .cta-section { padding: 72px 0; }

  .inspiration-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 12px;
  }

  .cta-buttons .v-btn {
    width: 100%;
  }

  .section-title { margin-bottom: 32px; }
}
</style>