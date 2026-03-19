import { createRouter, createWebHistory } from "vue-router"
import LandingPage from "@/pages/LandingPage.vue"
import Login from "@/pages/LoginPage.vue"
import Register from "@/pages/RegisterPage.vue"
import Dashboard from "@/pages/Dashboard.vue"
import PlanDetails from "@/pages/PlanDetails.vue"
import Profile from "@/pages/Profile.vue"
import AdminDashboard from "@/pages/AdminDashboard.vue"
import AdminOverview from "@/pages/AdminOverview.vue"
import TemplateManagement from "@/pages/TemplateManagement.vue"
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
      path: '/profile',
      component: Profile,
      meta: { requiresAuth: true }
    },
    {
      path: '/plan-details/:id?',
      name: 'plan-details',
      component: PlanDetails,
      meta: { requiresAuth: true }
    },
    {
      path: '/admin',
      component: AdminDashboard,
      redirect: '/admin/dashboard',
      children: [
        {
          path: 'dashboard',
          component: AdminOverview
        },
        {
          path: 'templates',
          component: TemplateManagement
        }
      ],
      meta: { requiresAuth: true, requiresAdmin: true }
    },
    
  ]
})

router.beforeEach((to) => {
  const auth = useAuth()
  const requiresAuth = to.matched.some(record => record.meta.requiresAuth)
  const requiresAdmin = to.matched.some(record => record.meta.requiresAdmin)

  if (to.path === '/login' && auth.isAuthenticated.value)
    return auth.isAdmin.value ? '/admin' : '/dashboard'

  if (requiresAuth && !auth.isAuthenticated.value)
    return '/login'

  if (requiresAdmin && !auth.isAdmin.value)
    return '/dashboard'

  return true
})

export default router