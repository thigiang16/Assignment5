import { createRouter, createWebHistory } from "vue-router"
import LandingPage from "@/pages/LandingPage.vue"
import Login from "@/pages/LoginPage.vue"
import Register from "@/pages/RegisterPage.vue"
import Dashboard from "@/pages/Dashboard.vue"
import AdminDashboard from "@/pages/AdminDashboard.vue"
import { useAuth } from "@/composables/useAuth"

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      component: LandingPage
    },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
    {
      path: '/dashboard',
      component: Dashboard,
      meta: { requiresAuth: true }
    },
    {
      path: '/admin',
      component: AdminDashboard,
      meta: { requiresAuth: true, requiresAdmin: true }
    },
  ]
})

router.beforeEach((to) => {
  const auth = useAuth()
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const requiresAdmin = to.matched.some(record => record.meta.requiresAdmin)

  if (requiresAuth && !auth.isAuthenticated.value)
    return '/login'

  if (requiresAdmin && !auth.isAdmin.value)
    return '/dashboard'

  return true
})

export default router