<template>

    <BaseNode v-bind="$props">
        <div class="cdt" v-for="cdt in properties.Cdts" :key="cdt.Id">
            <div class="cdt-text">{{cdt.Text}}</div>
        </div>
    </BaseNode>

   

</template>
<script>

    import BaseNode from './BaseNode.vue';
    export default {
        components: {
            BaseNode
        },
        props: {
            model: Object,
            isSelected: Boolean,
            isHovered: Boolean,
            disabled: Boolean,
            properties: Object,
            text: String,
        },

        data() {
            return {

            }
        },


    }


</script>

