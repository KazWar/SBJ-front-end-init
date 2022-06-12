<template>
  <div class="wrapper__iban-checker">
    <label class="uk-form-label" :for="`form-stacked-iban-checker__${name}`" v-html="label">{{
      label
    }}</label>

    <InputText
      v-model="inputValue"
      :class="{ 'is-wrong': isWrong }"
      :id="`form-stacked-iban-checker__${name}`"
      :name="`form-stacked-iban-checker__${name}`"
      :placeholder="placeholder"
      :isReadOnly="isReadOnly"
      @input="onInput"
    />


    <br />
    <label
      class="uk-form-label"
      id="form-stacked-iban-checker-second-label"
      :for="`form-stacked-iban-checker__${name}`"
      v-html="label"
    >
      {{ $t('registration.confirmIban') }}
    </label>
    <InputText
      id="form-stacked-iban-checker-second"
      v-model="confirmationValue"
      :class="{ 'is-wrong': isWrong }"
      :name="`form-stacked-iban-checker__${name}__bank-number-confirmation`"
      :value="value"
      :isReadOnly="isReadOnly"
      @input="onInput"
    />
  </div>
</template>

<!--suppress SpellCheckingInspection -->
<script lang="ts">
import { Component, Prop, Emit, Vue } from 'vue-property-decorator';
import axios from 'axios';
import IbanRechner from '@/$configs/ibanrechner.json';
import InputText from '@/components/form-builder/InputText.vue';

const BASE_URL_IBAN_API = 'https://ssl.ibanrechner.de/soap/index.php';

@Component({
  name: 'IbanChecker',
  components: { InputText },
})
export default class IbanChecker extends Vue {
  @Prop() private readonly name!: string;
  @Prop() private readonly label!: string;
  @Prop() private readonly placeholder!: string;
  @Prop() private value!: string;
  @Prop() private isReadOnly!: any;

  private readonly inputValue: string;
  private readonly confirmationValue: string;
  private bic: string;
  private iban: string;
  private isWrong: boolean;
  private isCorrect: boolean;

  constructor() {
    super();
    this.inputValue = '';
    this.confirmationValue = '';
    this.bic = '';
    this.iban = '';
    this.isWrong = false;
    this.isCorrect = false;

    if(this.value != null){
      this.inputValue = this.value;
      this.confirmationValue = this.value;
    }

  }

  @Emit('input') onInput(value: string): any | undefined {
      if (this.confirmationValue === this.inputValue && this.confirmationValue.length === this.inputValue.length) {
          this.isWrong = false;
      }
      else if (this.confirmationValue !== this.inputValue && this.confirmationValue.length !== this.inputValue.length) {
          this.isWrong = true;
          return this.isWrong;
      }
      else if (this.confirmationValue !== this.inputValue) {
          this.isWrong = true;
          return this.isWrong;
      }
      else if (this.confirmationValue.length !== this.inputValue.length) {
          this.isWrong = true;
          return this.isWrong;
      }

    const schema = IbanRechner.schema;
    const parser = new DOMParser();
    const xmlContents = parser.parseFromString(schema, 'text/xml');
    const xmlSerializer = new XMLSerializer();
    xmlContents.getElementsByTagName('iban')[0].innerHTML = this.inputValue;
    //console.log('iban', xmlContents.getElementsByTagName('iban')[0].innerHTML);

    const xmlString = xmlSerializer.serializeToString(xmlContents);

    const bicIban = axios
      .post(BASE_URL_IBAN_API, xmlString, {
        headers: {
          'Content-Type': 'text/xml',
        },
      })
      .then((res) => {
        const data: string = res.data;
        const parser = new DOMParser();
        const xmlContents = parser.parseFromString(data, 'text/xml');
        const result = xmlContents.getElementsByTagName('result')[0].innerHTML;
        const bic = xmlContents.getElementsByTagName('bic')[0].innerHTML;
        const iban = xmlContents.getElementsByTagName('iban')[0].innerHTML;
        this.iban = iban;
        this.bic = bic;

        if (result == 'passed') {
          console.log('inpassed', bic);
          this.bic = bic;
          this.isWrong = false;

          return {
            iban: this.iban,
            bic: this.bic,
          };
        }

        if (result !== 'passed') {
          this.isWrong = true;
          this.bic = bic;

          return this.isWrong;
        }

        return {
          Iban: this.iban,
          Bic: this.bic,
        };
      })
      .catch((err) => {
        this.isWrong = true;
        console.error(err.data);
        return {
          Iban: this.iban,
          Bic: this.bic,
        };
      });

    return bicIban;
  }
}
</script>

<style scoped lang="scss">
#form-stacked-iban-checker__IbanChecker {
  margin-top: 0 !important;
}

#form-stacked-iban-checker-second-label {
  margin-bottom: 0 !important;
}

#form-stacked-iban-checker-second {
  margin-top: 0 !important;
}
</style>
