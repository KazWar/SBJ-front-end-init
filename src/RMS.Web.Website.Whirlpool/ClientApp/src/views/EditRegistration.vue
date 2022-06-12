<template>
    <div class="wrapper__edit-registration">
        <div class="centered">
            <template v-if="loading">
                <p>{{ $t('general.loading') }}</p>
            </template>
            <template v-else-if="!this.loading && this.showForm == true">
                <form-builder v-model="formData"
                              :formSchema="campaignData"></form-builder>
            </template>
            <template v-else>
                <div class="edit-registration__enter-password">
                    <h1>{{ $t('registration.edit') }}</h1>
                    <label class="uk-form-label">{{ $t('registration.enterPassword') }}</label>
                    <input :id="'form-stacked-text__get-registration'"
                           class="btn btn-lg uk-input"
                           type="text"
                           v-model="password" />

                    <button class="no-select page-separator__next uk-button uk-button-primary button-margin" type="primary" :disabled="!password" @click="goToRegistration(password)">
                        {{ $t('general.next') }}
                    </button>
                </div>
            </template>
        </div>
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import { Constants } from '@/constants';
    import Campaign from '@/shared/models/campaign.interface';
    import FormBuilder from '@/components/form-builder/FormBuilder.vue';
    import LocaleSelection from '../components/LocaleSelection.vue';
    import InputText from '@/components/form-builder/InputText.vue';

    @Component({
        name: 'EditRegistration',
        components: { LocaleSelection, FormBuilder, InputText },
    })
    export default class EditRegistrationCheck extends Vue {
        private readonly locale: string;
        private readonly currentDate: Date;
        private readonly password: string;
        private readonly registrationId: string;
        private loading: boolean;
        private showForm: boolean;
        private campaignData: any = [];
        public formData: {}[] = [];
        private promiseData: any;

        constructor() {
            super();
            this.loading = false;
            this.showForm = false;

            this.locale = this.$parent.$route.params.locale;
            if (this.locale) {
                this.setLocale(this.locale);
            }
            this.registrationId = this.$parent.$route.params.registrationId;
            this.currentDate = new Date();
            this.campaignData = [];
            this.formData = [];
            this.password = '';
            this.promiseData = null;

        }

        setLocale(locale: string): void {
            this.$parent.$i18n.locale = locale;
        }
        setLocaleId(localeId: string): void {
            this.$parent.$i18n.locale = localeId;
        }


        async getEditForRegistration(input: {}): Promise<any> {
            this.getEditForm(input).then(data => {
                const parsedData = JSON.parse(data);
                const fieldBlocks = [];
                for (let i = 0; i < parsedData.result.formbuilder.exportBlocks.length; i++) {
                    const ArrayObjects = parsedData.result.formbuilder.exportBlocks[i].exportFields;
                    for (let j = 0; j < ArrayObjects.length; j++) {
                        fieldBlocks.push(ArrayObjects[j]);
                    }
                }
                this.campaignData.push(...fieldBlocks);
                this.loading = false;
                this.showForm = true;
                return fieldBlocks;
            });
        }

        async getEditForm(input: {}) {
            const url = `/Api/GetEditForm`;
            const getUpdateFormRequest = await fetch(url, {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Access-Control-Allow-Origin': '*',
                    'Content-Type': 'application/json'
                },
                redirect: 'follow',
                mode: 'same-origin',
                body: JSON.stringify(input)
            });
            const getUpdateFormResponse = await getUpdateFormRequest.json();

            return getUpdateFormResponse;
        }

        goToRegistration(password: string): void {
            this.loading = true;
            this.getEditForRegistration({
                registrationId: Number(this.registrationId), password: password, locale: this.locale,
            });            
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
  border-radius: 15px;
  display: grid;
  grid-template-columns: 1fr 1fr;
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

.button-margin {
    margin-top: 2rem;
}
</style>
