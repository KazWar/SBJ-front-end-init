<template>
    <div class="wrapper__locale-selection">
        <Loading v-if="this.loaded != true" />
        <multiselect v-if="this.options != 0"
                     v-model="internalValue"
                     track-by="Name"
                     label="Name"
                     open-direction="bottom"
                     :multiple="multi"
                     :options="options"
                     :searchable="searchable"
                     :placeholder="placeholder"
                     :close-on-select="closeOnSelect"
                     :clear-on-select="clearOnSelect"
                     @input="onChange($event)">
        </multiselect>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Emit, Vue } from 'vue-property-decorator';
    import Multiselect from 'vue-multiselect';
    import { Constants } from '@/constants';
    import Loading from '../components/form-builder/Loading.vue';
    import moment from 'moment';
    import settings from '../$configs/settings.json';

    @Component({
        name: 'LocaleSelection',
        components: { Multiselect, Loading },
    })
    export default class LocaleSelection extends Vue {
        @Prop() private multi!: boolean;
        @Prop() private searchable!: boolean;
        @Prop() private placeholder!: string;
        @Prop() private closeOnSelect!: boolean;
        @Prop() private clearOnSelect!: boolean;
        @Prop() private options!: Array<{}>;

        private internalValue!: Array<{}>;
        private locale!: string;
        private campaignData!: Array<{}>;
        private loaded: boolean;

        constructor() {
            super();
            this.loaded = false;
        }

        data() {
            return {
                loaded: Boolean,
            };
        }

        async authenticateAndGetCampaignLocales() {

            const campaignLocalesRequest = await fetch(`/Api/GetLocales`, { method: 'GET', credentials: 'include', redirect: 'follow', mode: 'same-origin' });

            const campaignLocalesResponse = await campaignLocalesRequest.json();

            //console.log('CampaignLocalesResponse:', campaignLocalesResponse);

            return campaignLocalesResponse;
        }

        async mounted() {
            this.internalValue = [];
            this.locale = '';
            this.campaignData = [];
            this.loaded = false;
            let authorization: any;
            const campaignsArray: [] = [];

            this.authenticateAndGetCampaignLocales().then(campaignLocales => {
                //console.log('Then, campaign', campaignLocales);
                //console.log('parsed', JSON.parse(campaignLocales));
                const parsedLocales = JSON.parse(campaignLocales);

                const campaignsArray = [];
                for (let i = 0; i < parsedLocales.result.length; i++) {
                    const ArrayObjects = parsedLocales.result[i].localeDescription;
                    const localeName = parsedLocales.result[i].localeName;
                    const localeObj = { Name: localeName, Description: ArrayObjects };

                    //campaignsArray.push(ArrayObjects);
                    campaignsArray.push(localeObj);
                }

                const seen = new Set();
                const filteredArr = campaignsArray.filter(el => {
                    const duplicate = seen.has(el.Description);
                    seen.add(el.Description);
                    return !duplicate;
                });

                this.options.push(...filteredArr)
                this.loaded = true;

                return this.campaignData;
            });

        }

        @Emit('input') onChange(event: {
            campaignData: Array<[]>;
            locale: string;
            Description: any;
        }): { campaignData: Array<[]>; locale: string; Description: any } {
            this.internalValue = this.campaignData;
            return event.Description;
        }
    }
</script>

<style scoped lang="scss">
    @import '../../node_modules/vue-multiselect/dist/vue-multiselect.min.css';

    .multiselect {
        cursor: pointer;
    }
</style>
