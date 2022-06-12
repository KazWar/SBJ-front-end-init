<template>
  <div class="wrapper__date-picker">
    <label class="uk-form-label" :for="'form-stacked-date-picker__' + name" v-html="label">{{
      label
    }}</label>

    <div class="uk-form-controls">
      <vc-date-picker
        v-model="selectedDate"
        v-if="this.isReadOnly == false"
        :locale="locale"
        :popover="{ placement: 'bottom-start', visibility: 'click' }"
        :max-date="new Date().setUTCFullYear(this.date.getUTCFullYear())"
        :input-props="{
          class: 'uk-input', 
          placeholder: placeholder,
        }"
        :masks="{ input: ['DD/MM/YY']}"
        @input="onChange(selectedDate)"
      />
      <vc-date-picker
        v-model="selectedDate" 
        v-else
        :locale="locale"
        :disabled="isReadOnly"
        :popover="{ visibility: 'hidden' }"
        :max-date="selectedDate"
        :input-props="{
          class: 'uk-input uk-input-disabled-calender',
          placeholder: placeholder,
        }"
        :masks="{ input: ['DD/MM/YY']}"
        @input="onChange(selectedDate)"
      />
      
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';
import moment from 'moment';

@Component({ 
  name: 'DatePicker',
})
export default class DatePicker extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private value!: string;
  @Prop() private placeholder!: string; 
  @Prop() private locale!: string;
  @Prop() private minAge!: number;
  @Prop() private isReadOnly!: any;

  private date: Date;
  private selectedDate: Date | string | null;
  private dateParts: any;
  private disable: boolean;

  private disableDate: Date;


  constructor() {
    super();
    this.date = new Date();
    this.selectedDate = null;

    if(this.value != null){
      this.dateParts = this.value.split("/");
      const getYear = this.dateParts[2].split(" ");
      const dateObject = new Date(+getYear[0], this.dateParts[1] - 1, +this.dateParts[0]); 
      this.selectedDate = dateObject
    }
    else{
      this.selectedDate = null;
    }

    this.disable = false;
    if(this.isReadOnly == false){
      this.disable = true;
    }

    this.disableDate = new Date();

  }

  private moment(date: Date): moment.Moment {
    return moment(date);
  }

  @Emit('input') onChange(selectedDate: Date): string {
    return this.moment(selectedDate).format('DD-MM-YYYY');
  }
}
</script>

<style scoped lang="scss">

.uk-input-disabled-calender{
    background-color: #f8f8f8;
    color: #999;
    border-color: #e5e5e5;
}


</style>
