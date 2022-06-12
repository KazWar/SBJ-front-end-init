<template>
    <div :class="'wrapper__page-separator ' + 'page-separator__page-' + pageNumber">
        <vk-button class="no-select page-separator__previous"
                   :disabled="pageNumber === 1 || isDisabled || disableButton"
                   :style="{ visibility: pageNumber === 1 ? 'hidden' : 'visible' }"
                   @click="navigateToPreviousPage(pageNumber, pageCount)">Vorige</vk-button>
        <vk-button class="no-select page-separator__next"
                   :disabled="isDisabled || disableButton"
                   type="primary"
                   @click="navigateToNextPage(pageNumber, pageCount)">{{ pageNumber === pageCount ? $t('general.send') : $t('general.next') }}</vk-button>
    </div>
</template>

<script lang="ts">
    import { Component, Prop, Emit, Vue } from 'vue-property-decorator';

    @Component({
        name: 'PageSeparator'
    })
    export default class PageSeparator extends Vue {
        @Prop() private pageNumber!: number;
        @Prop() private pageCount!: number;
        @Prop() private isDisabled!: boolean;
        @Prop() private disableButton!: boolean;


        @Emit('previous') navigateToPreviousPage(
            pageNumber: number,
            // eslint-disable-next-line @typescript-eslint/no-unused-vars
            pageCount: number
        ): number | null {
            if (pageNumber - 1 === 0) return null;
            return pageNumber - 1;
        }

        @Emit('next') navigateToNextPage(
            pageNumber: number,
            // eslint-disable-next-line @typescript-eslint/no-unused-vars
            pageCount: number
        ): number | null {
            return pageNumber + 1;
        }


    }
</script>

<style scoped lang="scss">
.wrapper__page-separator {
  display: grid;
  grid-template-columns: repeat(2, 1fr);

  .page-separator__previous {
    justify-self: flex-start;
    &:disabled {
    opacity: 0.4;
    pointer-events: none;
    }
  }

  .page-separator__next {
    justify-self: flex-end;
    &:disabled {
      opacity: 0.4;
      pointer-events: none;
    }
  }
}
</style>
