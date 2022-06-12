<template>
  <div class="wrapper__file-uploader">
    <CoolLightBox
      :items="uploadedFiles"
      :index="lightboxIndex"
      @close="lightboxIndex = null"
    ></CoolLightBox>
    <p class="uk-form-label" v-html="label">
      {{ label }}
    </p>
    <file-pond
      ref="uploader"
      name="name"
      allow-image-preview="true"      
      v-bind:labelIdle="uploadFile"
      allow-multiple="true"
      accepted-file-types="image/jpeg, image/png, application/pdf"
      maxFiles="5"
      maxFileSize="5MB"
      :allowFileEncode="true"
      :before-remove-file="onRemoveFile"
      :files="uploadedFiles"
      :disabled="isReadOnly"
      @addfile="onAddFile()"
    />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Emit, Ref, Vue } from 'vue-property-decorator';
import CoolLightBox from 'vue-cool-lightbox';
import vueFilePond from 'vue-filepond';
import 'filepond/dist/filepond.min.css';
import 'filepond-plugin-image-preview/dist/filepond-plugin-image-preview.min.css';
import FilePondPluginFileValidateType from 'filepond-plugin-file-validate-type';
import FilePondPluginImagePreview from 'filepond-plugin-image-preview';
import FilePondPluginFileEncode from 'filepond-plugin-file-encode';
import FilePondPluginFileValidateSize from 'filepond-plugin-file-validate-size';


const FilePond = vueFilePond(
  FilePondPluginFileValidateType,
  FilePondPluginImagePreview,
  FilePondPluginFileEncode,
  FilePondPluginFileValidateSize
);

@Component({
  name: 'FileUploader',
  components: { FilePond, CoolLightBox },
})
export default class FileUploader extends Vue {
  @Prop() private name!: string;
  @Prop() private label!: string;
  @Prop() private uploadText!: string;
  @Prop() private multi!: boolean;
  @Prop() private fileTypes!: string;
  @Prop() private isReadOnly!: any;

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  @Ref('uploader') private uploader: any;

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private uploadedFiles: any;
  private lightboxIndex: number | null;
  private uploadFile: string;

  constructor() {
    super();
    this.uploadedFiles = [];
    this.lightboxIndex = null;
    this.uploadFile = this.$parent.$i18n.t('component.filepond.labelIdle').toString();
    
  }

  private setLightboxIndex(index: number): void {
    if (index < 0) return;
    this.lightboxIndex = index;
  }

  @Emit('addfile') onAddFile(): [] | null {
    const files = this.uploader.getFiles();
    if (!Array(files).length) return null;
    const fileDataUrl = files[0].getFileEncodeDataURL();
    this.uploadedFiles.push(fileDataUrl);

    //console.log(fileDataUrl);

    function removeDuplicates(data: any) {
      return data.filter(
        (value: any, index: any) => data.indexOf(value) === index
      );
    }

    this.uploadedFiles = removeDuplicates(this.uploadedFiles);
    this.onRetrievedDataURL(this.uploadedFiles);

    //const BASE64_MARKER = ';base64,';

    //const base64Index = fileDataUrl.indexOf(BASE64_MARKER) + BASE64_MARKER.length;
    //const base64 = fileDataUrl.substring(base64Index);
    //const raw = window.atob(base64);
    //const rawLength = raw.length;
    //const array = new Uint8Array(new ArrayBuffer(rawLength));
    //for (let i = 0; i < rawLength; i++) {
    //     array[i] = raw.charCodeAt(i);
    // }

    //console.log(array);

    return this.uploadedFiles;
  }

  @Emit('input') onRetrievedDataURL(dataUrl: string | string[]) {
    return dataUrl;
  }

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  @Emit('input') onRemoveFile(file: any): void {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    this.uploadedFiles = this.uploadedFiles.filter((f: any) => {
      return f.id !== file.id;
    });
  }
}
</script>

<style scoped lang="scss">
.filepond--wrapper {
  cursor: pointer;
}
</style>
