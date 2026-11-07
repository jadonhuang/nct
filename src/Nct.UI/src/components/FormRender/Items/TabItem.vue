<template>
    <div class="item">
        <el-tabs class="tab-holder" v-model="activeName" :type="type" :tab-position="prop.TabPosition">

            <template v-for="(tab,index) in prop.Tabs">
                <el-tab-pane :label="tab.Title" :key="index" :name="String(index)"  v-if="state[prop.Id].Tabs[index].Status!==3" :disabled="state[prop.Id].Tabs[index].Status===2">
                    <span slot="label"><i :class="tab.Icon"></i> {{tab.Title}}</span>

                    <ItemGroup :items="tab.Items" @validate="validateEvent" :state="state" :model="value"></ItemGroup>

                </el-tab-pane>
            </template>
        </el-tabs>

    </div>
</template>
<script>

    import formMixin from '@/mixins/form.js'
    import emitter from '@/mixins/emitter';
    export default {
        mixins: [formMixin, emitter],
        props: {
            state: Object,
            value: Object,
            prop: Object,

        },
        data() {
            return {
                activeName: this.state.ActiveTab,
            }
        },
        watch: {
            "state.ActiveTab": function (val) {
                this.activeName = String(val) || "0";
            },
            activeName(value) {
                this.$emit('change', value, this.state);

            }

        },

        computed: {
            type() {

                if (this.prop.Style != 'simple') return this.prop.Style;
                return null;
            },

        },
        methods: {

            validateEvent(prop, passed) {

                if (!passed) {
                    let index = 0;
                    for (const tab of this.prop.Tabs) {

                        for (const item of tab.Items) {
                            if (item.Name == prop) {

                                this.activeName = String(index);
                                return;
                            }


                        }
                        index++;

                    }


                }

            }
          
        }


    }


</script>

<style scoped>
    .item {
      height:100%;
    
    }

    .tab-holder {
        display: flex;
        flex-direction: column;
        height: 100%;
        box-sizing: border-box;
    }

    /deep/ .tab-holder > .el-tabs__content {
        padding: 15px;
        flex:auto;
        overflow-x:hidden;
        overflow-y:auto;
        height:0;
    }

</style>
