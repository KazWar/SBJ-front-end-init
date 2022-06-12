<template>
  <div class="wrapper__html-text" v-html="label"></div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component({
  name: 'HtmlText',
})
export default class HtmlText extends Vue {
  @Prop() private label!: string;

  constructor() {
    super();
  }
}
</script>

<style scoped lang="scss"></style>
