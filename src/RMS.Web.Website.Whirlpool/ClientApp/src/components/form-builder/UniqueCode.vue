<template>
    <div class="wrapper__unique-code">
        <label class="uk-form-label" :for="'form-stacked-unique-code__' + name" v-html="label">
            {{
      label
            }}
        </label>

        <div :class="`uk-inline ${this.className}`">
            <vk-icon icon="lock" :class="`uk-form-icon`"></vk-icon>
            <input :id="'form-stacked-unique-code__' + name"
                   class="uk-input input__unique-code"
                   type="text"
                   :name="name"
                   :value="value"
                   :disabled="isReadOnly"
                   :placeholder="this.checkedCode"
                   @input="onInput($event.target.value)" />
        </div>

        <vk-button class="no-select button__validate-unique-code"
                   :disabled="isDisabledButton"
                   type="primary"
                   @click="disableButtonAndCheckCode()">{{ $t('general.validateUniqueCode') }}</vk-button>

        <p :class="{
        danger: showWarningInvalidCode,
        hidden: !showWarningInvalidCode,
      }">
            {{ $t('general.unknownUniqueCode') }}
        </p>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Emit, Vue } from 'vue-property-decorator';
    import { Constants } from '@/constants';

    @Component({
        name: 'UniqueCode',
    })
    export default class InputText extends Vue {
        @Prop() private name!: string;
        @Prop() private label!: string;
        @Prop() private value!: string;
        @Prop() private placeholder!: string;
        @Prop() private className!: string;
        @Prop() private isReadOnly!: boolean;


        isDisabledButton: boolean;
        showWarningInvalidCode: boolean;
        private internalValue: any;
        private checkedCode: any;

        constructor() {
            super();
            this.isDisabledButton = false;
            this.showWarningInvalidCode = false;
            this.internalValue = '';
            this.checkedCode = '';
        }

        @Emit('input') onInput(value: string): string {

            this.internalValue = value;
            //console.log('code', this.internalValue);

            return value;
        }

        async checkCodeValidity() {
            console.log("checkCodeValidity");
            const code = this.internalValue;
            const uniqueCodesRequest = await fetch(`/Api/UniqueCode?code=${code}`, { method: 'GET', headers: { 'Content-Type': 'application/json', }, credentials: 'omit', redirect: 'follow', mode: 'same-origin' });
            const uniqueCodesResponse = await uniqueCodesRequest;

            this.checkedCode = this.internalValue;

            return [uniqueCodesResponse];
        }

        disableButtonAndCheckCode(): void {
            this.isDisabledButton = true;

            this.checkCodeValidity()
                .then(([isCodeValidResponse]) => {
                    if (isCodeValidResponse.status == 200) {
                        const button = document.getElementsByClassName(
                            'page-separator__next'
                        )[0] as HTMLButtonElement;
                        const codeInput = document.getElementsByClassName(
                            'input__unique-code'
                        )[0] as HTMLInputElement;

                        codeInput.disabled = true;
                        codeInput.readOnly = true;

                        button.disabled = false;
                        button.click();

                        this.showWarningInvalidCode = false;
                    } else {
                        this.showWarningInvalidCode = true;
                        this.isDisabledButton = false;
                    }
                })
                .catch((error) => console.error('An error during code check:', error));
        }
    }
</script>

<style scoped lang="scss">
    .hidden {
        display: none !important;
    }

    .button__validate-unique-code {
        &:disabled,
        &.disabled {
            pointer-events: none !important;
        }
    }

    .danger {
        color: red;
        transition: 0.3s all ease-in-out;
    }
</style>
