<template>
    <div class="wrapper__campaign-overview" v-if="this.campaignData != 0">
        <div class="centered">
            <div class="wrapper__campaign-overview--campaigns-available">
                <h1>{{ $t('campaigns.availableCampaigns') }}</h1>

                <template v-for="campaign in campaigns">
                    <div :key="campaign.campaignId + Math.random()"
                        v-if="currentDate <= addMonth(campaign.endDate) && currentDate >= addMonth(campaign.startDate)" 
                         class="wrapper__campaign-tile"                          
                         @click="goToCampaign(campaign.campaignCode)">                        
                        <div class="campaign-tile__details">
                            <h2>{{ campaign.campaignName }}</h2>
                            <p v-html="campaign.campaignDescription">{{ campaign.campaignDescription }}</p>

                            <!--v-if="currentDate < addMonth(campaign.endDate) && currentDate > addMonth(campaign.startDate)"-->
                            <!--<p v-if="campaign.localeDescription">{{ campaign.localeDescription }}</p>
                                          <p>campaignId: {{ campaign.campaignId }}</p>
                                          <p>campaignCode: {{ campaign.campaignCode }}</p>
                                          <p>Version: {{ campaign.version }}</p>-->
                        </div>
                        <div v-if="campaign.campaignThumbnail" class="campaign-tile__thumbnail">
                            <img :src="campaign.campaignThumbnail" />
                        </div>
                    </div>
                </template>
            </div>

            <!--<div class="wrapper__campaign-overview--campaigns-expired">-->
            <!--<h1>{{ $t('campaigns.expiredCampaigns') }}</h1>-->
            <!--<template v-for="campaign in campaigns">
                <div :key="campaign.campaignId + Math.random()"
                     class="wrapper__campaign-tile"
                     v-if="currentDate > addMonth(campaign.endDate)">

                    <div class="campaign-tile__details">-->
            <!--<h2>{{ campaign.campaignName }}</h2>
            <p>{{ campaign.campaignDescription }}</p>

            <p v-if="campaign.localeDescription">{{ campaign.localeDescription }}</p>
                  <p>campaignId: {{ campaign.campaignId }}</p>
                  <p>campaignCode: {{ campaign.campaignCode }}</p>
                  <p>Version: {{ campaign.version }}</p>-->
            <!--</div>
            <div v-if="campaign.campaignThumbnail" class="campaign-tile__thumbnail">-->
            <!--<img :src="campaign.campaignThumbnail" />-->
            <!--</div>
                </div>
            </template>-->
            <!--</div>-->
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import Campaign from '@/shared/models/campaign.interface';
    import LocaleSelection from '../components/LocaleSelection.vue';
    import companySettings from '../$configs/settings.json';
    import { Constants } from '@/constants';

    @Component({
        name: 'CampaignOverview',
        components: { LocaleSelection },
    })
    export default class CampaignOverview extends Vue {
        private readonly locale: string;
        private readonly campaigns: Campaign[];
        private readonly currentDate: Date;
        private readonly campaignData: Campaign[];

        constructor() {
            super();
            if (this.$parent.$route.name === "CampaignOverview") {
                document.getElementById('header-img')!.style.backgroundImage = "url(https://rms2public.blob.core.windows.net/whirlpool/basic-header.jpg)";
            }

            this.currentDate = new Date();
            this.campaignData = [];
            this.locale = this.$parent.$route.params.locale;
            if (this.locale) {
                this.setLocale(this.locale);
            }

            this.getCampaignOverview().then(data => {
                const parsedData = JSON.parse(data);
                //console.log(parsedData);
                const resultArray = [];
                for (let i = 0; i < parsedData.result.length; i++) {
                    if (parsedData.result[i].localeDescription == this.locale) {
                        const ArrayObjects = parsedData.result[i];
                        resultArray.push(ArrayObjects);
                        this.campaignData.push(ArrayObjects);
                    }
                }

            });

            this.campaigns = this.campaignData;
        }

        async getCampaignOverview() {
            const locale = this.$parent.$route.params.locale;
            const campaignOverviewRequest = await fetch(`/Api/GetCampaignOverview?currentLocale=${locale}`, { method: 'GET', headers: { 'Content-Type': 'application/json', }, credentials: 'include', redirect: 'follow', mode: 'same-origin' });
            const campaignOverviewResponse = await campaignOverviewRequest.json();
            //console.log('campaignOverviewResponse:', campaignOverviewResponse);

            return campaignOverviewResponse;
        }

        private setLocale(locale: string): void {
            // this.$parent.$i18n.locale = locale;
        }
        private setLocaleId(localeId: string): void {
            // this.$parent.$i18n.locale = locale;
        }

        private goToCampaign(campaignCode: number): void {
            this.$router.push(`/${this.locale}/registrations/${campaignCode}`);
        }

        private addMonth(date: any) {
            const orginalDate = date.split(/[-+ :T]/);
            const newDate = new Date(orginalDate[0], orginalDate[1] - 1, orginalDate[2], orginalDate[3], orginalDate[4], orginalDate[5]);

            let yyyy = newDate.getFullYear();
            let mm = newDate.getMonth();// + 1;
            let dd = newDate.getDate();

            if (mm <= 11) {
                mm += 1;
            }
            else {
                mm = 1;
                dd = 1;
                yyyy += 1;
            }

            const dateNotation = yyyy + '/' + mm + '/' + dd;
            const goodDate = new Date(dateNotation);

            return goodDate;
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

    .wrapper__campaign-tile {
        border: 1px solid transparent;
        background: #f0f0f0;
        //border-radius: 15px;
        display: grid;
        grid-template-columns: 1fr 300px;

        &:hover {
            cursor: pointer;
            border: 1px solid #f0f0f0;
        }

        .campaign-tile__thumbnail {
            max-height: inherit;

            img {
                max-height: inherit;
            }
        }

        h1,
        h2,
        h3,
        h4,
        h5,
        h6 {
            font-weight: bold;
            color: var(--black);
        }
    }

    .wrapper__campaign-overview--campaigns-available {
        padding-bottom: 20px;
    }

    .campaign-tile__details {
        padding: 3rem;
    }

    @media (max-width: 769px) {
        .wrapper__campaign-tile {
            grid-template-columns: 1fr;
            max-width:500px!important;
        }



        .campaign-tile__thumbnail {
            //max-height: 350px !important
        }
    }
</style>
