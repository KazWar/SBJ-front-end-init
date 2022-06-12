<template>
  <div class="wrapper__input-text">
    <label class="uk-form-label" :for="'form-stacked-text__' + name" v-html="label">{{
      label
    }}</label>

    <div :class="`uk-form-controls ${this.className}`">
      <input
        :id="'form-stacked-text__' + name"
        class="uk-input"
        type="text"
        :name="name"
        :value="value"
        :disabled="isReadOnly"
        :placeholder="placeholder"
        autocomplete="nope"
        @input="onInput($event.target.value)"
      />
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';

@Component({
  name: 'InputText'
})
export default class InputText extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private value!: string;
  @Prop() private placeholder!: string;
  @Prop() private className!: string;
  @Prop() private isReadOnly!: any;
  constructor() {
    super();
  }

  @Emit('input') onInput(value: string): string {
    return value;
  }
}
</script>

<style scoped lang="scss"></style>
