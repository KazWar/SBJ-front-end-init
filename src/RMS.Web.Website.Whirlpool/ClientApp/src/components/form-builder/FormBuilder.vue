<template>
    <div class="wrapper__form">
        <form class="uk-form-stacked">
            <template v-for="(item, index) in orderedSchema">
                <div ref="wrapperFormPage"
                     :class="'wrapper__form-page ' + 'page-' + (index + 1)"
                     :key="index">
                    <template v-for="(field, index) in item">
                        <component v-if="field.fieldType === 'DropdownMenu'"
                                   v-bind="field"
                                   :isRequired="field.required"
                                   :class="{'is-required': showIsRequired}"
                                   :key="index"
                                   :is="field.fieldType"
                                   :value="field.values"
                                   :isReadOnly="field.readonly"
                                   :options="field.formFieldValueList"
                                   autocomplete="nope"
                                   @input="updateForm(field.name, $event)">
                        </component>
                        <component v-else-if="field.fieldType === 'RetailerLocation' || field.fieldType === 'Product' || field.fieldType === 'RetailerRadioButton'"
                                   v-bind="field"
                                   :isRequired="field.required"
                                   :class="{'is-required': showIsRequired}"
                                   :key="index"
                                   :is="field.fieldType"
                                   :value="field.values"
                                   :isReadOnly="field.readonly"
                                   :options="field.dropdownList"
                                   autocomplete="nope"
                                   @input="updateForm(field.name, $event)">
                        </component>

                        <component v-else
                                   v-bind="field"
                                   :key="index"
                                   :isRequired="field.required"
                                   :class="{'is-required': showIsRequired}"
                                   :is="field.fieldType"
                                   :value="field.values"
                                   :isReadOnly="field.readonly"
                                   :options="field.formFieldValueList"
                                   autocomplete="nope"
                                   @input="updateForm(field.name, $event)">
                        </component>
                    </template>

                    <template>
                        <br />
                        <div v-if="missingFields.length === 0">
                            <template v-if="invalidItems.length > 0">
                                <p :key="Math.random() + item" class="requiredFields">{{ $t('registration.incompleteFields') }}</p>
                            </template>
                        </div>
                        <div>
                            <template v-if="missingFields.length != 0">
                                <p :key="Math.random() + item" class="requiredFields" v-html="missingFields" >
                                    {{ $t('registration.incompleteFields') }} {{ missingFields }}
                                </p>
                            </template>
                        </div>
                    </template>

                    <PageSeparator :pageNumber="index + 1"
                                   :pageCount="orderedSchema.length"
                                   :disableButton="disableButton.length > 0"
                                   :isDisabled="invalidItems.length > 0"
                                   @next="goToNextStep"
                                   @previous="goToPreviousStep" />
                </div>

            </template>
        </form>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Ref, Emit, Vue } from 'vue-property-decorator';
    import { Constants } from '@/constants';
    import companySettings from '../../$configs/settings.json';

    import CheckBox from '@/components/form-builder/CheckBox.vue';
    import DatePicker from '@/components/form-builder/DatePicker.vue';
    import DropdownMenu from '@/components/form-builder/DropdownMenu.vue';
    import RetailerLocation from '@/components/form-builder/RetailerLocation.vue';
    import FileUploader from '@/components/form-builder/FileUploader.vue';
    import IbanChecker from '@/components/form-builder/IbanChecker.vue';
    import InputNumber from '@/components/form-builder/InputNumber.vue';
    import InputPassword from '@/components/form-builder/InputPassword.vue';
    import InputText from '@/components/form-builder/InputText.vue';
    import PageSeparator from '@/components/form-builder/PageSeparator.vue';
    import RadioButton from '@/components/form-builder/RadioButton.vue';
    import Rating from '@/components/form-builder/Rating.vue';
    import Tile from '@/components/form-builder/Tile.vue';
    import Product from '@/components/form-builder/Product.vue';
    import RetailerRadioButton from '@/components/form-builder/RetailerRadioButton.vue';
    import UniqueCode from '@/components/form-builder/UniqueCode.vue';
    import HtmlText from '@/components/form-builder/HtmlText.vue';

    @Component({
        name: 'FormBuilder',
        components: {
            CheckBox,
            DatePicker,
            RetailerLocation,
            DropdownMenu,
            FileUploader,
            IbanChecker,
            InputNumber,
            InputPassword,
            InputText,
            PageSeparator,
            RadioButton,
            Rating,
            Tile,
            Product,
            RetailerRadioButton,
            UniqueCode,
            HtmlText,
        },
    })
    export default class FormBuilder extends Vue {
        @Prop() private formSchema!:
            | Array<{ fieldType: string; name: string; required: any; label: string; RegularExpression: string }>
            | { fieldType: string; name: string; required: any; label: string; RegularExpression: string }[];
        @Prop() private value!: {};

        @Ref('wrapperFormPage') public readonly wrapperFormPage!: HTMLDivElement[];
        @Prop() private isRequired!: {};
        @Prop() private pattern!: {};

        private formData: {};
        private dropDownObjects: {};
        private stepNumber: number;
        public finishedData: string;
        private invalidItems: string[];
        private disableButton: string[];
        private showIsRequired: boolean;
        campaignCode!: string;
        selectedLocale!: string;
        private regEx!: any;
        private readonly locale: string;
        missingFields!: Array<string>;
        private url: string;

        constructor() {
            super();
            this.formData = this.value || {};
            this.stepNumber = 0;
            this.finishedData = '';
            this.dropDownObjects = '';
            this.showIsRequired = false;
            this.invalidItems = [];
            this.disableButton = [];
            this.missingFields = [];
            this.regEx = '';
            this.locale = this.$parent.$route.params.locale;
            this.url = '';
        }

        mounted() {
            if (this.wrapperFormPage) {
                for (let i = 0; i < this.wrapperFormPage.length; i++) {
                    if (i === 0) {
                        continue;
                    }

                    this.wrapperFormPage[i].hidden = true;
                }
            }
        }

        get orderedSchema(): Array<{}> | {}[] | null {
            if (
                this.formSchema === undefined ||
                this.formSchema === null ||
                this.formSchema.length === 0
            ) {
                return null;
            }

            const listing: Array<{ fieldType: string }> = this.formSchema;
            const newListing: {}[] = [];
            let start = -1;

            for (let i = 0; i < listing.length; i++) {
                const pageSeparatorName = 'PageSeparator';
                if (!listing[i]) continue;
                const pos: boolean = listing[i].fieldType === pageSeparatorName;

                if (!pos) continue;
                if (pos && listing[i + 1].fieldType === pageSeparatorName) continue;
                if (pos && listing[i - 1].fieldType === pageSeparatorName) continue;

                let result: Array<{ fieldType: string }> = [];

                result = listing.filter((value, index) => {
                    if (index > start && index < i) {
                        return true;
                    }
                });

                start = i;

                newListing.push(result);
            }

            newListing.push(
                listing.filter((value, index) => {
                    if (index > start) {
                        return true;
                    }
                })
            );

            return newListing;
        }

        private finalizeRegistration(): void {
            this.disableButton.push("disable");
            const data = JSON.stringify({ ...this.formData });
            const jsonObj = JSON.parse(data);
            jsonObj.CampaignCode = this.$route.params.campaignCode;
            jsonObj.Locale = this.$route.params.locale;
            this.finishedData = JSON.stringify(jsonObj);
            console.log('data', data);

            let filledForm = this.finishedData;
            let formUrl = `/Registrations/SendFormData`;
            let sendMethod = 'POST';

            if (this.$route.params.registrationId != undefined) {
                jsonObj.registrationId = this.$route.params.registrationId;
                this.finishedData = JSON.stringify(jsonObj);
                filledForm = this.finishedData;
                formUrl = `/Registrations/UpdateFormData`;
                sendMethod = "PUT"

                for (let i = 0; i < this.formSchema.length; ++i) {
                    this.formSchema[i].required = false;
                }
            }

            this.missingFields = [];
            for (let i = 0; i < this.formSchema.length; ++i) {
                if (this.formSchema[i].name in jsonObj && this.formSchema[i].required == true) {
                    null;
                }
                else {
                    if (this.formSchema[i].required == false) {
                        null;
                    }
                    else {
                        this.missingFields.push(this.formSchema[i].label);
                        this.disableButton.length = 0;
                    }
                }
            }


            if (this.missingFields.length != 0) {
                //console.log(this.missingFields);
                const index = this.missingFields.indexOf("Pagina onderbreker");
                if (index > -1) {
                    this.missingFields.splice(index, 1);
                }

                return;
            }

            this.authenticateAndSendForm(filledForm, sendMethod);

            //this.setUniqueCodeAsUsed().then((response) => {
            //    //console.log('Result set used:', response.result);
            //    if (response.status === 500) {
            //        for (let i = 0; i < this.formSchema.length; ++i) {
            //            if (this.formSchema[i].name === "UniqueCode") {
            //                this.missingFields.push(this.formSchema[i].label);
            //                this.disableButton.length = 0;
            //            }
            //        }
            //        return
            //    }
            //    else if (response.status === 200) {
            //        this.authenticateAndSendForm(filledForm, sendMethod);
            //    }
            //});


        }


        async setUniqueCodeAsUsed() {

            const setUniqueCodesRequest = await fetch(`/Api/SetUniqueCode`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json', },
                body: JSON.stringify({
                    code: (this.formData as any).UniqueCode,
                }),
                credentials: 'omit',
                redirect: 'follow',
                mode: 'same-origin'
            });
            const setUniqueCodesResponse = await setUniqueCodesRequest;


            return setUniqueCodesResponse;
        }

        async authenticateAndSendForm(filledForm: any, sendMethod: any) {
            //console.log(filledForm);
            //console.log(sendMethod);


            if (sendMethod == "POST") {
                this.url = "/Api/SendForm";
            }
            else if (sendMethod == "PUT") {
                this.url = "/Api/UpdateForm";
            }
            console.log(this.url);

            const sendFormRequest = await fetch(this.url,
                {
                    method: sendMethod,
                    headers: {
                        'Access-Control-Allow-Origin': '*',
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    redirect: 'follow',
                    mode: 'same-origin',
                    body: JSON.stringify({ data: filledForm, })
                });
            const sendFormResponse = await sendFormRequest;
            //console.log('response', sendFormResponse);

            if (sendFormResponse.status == 200) {
                if (this.$route.params.registrationId != undefined) {
                    this.$router.push(`/${this.locale}/edit/thank-you`);
                } else {
                    this.$router.push(`/${this.locale}/thank-you`);
                }
            } else {
                if (this.$route.params.registrationId != undefined) {

                    this.$router.push(`/${this.locale}/edit/error-page`);
                } else {

                    this.$router.push(`/${this.locale}/error-page`);
                }
            }

            return sendFormResponse;
        }

        goToNextStep(stepNumber: number): void {
            if (stepNumber - 1 === this.wrapperFormPage.length) {
                this.finalizeRegistration();
                //console.log('this.finalizeRegistration');
                return;
            }

            this.wrapperFormPage[stepNumber - 2].hidden = true;
            this.wrapperFormPage[stepNumber - 1].hidden = false;
        }

        private goToPreviousStep(stepNumber: number): void {
            this.wrapperFormPage[stepNumber - 1].hidden = false;
            this.wrapperFormPage[stepNumber].hidden = true;
        }

        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        @Emit('updateForm') updateForm(fieldName: string, value: any): any {
            const formData = this.$set(this.formData, fieldName, value);
            //console.log('values', value);

            for (let i = 0; i < this.formSchema.length; ++i) {
                if (fieldName === this.formSchema[i].name) {
                    const regExpression = this.formSchema[i].RegularExpression;
                    let checkedValue = true;

                    if (regExpression != null) {
                        const regEx = new RegExp(this.formSchema[i].RegularExpression);
                        checkedValue = regEx.test(value);

                    }

                    if (checkedValue === false && !this.invalidItems.includes(fieldName)) {

                        this.invalidItems.push(fieldName);
                    }
                    else if (checkedValue === true) {
                        if (this.invalidItems.includes(fieldName)) {

                            const index = this.invalidItems.indexOf(fieldName);
                            this.invalidItems = this.invalidItems.filter((value, index) => this.invalidItems.indexOf(value) !== index);
                            if (index >= 0) {
                                this.invalidItems.splice(index, 1);
                            }
                        }
                    }
                }
            }

            if (!this.invalidItems.includes(fieldName)) {
                if (value === '' || value === true) {
                    this.invalidItems.push(fieldName);
                    //console.log('uhh', value);
                } else {
                    const index = this.invalidItems.indexOf(fieldName);
                    if (index >= 0) {
                        this.invalidItems.splice(index, 1);
                    }

                }
            } else if (value === false) {
                const index = this.invalidItems.indexOf(fieldName);
                if (index >= 0) {
                    this.invalidItems.splice(index, 1);
                }
            }


        }
    }
</script>

<style lang="scss">
    [class^='wrapper__']:not(:first-child) {
        margin-top: 2rem;
    }

    .requiredFields {
        color: red;
    }
</style>
