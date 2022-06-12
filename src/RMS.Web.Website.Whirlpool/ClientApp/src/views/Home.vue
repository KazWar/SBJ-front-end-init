<template>
  <div class="home">
    <p>Waiting...</p>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import companySettings from '../$configs/settings.json';
import RegistrationForm from '@/components/RegistrationForm.vue';

@Component({
  name: 'Home',
  components: { RegistrationForm }
})
export default class Home extends Vue {
  constructor() {
    super();
  }

  // Lifecycle hook
  created() {
    const companyOverviewEnabled = companySettings.campaignOverview.isEnabled;
    if (companyOverviewEnabled) this.$router.push('locale-selection');
  }
}
</script>
