<template>
    <div>
        <el-form-item label="">

            <div v-if="model.props.Cases.length<=1"><span class="case-title">IF</span>   <el-button style="margin-left:20px;" icon="el-icon-plus" size="mini" @click="handleAddGroup">添加条件</el-button></div>

            <template v-else>

                <template v-for="(item,index) in model.props.Cases">

                    <div v-if="!item.IsElse" :key="index">
                        <div class="group">
                            <div class="group-left">
                                <div class="group-title">
                                    <div class="group-title-name">
                                        {{index==0?"IF":"ELIF"}}
                                    </div>
                                    <div class="group-title-index">Case{{index+1}}</div>

                                </div>
                                <template v-if="item.Conditions.length>1">
                                    <div class="logic-btn-wrap">
                                        <el-link :underline="false" @click="handleSwichLogicalOperator(item)">  {{item.LogicalOperator}}<i class="el-icon-refresh"></i> </el-link>
                                    </div>

                                    <div class="bracket">
                                    </div>
                                </template>
                            </div>

                            <div class="group-item-wrap">
                                <div class="group-item" v-for="(cdt,index2) in item.Conditions" :key="index2+'cdt'">
                                    <div class="group-item-middle">
                                        <table class="table">
                                            <tr>
                                                <td>
                                                    <VariableEditor :minHeight="25" :node="nodeModel"  v-model="cdt.Left" :readonly="true" @input="propsChange"></VariableEditor>
                                                </td>
                                                <td width="85" align="center">
                                                    <el-select v-model="cdt.ComparisonOperator" placeholder="">
                                                        <el-option :label="op.label" :value="op.value" v-for="(op,opIndex) in getComparisonOperators(cdt.Left)" :key="opIndex+'op'"> </el-option>
                                                    </el-select>
                                                </td>
                                            </tr>

                                            <tr v-if="cdt.ComparisonOperator!='empty'&&cdt.ComparisonOperator!='notempty'">
                                                <td colspan="2">
                                                    <VariableEditor :minHeight="25" :node="nodeModel" :allowTypes="getRightAllowTypes(cdt.Left)" v-model="cdt.Right" @input="propsChange"></VariableEditor>
                                                </td>
                                            </tr>
                                        </table>

                                    </div>
                                    <div class="group-item-right">
                                        <el-link icon="el-icon-delete" :underline="false" @click="handleRemoveCdt(item.Conditions,index2,index)"></el-link>

                                    </div>
                                </div>
                            </div>

                        </div>

                        <div class="group-cmd-wrap">

                            <el-button icon="el-icon-plus" size="mini" @click="handleAddCdt(item.Conditions)">添加条件</el-button>
                            <el-link icon="el-icon-delete" style="margin-left:100px" :underline="false" @click="handleRemoveGroup(index)">移除</el-link>
                        </div>
                        <el-divider class="divider"></el-divider>

                    </div>


                </template>

                <div class="cmd-esle-wrap" @click="handleAddGroup">
                    <el-link icon="el-icon-plus" :underline="false">ELIF</el-link>
                </div>
            </template>
            <el-divider class="divider"></el-divider>
            <div class="group-else">
                <div>ELSE</div>
                <div>用于定义当 if 条件不满足时应执行的逻辑</div>

            </div>

        </el-form-item>

    </div>
</template>

<script>
    import flow from '@/mixins/flow.js'
    import VariableEditor from './VariableEditor.vue'

    export default {
        name: 'CaseNodePropEditor',
        mixins: [flow],
        components: {
            VariableEditor
        },
        props: {

            model: Object,
            lf: null

        },
        data() {
            return {

            }
        },

        watch: {
            ////会导致死循环，因为赋值edgids
            //"model.props.Cases": {
            //    handler(newValue, oldValue) {
            //        this.$eventHub.$emit('node-prop-change');
            //    },
            //    deep: true
            //}

        },

        computed: {
            nodeModel() {
                let model = this.lf.getNodeModelById(this.model.id);
                return model;
            }
        },

        methods: {

            propsChange() {
                this.$eventHub.$emit('node-prop-change');

            },

            handleSwichLogicalOperator(item) {

                if (item.LogicalOperator == 'AND') {
                    item.LogicalOperator = 'OR';
                } else {
                    item.LogicalOperator = 'AND';
                }

                this.propsChange();

            },
            layoutCaseEdges() {             
                this.nodeModel.layoutCaseEdges();
            },

            handleAddCdt(cdts) {

                cdts.push({ Left: "", ComparisonOperator: "", Right: "" });
                this.propsChange();
                this.layoutCaseEdges();
            },

            removeCaseEdges(edgeIds) {
                if (!edgeIds) return;
                for (let id of edgeIds) {
                    this.lf.deleteEdge(id);

                }
            
            },

            handleRemoveCdt(cdts, index, grpIndex) {
                cdts.splice(index, 1);
                if (cdts.length == 0) {
                    this.removeCaseEdges(this.model.props.Cases[grpIndex].EdgeIds);
                    this.model.props.Cases.splice(grpIndex, 1);
                }
                this.propsChange();
                this.layoutCaseEdges();
            },

            handleAddGroup() {
                this.model.props.Cases.splice(this.model.props.Cases.length - 1, 0, { LogicalOperator: 'AND', Conditions: [{ Left: "", ComparisonOperator: "", Right: "" }] });
                this.propsChange();
                this.layoutCaseEdges();
            },

            handleRemoveGroup(index) {
                this.removeCaseEdges(this.model.props.Cases[index].EdgeIds);
                this.model.props.Cases.splice(index, 1);
                this.propsChange();
                this.layoutCaseEdges();

            },


            getRightAllowTypes(left) {
                if (!left) return [];
                var variables = this.getParentNodeVariables(this.nodeModel);
                var variable = this.tranVariableTextToObject(variables, left);
                if (!variable) return [];
                var type = variable.type;
                if (type == 'String') {
                    return ['String', 'Number'];
                }
                if (type == 'Number') {
                    return ['Number'];
                }
                //else if (type == 'Array[String]') {

                //    return ['String'];

                //}
                //else if (type == 'Array[Number]') {
                //    return ['Number'];
                //}
                return [];

            },

            getComparisonOperators(left) {

                const ops = this.$consts.logicOperators;
                if (!left) return [];
                var variables = this.getParentNodeVariables(this.nodeModel);
                var variable = this.tranVariableTextToObject(variables, left);
                if (!variable) return [];
                return ops.filter(x => x.types.indexOf(variable.type) >= 0).map(x => { return { label: x.name, value: x.value || x.name } });

            },











        }


    }</script>

<style scoped>
    .group-else {
        line-height: 20px !important;
    }

    .group-title {
        width: 40px;
        height: 40px;
        line-height: 18px;
    }

        .group-title .group-title-name {
            font-weight: bold;
        }


    /deep/ .table .el-input__inner {
        border: none !important;
    }

    .logic-btn-wrap {
        background-color: #fff;
        border: solid 1px#DCDFE6;
        position: absolute;
        right: 5px;
        top: 50%;
        transform: translateY(-50%);
        width: 50px;
        border-radius: 4px;
        z-index: 10;
        display: flex;
        justify-content: center;
        align-items: center;
        height: 25px;
    }

    .group-cmd-wrap {
        padding-left: 80px;
        margin-top: 5px;
    }

    /deep/ .logic-btn-wrap .el-link {
        color: #296dff !important;
        font-weight: bold !important;
    }

    .bracket {
        position: absolute;
        bottom: 10px;
        top: 10px;
        right: 3px;
        width: 10px;
        border: solid 1px #DCDFE6;
        border-radius: 8px 0 0 8px;
        border-right: 0;
    }

    .group {
        display: flex;
        flex-direction: row;
    }

    .group-left {
        width: 80px;
        position: relative;
    }

    .group-item-wrap {
        flex: 1;
    }

    .case-title {
        font-weight: bold;
    }



    .divider {
        margin: 10px 0;
    }

    .cmd-esle-wrap {
        text-align: center;
        background-color: #DCDFE6;
        border-radius: 4px;
        cursor: pointer;
    }

    .group-item {
        display: flex;
        flex-direction: row;
    }

        .group-item:not(:last-child) {
            margin-bottom: 5px;
        }



    .group-item-middle {
        flex: 1;
    }


    .group-item-right {
        width: 30px;
        text-align: center;
    }

   

    /deep/ .table .editor {
        border: none;
    }
</style>

<style scoped src="../PropEditor.css"></style>
