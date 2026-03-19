import { ref } from "vue"
import type { GeneratedEventPlan } from "@/services/aiService"

export type EventSearchResult = GeneratedEventPlan & {
  id: string
  saved?: boolean
}

const searchPrompt = ref("")
const events = ref<EventSearchResult[]>([])

function buildEventId(plan: GeneratedEventPlan, index: number): string {
  const slug = plan.title
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")

  return `${slug || "plan"}-${index}`
}

export function useEventSearchState() {
  function setPrompt(value: string) {
    searchPrompt.value = value
  }

  function setEvents(plans: GeneratedEventPlan[]) {
    events.value = plans.map((plan, index) => ({
      ...plan,
      id: buildEventId(plan, index),
      saved: false
    }))
  }

  function updateEvent(eventId: string, patch: Partial<EventSearchResult>) {
    const target = events.value.find(event => event.id === eventId)

    if (!target)
      return

    Object.assign(target, patch)
  }

  function getEventById(eventId: string) {
    return events.value.find(event => event.id === eventId)
  }

  return {
    searchPrompt,
    events,
    setPrompt,
    setEvents,
    updateEvent,
    getEventById
  }
}
