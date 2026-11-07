

export default (option = {}) => {


    let mixins = {
        data() {
            return {
                data: [],
                model: {},
                params: {},
                loading: false,
                api: {},
                page: { pageIndex: 1, pageSize: 10, total: 0 },
                orderBy: '',
                isShow: false,
                selectId: null,
                selectIds: [],
                auth: {}


            }
        },

        computed: {

            rowKey() {
                return option.rowKey || 'Id'
            }
        },
        created() {
            this.isShow = option.isShow || false;
            this.api = option.api;
        },

        methods: {
            initPageAuth(baseCode) {
             
                var auth = {};
                const codes = ['view', 'add', 'edit', 'del'];
                for (const code of codes) {
                    let psCode = code + baseCode;
                    auth[psCode] = this.$canDo(psCode);

                }
                this.auth = auth;

            },
            getRowIndex(index) {
                return (this.page.pageIndex - 1) * this.page.pageSize + index + 1

            },

            showForm(name, id) {

                var form = this.$refs[name];
                form.show(id);


            },


            show(id) {
                this.isShow = true;
                if (id || id==0)
                    this.getRow(id);
                else
                    this.model = Object.assign({}, this.emptyModel || {});
            },

            hide() {
                this.isShow = false;
                this.$emit('refresh', this.model);

            },


            getList() {

                const callback = () => {

                    this.loading = true;
                    var queryParams = { pageIndex: this.page.pageIndex, pageSize: this.page.pageSize, Query: this.params }
                    if (this.processParams) {

                        queryParams = this.processParams(this.params)
                    }

                    var api = this.api[option.list || 'list'] || this.api.getList;

                    api(queryParams).then(res => {

                        this.loading = false;
                        if (res.IsSuccess) {
                            this.page.total = res.Data.Total;
                            if (this.processData) {
                                this.data = this.processData(res.Data);

                            } else {
                                this.data = res.Data.Rows;
                            }

                            this.$emit('listDataChange', this.data);

                        } else {

                            this.$errTip(res.Msg);
                        }

                    })

                }
                if (this.listBefore) {
                    this.listBefore()
                }
                callback()
            },


            getRow(id) {

                const callback = () => {
                    this.loading = true;
                    this.api[option.get || 'get'](id).then(res => {
                        this.loading = false;
                        if (res.IsSuccess) {

                            if (this.processData) {
                                this.model = this.processData(res.Data);

                            } else {
                                this.model = res.Data;
                            }


                         
                        } else {
                            this.$errTip(res.Msg);
                        }

                    })

                }

                callback()
            },


            addRow() {

                const callback = () => {
                    this.loading = true;          
                    this.api[option.add || 'create'](this.model).then((res) => {
                        if (res.IsSuccess) {
                           
                            if (this.addAfter) {
                                this.addAfter(res)
                            } else {
                              
                                this.$okTip('新增成功')
                                this.$emit("change", this.model);
                                this.hide();
                            }
                        } else {
                            this.$errTip(res.Msg);
                        }
                        this.loading = false;

                    })

                }

                if (this.addBefore) {
                    this.addBefore()
                }

                this.$refs[option.formId || 'form'].validate(valid => {

                    if (valid) callback();
                })

            },


            updateRow() {

                const callback = () => {
                    this.loading = true;
                    this.api[option.update || 'update'](this.model).then((res) => {
                        if (res.IsSuccess) {
                          
                            if (this.updateAfter) {
                                this.updateAfter(res)
                            } else {

                                this.$okTip('编辑成功')
                                this.$emit("change", this.model);
                                this.hide();

                            }
                        } else {
                            this.$errTip(res.Msg);
                        }
                        this.loading = false;

                    })

                }

                if (this.updateBefore) {
                    this.updateBefore()
                }

                this.$refs[option.formId || 'form'].validate(valid => {

                    if (valid) callback();
                })




            },


            multiDelRow() {

                if (this.selectIds.length == 0) {

                    this.$errTip("请选择要删除的数据")
                    return;
                }

                const callback = () => {

                    this.api[option.multiDel || 'multiDelete'](this.selectIds).then((res) => {

                        if (res.IsSuccess) {
                            this.getList();
                            this.$okTip('删除成功');
                        }
                        else {
                            this.$errTip(res.Msg);
                        }

                    })
                }

                var tip = `将永久删除所选数据, 是否继续?`;
                if (this.multiDelTip) {

                    tip = this.multiDelTip(row, index);
                }

                if (this.multiDelBefore) {
                    this.multiDelBefore()
                    callback()
                } else {
                    this.$confirm(tip, '提示', {
                        confirmButtonText: '确定',
                        cancelButtonText: '取消',
                        type: 'warning'
                    }).then(() => {
                        callback()
                    })
                }
            },


            delRow(row, index) {
                const callback = () => {

                    this.selectId = row[this.rowKey];

                    this.api[option.del || 'delete'](this.selectId).then((res) => {

                        if (res.IsSuccess) {
                          
                            if (this.delAfter) {
                                this.delAfter(res, row, index)
                            } else {
                                this.getList();
                                this.$okTip('删除成功')
                            }
                        }
                        else {
                            this.$errTip(res.Msg);
                        }

                    })
                }

                var tip = `将永久删除序号【${index}】的数据, 是否继续?`;
                if (this.delTip) {

                    tip = this.delTip(row, index);
                }

                if (this.delBefore) {
                    this.delBefore()
                    callback()
                } else {
                    this.$confirm(tip, '提示', {
                        confirmButtonText: '确定',
                        cancelButtonText: '取消',
                        type: 'warning'
                    }).then(() => {
                        callback()
                    })
                }
            },
            searchChange() {
                this.page.pageIndex = 1;
                this.getList();
            },
            refreshChange(clear) {
                if (clear === true) this.$emptyObject(this.params);
                this.getList();
            },
            sizeChange(val) {
                this.page.pageIndex = 1
                this.page.pageSize = val
                this.getList()
            },
            currentChange(val) {
                this.page.pageIndex = val
                this.getList()
            },

            selectionChange(selection) {

                var ids = [];
                var rowKey = this.rowKey;
                selection.forEach(function (m) {
                    ids.push(m[rowKey]);

                });
                this.selectIds = ids;
            }

        }
    }

    return mixins;
}
