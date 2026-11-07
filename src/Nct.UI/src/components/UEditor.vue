<template>
    <div :id="id" :style="{width:width,height:height}"></div>

</template>
<script>export default {
        props: {
            config: Object,
            value: String,


        },
        data() {
            return {
                id: this.$newId(),
            }
        },
        computed: {
            width() {
                if (this.config.width) {

                    if (typeof this.config.width === 'number')
                        return this.config.width + 'px';
                    return this.config.width;
                }
                return '100%';

            },
            height() {
                if (this.config.height) {
                    if (typeof this.config.height === 'number')
                        return this.config.height + 'px';
                    return this.config.height;

                }
                return '500px';

            },


        },
        watch: {

        },
        methods: {
          
        },

        async mounted() {

            if (!window.UE) {
                await this.$loadScriptAsync(this.$publicPath + 'lib/ueditor/ueditor.config.js');
                await this.$loadScriptAsync(this.$publicPath + 'lib/ueditor/ueditor.all.js');
            }

            let toolbars = this.$consts.form.editorToolbar[this.config.toolbar || 'simple'];
            let config = {
                readonly: this.config.readonly || false, toolbars: toolbars, wordCount: false,
                serverUrl: this.$appUrl + '/util/ue',
                zIndex: this.config.zIndex || 1000

            }

            const self = this;
            let holder = document.getElementById(self.id);
            if (!holder) return;
            let ue = UE.getEditor(self.id, config);
            ue.ready(function () {
                ue.setContent(self.value || '');
                self.ue = ue;
                self.$emit("ready", ue)
                ue.addListener('selectionchange', () => {
                    self.$emit("input", ue.getContent())
                })


            })



        }







    }</script>

<style scoped>
</style>
