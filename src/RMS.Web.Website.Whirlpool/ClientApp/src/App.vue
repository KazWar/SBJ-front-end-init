<template>
    <div id="app">
        <div class="main-header">
            <header id="wrapper__header">
                <nav class="navbar navbar-fixed-top navbar-default" id="navbar">
                    <div class="container-fluid">
                        <div class="navbar-header">
                            <a href="#" class="navbar-brand ">
                                <img src="./assets/img/FB_AW_WHIRLPOOL_LOGO_WITH_PAYOFF_RGB_POS.svg" class="logo-primary" />
                            </a>
                        </div>

                    </div>
                </nav>

            </header>
        </div>
        <main id="wrapper__main-section">
            <div class="header-img" id="header-img"></div>
            <div class="sizing-container">
                <router-view />
            </div>
        </main>

        <footer id="wrapper__footer" class="m-footer">
            <div class="container">
                <div id="footerBase" class="footer_colum logo-column">
                    <img src="./assets/img/Whirlpool_Corporation_Logo.svg" class="footer-logo" />
                    <p>Whirlpool is een geregistreerd handelsmerk van Whirlpool, U.S.A.</p>
                    <p><a href="https://www.whirlpool.nl/pages/privacybeleid" target="_blank">Privacyvoorwaarden</a></p>
                </div>
                <div id="footerBase" class="footer_colum">

                </div>
                <div id="footerBase" class="footer_colum">

                </div>
                <div id="footerBase" class="footer_colum">

                </div>

            </div>
        </footer>
    </div>
</template>

<script lang="ts">
    import { Vue, Component } from 'vue-property-decorator';
    import { Constants } from '@/constants';
    import companySettings from '../src/$configs/settings.json';

    @Component({
        name: 'BasePage',
        components: {}
    })
    export default class BasePage extends Vue {

        constructor() {
            super();
            //document.getElementById('header-img')!.style.backgroundImage = "url(./assets/img/washingmachinesLarge.jpg)";




        }






    }
</script>


<style lang="scss">
    @import './assets/styles/reset.scss';
    @import './assets/styles/vars.scss';
    @import './assets/styles/typography.scss';
    @import url('https://fonts.googleapis.com/css?family=Fira+Sans&display=swap');
    @import url("https://use.typekit.net/ybc4rad.css");

    .main-header {
        top: 0;
        left: 0;
        width: 100%;
    }

    header#wrapper__header {
        float: left;
        width: 100%;
    }

    .navbar {
        height: 8rem;
        display: -webkit-box;
        display: -ms-flexbox;
        display: flex;
        -webkit-box-flex: 1;
        -ms-flex: 1;
        flex: 1;
        margin: 0 auto;
        padding: 0 3.125rem;
        max-width: 140rem;
        padding-bottom: 5px;
    }

    .logo-primary {
        width: 15.1875rem;
    }

    .navbar-menu {
        float: left;
    }

    .navbar-header {
        float: left;
    }

    .navbar-menu li {
        float: left;
        margin-left: 1rem;
        list-style: none;
    }

    .header-img {
        background-image: url(https://rms2public.blob.core.windows.net/whirlpool/basic-header.jpg);
        background-repeat: no-repeat;
        background-size: cover;
        background-position: 100% 80%;
        height: 400px;
        width: 100%;
        float: left;
        margin-bottom: 120px;
        margin-top: 5px;
    }



    .container-fluid {
        width: 100%;
        height: 100%;
        display: -webkit-box;
        display: -ms-flexbox;
        display: flex;
        position: relative;
        -webkit-box-pack: justify;
        -ms-flex-pack: justify;
        justify-content: space-between;
        -ms-flex-wrap: wrap;
        flex-wrap: wrap;
        -webkit-box-align: center;
        -ms-flex-align: center;
        align-items: center;
    }

    .m-footer {
        background: var(--grey);
        padding-top: 1%;
        padding-bottom: 1%;
    }

    .container {
        display: -webkit-box;
        display: -ms-flexbox;
        display: flex;
        -webkit-box-orient: horizontal;
        /* -webkit-box-direction: normal; */
        -ms-flex-flow: row wrap;
        flex-flow: row wrap;
        -webkit-box-pack: justify;
        -ms-flex-pack: justify;
        justify-content: space-between;
        padding: 3.125rem 1.25rem;
        max-width: rem;
        margin: 0 auto;
    }

    .logo-column {
        text-align: left;
        -ms-flex-preferred-size: 20%;
        flex-basis: 20%;
        -webkit-box-ordinal-group: 1;
        -ms-flex-order: 0;
        order: 0;
    }

    .logo-column img {
        width: 50%;
    }



    @media (max-width: 1200px) {
        .header-img {
            background-repeat: no-repeat;
            background-position: bottom;
            background-size: cover;
            height: 400px;
            width: 100%;
            float: left;
            margin-left: auto;
            margin-bottom: 5%;
            margin-right: auto;
            overflow: hidden;
        }

    }

    @media (max-width: 992px) {
        .header-img {
            margin-bottom: 50px;
        }
    }

    @media (max-width: 850px) {
        .header-img {
            height: 350px;
        }
    }

    @media (max-width: 700px) {
        .header-img {
            height: 300px;
        }
    }

    @media (max-width: 550px) {
        .header-img {
            height: 250px;
        }
    }

    @media (max-width: 350px) {
        .header-img {
            margin-top: 5%;
            height: 150px;
        }
    }

    @media (min-width: 768px) {
        .container {
            width: 750px;
        }

    }

    @media (min-width: 992px) {
        .container {
            width: 960px;
        }

    }

    @media (min-width: 1200px) {
        .container {
            width: 1380px;
        }


    }

    .col-md-3, .col-sm-12 {
        position: relative;
        min-height: 1px;
        padding-right: 15px;
        padding-left: 15px;
        float: left;
    }

    @media (min-width: 768px) {
        .col-sm-12 {
            width: 100%;
        }
    }

    @media (min-width: 768px) {
        .col-sm-6 {
            width: 40%;
        }
    }

    @media (min-width: 992px) {
        .col-md-3 {
            width: 25%;
        }
    }



    #wrapper__main-section {
        padding-bottom: 150px;
        padding-top: 30px;
        float: left;
    }

    [v-cloak] {
        display: none !important;
    }

    ::selection {
        color: var(--sbj-white);
        background-color: var(--sbj-red);
    }


    .sizing-container {
        width: 85%;
        max-width: 96rem;
        margin: 0 auto;
    }

    p {
        margin-bottom: 0 !important;
    }

    label {
        cursor: pointer;
    }

    #app {
        min-height: 100%;
        min-height: 100vh;
        display: grid;
        grid-template-rows: 4.8rem 1fr auto;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
    }

    #wrapper__logo {
        position: relative;
    }

    @media (max-width: 767px) {
/*        .navbar-brand {
            float: none;
            margin-right: 50px;
            margin-left: 50px;
            padding-right: 0;
            padding-left: 0;
            -webkit-box-pack: center;
            justify-content: center;
        }

        .navbar-brand img {
            margin-top: 10px;
        }*/

        .sizing-container {
            width: 85% !important;
        }
    }



    .multiselect__option--highlight {
        background: var(--yellow) !important;
        outline: none;
        color: #fff;
    }

    .multiselect__option--highlight:after {
        content: attr(data-select);
        background: var(--yellow) !important;
        color: #fff;
    }

    .page-separator__next {
        background: var(--yellow) !important;
        //border-radius: 50rem !important;
        color: white !important;
    }

    .page-separator__previous {
        //border-radius: 50rem !important;
        color: var(--weber-black) !important;
    }

    .uk-input {
        border-radius: 7px !important;
    }

    .headings__sub-heading strong {
        font-weight: 600 !important;
        color: var(--weber-black);
    }

    .headings__sub-heading {
        color: var(--weber-black);
    }

    .is-wrong .uk-form-controls .uk-input {
        border: 1px solid red !important;
    }

    .filepond--root .filepond--credits {
        display: none !important;
    }

    .uk-input-disabled-calender {
        background-color: #f8f8f8 !important;
        color: #999 !important;
        border-color: #e5e5e5 !important;
        touch-action: none !important;
        cursor: default !important;
        pointer-events: none !important;
    }

    .edit-registration__enter-password .uk-form-label {
        display: block;
        margin-bottom: 5px;
    }

    .main-button:focus {
        outline: none !important;
    }

    #footerBase {
        width: 100%;
        float: left;
        position: relative;
        color: black;
    }

    #footerBase .ow_fnav_ul {
        float: left;
        font-size: 1em;
    }

    #footerBase ul {
        float: left;
        list-style: none;
        margin: 0;
        padding: 0;
        position: relative;
        z-index: 2;
        color: #00b1eb;
    }

    #footerBase li a {
        color: var(--yellow) !important;
    }
</style>




