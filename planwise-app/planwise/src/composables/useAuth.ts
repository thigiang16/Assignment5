import { ref, computed } from "vue"

const accessToken = ref("")
const roles = ref<string[]>([])

const API = "http://localhost:5080"

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

    localStorage.setItem("token", accessToken.value)
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
    localStorage.removeItem("token")
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