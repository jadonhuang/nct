<template>
    <el-dialog title="字典管理" :visible.sync="isShow" :close-on-click-modal="false" append-to-body>
        <el-form :model="model" label-width="100px" :rules="rules" ref="form" size="medium">

            <el-form-item label="字典名称：" prop="Name">
                <el-input v-model="model.Name"></el-input>
            </el-form-item>

            <el-form-item label="字典选项：" prop="Values" size="mini">
                <el-table ref="dicTable" :data="model.Values" border size="mini" style="width: 100%;margin-bottom:2px;" class="edit-table" row-key="rid" height="300" >

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

                    <el-table-column prop="Name" label="值" min-width="60">
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
                <el-button icon="el-icon-circle-plus-outline" size="small" @click="addRow(-1)">新增</el-button>   
                <el-button icon="el-icon-plus" size="small" @click="showQuickAddForm">批量新增</el-button>
            </el-form-item>




        </el-form>
        <div slot="footer">

            <el-button type="primary" @click="save" :loading="loading" size="small" icon="el-icon-circle-check">提交</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>

        </div>
        <QuickAdd ref="quickAdd" @change="optionChange"> </QuickAdd>
    </el-dialog>
</template>
<script>

    import Sortable from 'sortablejs'
    import QuickAdd from './QuickAdd.vue';
    export default {

        data() {

            var validateValues = (rule, value, callback) => {
                if (this.model.Values.length === 0) {
                    callback(new Error('请添加字典'));
                } else {

                    var index = 0;
                    var err = '';
                    for (var f of this.model.Values) {
                        index++;
                        if (this.$isEmptyStr(f.Name)) {
                            err = `第${index}行名称不能为空`
                            break;
                        }
                        if (this.$isEmptyStr(f.Value)) {
                            err = `第${index}行字段类型不能为空`
                            break;
                        }


                    }
                    if (err) {
                        callback(new Error(err));
                    } else {

                        callback();
                    }
                }



            };

            return {
                isShow: false,
                code: '',
                loading: false,
                model: { Name: '', DicId: 0, Values: [] },
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' }

                    ],
                    Values: [
                        { validator: validateValues }

                    ]


                },




            }
        },

        watch: {
            isShow: function (v) {
                if (!v) {
                    this.$refs.form.clearValidate();

                    this.model.Values = [];
                    this.model.Name = '';
                    this.model.Id = 0;

                }



            },


        },

        methods: {

            showQuickAddForm() {
              
                this.$refs.quickAdd.show();

            },
            optionChange(opts) {
                var value = this.model.Values.length + 1;
                for (var opt of opts) {
                    var row = { rid: this.$newId(), Id: 0, Name: opt, Value: String(value) };
                    this.model.Values.push(row);
                    value++
                }

            },


            setData(dataTransfer) {
                dataTransfer.setData('Text', '');
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


            show(code) {
                this.code = code;
                this.isShow = true;
                if (code) {
                    this.getData();
                } else {

                    this.$nextTick(function () {
                        this.bindTableRowDrop('edit-table', this.model.Values);

                    })

                }

            },

            delRow(index) {

                this.model.Values.splice(index, 1);

            },
            addRow(index) {
                var value = (this.model.Values.length + 1).toString();
                var row = { rid: this.$newId(), Id: 0, Name: '', Value: value };
                if (index>=0) {
                    this.model.Values.splice(index, 0, row);
                } else {
                    this.model.Values.push(row);
                }

            },
            getData() {
                this.loading = true;
                $dicValueApi.getNameAndValuesByCode(this.code).then(res => {

                    var data = res.Data;
                    for (var v of data.Values) {
                        v.rid = this.$newId();
                    }
                    this.model = data;
                    this.loading = false;
                    this.$nextTick(function () {
                        this.bindTableRowDrop('edit-table', this.model.Values);

                    })
                });
            },
            save() {

                this.$refs['form'].validate(valid => {

                    if (valid) {

                        $dicValueApi.multiCreate(this.model).then(res => {
                            if (res.IsSuccess) {
                                this.$okTip('保存成功');
                                this.$emit('change', res.Data);
                                this.isShow = false;                             
                            } else {
                                this.$errTip(res.Msg);
                            }

                        });

                    }
                })


            },


        },


        components: { QuickAdd }


    }
</script>



