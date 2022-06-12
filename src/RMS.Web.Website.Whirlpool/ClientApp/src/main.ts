import Vue from 'vue';
import App from './App.vue';
import './registerServiceWorker';
import router from './router';
import moment from 'moment';
import VCalendar from 'v-calendar';
import Vuikit from 'vuikit';
import VuikitIcons from '@vuikit/icons';
import '@vuikit/theme';
import VueI18n from 'vue-i18n';
import { locales, defaultLocale } from '@/locales/index.js';
import settings from './$configs/settings.json';

const localizedMessages = Object.assign(locales);

Vue.config.productionTip = false;

Vue.filter('dateTime', (value: string) => {
    if (value) {
        return moment(String(value)).format('DD-MM-YYYY hh:mm');
    }
});

Vue.filter('date', (value: string) => {
    if (value) {
        return moment(String(value)).format('DD-MM-YYYY');
    }
});

Vue.use(Vuikit);
Vue.use(VuikitIcons);
Vue.use(VCalendar, {
    componentPrefix: 'vc',
    locales: {
        'nl-NL': {
            firstDayOfWeek: 1,
            masks: {
                L: 'DD-MM-YYYY',
            },
        },
    },
});
Vue.use(VueI18n);


const i18n = new VueI18n({
    locale: defaultLocale,
    fallbackLocale: 'en_gb',
    messages: localizedMessages,
});

new Vue({
    router,
    i18n,
    render: (h) => h(App),
}).$mount('#app');
