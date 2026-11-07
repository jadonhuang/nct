<template>
    <div class="item">
        <template v-if="prop.Layout==1">
            <el-table :data="value" border size="small" class="table"
                      style="width: 100%">

                <el-table-column label="序号" width="50">
                    <template slot-scope="scope">
                        <span>{{ scope.$index + 1 }}</span>
                    </template>
                </el-table-column>

                <el-table-column :prop="item.Name" :key="'col'+item.Id+item.Title" v-for="(item) in prop.Items"
                                 :label="item.Title"
                                 :min-width="item.Span">
                    <template slot="header">
                        <label :style="getItemTitleStyle(item,nctForm.designData.Theme)">
                            {{item.Title}}
                        </label>
                    </template>
                    <template slot-scope="scope">
                        <el-form size="medium" label-width="0px" :model="scope.row" v-if="state.ChildStates[scope.$index][item.Id].Visible">
                            <form-item :prop="item.Name" class="table-sub-item" :parentModel="nctForm.formData" :rules="item.$Rules">
                                <component :is="getItemRenderName(item)" v-on="prop.$Events" :state="state.ChildStates[scope.$index][item.Id]" v-model="scope.row[item.Name]" :prop="item"></component>
                            </form-item>
                        </el-form>
                    </template>

                </el-table-column>

                <el-table-column label="操作" width="150" align="center" v-if="!state.Readonly">
                    <template slot="header">
                        <el-link type="primary" :underline="false" v-if="state.AddBtn>0" :disabled="state.AddBtn==2" icon="el-icon-plus" @click="addRow(null,-1)">添加</el-link>
                    </template>

                    <template slot-scope="scope">
                        <el-link type="primary" :underline="false" v-if="state.AddBtn>0" :disabled="state.AddBtn==2" icon='el-icon-plus' @click="addRow(null,scope.$index)">插入</el-link>
                        <el-link type="primary" :underline="false" v-if="state.ChildStates[scope.$index].DelBtn>0" :disabled="state.ChildStates[scope.$index].DelBtn==2" icon='el-icon-delete' @click="delRow(scope.$index)">删除</el-link>

                    </template>
                </el-table-column>
            </el-table>

        </template>
        <template v-else>

            <div class="form-card">
                <el-card :key="index" v-for="(row,index) in value" shadow="never">
                    <div slot="header" class="clearfix">
                        <span>第{{index+1}}行</span>
                        <span class="actions" v-if="!state.Readonly">
                            <el-link type="primary" :underline="false" v-if="state.AddBtn>0" :disabled="state.AddBtn==2" icon='el-icon-plus' @click="addRow(null,index)">插入</el-link>
                            <el-link type="primary" :underline="false" v-if="state.ChildStates[index].DelBtn>0" :disabled="state.ChildStates[index].DelBtn==2" icon='el-icon-delete' @click="delRow(index)">删除</el-link>

                        </span>
                    </div>

                    <ItemGroup :items="prop.Items" :state="state.ChildStates[index]" :model="row" :parentModel="nctForm.formData"></ItemGroup>

                </el-card>
            </div>
            <div v-if="!state.Readonly&&state.AddBtn>0">
                <el-link type="primary" :underline="false" :disabled="state.AddBtn==2" icon='el-icon-plus' @click="addRow(null,-1)">添加</el-link>
            </div>
        </template>


    </div>
</template>
<script>

    import formMixin from '@/mixins/form.js'
    import emitter from '@/mixins/emitter';
    export default {
        name: 'TableItem',
        componentName: 'TableItem',
        mixins: [formMixin, emitter],
        inject: ['nctForm'],
        props: {
            state: Object,
            value: Array,
            prop: Object,

        },
        data() {
            return {
                value2: [],
            }
        },
        watch: {


        },
        methods: {

            async addRow(rowData, index) {

                let row = this.nctForm.getRowData(this.prop.Items, rowData || {})

                let itemState = await this.nctForm.buildTableRowState(this.prop, this.state, row);

                if (index == -1) {
                    this.value.push(row);
                    this.state.ChildStates.push(itemState);
                }
                else {
                    if (index > 0) index--;
                    this.value.splice(index, 0, row);
                    this.state.ChildStates.splice(index, 0, itemState);

                }
                this.dispatch('ElFormItem', 'el.form.change', this.value);
                this.$emit("addRow", row, index);


            },
            delRow(index) {

                let row = this.value[index];
                this.value.splice(index, 1);
                this.state.ChildStates.splice(index, 1);

                this.dispatch('ElFormItem', 'el.form.change', this.value);
                this.$emit("delRow", row, index);
            }


        },
        mounted() {
            this.$on('table.addrow', async (data) => {
                if (data.name !== this.prop.Name) return;
                await this.addRow(data.row, data.index);

            });

            this.$on('table.delrow', (data) => {
                if (data.name !== this.prop.Name) return;
                this.delRow(data.index);

            });
        }





    }


</script>

<style scoped>
    .actions {
        float: right;
    }

    /deep/ .actions .el-link {
        margin-left: 10px;
    }

    /deep/ .form-card .el-card {
        margin-bottom: 20px;
    }

    .form-card .el-card:last-child {
        margin-bottom: 0px;
    }

    /deep/ .form-card .el-card__header {
        padding-bottom: 5px !important;
        padding-top: 5px !important;
    }

    /deep/ .form-card .el-form-item {
        margin-bottom: 22px;
    }

    /deep/ .table-sub-item .el-form-item__error {
        padding: 3px 6px;
        background: #f56c6c;
        color: #fff;
        border-radius: 4px;
        z-index: 100;
        margin-top: -8px;
    }

        /deep/ .table-sub-item .el-form-item__error::after {
            width: 0;
            height: 0;
            display: block;
            content: "";
            position: absolute;
            left: 10px;
            top: -8px;
            z-index: 5;
            border-bottom: 4px solid #f56c6c;
            border-left: 4px solid transparent;
            border-right: 4px solid transparent;
            border-top: 4px solid transparent;
            overflow: hidden;
        }


    /deep/ .table-item .el-form-item__content {
    }

    /deep/ .table .cell {
        overflow: initial;
    }

    /deep/ .table .el-table__body-wrapper {
    }
</style>
