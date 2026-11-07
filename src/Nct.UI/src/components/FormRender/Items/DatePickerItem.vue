<template>
    <div class="item">

        <template v-if="state.Viewonly">

            {{valueText}}

        </template>

        <template v-else>

            <el-date-picker v-model="modelValue" v-if="prop.DateFormat==2" value-format="yyyy-MM-dd HH:mm:ss" :disabled="state.Readonly" class="w100" clearable
                            type="datetime"
                            :placeholder="prop.Tip"
                            align="left"
                            :picker-options="pickerOptions">
            </el-date-picker>


            <el-date-picker v-model="modelValue" value-format="yyyy-MM-dd" :disabled="state.Readonly" v-else class="w100" clearable
                            align="left"
                            type="date"
                            :placeholder="prop.Tip"
                            :picker-options="pickerOptions">
            </el-date-picker>

        </template>

    </div>
</template>
<script>


    export default {
        props: {
            state: Object,
            value: String,
            prop: Object,

        },
        data() {
            return {
                modelValue: this.value ? new Date(this.value) : null,
                pickerOptions: {

                    shortcuts: [{
                        text: '今天',
                        onClick(picker) {
                            picker.$emit('pick', new Date());
                        }
                    }, {
                        text: '昨天',
                        onClick(picker) {
                            const date = new Date();
                            date.setTime(date.getTime() - 3600 * 1000 * 24);
                            picker.$emit('pick', date);
                        }
                    }, {
                        text: '一周前',
                        onClick(picker) {
                            const date = new Date();
                            date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
                            picker.$emit('pick', date);
                        }
                    }]
                },
            }
        },

        computed: {
            valueText() {

                if (!this.modelValue) return '暂无数据'
                let format = this.prop.DateFormat == 2 ? 'yyyy-MM-dd HH:mm:ss' :"yyyy-MM-dd"
                return this.$formatDate(this.modelValue, format);

            },

        },
        watch: {

            value(val) {
                this.modelValue = val;
            },

            modelValue() {
                this.$emit('input', this.modelValue);
                this.$emit('change', this.modelValue, this.state);
            }

        }


    }


</script>
