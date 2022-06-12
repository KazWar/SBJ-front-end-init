<template>
  <div class="wrapper__tile-control">
    <label class="uk-form-label" :for="'form-stacked-tile__' + name">{{
      label
    }}</label>

    <div class="tile-control__grid-wrapper">
      <div
        v-for="item in items"
        :key="item.id"
        :class="`tile-control__id-${item.id}`"
      >
        <h3>{{ item.title }}</h3>
        <p>{{ item.summary }}</p>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component({
  name: 'Tile'
})
export default class Tile extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private items!: Array<{
    id: number;
    title: string;
    summary: string;
    thumbnail: any;
  }>;

  private selectedItem!: {
    id: number;
    title: string;
    summary: string;
    thumbnail: any;
  };

  constructor() {
    super();
  }
}
</script>

<style scoped lang="scss">
.uk-form-label {
  font-size: 3rem;
  font-weight: 400;
}

.tile-control__grid-wrapper {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(25rem, 1fr));
}

[class^='tile-control__']:not(.tile-control__grid-wrapper) {
  margin: 0.5rem;
  padding: 0.5rem 0.5rem 2rem 0.5rem;
  color: #636363;
  border: 1px solid #f1f1f1;
  background: #fdfdfd;
  transition: all 0.2s ease;
  user-select: none;

  &:hover {
    cursor: pointer;
    border: 1px solid darken(#f1f1f1, 5%);
    background: darken(#fdfdfd, 1%);
  }
}
</style>
