<template>
  <div>
    <div class="input-number-range" :class="{ 'is-disabled': disabled }">
      <div class="flex">
        <div class="from">
          <el-input
            ref="input_from"
            v-model="userInputForm"
            :disabled="disabled"
            placeholder="最小值"
                    size="medium"
            @blur="handleBlurFrom"
            @focus="handleFocusFrom"
            @input="handleInputFrom"
            @change="handleInputChangeFrom"
                    clearable

          ></el-input>
        </div>
      <span class="el-range-separator">至</span>
        <div class="to">
          <el-input
            ref="input_to"
            v-model="userInputTo"
            :disabled="disabled"
            placeholder="最大值"
            @blur="handleBlurTo"
                    size="medium"
            @focus="handleFocusTo"
            @input="handleInputTo"
            @change="handleInputChangeTo"
                    clearable
          ></el-input>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'InputNumberRange',

  props: {
    value: { required: true },

    disabled: {
      type: Boolean,
      default: false
    },

    precision: {
      type: Number,
      default: 0,
      validator(val) {
        return val >= 0 && val === parseInt(val, 10)
      }
    }
  },

  data() {
    return {
      userInputForm: null,
      userInputTo: null
    }
  },

  watch: {
    value: {
      immediate: true,
      handler(value) {
        if (value instanceof Array && this.precision !== undefined) {
          this.userInputForm = typeof value[0] === 'number' ? value[0] : null
          this.userInputTo = typeof value[1] === 'number' ? value[1] : null
        }
      }
    }
  },

  methods: {

    toPrecision(num, precision) {
      if (precision === undefined) precision = 0
      return parseFloat(
        Math.round(num * Math.pow(10, precision)) / Math.pow(10, precision)
      )
    },

    handleBlurFrom(event) {
      this.$emit('blurfrom', event)
    },

    handleFocusFrom(event) {
      this.$emit('focusfrom', event)
    },

    handleBlurTo(event) {
      this.$emit('blurto', event)
    },

    handleFocusTo(event) {
      this.$emit('focusto', event)
    },

    handleInputFrom(value) {
      this.$emit('inputfrom', value)
      // this.userInputFrom = value
    },

    handleInputTo(value) {
      this.$emit('inputto', value)

    },

    // from输入框change事件
    handleInputChangeFrom(value) {
      // 如果是非数字空返回null
      if (isNaN(value) || value === '') {
        this.$emit('input', [null, this.userInputTo])
        this.$emit('changefrom', this.userInputForm)
        return
      }

      // 初始化数字精度
      this.userInputForm = this.setPrecisionValue(value)

      // 如果from > to 将from值替换成to
      if (typeof this.userInputTo === 'number') {
        this.userInputForm =
          parseFloat(this.userInputForm) <= parseFloat(this.userInputTo)
            ? this.userInputForm
            : this.userInputTo
      }
      this.$emit('input', [this.userInputForm, this.userInputTo])
      this.$emit('changefrom', this.userInputForm)
    },

    // to输入框change事件
    handleInputChangeTo(value) {
      // 如果是非数字空返回null
      if (isNaN(value) || value === '') {
        this.$emit('input', [this.userInputForm, null])
        this.$emit('changefrom', this.userInputTo)
        return
      }

      // 初始化数字精度
      this.userInputTo = this.setPrecisionValue(value)

      if (typeof this.userInputForm === 'number') {
        this.userInputTo =
          parseFloat(this.userInputTo) >= parseFloat(this.userInputForm)
            ? this.userInputTo
            : this.userInputForm
      }
      this.$emit('input', [this.userInputForm, this.userInputTo])
      this.$emit('changeto', this.userInputTo)
    },

    // 设置成精度数字
    setPrecisionValue(value) {
      if (this.precision !== undefined) {
        const val = this.toPrecision(value, this.precision)
        return val
      }
      return null
    }
  }
}
</script>
<style  scoped>

/deep/ .el-input .el-input__inner {
  border: 0px;
  margin: 0;
  padding: 0 15px;
  background-color: transparent;
}
.input-number-range {
  background-color: #fff;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
}
.flex {
  display: flex;
  flex-direction: row;
  width: 100%;
  justify-content: center;
  align-items: center;
  
}
.is-disabled {
  background-color: #eef0f6;
  border-color: #e4e7ed;
  color: #c0c4cc;
  cursor: not-allowed;
}
</style>
