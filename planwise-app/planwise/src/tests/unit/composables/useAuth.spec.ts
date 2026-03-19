import { beforeEach, describe, expect, it, vi } from "vitest"

function createJwt(payload: Record<string, unknown>) {
  const header = { alg: "HS256", typ: "JWT" }
  const encode = (value: object) => btoa(JSON.stringify(value))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=+$/g, "")

  return `${encode(header)}.${encode(payload)}.signature`
}

beforeEach(() => {
  vi.resetModules()
})

describe("useAuth", () => {
  it("logs in and stores decoded auth state", async () => {
    const token = createJwt({
      exp: Math.floor(Date.now() / 1000) + 60,
      role: ["Admin"],
      name: "Test User",
      email: "test@example.com"
    })

    vi.stubGlobal("fetch", vi.fn(async () => ({
      ok: true,
      json: async () => ({
        accessToken: token,
        email: "test@example.com",
        roles: ["Admin"]
      })
    })))

    const { useAuth } = await import("../../../composables/useAuth")
    const auth = useAuth()

    await auth.login("test@example.com", "password123")

    expect(auth.isAuthenticated.value).toBe(true)
    expect(auth.isAdmin.value).toBe(true)
    expect(auth.userName.value).toBe("Test User")
    expect(auth.userEmail.value).toBe("test@example.com")
    expect(localStorage.getItem("token")).toBe(token)
    expect(auth.getAuthHeaders().Authorization).toBe(`Bearer ${token}`)
  })

  it("clears invalid stored token during module restore", async () => {
    localStorage.setItem("token", "invalid.token")
    localStorage.setItem("email", "old@example.com")
    localStorage.setItem("roles", JSON.stringify(["Admin"]))

    const { useAuth } = await import("../../../composables/useAuth")
    const auth = useAuth()

    expect(auth.isAuthenticated.value).toBe(false)
    expect(localStorage.getItem("token")).toBeNull()
    expect(localStorage.getItem("email")).toBeNull()
    expect(localStorage.getItem("roles")).toBeNull()
  })
})
