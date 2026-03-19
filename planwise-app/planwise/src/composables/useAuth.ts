import { ref, computed } from "vue"
import { API_BASE } from "@/services/api"

const accessToken = ref("")
const roles = ref<string[]>([])
const userName = ref("")
const userEmail = ref("")
const TOKEN_KEY = "token"
const EMAIL_KEY = "email"
const ROLES_KEY = "roles"

function parseJwt(token: string) {
  const base64Url = token.split(".")[1]
  if (!base64Url)
    throw new Error("Invalid JWT")

  const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/")
  return JSON.parse(atob(base64))
}

function clearAuthState() {
  accessToken.value = ""
  roles.value = []
  userName.value = ""
  userEmail.value = ""
}

function clearStoredAuth() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(EMAIL_KEY)
  localStorage.removeItem(ROLES_KEY)
}

function extractRole(payload: Record<string, unknown>) {
  const rawRoles = payload.role
    ?? payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]

  if (Array.isArray(rawRoles)) {
    return rawRoles.filter((value): value is string => typeof value === "string")
  }

  if (typeof rawRoles === "string") {
    return [rawRoles]
  }

  return []
}

function extractEmail(payload: Record<string, unknown>) {
  const email = payload.email
    ?? payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]

  return typeof email === "string" ? email : ""
}

function extractName(payload: Record<string, unknown>) {
  const name = payload.name
    ?? payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]

  return typeof name === "string" ? name : ""
}

function isTokenValid(token: string): boolean {
  try {
    const payload = parseJwt(token)

    if (typeof payload.exp !== "number")
      return true

    return payload.exp * 1000 > Date.now()
  } catch {
    return false
  }
}

function restoreAuthState() {
  const savedToken = localStorage.getItem(TOKEN_KEY)

  if (!savedToken)
    return

  if (!isTokenValid(savedToken)) {
    clearAuthState()
    clearStoredAuth()
    return
  }

  try {
    accessToken.value = savedToken

    const decoded = parseJwt(savedToken)
    const decodedRoles = extractRole(decoded)

    userEmail.value = localStorage.getItem(EMAIL_KEY) || extractEmail(decoded)
    userName.value = extractName(decoded)
    roles.value = decodedRoles.length
      ? decodedRoles
      : JSON.parse(localStorage.getItem(ROLES_KEY) || "[]")

    if (userEmail.value)
      localStorage.setItem(EMAIL_KEY, userEmail.value)

    localStorage.setItem(ROLES_KEY, JSON.stringify(roles.value))
  } catch {
    clearAuthState()
    clearStoredAuth()
  }
}

restoreAuthState()

export function useAuth() {

  const isAuthenticated = computed(() => !!accessToken.value && isTokenValid(accessToken.value))
  const isAdmin = computed(() => roles.value.includes("Admin"))

  const login = async (email: string, password: string) => {

    const res = await fetch(`${API_BASE}/api/auth/login`, {
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
    const decodedRoles = extractRole(decoded)

    userEmail.value = data.email || extractEmail(decoded) || email
    userName.value = extractName(decoded)
    roles.value = decodedRoles.length ? decodedRoles : Array.isArray(data.roles) ? data.roles : []

    localStorage.setItem(EMAIL_KEY, userEmail.value)
    localStorage.setItem(ROLES_KEY, JSON.stringify(roles.value))
  }

  const register = async (name: string, email: string, password: string) => {

    const res = await fetch(`${API_BASE}/api/auth/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ name, email, password })
    })

    if (!res.ok)
      throw new Error("Register failed")
  }

  const logout = async () => {
    clearAuthState()
    clearStoredAuth()

    const { default: router } = await import("@/router")

    if (router.currentRoute.value.path !== "/login") {
      router.push("/login")
    }
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
    userName,
    userEmail,
    getAuthHeaders
  }
}