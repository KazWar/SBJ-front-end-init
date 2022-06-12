import Vue from 'vue';
import VueRouter, { RouteConfig } from 'vue-router';
import Home from '../views/Home.vue';

Vue.use(VueRouter);

const routes: Array<RouteConfig> = [
    {
        path: '/',
        name: 'Home',
        component: Home,
    },
    {
        path: '/locale-selection',
        name: 'LocaleSelectionPage',
        component: () => import('../views/LocaleSelection.vue'),
    },
    {
        path: '/:locale/campaign-overview/',
        name: 'CampaignOverview',
        component: () => import('../views/CampaignOverview.vue'),
    },
    {
        path: '/:locale/registrations/:campaignCode',
        name: 'RegistrationForm',
        component: () => import('../views/RegistrationForm.vue'),
    },
    {
        path: '/:locale/registrations/:registrationId/edit',
        name: 'EditRegistrationCheck',
        component: () => import('../views/EditRegistration.vue'),
    },
    {
        path: '/:locale/thank-you',
        name: 'ThankYou',
        component: () => import('../views/ThankYou.vue'),
    },
    {
        path: '/:locale/edit/thank-you',
        name: 'ThankYou',
        component: () => import('../views/ThankYou.vue'),
    },
    {
        path: '/:locale/error-page',
        name: 'Error',
        component: () => import('../views/Error.vue'),
    },
    {
        path: '/:locale/edit/error-page',
        name: 'Error',
        component: () => import('../views/Error.vue'),
    },
];

const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes,
});

export default router;
