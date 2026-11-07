<template>
  <div slot="footer">
    test page

    <el-button size="small" icon="el-icon-circle-close" @click="close">取消</el-button>
  </div>



</template>
<script>

 
  import form from '@/mixins/form.js'
  import emitter from '@/mixins/emitter';
  export default {
    mixins: [form, emitter],
  
    data() {
      return {
        isShow: false
       

      }
    },
 
    watch: {

    },
    methods: {
      close() {
        this.dispatch('Dialog', 'dialog.close', [this]);

      }

    },
 


    async created() {

      console.log('******', this.$parent, this.$root, this.$route)
      console.log('******', this)
      console.log(this.$store.state)

    },

    components: {}


  }


</script>


