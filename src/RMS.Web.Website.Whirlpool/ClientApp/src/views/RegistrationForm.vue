<template>
    <div class="wrapper__registration-page">
        <RegistrationForm />
    </div>
</template>

<script lang="ts">
    import { Component, Vue } from 'vue-property-decorator';
    import RegistrationForm from '@/components/RegistrationForm.vue';

    @Component({
        name: 'Registration',
        components: { RegistrationForm },
    })
    export default class Registration extends Vue {
        private locale: string;
        constructor() {
            super();
            this.locale = this.$parent.$route.params.locale;
            if (this.locale) {
                this.$parent.$i18n.locale = this.locale;
            }
        }
    }
</script>

<style scoped lang="scss"></style>
