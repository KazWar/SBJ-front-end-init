<template>
  <div class="wrapper__dropdown-menu">
    <label class="uk-form-label" :for="'form-stacked-dropdown__' + name" v-html="label">
      {{ label }}
    </label>

    <multiselect
      v-model="internalValue"
      :multiple="multi"
      :options="options"
      :searchable="true"
      :placeholder="placeholder"
      :disabled="isReadOnly"
      :close-on-select="closeOnSelect"
      :clear-on-select="clearOnSelect"      
      track-by="retailerLocationId"
      label="retailerAddress"
      @input="onChange(internalValue)"
    >
      <template slot="singleLabel" slot-scope="{ option }">
        <strong>{{ option.retailerAddress }}</strong>
      </template>
    </multiselect>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';
import Multiselect from 'vue-multiselect';

@Component({
  name: 'RetailerLocation',
  components: { Multiselect },
})
export default class RetailerLocation extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private value!: string;
  @Prop() private placeholder!: string;
  @Prop() private multi!: boolean;
  @Prop() private searchable!: boolean;
  @Prop() private closeOnSelect!: boolean;
  @Prop() private clearOnSelect!: boolean;
  @Prop() private options!: [];
  @Prop() private id!: any;
  @Prop() private isReadOnly!: any;

  private remappedName!: [];
  private secondaryValue: any;
  private internalValue: any;
  private internalValueHandling: any;
  private chosenRetailer: any;

  constructor() {
    super();

    this.secondaryValue = '';
    this.internalValue = '';
    this.internalValueHandling = '';
    if(this.value != null){
      this.internalValue = this.options.find( (p: any) => p.retailerLocationId == this.value );
    }
 
  }

  @Emit('input') onChange(value: any): any {
    // To avoid mutating the v-model...
    this.internalValue = value;
    if (value == null) {
      this.secondaryValue = '';
    } else {
      this.secondaryValue = value.HandlingLine;
    }
    //console.log(value.retailerLocationId);
    return value.retailerLocationId;
  }

  @Emit('input') onChangeHandling(value: any): any {
    // To avoid mutating the v-model...
    this.internalValueHandling = value;
    return value;
  }
}
</script>

<style scoped lang="scss">
@import '../../../node_modules/vue-multiselect/dist/vue-multiselect.min.css';

.wrapper__dropdown-menu {
  cursor: pointer;
}
</style>
