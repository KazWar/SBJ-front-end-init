<template>
    <div class="wrapper__radio-button">
        <label class="uk-form-label" :for="'form-stacked-radio-button__' + name" v-html="label">
            {{ label }}
        </label>
        <template v-for="(item, index) in options">
            <label :key="`${item}-${index}`">
                <input class="uk-radio"
                       type="radio"
                       :key="`${item}-${index}`"
                       :id="'form-stacked-radio-button__' + name + '-' + index"
                       :name="name"
                       :disabled="isReadOnly"
                       @input="onChange(item)" />
                {{ item.retailerAddress }}
            </label>
        </template>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Emit, Vue } from 'vue-property-decorator';

    @Component({
        name: 'RetailerRadioButton',
    })
    export default class RetailerRadioButton extends Vue {
        @Prop() private name!: string;
        @Prop() private label!: string;
        @Prop() private values!: Array<string>;
        @Prop() private options!: [];
        @Prop() private isReadOnly!: any;

        @Emit('input') onChange(value: string): string {
            return value;
        }
    }
</script>

<style scoped lang="scss">
.wrapper__radio-button {
  label:not(.uk-form-label) {
    margin-right: 1rem;
  }
}
</style>
