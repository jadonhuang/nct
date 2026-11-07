<template>

    <div v-loading="loading" class="container">
        <template v-if="isPdf">
            <pdf v-for="i in pdfNumPages" :key="i" :src="pdfSrc" :page="i" class="pdf"></pdf>
        </template>
        <iframe :src="previewUrl" id="preview" class="iframe" v-else> </iframe>

    </div>
</template>
<script>

    import pdf from 'vue-pdf'
    export default {
        components: {
            pdf
        },
        data() {
            return {
                loading: true,
                previewUrl: null,
                ext: null,
                pdfSrc: null,
                pdfNumPages: null
            }
        },
        computed: {
            isPdf() {
                return this.ext == '.pdf';

            },
        },
        methods: {



        },

        created() {
            let url = this.$route.query.url;
            this.ext = this.$route.query.ext;
            if (this.isPdf) {
                this.previewUrl = this.$urlDecode(url);

            } else {
                this.previewUrl = `https://view.officeapps.live.com/op/view.aspx?src=${url}`;
            }

        },

        mounted() {
            if (!this.isPdf) {
                var iframe = document.getElementById("preview");
                var self = this;
                iframe.addEventListener("load", function () {
                    self.loading = false;

                });

            } else {

                var loadingTask = pdf.createLoadingTask(this.previewUrl);
                this.pdfSrc = loadingTask
                this.pdfSrc.promise.then(pdf => {
                    this.loading = false;
                    this.pdfNumPages = pdf.numPages;
                });


            }

        }




    }
</script>

<style scoped>
    .container {
        height: 100%;
        position: relative;
        overflow: auto;
        background-color: darkgrey;
        display:flex;
        flex-direction:column;
        align-items:center;
    }

    .iframe {
        width: 100%;
        position: absolute;
        border: none;
        top: -90px;
        height: 733px;
    }

    .pdf {
        width:90%;
        margin-top: 10px;
    }
</style>


