import { createWebHistory, createRouter } from "vue-router";
import { RouteRecordRaw } from "vue-router";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    alias: "/home",
    name: "home",
    component: () => import("./components/Home.vue"),
  },
  {
    path: "/student/:hash_id",
    name: "student-details",
    component: () => import("./components/StudentDetails.vue"),
  },
  {
    path: "/add",
    name: "add",
    component: () => import("./components/AddStudent.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;