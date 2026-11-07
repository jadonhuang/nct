<template>
    <div>
        <el-form-item>
            <div class="title-wrap">输入变量 <span class="type-tip">Array</span></div>

            <VariableEditor :minHeight="30" :allowTypes="['Array[String]','Array[Number]','Array[Object]']" :node="nodeModel" v-model="model.props.Input" :readonly="true" @input="propsChange"></VariableEditor>

            <div class="title-wrap">输出变量  <span class="type-tip">Array</span></div>

            <VariableEditor :minHeight="30" :node="nodeModel" :vars="outputVars" v-model="model.props.Output" :readonly="true" @input="propsChange"></VariableEditor>

            <div class="title-wrap">错误处理方式</div>

            <el-select v-model="model.props.ErrHandleMode" style="width:100%" @change="propsChange">
                <el-option label="终止执行" value="stop"></el-option>
                <el-option label="继续执行" value="continue"></el-option>
            </el-select>

        </el-form-item>
     
  

       

    </div>
</template>

<script>
    import VariableEditor from './VariableEditor.vue'
    import flow from '@/mixins/flow.js'
    export default {
        mixins: [flow],
        name: 'IterationNodePropEditor',
        components: {
            VariableEditor
        },
        props: {

            model: Object,
            lf: null,

        },
        data() {
            return {
   
                
            }
        },



        computed: {
            nodeModel() {
                return this.lf.getNodeModelById(this.model.id);
            },
            outputVars() {

                let children = this.nodeModel.getChildren();
                let graphModel = this.nodeModel.graphModel;
                let nodeModels = [];
                for (let child of children) {
                    const nodeModel = graphModel.getNodeModelById(child);
                    nodeModels.push(nodeModel);

                }
                return this.getNodeVariables(nodeModels);
          
            }
    

        },

        methods: {
            propsChange() {
                this.$eventHub.$emit('node-prop-change');

            },
           




        }


    }</script>

<style scoped>
    .type-tip {
       font-size:12px;
       position:absolute;
       right:0;
       top:0;
       color:#808080;
       font-weight:normal;
    
    }
    .title-wrap {
        height: 30px;
        line-height: 30px;
        font-weight: bold;
        color: #606266;
        position: relative;
    }

 

  
</style>

