<template>
    <div class="item">

        <template v-if="state.Viewonly">

            {{valueText||'暂无数据'}}

        </template>

        <template v-else>
            <el-input ref="hideInput" clearable style="width:0px;height:0px"></el-input>
            <el-input :placeholder="prop.Tip" :value="valueText" :disabled="state.Readonly" @focus="loseFocus" @clear="clearValue" clearable>
                <el-button slot="append" icon="el-icon-location" @click="showSelect" :disabled="state.Readonly"></el-button>
            </el-input>
        </template>



    </div>



</template>
<script>

    import emitter from '@/mixins/emitter';
    export default {
        mixins: [emitter],
        inject: ['nctForm'],
        props: {
            state: Object,
            value: String,
            prop: Object

        },
        data() {
            return {
                modelValue: this.value,

            }
        },
        watch: {
            value(val) {
                this.modelValue = val;
            },
            modelValue() {
                this.$emit('input', this.modelValue);

            },


        },
        methods: {
            clearValue() {

                this.modelValue = null;
            },

            loseFocus() {
                this.$refs.hideInput.focus();
            },
            async showSelect() {

                var point = '';
                var obj = this.objValue;
                if (obj && 'lng' in obj)
                    point = JSON.stringify({ lng: obj.lng || 0, lat: obj.lat || 0 });
             
                var url = `/form/locationselect?point=${point}`;

                var title = `选择地址（双击选择）`
                var width = 1080;
                var opts = { url: url, title: title, width: width, resizeable: false, headerVisible: false };
                var data = await this.$showDialogWithValue(opts);
                if (data && typeof data === 'object') {

                    console.log(data)
                    this.modelValue = JSON.stringify(data);
                    this.$emit('change', this.modelValue, this.state);
                }


            }


        },


        computed: {
            objValue() {
                return JSON.parse(this.modelValue ||"{}");
               
            }, valueText() {

                return (this.objValue.title || '') + (this.objValue.address || '')

            }

        },


    }


</script>

<style scoped>
    #map {
        height: 100%;
        width: 100%;
    }

    .el-main {
        padding: 0px;
    }

    .el-aside {
        display: flex;
        flex-direction: column;
        height: 400px;
        border: 1px solid #EBEEF5;
        padding: 5px;
    }

    .suguests {
        flex: 1;
        overflow-y: auto;
       
    }

    .address-item {
        padding: 10px;
    }

    .title {
        font-weight: bold;
    }

    /deep/ .el-dialog__body {
        padding: 10px;
    }
</style>
