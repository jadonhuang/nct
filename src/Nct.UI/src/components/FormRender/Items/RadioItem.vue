<template>
    <div class="item">

        <template v-if="state.Viewonly">
            {{valueText}}
        </template>

        <el-radio-group v-else v-model="modelValue" :disabled="state.Readonly">
            <el-radio :label="opt.value" :key="index" v-for="(opt,index) in state.Opts">{{opt.name}}</el-radio>
        </el-radio-group>

    </div>
</template>
<script>


    import emitter from '@/mixins/emitter';
    export default {
        mixins: [emitter],
        props: {
            state: Object,
            value: [String, Number],
            prop: Object


        },
        data() {
            return {
                modelValue: String(this.value)
            }
        },
        computed: {

            valueText() {
                let text
                if (this.state.Opts) {

                    let opt = this.state.Opts.find(m => m.value == this.modelValue);
                    if (opt) text = opt.name
                }

                if (!this.$isEmptyStr(text))
                    return text;

                return '暂无数据'
            }

        },
        watch: {
            value(val) {
                this.modelValue = val;
            },
            modelValue() {
                this.$emit('input', this.modelValue);
                this.$emit('change', this.modelValue, this.state);
                this.dispatchItemValueChangeEvent();
            }

        }


    }


</script>
