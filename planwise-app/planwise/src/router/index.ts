import { createRouter, createWebHistory } from "vue-router"
import LandingPage from "@/pages/LandingPage.vue"
import Login from "@/pages/LoginPage.vue"
import Register from "@/pages/RegisterPage.vue"
import Dashboard from "@/pages/Dashboard.vue"

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      component: LandingPage
    },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
    { path: '/dashboard', component: Dashboard },
  ]
})

export default router