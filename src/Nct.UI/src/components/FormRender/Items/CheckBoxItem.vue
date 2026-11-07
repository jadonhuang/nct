<template>
    <div class="item">

        <template v-if="state.Viewonly">
            {{valueText}}
        </template>

        <el-checkbox-group v-else v-model="modelValue" :disabled="state.Readonly" @change="handleChange">
            <el-checkbox :label="opt.value" :key="index" v-for="(opt,index) in state.Opts">{{opt.name}}</el-checkbox>
        </el-checkbox-group>

    </div>
</template>
<script>

    import emitter from '@/mixins/emitter';
    export default {
        mixins: [emitter],
        props: {
            state: Object,
            value: [String, Array], //规则设置值的时候可以为array
            prop: Object,
          
        },
        data() {
            return {
                modelValue: null
            }
        },
        computed: {
            computedValue() {

                if (this.value)
                {
                    if (!Array.isArray(this.value))
                        return this.value.split(',');

                    return this.value;
                }
                return [];

            },
            valueText() {

                let texts = [];
                if (this.state.Opts) {

                    for (let val of this.computedValue) {

                        let opt = this.state.Opts.find(m => m.value == val);
                        if (opt) texts.push(opt.name);
                    }
                  
                }

                if (texts.length>0)
                    return texts.join(',');

                return '暂无数据'
            }

        },
        created() {
          this.modelValue = this.computedValue;

        },
        watch: {

            value() {
               this.modelValue = this.computedValue;
                this.handleChange(this.modelValue);
            },

           

        },

        methods: {
   
            handleChange(val) {

                let value = val.join(',');
                this.$emit('input', value);
                this.$emit('change', value, this.state);
                this.dispatchItemValueChangeEvent();

            }
        }




    }


</script>
