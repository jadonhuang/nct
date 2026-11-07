<template>
    <div class="item">

        <template v-if="state.Viewonly">
            {{modelValue||'暂无数据'}}
        </template>
        <el-input v-else v-model="modelValue" @blur="handleBlur" @focus="handleFocus" @change="handleChange" :placeholder="prop.Tip" clearable :minlength="prop.MinLen" :maxlength="maxLen" :disabled="state.Readonly"></el-input>
    </div>
</template>
<script>


    import emitter from '@/mixins/emitter';
    export default {
        mixins: [emitter],
        props: {
            state: Object,
            value: String,
            prop: Object,

        },
        data() {
            return {
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
            }

        },
        methods: {
            handleBlur() {

                this.$emit('blur');
            },
            handleFocus() {
                this.$emit('focus');
            },

            handleChange(value) {
                this.$emit('change', value, this.state);
            }
        },
        computed: {
            maxLen() {

                if (this.prop.MaxLen <= 0) return Infinity;
                return this.prop.MaxLen;

            },

        }



    }


</script>
