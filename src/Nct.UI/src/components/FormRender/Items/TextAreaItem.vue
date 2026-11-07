<template>
    <div class="item">

        <template v-if="state.Viewonly">
            <span v-html="valueText"></span>
        </template>

        <el-input v-else v-model="modelValue" :minlength="prop.MinLen" @blur="handleBlur" @focus="handleFocus" @change="handleChange" :maxlength="maxLen" clearable type="textarea" :rows="prop.Rows" :placeholder="prop.Tip" :disabled="state.Readonly"></el-input>
    </div>
</template>
<script>



    export default {
        props: {
            value: String,
            prop: Object,
            state: Object,

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
            }

        },
        computed: {
            maxLen() {

                if (this.prop.MaxLen <= 0) return Infinity;
                return this.prop.MaxLen;
            },
            valueText() {

                if (!this.modelValue) return '暂无数据';
                return this.modelValue.replaceAll('\n','<br/>')


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
        }




    }


</script>
