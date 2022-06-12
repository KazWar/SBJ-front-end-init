<template>
  <div class="wrapper__dropdown-menu">
    <label class="uk-form-label" :for="'form-stacked-dropdown__' + name" v-html="label">
      {{ label }}
    </label>

    <multiselect
      v-model="internalValue"
      open-direction="bottom"
      :multiple="multi"
      :options="options"
      :searchable="true"
      :placeholder="placeholder"
      :disabled="isReadOnly"
      :close-on-select="closeOnSelect"
      :clear-on-select="clearOnSelect"
      track-by="ListValueTranslationKeyValue"
      label="ListValueTranslationDescription"
      @input="onChange(internalValue)"
    >
      <template slot="singleLabel" slot-scope="{ option }">
        <strong>{{ option.ListValueTranslationDescription }}</strong>
      </template>
    </multiselect>

    <br v-if="this.secondaryValue != ''" />

    <multiselect
      v-if="this.secondaryValue != ''"
      v-model="internalValueHandling"
      open-direction="bottom"
      :options="this.secondaryValue"
      :searchable="true"
      :placeholder="placeholder"
      :close-on-select="closeOnSelect"
      :clear-on-select="clearOnSelect"
      :disabled="isReadOnly || this.disableDropdown"
      track-by="HandlingLineId"
      label="HandlingLineDescription"
      @input="onChangeHandling(internalValueHandling)"
    >
      <template slot="singleLabel" slot-scope="{ option }">
        <strong>{{ option.HandlingLineDescription }}</strong>
      </template>
    </multiselect>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';
import Multiselect from 'vue-multiselect';

@Component({
  name: 'DropdownMenu',
    components: { Multiselect }
})
export default class DropdownMenu extends Vue {
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
  private disableDropdown: any;

  constructor() {
    super();

      this.secondaryValue = '';
      this.internalValue = '';
      this.internalValueHandling = '';
      this.disableDropdown = false;
      //if (this.options.length <= 1 ) {
      //    this.internalValue = this.options[0];
      //    return this.internalValue;
      //}

      
    }

    mounted() {
      this.internalValue = this.options.find((option: any) => option.ListValueTranslationKeyValue === this.value)
    }

  @Emit('input') onChange(value: any): any {
    // To avoid mutating the v-model...
    this.internalValue = value;
    //console.log('options', this.options);

    if (value == null) {
        this.secondaryValue = '';
        this.disableDropdown = false;
    }else if(value.HandlingLine.length === 1) {
        this.internalValueHandling = [];
        this.secondaryValue = value.HandlingLine;
        this.internalValueHandling.push(this.secondaryValue[0]);
        this.disableDropdown = true;
      return this.secondaryValue;
    } else {
        this.secondaryValue = value.HandlingLine;
        this.disableDropdown = false;
      return value;
    }

    return value;
  }

  @Emit('input') onChangeHandling(value: any): any {
    // To avoid mutating the v-model...
    this.internalValueHandling = value;

    //console.log('this.internalValueHandling', this.internalValueHandling);
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
