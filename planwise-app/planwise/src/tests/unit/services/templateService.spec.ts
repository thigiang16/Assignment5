import { beforeEach, describe, expect, it, vi } from "vitest"
import type { AxiosResponse } from "axios"
import type { EventTemplate, TemplatePayload } from "@/services/templateService"

beforeEach(() => {
  vi.resetModules()
})

describe("templateService", () => {
  it("gets templates for admin management", async () => {
    const { templateService } = await import("@/services/templateService")
    const api = (await import("@/services/api")).default

    const templates: EventTemplate[] = [
      {
        id: 1,
        title: "Office Team Lunch",
        category: "Corporate",
        description: "Casual lunch plan",
        locationType: "Indoor",
        suggestedGroupSize: 12,
        foodIdeas: "Sandwiches",
        activities: "Icebreakers",
        decorations: "Minimal",
        budgetLevel: "Medium",
        tags: "office,lunch",
        createdAt: "2026-03-18T00:00:00Z"
      }
    ]

    vi.spyOn(api, "get").mockResolvedValue({ data: templates } as AxiosResponse<EventTemplate[]>)

    const result = await templateService.getTemplates()

    expect(api.get).toHaveBeenCalledWith("/api/eventtemplates")
    expect(result).toEqual(templates)
  })

  it("creates a template", async () => {
    const { templateService } = await import("@/services/templateService")
    const api = (await import("@/services/api")).default

    const payload: TemplatePayload = {
      title: "Birthday Bash",
      category: "Birthday",
      description: "Fun birthday setup",
      locationType: "Outdoor",
      suggestedGroupSize: 20,
      foodIdeas: "Cake, snacks",
      activities: "Games",
      decorations: "Balloons",
      budgetLevel: "High",
      tags: "birthday,outdoor"
    }

    const created: EventTemplate = {
      id: 7,
      ...payload,
      createdAt: "2026-03-18T00:00:00Z"
    }

    vi.spyOn(api, "post").mockResolvedValue({ data: created } as AxiosResponse<EventTemplate>)

    const result = await templateService.createTemplate(payload)

    expect(api.post).toHaveBeenCalledWith("/api/eventtemplates", payload)
    expect(result).toEqual(created)
  })

  it("updates a template by id", async () => {
    const { templateService } = await import("@/services/templateService")
    const api = (await import("@/services/api")).default

    const template: EventTemplate = {
      id: 9,
      title: "Updated Template",
      category: "General",
      description: "Updated details",
      locationType: "Indoor",
      suggestedGroupSize: 5,
      foodIdeas: "Pizza",
      activities: "Trivia",
      decorations: "Lights",
      budgetLevel: "Low",
      tags: "small,indoor",
      createdAt: "2026-03-18T00:00:00Z"
    }

    vi.spyOn(api, "put").mockResolvedValue({} as AxiosResponse<void>)

    await templateService.updateTemplate(9, template)

    expect(api.put).toHaveBeenCalledWith("/api/eventtemplates/9", template)
  })

  it("deletes a template by id", async () => {
    const { templateService } = await import("@/services/templateService")
    const api = (await import("@/services/api")).default

    vi.spyOn(api, "delete").mockResolvedValue({} as AxiosResponse<void>)

    await templateService.deleteTemplate(4)

    expect(api.delete).toHaveBeenCalledWith("/api/eventtemplates/4")
  })
})
