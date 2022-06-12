<template>
  <div class="wrapper__input-password">
    <label class="uk-form-label" :for="'form-stacked-password__' + name" v-html="label">{{
      label
    }}</label>
    <input
      class="uk-input"
      type="password"
      :id="'form-stacked-password__' + name"
      :name="name"
      :disabled="isReadOnly"
      :value="value"
      :placeholder="placeholder"
      @input="onChange($event.target.value)"
    />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';

@Component({
  name: 'InputPassword'
})
export default class InputPassword extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private value!: string;
  @Prop() private placeholder!: string;
  @Prop() private isReadOnly!: any;

  @Emit('input') onChange(value: string): string {
    return value;
  }
}
</script>

<style scoped lang="scss"></style>
