<template>
    <div class="item" v-if="!state.Viewonly" :style="getLabelStyle">
        <el-button :type="prop.BtnType" :round="prop.Round==1" :size="prop.Size" :icon="prop.Icon" :disabled="state.Readonly" :circle="prop.Round==2" @click="handleClick">
            <font :style="getLabelStyle" v-if="prop.Text">{{prop.Text}}</font>
        </el-button>

    </div>
</template>
<script>
  import form from '@/mixins/form.js'
  export default {
    mixins: [form],
    props: {
      value: String,
        prop: Object,
        state: Object

    },
    data() {
      return {
       
      }
    },
  
    methods: {
        handleClick() {

            this.$emit('click', null, this.state);

        }

    },
    computed: {
      getLabelStyle(){

        return this.getItemTextStyle(this.prop);

      }

    }




  }


</script>
