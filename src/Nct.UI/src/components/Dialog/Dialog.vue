<template>
    <el-dialog :title="options.title" :visible.sync="isShow" v-draggable :headerVisible="headerVisible" :resizeable="resizeable" :height="height" :width="width" append-to-body destroy-on-close :close-on-click-modal="false" :before-close="handleBeforeClose">

        <component :is="componentName"></component>

    </el-dialog>

</template>
<script>


    export default {
        componentName: 'Dialog',
        data() {
            return {
                isShow: false,
                componentName: '',
                query: {},
                queryBak: {},
                options: {},
                callback: null,
                retValue: null


            }
        },
        computed: {

            headerVisible() {

                if (this.options.headerVisible === undefined || this.options.headerVisible === true) return 'Y';
                return 'N';

            },

            resizeable() {

                if (this.options.resizeable === undefined || this.options.resizeable === true) return 'Y';
                return 'N';

            },
            width() {

                if (this.options.width) return this.options.width + 'px';
                return '800px';

            },
            height() {

                const h = this.options.height;
                if (h&&h!='0'&&h!='auto') return h + 'px';
                return 'auto';

            }

        },

        watch: {
            isShow(value) {
                if (value) {

                    this.queryBak = this.$copyObj(this.$route.query);
                  
                    for (let key in this.queryBak) {
                        delete this.$route.query[key]

                    }
                    for (let key in this.query) {
                        this.$route.query[key] = this.query[key]

                    }

                } else {

                    for (let key in this.query) {
                        delete this.$route.query[key]

                    }

                    for (let key in this.queryBak) {
                        this.$route.query[key] = this.queryBak[key]
                    }
                    this.$destroy();
                }

            }

        },

        methods: {
          
            setCloseCallback(fun) {
                this.options.beforeClose = fun;

            },
            async handleBeforeClose(done) {
                var fn = this.options.beforeClose;
                if (fn && typeof fn === 'function') {
                    var result = fn();
                    if (result && result.then)
                        result = await result;
                  
                    if (!result) return;
                   
                }
   
                done();

                if (this.callback) {
                    this.callback(this.retValue);
                }

            },


        },

        async created() {

            var self = this;
            this.$on('dialog.close', (value) => {              
                this.handleBeforeClose(function () {
                    self.retValue = value;
                    self.isShow = false;
                  
                })
             
            });


        },

        components: {}


    }


</script>



