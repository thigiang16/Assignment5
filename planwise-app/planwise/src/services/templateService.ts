import api from "@/services/api"

export interface EventTemplate {
  id: number
  title: string
  category: string
  description: string
  locationType: string
  suggestedGroupSize: number
  foodIdeas: string
  activities: string
  decorations: string
  budgetLevel: string
  tags: string
  createdAt: string
}

export type TemplatePayload = Omit<EventTemplate, "id" | "createdAt">

async function getTemplates(): Promise<EventTemplate[]> {
  const response = await api.get<EventTemplate[]>("/api/eventtemplates")
  return response.data
}

async function createTemplate(template: TemplatePayload): Promise<EventTemplate> {
  const response = await api.post<EventTemplate>("/api/eventtemplates", template)
  return response.data
}

async function updateTemplate(id: number, template: EventTemplate): Promise<void> {
  await api.put(`/api/eventtemplates/${id}`, template)
}

async function deleteTemplate(id: number): Promise<void> {
  await api.delete(`/api/eventtemplates/${id}`)
}

export const templateService = {
  getTemplates,
  createTemplate,
  updateTemplate,
  deleteTemplate
}
