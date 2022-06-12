<template>
    <div class="wrapper__checkbox">
        <label class="uk-form-label" v-html="label">{{ label }}</label>
        <!--<input class="uk-checkbox"
               type="checkbox"
               :name="name"
               :value="value"
               :checked="checked"
               @input="onChange($event.target.checked)" />-->

        <template v-for="(item, index) in options">
            <label :key="`${item}-${index}`">
                <input class="uk-checkbox"
                       type="checkbox"
                       :key="`${item}-${index}`"
                       :id="'form-stacked-checkbox__' + name + '-' + index"
                       :name="name"
                       :checked="value"
                       :disabled="isReadOnly"
                       @input="onChange($event.target.checked)" />
                <span v-html="item.ListValueTranslationDescription"></span>
            </label>
        </template>


    </div>
</template>

<script lang="ts">
    import { Component, Prop, Emit, Vue } from 'vue-property-decorator';

    @Component({
        name: 'CheckBox'
    })
    export default class CheckBox extends Vue {
        @Prop() private name!: string;
        @Prop() private label!: string;
        @Prop() private value!: string;
        @Prop() private options!: [];
        @Prop() private checked!: boolean | string;
        @Prop() private isReadOnly!: any;
        @Prop() private isRequired!: boolean;
        private gotValue!: boolean;

        constructor() {
            super();
            if (this.value != null) {

                this.gotValue = (this.value === 'true')
            }

        }

        @Emit('input') onChange(isChecked: boolean | string): string {
            if (this.isRequired === true) {
                if (isChecked === false) {
                    return '';
                }
                else {
                    return isChecked.toString();
                }
            }
            return isChecked.toString();
        }
    }
</script>

<style lang="scss">
    .uk-checkbox {
        margin-right: 5px !important;
    }
</style>