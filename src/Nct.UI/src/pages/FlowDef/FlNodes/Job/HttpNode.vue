<template>

    <BaseNode v-bind="$props">
        <div class="http-node node-item" :title="properties.Url" v-if="properties.Url">
            <VariableEditor :value="properties.Url" :node="model" :minHeight="40" :enabled="false"></VariableEditor>
        </div>

        <div class="node-item"  v-else>         
            <span class="node-tip">未设置Url</span>
        </div>

    </BaseNode>

   

</template>
<script>
    import flow from '@/mixins/flow.js'
    import BaseNode from './BaseNode.vue';
   import VariableEditor from '../../PropertyEditor/Job/VariableEditor.vue'
    export default {
        mixins: [flow],
        components: {
            BaseNode, VariableEditor
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

 
    
    
    /deep/ .http-node .editor {
        border: none !important;
        white-space: nowrap;
        display: block;
        overflow: hidden;
        text-overflow: ellipsis
    }

</style>

