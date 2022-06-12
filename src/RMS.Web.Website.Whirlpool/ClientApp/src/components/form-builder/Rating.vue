<template>
  <div class="wrapper__rating">
    <label class="uk-form-label" :for="'form-stacked-rating__' + name">{{
      label
    }}</label>
    <star-rating
      :read-only="readOnly"
      :max-rating="maxRating"
      :show-rating="showRating"
      :rtl="rtl"
      :border-width="3"
      @rating-selected="onChange"
    ></star-rating>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';
import StarRating from 'vue-star-rating';

@Component({
  name: 'Rating',
  components: { StarRating }
})
export default class Rating extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private rtl!: boolean;
  @Prop() private readOnly!: boolean;
  @Prop() private showRating!: boolean;
  @Prop({ default: 5 }) private maxRating!: number;

  private rating: number | null;

  constructor() {
    super();
    this.rating = null;
  }

  @Emit('input') onChange(rating: number): number {
    // Set rating and emit it to the parent component
    this.rating = rating;
    return rating;
  }
}
</script>

<style scoped lang="scss"></style>
