import { beforeEach, describe, expect, it, vi } from "vitest"

beforeEach(() => {
  vi.resetModules()
})

describe("useEventSearchState", () => {
  it("stores prompt and maps plans into saved event records", async () => {
    const { useEventSearchState } = await import("../../../composables/useEventSearchState")

    const state = useEventSearchState()

    state.setPrompt("Birthday ideas")
    state.setEvents([
      {
        title: "Beach Party",
        description: "Fun by the sea",
        budgetLevel: "Medium",
        suggestedGroupSize: 10,
        locationType: "Outdoor",
        foodIdeas: ["BBQ"],
        activities: ["Volleyball"],
        decorations: ["Lanterns"],
        score: 0.9
      }
    ])

    expect(state.searchPrompt.value).toBe("Birthday ideas")
    expect(state.events.value).toHaveLength(1)
    expect(state.events.value[0]?.id).toBe("beach-party-0")
    expect(state.events.value[0]?.saved).toBe(false)
  })

  it("updates and retrieves events by id", async () => {
    const { useEventSearchState } = await import("../../../composables/useEventSearchState")

    const state = useEventSearchState()

    state.setEvents([
      {
        title: "Game Night",
        description: "Cozy indoor event",
        budgetLevel: "Low",
        suggestedGroupSize: 6,
        locationType: "Indoor",
        foodIdeas: ["Pizza"],
        activities: ["Board Games"],
        decorations: ["Candles"],
        score: 0.8
      }
    ])

    state.updateEvent("game-night-0", {
      saved: true,
      title: "Updated Game Night"
    })

    const event = state.getEventById("game-night-0")

    expect(event?.saved).toBe(true)
    expect(event?.title).toBe("Updated Game Night")
  })
})
