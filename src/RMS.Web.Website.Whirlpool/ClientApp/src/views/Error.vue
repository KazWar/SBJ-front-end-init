<template>
    <div class="wrapper__error-page">
        <div class="error-message">
            <h1>{{ $t('errorPage.heading') }}</h1>
            <h4 v-if="url === false">
                {{ $t('thankYou.checkYourEmail') }}
            </h4>
            <h4 v-else>
                {{ $t('thankYou.editMessage') }}
            </h4>

            <button class="main-button"
                    onclick="window.location.href='/locale-selection'">
                {{ $t('general.goBack') }}
            </button>
        </div>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component({
  name: 'Error',
})
export default class Registration extends Vue {
    private readonly url: boolean;

    constructor() {
        super();
        this.url = this.$route.path.includes("edit");
    }
}
</script>

<style scoped lang="scss">
    .error-message {
        margin: 0 auto;
        background-color: var(--weber-grey);
        border-radius: 15px;
        padding: 35px;
        text-align: center;
    }

    h1,
    h2,
    h3,
    h4,
    h5,
    h6 {
        font-weight: bold;
        color: var(--black);
    }

    .main-button {
        margin: 20px;
        overflow: visible;
        font: inherit;
        display: inline-block;
        box-sizing: border-box;
        padding: 0 30px;
        vertical-align: middle;
        font-size: 14px;
        line-height: 38px;
        cursor: pointer;
        background-color: var(--weber-grey);
        border: 2px solid #78be20 !important;
        color: #000 !important;
        text-transform: uppercase;
        transition: 0.1s ease-in-out;
        transition-property: color, background-color;
    }

    .main-button:hover {
        text-decoration: none !important;
        color: #78be20 !important;
    }
</style>
