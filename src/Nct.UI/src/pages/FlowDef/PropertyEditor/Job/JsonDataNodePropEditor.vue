<template>
    <div>
        <el-form-item>
            <div class="title-wrap">输入变量 <span class="type-tip">String</span></div>
            <VariableEditor :minHeight="30" :allowTypes="['String']" :node="nodeModel" v-model="model.props.Input" :readonly="true" @input="propsChange"></VariableEditor>
        </el-form-item>


        <el-form-item>
            <div class="title-wrap">输出变量 <el-link title="新增变量" icon="el-icon-circle-plus-outline" @click="addDataItem()" :underline="false"></el-link></div>
            <table class="table">
                <tr>
                    <th>变量名</th>
                    <th>JsonPath</th>
                    <th  width="130">变量类型</th>
                </tr>

                <tr v-for="(item,index) in model.props.DataItems" :key="index">
                    <td>
                        <el-input v-model="item.Name"  size="mini" />
                    </td>
                    <td>
                        <el-input v-model="item.Path"  size="mini" />
                    </td>
                    <td>
                        <div class="wrap">
                            <div class="var-right">
                                <el-select v-model="item.DataType"  size="mini" style="width:100%" @change="propsChange">
                                    <el-option :label="op.name" :value="op.value||op.name" v-for="op in $consts.variableTypes" :key="op.name"> </el-option>
                                </el-select>
                            </div>
                            <span class="btn-area">
                                <el-link icon="el-icon-delete" @click="removeDataItem(index)" :underline="false"></el-link>
                            </span>
                        </div>

                    </td>
                  
                </tr>

            </table>

        </el-form-item>






    </div>
</template>

<script>
    import VariableEditor from './VariableEditor.vue'
    export default {
        name: 'JsonDataNodePropEditor',
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
           

        },

        methods: {
            propsChange() {
                this.$eventHub.$emit('node-prop-change');

            },
            removeDataItem(index) {
                this.model.props.DataItems.splice(index, 1);
                this.propsChange();

            },
             addDataItem() {
                 this.model.props.DataItems.push({ Name: "", Path: "", DataType: "" });
                 this.propsChange();

            }
           




        }


    }</script>

<style scoped>
   
    .title-wrap {
        height: 30px;
        line-height: 30px;
        font-weight: bold;
        color: #606266;
        position: relative;
    }

    /deep/ .title-wrap .el-link {
        font-size: 14px;
        margin-left: 50px;
    }


    /deep/ td .el-input__inner {
        border-width:0;
        padding:0 8px;
    }

    .wrap .var-right {
        flex: 1;
    }

    .right {
        text-align: right;
    }

    .wrap {
        display: flex;
        flex-direction: row;
    }

    .btn-area {
        width: 25px;
        text-align: center;
    }

  
</style>

<style scoped src="../PropEditor.css"></style>

