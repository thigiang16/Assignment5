<template>
  <v-main>
    <v-container v-if="plan" class="plan-page py-8">
      <v-btn variant="text" class="back-btn mb-4" prepend-icon="mdi-arrow-left" @click="router.back()">
        Back to Results
      </v-btn>

        <v-row>
          <v-col cols="12" md="8">
            <v-card class="hero-card pa-6 mb-4" rounded="xl" elevation="3">
              <div class="heading-wrap">
                <h1 class="text-h4 mb-2">{{ plan.title }}</h1>
                <p class="subtitle mb-0">A complete event outline tailored to your prompt.</p>
              </div>

              <div class="chip-row mt-4">
                <v-chip color="teal" variant="tonal" size="small">{{ plan.budgetLevel }}</v-chip>
                <v-chip color="teal" variant="tonal" size="small">{{ plan.locationType }}</v-chip>
                <v-chip color="teal" variant="tonal" size="small">{{ plan.suggestedGroupSize }} people</v-chip>
              </div>

              <p class="description mt-5 mb-0">
                {{ plan.description }}
              </p>
            </v-card>

            <v-card class="detail-card pa-6 mb-4" rounded="xl" elevation="2">
              <div class="section-title mb-4">
                <v-icon color="teal" class="mr-2">mdi-silverware-fork-knife</v-icon>
                <span>Food Ideas</span>
              </div>

              <ul class="detail-list">
                <li v-for="item in plan.foodIdeas" :key="item">
                  {{ item }}
                </li>
              </ul>
            </v-card>

            <v-card class="detail-card pa-6 mb-4" rounded="xl" elevation="2">
              <div class="section-title mb-4">
                <v-icon color="teal" class="mr-2">mdi-party-popper</v-icon>
                <span>Activities</span>
              </div>

              <ul class="detail-list">
                <li v-for="item in plan.activities" :key="item">
                  {{ item }}
                </li>
              </ul>
            </v-card>

            <v-card class="detail-card pa-6" rounded="xl" elevation="2">
              <div class="section-title mb-4">
                <v-icon color="teal" class="mr-2">mdi-string-lights</v-icon>
                <span>Decorations</span>
              </div>

              <ul class="detail-list">
                <li v-for="item in plan.decorations" :key="item">
                  {{ item }}
                </li>
              </ul>
            </v-card>
          </v-col>

          <v-col cols="12" md="4">
            <v-card class="side-card pa-5 mb-4" rounded="xl" elevation="2">
              <div class="section-title mb-4">
                <v-icon color="teal" class="mr-2">mdi-lightning-bolt-outline</v-icon>
                <span>Actions</span>
              </div>

              <v-btn block color="teal" size="large" @click="toggleFavorite">
                {{ plan.saved ? "❤ Saved to Favorites" : "♡ Save to Favorites" }}
              </v-btn>

              <v-btn
                block
                color="teal"
                variant="outlined"
                size="large"
                :loading="loadingAI"
                :disabled="loadingAI"
                @click="expandPlan"
              >
                Expand This Plan
              </v-btn>
            </v-card>

            <v-card class="side-card pa-5" rounded="xl" elevation="2">
              <div class="section-title mb-4">
                <v-icon color="teal" class="mr-2">mdi-information-outline</v-icon>
                <span>Quick Info</span>
              </div>

              <div class="info-stack">
                <div class="info-row">
                  <span class="label">Location</span>
                  <strong>{{ plan.locationType }}</strong>
                </div>

                <div class="info-row">
                  <span class="label">Group Size</span>
                  <strong>{{ plan.suggestedGroupSize }}</strong>
                </div>

                <div class="info-row">
                  <span class="label">Budget</span>
                  <strong>{{ plan.budgetLevel }}</strong>
                </div>
              </div>
            </v-card>
          </v-col>
        </v-row>

        <v-card v-if="loadingAI || aiSuggestions || aiError" class="detail-card pa-6 mt-4" rounded="xl" elevation="2">
          <div class="section-title mb-4">
            <v-icon color="teal" class="mr-2">mdi-robot-outline</v-icon>
            <span>AI-Enhanced Suggestions</span>
          </div>

          <p v-if="loadingAI" class="subtitle mb-0">Generating suggestions...</p>

          <v-alert v-else-if="aiError" type="error" variant="tonal" class="mb-0">
            {{ aiError }}
          </v-alert>

          <div v-else-if="aiSuggestions" class="ai-sections">
            <div class="ai-section">
              <h3>Music Playlist Ideas</h3>
              <ul class="detail-list">
                <li v-for="item in aiSuggestions.musicPlaylistIdeas" :key="`music-${item}`">{{ item }}</li>
              </ul>
            </div>

            <div class="ai-section">
              <h3>Timeline Suggestions</h3>
              <ul class="detail-list">
                <li v-for="item in aiSuggestions.timelineSuggestions" :key="`timeline-${item}`">{{ item }}</li>
              </ul>
            </div>

            <div class="ai-section">
              <h3>Shopping List Essentials</h3>
              <ul class="detail-list">
                <li v-for="item in aiSuggestions.shoppingListEssentials" :key="`shop-${item}`">{{ item }}</li>
              </ul>
            </div>
          </div>
        </v-card>
    </v-container>

    <v-container v-else-if="isLoading" class="plan-page py-8">
      <v-card class="hero-card pa-6" rounded="xl" elevation="2">
        <p class="subtitle mb-0">Loading plan details...</p>
      </v-card>
    </v-container>

    <v-container v-else class="plan-page py-8">
      <v-card class="hero-card pa-6" rounded="xl" elevation="2">
        <h2 class="text-h5 mb-2">Plan not found</h2>
        <p class="subtitle mb-0">We could not load this saved plan right now.</p>
      </v-card>
    </v-container>
  </v-main>
</template>

<script setup lang="ts">
import { ref } from "vue"
import { useRoute, useRouter } from "vue-router"
import { useEventSearchState } from "@/composables/useEventSearchState"
import {
  aiService,
  expandPlan as expandPlanApi,
  savePlan,
  type ExpandPlanSuggestions,
  type GeneratedEventPlan
} from "@/services/aiService"

type PlanDetailsModel = GeneratedEventPlan & {
  saved?: boolean
}

const router = useRouter()
const route = useRoute()
const { getEventById } = useEventSearchState()

const routeId = typeof route.params.id === "string" ? route.params.id : ""
const savedPlan = sessionStorage.getItem("selectedPlan")
const rawSavedPlan = savedPlan ? JSON.parse(savedPlan) : null
const rawPlan = getEventById(routeId) || rawSavedPlan || history.state.plan
const plan = ref<PlanDetailsModel | null>(normalizePlan(rawPlan))
const isLoading = ref(false)
const aiSuggestions = ref<ExpandPlanSuggestions | null>(null)
const aiError = ref("")
const loadingAI = ref(false)

function getPlanId() {
  if (!plan.value)
    return ""

  return routeId || plan.value.title
}

const expandPlan = async () => {
  if (!plan.value)
    return

  loadingAI.value = true
  aiError.value = ""

  try {
    const planId = getPlanId()

    aiSuggestions.value = await expandPlanApi(
      planId,
      plan.value.title,
      plan.value.description
    )
  } catch {
    aiError.value = "We could not generate AI suggestions right now. Please try again."
  } finally {
    loadingAI.value = false
  }
}

function normalizeList(value: unknown): string[] {
  if (Array.isArray(value)) {
    return value.filter((x): x is string => typeof x === "string" && x.trim().length > 0)
  }

  if (typeof value === "string") {
    return value
      .split(/\n|;|,/)
      .map(x => x.trim())
      .filter(Boolean)
  }

  return []
}

function normalizePlan(value: unknown): PlanDetailsModel | null {
  if (!value || typeof value !== "object") {
    return null
  }

  const candidate = value as Partial<PlanDetailsModel>

  if (!candidate.title) {
    return null
  }

  return {
    title: candidate.title ?? "",
    description: candidate.description ?? "",
    budgetLevel: candidate.budgetLevel ?? "",
    suggestedGroupSize: candidate.suggestedGroupSize ?? 0,
    locationType: candidate.locationType ?? "",
    foodIdeas: normalizeList(candidate.foodIdeas),
    activities: normalizeList(candidate.activities),
    decorations: normalizeList(candidate.decorations),
    score: candidate.score ?? 0,
    saved: candidate.saved ?? false
  }
}

function decodeRouteIdToTitle(id: string) {
  try {
    return decodeURIComponent(id).trim()
  } catch {
    return ""
  }
}

async function loadPlanByRouteId() {
  if (plan.value || !routeId)
    return

  const possibleTitle = decodeRouteIdToTitle(routeId)

  if (!possibleTitle)
    return

  isLoading.value = true

  try {
    const generatedPlans = await aiService.generateEventIdeas(possibleTitle)
    const matchedPlan = generatedPlans.find(
      item => item.title.trim().toLowerCase() === possibleTitle.toLowerCase()
    ) || generatedPlans[0]

    if (!matchedPlan)
      return

    plan.value = normalizePlan({ ...matchedPlan, saved: true })

    if (plan.value)
      sessionStorage.setItem("selectedPlan", JSON.stringify(plan.value))
  } catch {
    // Keep fallback UI when loading fails.
  } finally {
    isLoading.value = false
  }
}

loadPlanByRouteId()

async function toggleFavorite() {
  if (!plan.value?.title)
    return

  try {
    await savePlan(plan.value.title)
    plan.value.saved = true
    sessionStorage.setItem("selectedPlan", JSON.stringify(plan.value))
  } catch {
    // Keep UI stable even if save fails.
  }
}
</script>

<style scoped>
.plan-page {
  max-width: 1100px;
  margin: auto;
}

.back-btn {
  text-transform: none;
  font-weight: 600;
}

.hero-card {
  border: 1px solid rgba(20, 184, 166, 0.18);
  background: linear-gradient(180deg, #f0fdfa 0%, #ffffff 100%);
}

.detail-card,
.side-card {
  border: 1px solid rgba(20, 184, 166, 0.14);
  background: linear-gradient(180deg, #f8fafc 0%, #ffffff 100%);
}

.heading-wrap {
  border-left: 5px solid #14b8a6;
  padding-left: 12px;
}

.subtitle {
  color: #64748b;
}

.description {
  color: #475569;
  line-height: 1.7;
}

.chip-row {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.section-title {
  display: flex;
  align-items: center;
  font-size: 18px;
  font-weight: 700;
  color: #0f172a;
}

.detail-list {
  margin: 0;
  padding-left: 20px;
  color: #475569;
  line-height: 1.8;
}

.detail-list li + li {
  margin-top: 8px;
}

.info-stack {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 12px;
  border-bottom: 1px solid #e2e8f0;
}

.info-row:last-child {
  border-bottom: 0;
  padding-bottom: 0;
}

.label {
  color: #64748b;
}

.ai-sections {
  display: grid;
  grid-template-columns: 1fr;
  gap: 18px;
}

.ai-section h3 {
  font-size: 16px;
  margin-bottom: 8px;
  color: #0f172a;
}

@media (max-width: 640px) {
  .plan-page {
    padding-top: 12px;
  }

  .hero-card,
  .detail-card,
  .side-card {
    padding: 18px !important;
  }

  .info-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 6px;
  }
}
</style>