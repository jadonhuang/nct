<template>
    <div class="item">

        <template v-if="state.Viewonly">

            {{valueText||'暂无数据'}}

        </template>

        <template v-else>
            <el-input ref="hideInput" clearable style="width:0px;height:0px"></el-input>
            <el-input :placeholder="prop.Tip" :value="valueText" :disabled="state.Readonly" @focus="loseFocus" @clear="clearValue" clearable>
                <el-button slot="append" icon="el-icon-search" @click="showSelect" :disabled="state.Readonly"></el-button>
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
            value: [String, Number],
            prop: Object

        },
        data() {
            return {
                text: this.state.Text,
                modelValue: this.value
            }
        },
        watch: {
            value(val) {
                this.modelValue = val;
            },
            modelValue() {
                this.$emit('input', this.modelValue);
                this.dispatchItemValueChangeEvent();
            },
              "state.Text"(val) {
                  this.text = val;
            },

        },
        methods: {
            clearValue() {

                this.modelValue = null;
                this.text = null;
            },

            loseFocus() {
                this.$refs.hideInput.focus();
            },
            async showSelect() {
                var url = `/form/list?code=${this.prop.DataSourceCode}&mode=select`;
                var queryJson = "";
                if (this.prop.QueryJson) {

                    var FORM = this.nctForm.FORM;

                    var code = `(function () {
                          return ${this.prop.QueryJson}
                        }

                        )()`

                    var result = eval(code);
                    queryJson = JSON.stringify(result);
                    url += '&q=' + queryJson;
                }
                var title = this.prop.DialogTitle || `选择数据`
                var width = this.prop.DialogWidth || 1200;
                var height = this.prop.DialogHeight || 600;
                var opts = { url: url, title: title, width: width, height: height };
                var data = await this.$showDialogWithValue(opts);
                if (data && typeof data === 'object') {
                    this.modelValue = data[this.prop.ValueField];
                    this.text = data[this.prop.TextField];
                    this.$emit('change', data, this.state);
                }


            }
        },
        created() {


        },
        computed: {
            valueText() {
            
                if (this.text)
                    return this.text;

                return this.value;

            }

        },


    }


</script>
