<template>
  <div class="template-page">
    <div class="d-flex justify-space-between align-center mb-4 heading-wrap">
      <h2 class="text-h5">Template Management</h2>
      <v-btn color="teal" @click="openCreateDialog">
        Add Template
      </v-btn>
    </div>

    <v-alert
      v-if="errorMessage"
      type="error"
      variant="tonal"
      class="mb-4"
    >
      {{ errorMessage }}
    </v-alert>

    <v-card class="mb-6 section-card" rounded="xl" elevation="3">
      <v-card-text>
        <div v-if="isLoading" class="py-10 text-center">
          <v-progress-circular indeterminate color="teal" />
        </div>

        <v-row v-else>
          <v-col
            v-for="template in templates"
            :key="template.id"
            cols="12"
            md="6"
          >
            <v-card class="template-card h-100" rounded="xl" elevation="2">
              <v-card-text>
                <div class="d-flex justify-space-between align-start mb-2">
                  <h3 class="text-subtitle-1 font-weight-bold mb-0">{{ template.title }}</h3>
                  <v-chip color="teal" size="small" variant="tonal">{{ template.category }}</v-chip>
                </div>

                <p class="description-cell mb-3">{{ template.description }}</p>

                <div class="chip-wrap mb-3">
                  <v-chip size="small" variant="outlined">{{ template.locationType }}</v-chip>
                  <v-chip size="small" variant="outlined">Group: {{ template.suggestedGroupSize }}</v-chip>
                  <v-chip size="small" variant="outlined">Budget: {{ template.budgetLevel }}</v-chip>
                  <v-chip size="small" variant="outlined">{{ template.tags }}</v-chip>
                </div>

                <div class="d-flex ga-2 mt-2">
                  <v-btn size="small" variant="outlined" @click="openEditDialog(template)">
                    Edit
                  </v-btn>
                  <v-btn size="small" color="red" variant="tonal" @click="confirmDelete(template)">
                    Delete
                  </v-btn>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-dialog v-model="isFormDialogOpen" max-width="860">
      <v-card class="form-card" rounded="xl">
        <v-card-title class="form-header pa-5">
          <div class="d-flex align-center ga-3">
            <v-avatar color="teal-lighten-4" size="40">
              <v-icon color="teal">mdi-file-document-edit-outline</v-icon>
            </v-avatar>
            <div>
              <div class="text-h6">{{ isEditMode ? "Edit Template" : "Add Template" }}</div>
            </div>
          </div>
        </v-card-title>

        <v-divider />

        <v-card-text class="pa-5">
          <v-form @submit.prevent="submitForm">
            <v-row>
              <v-col cols="12">
                <div class="section-label">Basics</div>
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field v-model="form.title" label="Title" variant="outlined" density="comfortable" required />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field v-model="form.category" label="Category" variant="outlined" density="comfortable" required />
              </v-col>
              <v-col cols="12">
                <v-textarea v-model="form.description" label="Description" rows="3" variant="outlined" density="comfortable" required />
              </v-col>

              <v-col cols="12">
                <div class="section-label">Event Details</div>
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field v-model="form.locationType" label="Location Type" variant="outlined" density="comfortable" required />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field
                  v-model.number="form.suggestedGroupSize"
                  type="number"
                  min="1"
                  label="Suggested Group Size"
                  variant="outlined"
                  density="comfortable"
                  required
                />
              </v-col>

              <v-col cols="12">
                <div class="section-label">Ideas & Theme</div>
              </v-col>

              <v-col cols="12">
                <v-textarea v-model="form.foodIdeas" label="Food Ideas" rows="2" variant="outlined" density="comfortable" required />
              </v-col>
              <v-col cols="12">
                <v-textarea v-model="form.activities" label="Activities" rows="2" variant="outlined" density="comfortable" required />
              </v-col>
              <v-col cols="12">
                <v-textarea v-model="form.decorations" label="Decorations" rows="2" variant="outlined" density="comfortable" required />
              </v-col>

              <v-col cols="12">
                <div class="section-label">Budget & Tags</div>
              </v-col>

              <v-col cols="12" md="6">
                <v-text-field v-model="form.budgetLevel" label="Budget Level" variant="outlined" density="comfortable" required />
              </v-col>
              <v-col cols="12" md="6">
                <v-text-field v-model="form.tags" label="Tags" hint="Example: indoor, birthday, low-budget" persistent-hint variant="outlined" density="comfortable" required />
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>

        <v-divider />

        <v-card-actions class="pa-4">
          <v-spacer />
          <v-btn variant="text" @click="closeFormDialog">Cancel</v-btn>
          <v-btn color="teal" :loading="isSaving" @click="submitForm">
            {{ isEditMode ? "Save Changes" : "Create Template" }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="isDeleteDialogOpen" max-width="420">
      <v-card>
        <v-card-title>Delete Template</v-card-title>
        <v-card-text>
          Are you sure you want to delete
          <strong>{{ selectedTemplate?.title }}</strong>?
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn variant="text" @click="isDeleteDialogOpen = false">Cancel</v-btn>
          <v-btn color="red" :loading="isDeleting" @click="deleteSelectedTemplate">
            Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue"
import {
  templateService,
  type EventTemplate,
  type TemplatePayload
} from "@/services/templateService"

const templates = ref<EventTemplate[]>([])
const isLoading = ref(false)
const isSaving = ref(false)
const isDeleting = ref(false)
const errorMessage = ref("")

const isFormDialogOpen = ref(false)
const isDeleteDialogOpen = ref(false)
const isEditMode = ref(false)
const selectedTemplate = ref<EventTemplate | null>(null)

const emptyForm: TemplatePayload = {
  title: "",
  category: "",
  description: "",
  locationType: "",
  suggestedGroupSize: 1,
  foodIdeas: "",
  activities: "",
  decorations: "",
  budgetLevel: "",
  tags: ""
}

const form = ref<TemplatePayload>({ ...emptyForm })

async function loadTemplates() {
  isLoading.value = true
  errorMessage.value = ""

  try {
    templates.value = await templateService.getTemplates()
  } catch {
    errorMessage.value = "Unable to load templates. Please refresh and try again."
  } finally {
    isLoading.value = false
  }
}

function openCreateDialog() {
  isEditMode.value = false
  selectedTemplate.value = null
  form.value = { ...emptyForm }
  isFormDialogOpen.value = true
}

function openEditDialog(template: EventTemplate) {
  isEditMode.value = true
  selectedTemplate.value = template
  form.value = {
    title: template.title,
    category: template.category,
    description: template.description,
    locationType: template.locationType,
    suggestedGroupSize: template.suggestedGroupSize,
    foodIdeas: template.foodIdeas,
    activities: template.activities,
    decorations: template.decorations,
    budgetLevel: template.budgetLevel,
    tags: template.tags
  }
  isFormDialogOpen.value = true
}

function closeFormDialog() {
  isFormDialogOpen.value = false
}

async function submitForm() {
  if (!form.value.title.trim() || !form.value.category.trim() || !form.value.description.trim()) {
    errorMessage.value = "Title, category, and description are required."
    return
  }

  isSaving.value = true
  errorMessage.value = ""

  try {
    if (isEditMode.value && selectedTemplate.value) {
      const updatedTemplate: EventTemplate = {
        ...selectedTemplate.value,
        ...form.value
      }

      await templateService.updateTemplate(selectedTemplate.value.id, updatedTemplate)
    } else {
      await templateService.createTemplate(form.value)
    }

    closeFormDialog()
    await loadTemplates()
  } catch {
    errorMessage.value = "Unable to save template. Please try again."
  } finally {
    isSaving.value = false
  }
}

function confirmDelete(template: EventTemplate) {
  selectedTemplate.value = template
  isDeleteDialogOpen.value = true
}

async function deleteSelectedTemplate() {
  if (!selectedTemplate.value) {
    return
  }

  isDeleting.value = true
  errorMessage.value = ""

  try {
    await templateService.deleteTemplate(selectedTemplate.value.id)
    isDeleteDialogOpen.value = false
    await loadTemplates()
  } catch {
    errorMessage.value = "Unable to delete template. Please try again."
  } finally {
    isDeleting.value = false
  }
}

onMounted(async () => {
  await loadTemplates()
})
</script>

<style scoped>
.template-page {
  width: 100%;
}

.heading-wrap {
  border-left: 5px solid #14b8a6;
  padding-left: 12px;
}

.section-card {
  border: 1px solid rgba(20, 184, 166, 0.14);
  background: linear-gradient(180deg, #f8fafc 0%, #ffffff 100%);
}

.template-card {
  border: 1px solid rgba(20, 184, 166, 0.14);
  background: linear-gradient(180deg, #f0fdfa 0%, #ffffff 100%);
}

.description-cell {
  max-width: 100%;
  white-space: normal;
  color: #475569;
}

.chip-wrap {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}

.form-card {
  border: 1px solid rgba(20, 184, 166, 0.2);
}

.form-header {
  background: linear-gradient(180deg, #ecfeff 0%, #ffffff 100%);
}

.section-label {
  font-size: 12px;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  color: #0f766e;
  font-weight: 700;
  margin-bottom: 2px;
}

@media (max-width: 640px) {
  .template-page .d-flex.justify-space-between {
    flex-direction: column;
    align-items: flex-start !important;
    gap: 12px;
  }

  .template-page .d-flex.justify-space-between > .v-btn {
    width: 100%;
  }

  .template-card .d-flex.ga-2 {
    flex-direction: column;
  }

  .template-card .d-flex.ga-2 .v-btn {
    width: 100%;
  }

  .form-header {
    padding: 18px !important;
  }
}
</style>
