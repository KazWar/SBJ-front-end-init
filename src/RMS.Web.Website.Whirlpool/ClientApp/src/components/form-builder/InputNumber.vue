<template>
  <div class="wrapper__input-number">
    <label class="uk-form-label" :for="'form-stacked-number__' + name" v-html="label">{{
      label
    }}</label>
    <input
      class="uk-input"
      type="number"
      :id="'form-stacked-number__' + name"
      :name="name"
      :value="value"
      :disabled="isReadOnly"
      :placeholder="placeholder"
      @input="onChange($event.target.value)"
    />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';

@Component({
  name: 'InputNumber'
})
export default class InputNumber extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private value!: number;
  @Prop() private placeholder!: string;
  @Prop() private isReadOnly!: any;
  @Emit('input') onChange(value: number): number {
    return value;
  }
}
</script>

<style scoped lang="scss"></style>
