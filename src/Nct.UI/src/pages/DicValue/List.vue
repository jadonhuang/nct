<template>

    <el-container class="nct-hastree-container">
        <el-aside width="200px">

            <DicTree parentCode="dic" title="字典分类" :canEdit="true" @change="codeChange"></DicTree>

        </el-aside>

        <el-main class="nct-work-area">

            <div class="nct-cmd-area">

                <div class="nct-toolbar">
                    <div style="padding:0 0 10px 0;font-size:14px;">

                        当前字典：{{dic.Id?dic.Name:'暂未选择'}}

                    </div>

                    <el-button icon="el-icon-plus" type="primary" size="small" @click="addRow(-1)" v-if="$canDo('adddicvalue')"> 添加</el-button>
                    <el-button icon="el-icon-circle-plus-outline" type="primary" size="small" @click="showQuickAddForm" v-if="$canDo('adddicvalue')"> 批量添加</el-button>

                    <el-button icon="el-icon-circle-check" type="primary" plain :disabled="!dic.Id" size="small" @click="save" v-if="$canDo('adddicvalue')"> 保存</el-button>

                    <div class="right-tool">
                        <el-button icon="el-icon-refresh" size="small" circle @click="getValues"></el-button>

                    </div>
                </div>
            </div>

            <el-table ref="dicTable" :data="data" border element-loading-text="拼命加载中" row-key="rid" size="small" v-loading="loading" height="200" class="nct-list-table edit-table">


                <el-table-column width="60" align="center" class-name="selection">
                    <template>
                        <i class="el-icon-rank" style="font-size:18px;" title="拖动排序"></i>
                    </template>
                </el-table-column>


                <el-table-column prop="Name" label="名称" min-width="60">
                    <template slot-scope="scope">
                        <el-input v-model="scope.row.Name" size="small"></el-input>

                    </template>
                </el-table-column>

                <el-table-column prop="Value" label="值" min-width="60">
                    <template slot-scope="scope">
                        <el-input v-model="scope.row.Value" size="small"></el-input>
                    </template>
                </el-table-column>


                <el-table-column prop="Color" label="颜色" min-width="60">
                    <template slot-scope="scope">

                        <el-color-picker v-model="scope.row.Color" size="small" style="display:inherit"
                                         show-alpha
                                         :predefine="$consts.predefineColors">
                        </el-color-picker>

                    </template>
                </el-table-column>

                <el-table-column label="操作" width="180" align="center">
                    <template slot-scope="scope">

                        <el-link type="primary" :underline="false" icon='el-icon-plus' @click="addRow(scope.$index)">插入</el-link>
                        <el-link type="primary" :underline="false" icon='el-icon-delete' @click="delRow(scope.$index)">删除</el-link>

                    </template>
                </el-table-column>
            </el-table>


        </el-main>

        <QuickAdd ref="quickAdd" @change="optionChange"> </QuickAdd>

    </el-container>





</template>
<script>

    import Sortable from 'sortablejs'
    import QuickAdd from './QuickAdd.vue';

    export default {
        name: 'dicvaluelist',
        permissions: 'viewdicvalue',
        data() {
            return {
                data: [],
                code: '',
                dic: {},
                loading: false,

            }
        },

        methods: {
            showQuickAddForm() {
                if (!this.dic.Id) {
                    return this.$errTip("请选择字典分类");
                }
                this.$refs.quickAdd.show();

            },
            optionChange(opts) {
                var value = this.data.length + 1;
                for (var opt of opts) {
                    var row = { rid: this.$newId(), Id: 0, Name: opt, Value: String(value)};
                    this.data.push(row);
                    value++
                }

            },
            bindTableRowDrop(selector, tableData) {

                var self = this;
                const el = document.querySelector(`.${selector} .el-table__body-wrapper tbody`)
                new Sortable(el, {
                    animation: 150,
                    setData: self.setData,
                    handle: '.selection',
                    onEnd: function (evt) {
                        var list = tableData;
                        const oldItem = list[evt.oldIndex]
                        list.splice(evt.oldIndex, 1)
                        list.splice(evt.newIndex, 0, oldItem)


                    }

                });

            },

            setData(dataTransfer) {
                dataTransfer.setData('Text', '');
            },

            codeChange(dic) {
                this.code = dic.Code;
                this.dic = dic;
                this.getValues();

            },

            delRow(index) {

                this.data.splice(index, 1);

            },
            addRow(index) {
                if (!this.dic.Id) {
                    this.$errTip("请选择字典分类");
                    return;
                }

                var value = (this.data.length + 1).toString();
                var row = { rid: this.$newId(), Id: 0, Name: '', Value: value};
                if (index == -1)
                    this.data.push(row);
                else
                    this.data.splice(index, 0, row);

            },
            getValues() {
                this.loading = true;
                $dicValueApi.getValuesByCode(this.code).then(res => {

                    var values = res.Data;
                    for (var v of values) {
                        v.rid = this.$newId();
                    }
                    this.data = values;
                    this.loading = false;

                    this.$nextTick(function () {
                        this.bindTableRowDrop('nct-list-table', this.data);

                    })

                });
            },
            save() {

                var msg = '';
                var index = 0;
                for (var row of this.data) {
                    index++;
                    if (this.$isEmptyStr(row.Name)) {
                        msg = `第${index}行名称不能为空`;
                        break;

                    }
                    if (this.$isEmptyStr(row.Value)) {
                        msg = `第${index}行值不能为空`;
                        break;

                    }


                }

                if (msg) {

                    this.$errTip(msg);
                    return;
                }

                var dto = { DicId: this.dic.Id, Values: this.data,Name:'' }

                $dicValueApi.multiCreate(dto).then(res => {

                    if (res.IsSuccess) {
                        this.$okTip('保存成功');
                        this.getValues();
                    } else {
                        this.$errTip(res.Msg);
                    }

                });


            },


        },
        mounted() {

        },
        components: { QuickAdd }


    }


</script>

