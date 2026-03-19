import { beforeEach, describe, expect, it, vi } from "vitest"

vi.mock("@/pages/LandingPage.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/LoginPage.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/RegisterPage.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/Dashboard.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/PlanDetails.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/Profile.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/AdminDashboard.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/AdminOverview.vue", () => ({ default: { template: "<div />" } }))
vi.mock("@/pages/TemplateManagement.vue", () => ({ default: { template: "<div />" } }))

function createJwt(payload: Record<string, unknown>) {
  const header = { alg: "HS256", typ: "JWT" }
  const encode = (value: object) => btoa(JSON.stringify(value))
    .replace(/\+/g, "-")
    .replace(/\//g, "_")
    .replace(/=+$/g, "")

  return `${encode(header)}.${encode(payload)}.signature`
}

beforeEach(() => {
  localStorage.clear()
  vi.resetModules()
})

describe("admin route guards", () => {
  it("redirects unauthenticated users away from admin routes", async () => {
    const router = (await import("@/router")).default

    await router.push("/admin/templates")
    await router.isReady()

    expect(router.currentRoute.value.fullPath).toBe("/login")
  })

  it("redirects non-admin authenticated users from admin routes", async () => {
    const token = createJwt({
      exp: Math.floor(Date.now() / 1000) + 120,
      role: ["User"],
      name: "Regular User",
      email: "user@example.com"
    })

    localStorage.setItem("token", token)

    const router = (await import("@/router")).default

    await router.push("/admin")
    await router.isReady()

    expect(router.currentRoute.value.fullPath).toBe("/dashboard")
  })

  it("allows admin users to access admin template route", async () => {
    const token = createJwt({
      exp: Math.floor(Date.now() / 1000) + 120,
      role: ["Admin"],
      name: "Admin User",
      email: "admin@example.com"
    })

    localStorage.setItem("token", token)

    const router = (await import("@/router")).default

    await router.push("/admin/templates")
    await router.isReady()

    expect(router.currentRoute.value.fullPath).toBe("/admin/templates")
  })
})
