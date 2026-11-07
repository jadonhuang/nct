<template>

    <BaseNode v-bind="$props">
        <div class="node-item" v-if="properties.ModelType">
            <div class="model-name"> {{ getModelTypeName }}</div>
        </div>

        <div class="node-item"  v-else>         
            <span class="node-tip">未设置模型</span>
        </div>

    </BaseNode>

   

</template>
<script>
    import flow from '@/mixins/flow.js'
    import BaseNode from './BaseNode.vue';
    export default {
        mixins: [flow],
        components: {
            BaseNode
        },
        computed:{
            getModelTypeName() {

                const models = this.model.getModels();
                const model = models.find(m => m.Value == this.properties.ModelType);
                return model?.Name||'';

            }

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
<style scoped>
    .model-name {
    
       margin-left:10px;
    
    }


</style>