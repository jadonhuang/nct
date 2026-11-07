<template>
  <div class="item">
   
    <div v-html="formatTitle(prop.Text)" style="padding-left:5px;padding-right:5px;" :style="getLabelStyle"></div>
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
      formatTitle(text) {
            if (!text) return '';
            return text.replaceAll('\\n', '<br/>').replaceAll('\n', '<br/>');

      },
   


    },
    computed: {
      getLabelStyle(){

        return this.getItemTextStyle(this.prop);

      }

    }




  }


</script>
