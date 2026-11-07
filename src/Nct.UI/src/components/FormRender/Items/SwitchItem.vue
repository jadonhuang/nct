<template>
    <div class="item">

        <template v-if="state.Viewonly">
            {{valueText}}
        </template>

        <el-switch v-else :disabled="state.Readonly" :active-value="1"
                   :inactive-value="0" v-model="modelValue" :active-text="prop.ActiveText" :inactive-text="prop.InActiveText"> </el-switch>

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
                modelValue: this.value 
            }
        },
        computed: {

            valueText() {
                if (this.modelValue == 1)
                    return this.prop.ActiveText || '开'

                return this.prop.InActiveText || '关'

            }

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
