import api from "@/services/api"

export interface SavedPlanListItem {
  id: string
  title: string
}

interface SavedPlanApiItem {
  id?: number
  title?: string
}

function toRouteIdFromTitle(title: string) {
  return encodeURIComponent(title.trim())
}

export async function getRecentSearches() {
  const res = await api.get<string[]>("/api/ai/recent-searches")
  return res.data
}

export async function getSavedPlans() {
  const res = await api.get<Array<string | SavedPlanApiItem>>("/api/ai/saved-plans")

  return res.data
    .map((item): SavedPlanListItem | null => {
      if (typeof item === "string") {
        const title = item.trim()

        if (!title)
          return null

        return {
          id: toRouteIdFromTitle(title),
          title
        }
      }

      if (!item || typeof item !== "object" || typeof item.title !== "string")
        return null

      const title = item.title.trim()

      if (!title)
        return null

      return {
        id: String(item.id ?? toRouteIdFromTitle(title)),
        title
      }
    })
    .filter((item): item is SavedPlanListItem => item !== null)
}