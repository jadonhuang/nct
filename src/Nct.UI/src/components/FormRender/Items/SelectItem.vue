<template>
    <div class="item">

        <template v-if="!state.Viewonly||!$isEmptyStr(modelValue)">

            <el-select v-model="modelValue" :disabled="state.Readonly||state.Viewonly" v-if="prop.RenderType==1" class="w100"
                       :multiple="prop.MultiSelect"
                       :collapse-tags="prop.collapseTags" clearable filterable @change="handleChange"
                       :placeholder="prop.Tip">
                <el-option v-for="item in state.Opts"
                           :key="item.value"
                           :label="item.name"
                           :value="item.value">
                </el-option>
            </el-select>


            <el-cascader :placeholder="prop.Tip" v-else-if="prop.RenderType==2" v-model="modelValue" :key="prop.Name" :collapse-tags="prop.CollapseTags" 
                         :disabled="state.Readonly||state.Viewonly" class="w100"
                         :options="state.Opts" :props="props" @change="handleChange"
                         filterable clearable></el-cascader>

        </template>
        <template v-else>
            暂无数据
        </template>
                 


    </div>
</template>
<script>

    import emitter from '@/mixins/emitter';
    export default {
        mixins: [emitter],
        props: {
            state: Object,
            value: [String, Array],
            prop: Object,


        },
        data() {

            return {
                modelValue: null
            }
        },
        watch: {
            value(value, oldvalue) {

                this.modelValue = this.computedValue;
                this.fireEvent();
            }
           
          
        },
        created() {
            this.modelValue = this.computedValue;

        },
        computed: {
            computedValue() {
                if (this.prop.MultiSelect) {
                    if (this.value) {
                        if (!Array.isArray(this.value))
                            return this.value.split(',');

                        return this.value;
                    }
                    return [];

                }

                return this.value;

            },

            props() {

                return {
                    multiple: this.prop.MultiSelect,
                    'checkStrictly': this.prop.CheckStrictly,
                    'emitPath': false, 'label': 'name',
                    'value': 'value', 'children': 'children', 'expandTrigger': 'click'
                }

            },

        },


        methods: {

            fireEvent() {
                this.$emit('change', this.value, this.state);
                this.dispatchItemValueChangeEvent();

            },
        

            handleChange(val) {
              
                let value = val;
                if (Array.isArray(value))
                    value = value.join(',');
                this.$emit('input', value);
             
            }
        }




    }


</script>

