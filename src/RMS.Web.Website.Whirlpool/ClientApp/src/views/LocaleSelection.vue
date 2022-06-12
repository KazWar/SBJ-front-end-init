<template>
    <div class="wrapper__locale-selection-page">
        <h1>{{ $t('general.chooseLanguage') }}</h1>
    
        <LocaleSelection :options="options" @input="onSelectedItem($event)" />
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import LocaleSelection from '@/components/LocaleSelection.vue';



    @Component({
        name: 'LocaleSelectionPage',
        components: { LocaleSelection }
    })
    export default class LocaleSelectionPage extends Vue {
        private localeId: string;
        private selectedItem: string;
        private locale: string;
        private options: Array<{}>;
        private campaignData: Array<{}>;
        public authcode: Array<{ key: string; value: string }>;

        constructor() {
            super();
            this.localeId = '';
            this.locale = '';
            this.selectedItem = '';
            this.options = [];
            this.campaignData = [];
            this.authcode = [];
            this.onSelectedItem("nl_nl");
        }

        onSelectedItem(selectedLocale: string): void {
            this.setLocale(selectedLocale);
            this.$router.push(`/${selectedLocale}/campaign-overview`);
        }

        setLocale(selectedLocale: string): void {
            this.$parent.$i18n.locale = selectedLocale;
        }
    }
</script>

<style scoped lang="scss">
    .centered {
        height: 100%;
        max-width: 76.8rem;
        margin: 0 auto;
        padding: 0 2rem;
    }

    .wrapper__locale-selection-page {
        margin-top: 10rem;
    }
</style>
