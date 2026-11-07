<template>
    <div class="item">

        <template v-if="state.Viewonly">
            {{valueText}}
        </template>

        <el-input v-else :value="displayValue" :class="{'has-suffix':!!prop.Suffix}" :min="min" :max="max" @blur="handleBlur" @focus="handleFocus" clearable :placeholder="prop.Tip" :disabled="state.Readonly" @input="handleInput"
                  @change="handleInputChange">
            <template slot="append" v-if="prop.Suffix">
                {{prop.Suffix}}
            </template>

        </el-input>

    </div>
</template>
<script>


    export default {
        props: {
            value: [Number, String],
            prop: Object,
            state: Object


        },
        data() {
            return {
                userInput: null,
                precision: this.prop.Precision
            }
        },
        watch: {

        },
        computed: {

            max() {
                if (this.prop.Max <= 0) return -1;
                return this.prop.Max;

            },
            min() {
                return this.prop.Min;

            },
            displayValue() {
                if (this.userInput !== null) {
                    return this.userInput;
                }

                let currentValue = this.value;

                if (typeof currentValue === 'number') {
                    currentValue = currentValue.toFixed(this.precision);

                }
                return currentValue;
            },

            valueText() {
                if (!this.displayValue) return '暂无数据';
               return  this.displayValue + ' ' + this.prop.Suffix;

            }



        },
        methods: {
            handleBlur() {

                this.$emit('blur');
            },
            handleFocus() {

                this.$emit('focus');
            },
            handleInput(value) {
                this.userInput = value;
            },
            handleInputChange(value) {
                const newVal = value === '' ? undefined : Number(value);
                if (!isNaN(newVal) || value === '') {
                    this.setCurrentValue(newVal);
                }
                this.userInput = null;

            },
            toPrecision(num, precision) {
                return parseFloat(Math.round(num * Math.pow(10, precision)) / Math.pow(10, precision));
            },
            setCurrentValue(newVal) {
                const oldVal = this.value;
                if (typeof newVal === 'number') {
                    newVal = this.toPrecision(newVal, this.precision);
                }
                if (this.max != -1 && newVal >= this.max) newVal = this.max;
                if (newVal <= this.min) newVal = this.min;
                if (oldVal === newVal) return;
                this.userInput = null;
                this.$emit('input', newVal);
                this.$emit('change', newVal, this.state);
            },
        }



    }


</script>

<style scoped>
    /deep/ .has-suffix .el-input-group__append {
        background-color: #fff;
        padding: 0 8px;
        font-size: 12px;
    }

    /deep/ .has-suffix .el-input-group__append {
        border-left: 0;
        border-top-left-radius: 0;
        border-bottom-left-radius: 0;
    }

    /deep/ .has-suffix .el-input__inner {
    }
</style>
