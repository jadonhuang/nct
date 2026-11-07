<template>
    <div class="item">
        <el-divider :content-position="prop.TextAlign"><span :style="getLabelStyle">{{prop.Text}}</span></el-divider>
    </div>
</template>
<script>


    import form from '@/mixins/form.js'
    export default {
        mixins: [form],
        props: {
            value: [String, Number],
            prop: Object,
            state: Object


        },
        data() {
            return {
              
            }
        },
        computed: {
            getLabelStyle() {

                return this.getItemTextStyle(this.prop);

            }

        },




    }


</script>
