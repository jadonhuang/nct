<template>
  <el-select v-model="selectValue" placeholder="请选择" :size="size" class="w100" clearable filterable>
    <el-option v-for="item in items"
               :key="item.Value"
               :label="item.Name"
               :value="item.Value">
    </el-option>


  </el-select>
</template>
<script>

    export default {
        props: {
            value: String,
            parentCode: String,
            size: { type: String, default:'medium' }

        },

    data() {
      return {
        items: [],
        selectValue:  this.value
      }
    },
    watch: {
      selectValue: function (d) {
        this.$emit('input',d);
        
      },
      value: function (d) {
        this.selectValue = d;

      },
      parentCode: function (d)
      {
        this.getValues();
      }
    },
    methods: {
      getValues() {

            $dicValueApi.getValuesByCode(this.parentCode).then(res => {

          this.items = res.Data;

        })



      }
     

    },
    created() {
      this.getValues();
    }







  }
</script>
