<template>
    <div id="wrapper__sbj-form-builder-demo">
        <div v-if="this.expiredCampaign === true">
            <h2> {{ $t('campaigns.expiredMessage') }}</h2>
            <button class="main-button"
                    onclick="window.location.href='/locale-selection'">
                {{ $t('general.goBack') }}
            </button>
        </div>
        <Loading v-if="this.loaded != true" />
        <div v-else-if="this.expiredCampaign === false">
            <h1>{{ $t('registration.registrationForm') }}</h1>
            <p class="headings__sub-heading" v-html="$t('registration.message')">
                <strong> {{ $t('registration.title') }} </strong><br />
                {{ $t('registration.message') }}
            </p>


            <form-builder v-model="formData" :formSchema="formSchema" v-if="this.formSchema !=0"></form-builder>
        </div>

    </div>
</template>
  
<script lang="ts">
    import { Component, Prop, Vue } from 'vue-property-decorator';
    import FormBuilder from '@/components/form-builder/FormBuilder.vue';
    import { Constants } from '@/constants';
    import companySettings from '../$configs/settings.json';
    import Loading from '@/components/form-builder/Loading.vue';
    import dummyData from '@/assets/json/data.json';


    @Component({
        name: 'RegistrationForm',
        components: { FormBuilder, Loading },
    })
    export default class RegistrationForm extends Vue {
        @Prop() private msg!: string;
        public formData: {}[] = [];
        public formSchema: any = [];
        public formLocaleId!: number;
        locale!: string;
        private currentDate: any;

        campaignId!: number;
        campaignCode!: string;
        checkMode!: any;
        expiredCampaign!: any;

        private loaded: boolean;
        constructor() {
            super();
            this.loaded = false;
            this.expiredCampaign = '';
        }

        data() {
            return {
                loaded: Boolean
            }
        }

        async getCampaignForForm() {
            const locale = this.$parent.$route.params.locale;
            const campaignCode = this.$route.params.campaignCode;
            const url = `/Api/GetCampaignForForm?currentLocale=${locale}&currentCampaignCode=${campaignCode}`;
            const getCampaignForFormRequest = await fetch(url, { method: 'GET', credentials: 'include', redirect: 'follow', mode: 'same-origin' });
            const getCampaignForFormResponse = await getCampaignForFormRequest.json();

            return getCampaignForFormResponse;
        }

        async getFormAndProductHandling(currentCampaignId: any) {
            const locale = this.$parent.$route.params.locale;
            const url = `/Api/GetFormAndProductHandling?currentLocale=${locale}&currentCampaignId=${currentCampaignId}`;
            const getCampaignForFormRequest = await fetch(url, { method: 'GET', credentials: 'include', redirect: 'follow', mode: 'same-origin' });
            const getCampaignForFormResponse = await getCampaignForFormRequest.json();

            return getCampaignForFormResponse;
        }

        async mounted() {
            this.loaded = false;
            this.formLocaleId = 0;
            this.locale = this.$route.params.locale;
            this.campaignCode = this.$route.params.campaignCode;

            setTimeout(() => {
                this.getCampaignForForm().then(data => {
                    const parsedData = JSON.parse(data);

                    if (this.$route.params.registrationId === undefined) {
                        console.log('parsedData', parsedData);
                        const orginalDate = parsedData.result.campaignEndDate.split(/[-+ :T]/);
                        const newDate = new Date(orginalDate[0], orginalDate[1] - 1, orginalDate[2], orginalDate[3], orginalDate[4], orginalDate[5]);

                        const imgurl = parsedData.result.bannerImagePath;
                        console.log(imgurl);
                        if (imgurl != null) {
                            document.getElementById('header-img')!.style.backgroundImage = "url(" + imgurl + ")";
                        }

                        let yyyy = newDate.getFullYear();
                        let mm = newDate.getMonth();
                        let dd = newDate.getDate();
                        if (mm <= 11) {
                            mm += 1;
                        }
                        else {
                            mm = 1;
                            dd = 1;
                            yyyy += 1;
                        }

                        const addedMonth = yyyy + '/' + mm + '/' + dd;
                        const goodDate = new Date(addedMonth);
                        this.currentDate = new Date();
                        this.expiredCampaign = this.currentDate > goodDate;
                        if (this.expiredCampaign === true) {
                            this.loaded = true;
                            return;
                        }
                    }

                    this.campaignId = parsedData.result.campaignId;

                    this.getFormAndProductHandling(this.campaignId).then(data => {
                        const parsedData = JSON.parse(data);

                        const fieldBlocks = [];
                        for (let i = 0; i < parsedData.formbuilder.exportBlocks.length; i++) {
                            const ArrayObjects = parsedData.formbuilder.exportBlocks[i].exportFields;
                            for (let j = 0; j < ArrayObjects.length; j++) {
                                fieldBlocks.push(ArrayObjects[j]);
                            }
                        }
                        this.formSchema.push(...fieldBlocks);

                        return fieldBlocks;
                    });
                });
                this.loaded = true;
            }, 500);
        }
    }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="scss">
    #wrapper__sbj-form-builder-demo {
        max-width: 76.8rem;
        margin: 0 auto;
        padding: 0 2rem;
    }

    .headings__sub-heading {
        margin-bottom: 0;
        white-space: pre-line;
    }

    .main-button {
        margin-left: 5px;
        margin-right: 20px;
        margin-top: 20px;
        margin-bottom: 20px;
        overflow: visible;
        font: inherit;
        display: inline-block;
        box-sizing: border-box;
        padding: 0 30px;
        vertical-align: middle;
        font-size: 14px;
        line-height: 38px;
        cursor: pointer;
        background: var(--sage-purple) !important;
        border-radius: 50rem !important;
        border: none;
        color: white !important;
        text-transform: uppercase;
        transition: 0.1s ease-in-out;
        transition-property: color, background-color;
    }
</style>