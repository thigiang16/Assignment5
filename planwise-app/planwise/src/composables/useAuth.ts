import { ref, computed } from "vue"

const accessToken = ref("")
const roles = ref<string[]>([])
const TOKEN_KEY = "token"

const API = "http://localhost:5080"

function parseJwt(token: string) {
  const base64Url = token.split(".")[1]
  if (!base64Url)
    throw new Error("Invalid JWT")

  const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/")
  return JSON.parse(atob(base64))
}

function extractRole(payload: Record<string, unknown>) {
  const role = payload.role
    ?? payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]

  return typeof role === "string" ? role : ""
}

function restoreAuthState() {
  const savedToken = localStorage.getItem(TOKEN_KEY)

  if (!savedToken)
    return

  try {
    accessToken.value = savedToken

    const decoded = parseJwt(savedToken)
    const role = extractRole(decoded)

    roles.value = role ? [role] : []
  } catch {
    accessToken.value = ""
    roles.value = []
    localStorage.removeItem(TOKEN_KEY)
  }
}

restoreAuthState()

export function useAuth() {

  const isAuthenticated = computed(() => !!accessToken.value)
  const isAdmin = computed(() => roles.value.includes("Admin"))

  const login = async (email: string, password: string) => {

    const res = await fetch(`${API}/api/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ email, password })
    })

    if (!res.ok)
      throw new Error("Login failed")

    const data = await res.json()

    accessToken.value = data.accessToken
    localStorage.setItem(TOKEN_KEY, accessToken.value)

    const decoded = parseJwt(accessToken.value)
    const role = extractRole(decoded)
    roles.value = role ? [role] : []
  }

  const register = async (name: string, email: string, password: string) => {

    const res = await fetch(`${API}/api/auth/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ name, email, password })
    })

    if (!res.ok)
      throw new Error("Register failed")
  }

  const logout = () => {
    accessToken.value = ""
    roles.value = []
    localStorage.removeItem(TOKEN_KEY)
  }

  const getAuthHeaders = () => ({
    Authorization: `Bearer ${accessToken.value}`
  })

  return {
    login,
    register,
    logout,
    isAuthenticated,
    isAdmin,
    getAuthHeaders
  }
}