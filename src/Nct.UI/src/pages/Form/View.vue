<template>

    <section class="nct-work-area" :id="'view_'+formCode">
        <div :class="{'form-container':true}" :style="formPaddingStyle" v-if="!loading">
            <ItemGroup :items="items" :state="itemStates" :model="formData"></ItemGroup>
        </div>

        <div class="el-dialog__footer" v-if="!loading&&!isNested">
            <el-button type="primary" v-if="!isReadonly&&!isPreview" :loading="saveLoading" size="small" @click="saveData" icon="el-icon-circle-check">{{saveBtnTxt}}</el-button>
            <el-button size="small" icon="el-icon-circle-close" @click="close(false)">{{cancelBtnTxt}}</el-button>
        </div>
        <div style="display:none" v-html="style"></div>
    </section>
</template>
<script>

    import request from '@/api/request.js';
    import form from '@/mixins/form.js'
    import emitter from '@/mixins/emitter';
    export default {
        name: 'FormView',
        componentName: 'FormView',
        mixins: [form, emitter],
        props: {
            isNested: { type: Boolean, default: false },
            args: Object

        },
        provide() {
            return {
                nctForm: this
            };
        },
        data() {
            return {
                saveLoading: false,
                formData: {},
                rawFormData: {},
                items: [],
                allItems: [],
                formCode: null,
                formDef: {},
                designData: {},
                dataId: null,
                loading: true,
                itemData: {},
                bakFormData: {},
                urlArgs: {},
                cascadeItems: {},
                isAdd: true,
                mode: 'edit',
                itemStates: {},
                forceClose: false,
                saveBtnTxt: '提交',
                cancelBtnTxt: '取消',
                css: ''
            }
        },
        computed: {
            style() {
                return `<style>${this.css}</style>`
            },

            formPaddingStyle() {

                let padding = this.designData.Theme.Padding;
                if (this.$isNull(padding)) padding = 20;
                return { padding: padding + 'px' }
            },

            isReadonly() {

                return this.mode == 'view';
            },
            isPreview() {

                return this.mode == 'preview';
            },

            FORM() {
                var state = this.$store.state;
                var obj = { isAdd: this.isAdd, user: state.user, dataId: this.dataId, formData: this.formData, formCode: this.formCode, token: state.token.AccessToken, urlArgs: this.urlArgs };

                var setItemState = this.setItemState;
                var formData = this.formData;
                var isTableItem = this.isTableItem;
                var itemStates = this.itemStates;
                var isTabItem = this.isTabItem;

                obj.saveResult = this.saveResult;
                obj.setItemProp = this.setItemProp;
                obj.getItemProp = this.getItemProp;
                obj.findItem = this.findItem;
                obj.setItemState = setItemState;
                obj.getItemState = this.getItemState;
                obj.showDialog = this.$showDialog;
                obj.showDialogWithValue = this.$showDialogWithValue;
                obj.mode = this.mode;
                obj.$this = this;

                obj.showItem = function (name, index) {

                    setItemState(name, 'Visible', true, index);
                }
                obj.hideItem = function (name, index) {

                    setItemState(name, 'Visible', true, index);
                }

                obj.disableItem = function (name, index) {

                    setItemState(name, 'Readonly', true, index);
                }
                obj.enableItem = function (name, index) {

                    setItemState(name, 'Readonly', false, index);
                }
                obj.setItemOpts = function (name, opts, index) {

                    setItemState(name, 'Opts', opts, index);
                }
                obj.addTableRow = function (name, row, index) {

                    let item = this.findItem(name);
                    if (!isTableItem(item)) {
                        return;
                    }
                    obj.$this.broadcast("TableItem", 'table.addrow', { name, row, index })

                }


                obj.setItemTitle = function (name, title) {

                    if (typeof (title) !== 'string') return;
                    let item = this.findItem(name);
                    if (item) {
                        item.Title = title;
                    }

                }

                obj.setItemText = function (name, text) {

                    if (typeof (text) !== 'string') return;
                    let item = this.findItem(name);
                    if (item && 'Text' in item) {
                        item.Text = text;
                    }
                }

                obj.setTabStatus = function (name, status, index) {

                    let item = this.findItem(name);
                    if (!item || !isTabItem(item)) {
                        return;
                    }
                   
                    let state = itemStates[item.Id];
                    if (state.Tabs[index]) {
                        state.Tabs[index].Status = status;
                    }

                   
                }


                obj.removeTableRow = function (name, index) {

                    let item = this.findItem(name);
                    if (!isTableItem(item)) {
                        return;
                    }

                    obj.$this.broadcast("TableItem", 'table.delrow', { name, index });

                }

                obj.setItemValue = function (name, value, index) {
                    let item = this.findItem(name);
                    if (!item) {
                        return;
                    }
                    if (item.Parent) {
                        let rows = formData[item.Parent.Name];
                        if (index >= 0) {

                            let row = rows[index];
                            if (row) {
                                row[item.Name] = value;
                            }

                        } else {

                            for (let row of rows) {
                                row[item.Name] = value;
                            }

                        }

                    } else {

                        formData[item.Name] = value;
                    }

                }

                obj.getItemValue = function (name, index) {
                    let item = this.findItem(name);
                    if (!item) {
                        return;
                    }
                    if (item.Parent) {
                        let rows = formData[item.Parent.Name];
                        if (index >= 0) {
                            let row = rows[index];
                            if (row) {
                                return row[item.Name];

                            }

                        } else {

                            return rows.map(function (r) { return r[item.Name] });

                        }


                    } else {

                        return formData[item.Name];
                    }

                }

                obj.ajax = function (url, params, httpMethod) {

                    let isPost = httpMethod && httpMethod.toLowerCase() == 'post';
                    if (isPost)
                        return request.post(url, params);

                    return request.get(url, params);

                }
                obj.addValidateRule = function (name, rule) {
                    let item = this.findItem(name);
                    if (!item) {
                        return;
                    }
                    item.$Rules.push(rule);
                }

                obj.addListener = function (name, eventName, event) {

                    let item = this.findItem(name);
                    if (!item) {
                        return;
                    }
                    let eventObj = item.$Events || {};
                    eventObj[eventName] = event;
                    item.$Events = eventObj;


                }

                return obj;

            },


        },
        watch: {

        },
        methods: {

            async closeCallback() {

                if (this.forceClose || this.isReadonly || this.isPreview || this.isNested) return true;

                if (JSON.stringify(this.bakFormData) != JSON.stringify(this.formData)) {

                    return await this.$confirmTip('数据未保存，确认关闭？')
                }

                return true;
            },


            setItemState(name, prop, value, index) {

                let item = this.findItem(name);
                if (!item) {
                    return;
                }

                if (index !== undefined && this.isTableItem(item)) {

                    let state = this.itemStates[item.Id];
                    let childState = state.ChildStates[index];
                    if (childState) {

                        childState[prop] = value;
                    }

                    return;
                }

                if (item.Parent && this.isTableItem(item.Parent)) {

                    let states = this.itemStates[item.Parent.Id];
                    if (index >= 0) {
                        let state = states[index];
                        if (state)
                            state[prop] = value;

                    } else {

                        for (let state of states) {
                            state[prop] = value;

                        }

                    }

                } else {

                    this.itemStates[item.Id][prop] = value;
                }

            },

            getItemState(name, index) {

                let item = this.findItem(name);
                if (!item) {
                    return;
                }

                if (index !== undefined && this.isTableItem(item)) {

                    let state = this.itemStates[item.Id];
                    return state;
                }

                if (item.Parent && this.isTableItem(item.Parent)) {

                    let states = this.itemStates[item.Parent.Id];
                    if (index >= 0) {
                        let state = states[index];
                        return state;

                    } else {

                        return states;

                    }

                } else {

                    return this.itemStates[item.Id];
                }

            },

            setItemProp(name, prop, value) {

                var item = this.findItem(name);
                if (!item) {
                    return;
                }
                if (item[prop] === undefined) {
                    console.error(` 控件 ${name} ${prop}属性不存在`);
                    return;

                }
                item[prop] = value;

            },

            getItemProp(name, prop) {

                var item = this.findItem(name);
                if (!item) {
                    return;
                }

                if (item[prop] === undefined) {
                    console.error(` 控件 ${name} ${prop}属性不存在`);
                    return;

                }
                return item[prop];

            },


            findItem(name) {
                var item;
                var index = name.indexOf('.');
                if (index >= 1) {
                    var parentName = name.substr(0, index);
                    name = name.substr(index + 1);
                    var parent = this.allItems.find(m => m.Name == parentName);
                    if (parent)
                        item = parent.Items.find(m => m.Name == name);

                } else {

                    item = this.allItems.find(m => m.Name == name);
                }

                if (!item) {
                    console.error(`${name} 控件不存在`);
                }
                return item;
            },

            buildValidateRules(item) {

                var rules = [];
                let isTable = this.isTableItem(item);
                if (item.Required) {

                    if (isTable) {
                        let rule = {
                            trigger: 'change',
                            validator: function (rule, value, callback) {
                                var valid = value && value.length > 0
                                if (!valid) {
                                    callback(new Error('请至少添加一条数据'));

                                } else {
                                    callback();
                                }

                            }
                        }
                        rules.push(rule);

                    } else {
                        var trigger = 'blur';
                        var tip = '';
                        if (this.isInputTypeItem(item))
                            tip = `请输入${item.Title}`
                        else if (this.isSelectTypeItem(item)) {
                            trigger = 'change';
                            tip = `请选择${item.Title}`
                        }
                        else if (this.isFileItem(item)) {
                            trigger = 'change'
                            tip = `请上传${item.Title}`
                        }

                        let rule = { required: true, message: tip, trigger: trigger };
                        rules.push(rule);
                    }

                }

                if (isTable && item.MaxRows > 0) {

                    let rule = {
                        trigger: 'change',
                        validator: function (rule, value, callback) {
                            var valid = value && value.length <= item.MaxRows
                            if (!valid) {
                                callback(new Error(`最多只能添加${item.MaxRows}条数据`));

                            } else {
                                callback();
                            }

                        }
                    }
                    rules.push(rule);

                }

                if (item.Type == 'Text' && item.Verify) {

                    let rule = {
                        trigger: 'blur',
                        validator: function (rule, value, callback) {
                            let valid = new RegExp(item.Verify).test(value);
                            if (!valid) {
                                let tip = item.VerifyInfo || '格式不正确';
                                callback(new Error(tip));

                            } else {
                                callback();
                            }

                        }
                    }
                    rules.push(rule);

                }

                item.$Rules = rules;

            },

            async initDesignData() {

                if (this.isPreview && !this.formCode) {
                    this.formDef = window["$formDef"];
                } else {
                    let res = await $formDefApi.getByCode(this.formCode);
                    if (!res.IsSuccess) {
                        this.$errTip('表单不存在')
                        this.close(true);
                        return;
                    }
                    this.formDef = res.Data;
                }
                this.designData = JSON.parse(this.formDef.Data);
                this.ruleFn = this.tryCompileJs(this.formDef.ClientRule, this.$consts.form.dfRuleFnNames);

                let items = this.designData.Items;
                let allItems = this.getAllItems(items);

                for (let item of allItems) {
                    if (!item.Name) item.Name = this.$newId();
                    this.buildValidateRules(item);
                    if (this.isTableItem(item)) {
                        for (let subItem of item.Items) {
                            if (!subItem.Name) subItem.Name = this.$newId();
                            this.buildValidateRules(subItem);
                            subItem.Parent = item;
                        }
                    }
                }


                let cascadeItems = {};
                for (let item of allItems) {
                    if (this.isTableItem(item)) {
                        for (let subItem of item.Items) {
                            let itemList = item.Items.filter(m => this.isDataSourceItem(m) && m.DataSourceType == 2 && m.CascadeParent == subItem.Id);
                            if (itemList.length > 0) {
                                cascadeItems[subItem.Id] = itemList;
                            }

                        }

                    } else {

                        let itemList = allItems.filter(m => this.isDataSourceItem(m) && m.DataSourceType == 2 && m.CascadeParent == item.Id);
                        let tableItems = items.filter(m => this.isTableItem(m));
                        for (let tbItem of tableItems) {

                            let subChildren = tbItem.Items.filter(m => this.isDataSourceItem(m) && m.DataSourceType == 2 && m.CascadeParent == item.Id);
                            for (let child of subChildren) {
                                itemList.push(child)

                            }

                        }

                        if (itemList.length > 0) {

                            cascadeItems[item.Id] = itemList;
                        }

                    }

                }

                this.cascadeItems = cascadeItems;
                this.items = items;
                this.allItems = allItems;

            },

            getCasecadaItemValue(item, formData, row) {
                var parent;
                var rowData = row;
                if (item.DataSourceType == 2 && item.CascadeParent) {
                    if (item.Parent) {

                        parent = this.items.find(m => m.Id == item.CascadeParent);
                        rowData = formData;
                        if (!parent) {
                            parent = item.Parent.Items.find(m => m.Id == item.CascadeParent);
                            rowData = row;
                        }

                    } else {

                        parent = this.items.find(m => m.Id == item.CascadeParent);

                    }

                }

                if (parent) {
                    var data = {};
                    var value = rowData[parent.Name];
                    if (value) {

                        data[item.CascadeField] = value;
                        return data;
                    }

                }
                return null;
            },
            formatItemShowValue(item, value) {

                if (item.DefaultValue !== undefined && this.isAdd) {

                    value = item.DefaultValue;
                }

                if (value && this.isDataSourceItem(item)) {

                    value = this.getItemRealValue(value);
                }

                if (typeof value === 'number' && item.Type != 'Number' && item.Type != 'Switch') {
                    value = String(value);
                }
                return value || null;
            },

            getRowData(items, rawRowData) {
                var rowData = {};
                for (var item of items) {
                    if (!item.Field || this.isTableItem(item)) continue;
                    var name = item.Name;
                    var value = rawRowData[name];
                    rowData[name] = this.formatItemShowValue(item, value);

                }

                rowData["Id"] = rawRowData["Id"] || '';

                if (rawRowData["CreateTime"])
                    rowData["CreateTime"] = rawRowData["CreateTime"];

                return rowData;

            },

            async getDataSourceItemOpts(item, rowData, value) {

                var casecadaValue = !!value ? value : this.getCasecadaItemValue(item, this.formData, rowData);

                //数据字典类型不能关联直接返回全部数据
                if (!casecadaValue && item.DataSourceType == 2 && item.CascadeParent)
                    return [];

                var key = this.getItemDsDataKey(item, casecadaValue)
                if (!this.dsData[key]) {
                    await this.getItemDsData(item, casecadaValue);
                }
                return this.getItemBindData(item, casecadaValue);

            },

            async getItemOpts(item, rowData) {

                if (this.isDataSourceItem(item)) {
                    var data = await this.getDataSourceItemOpts(item, rowData);
                    return data;
                }

                return null;

            },

            async buildItemState(item, rowData, rawRowData) {

                let readonly = this.isReadonly;
                let viewonly = readonly;

                let _readonly = readonly ? true : item.Readonly;
                if (_readonly === undefined) _readonly = false;

                let opts = [];
                if (this.isDataSourceItem(item)) {
                    opts = await this.getItemOpts(item, rowData);
                } else if (this.isFileItem(item)) {
                    let ids = rowData[item.Name];
                    if (ids)
                        opts = await this.getAnnexs(ids.split(','));
                }

                //Row属性用于控件事件回调的时候可以快速获取其他控件的值
                let itemState = { Visible: item.Visible, Readonly: _readonly, Viewonly: viewonly, Row: rowData, Opts: opts };
                itemState.Id = this.$newGuid();

                if (this.isTabItem(item)) {
                    itemState.ActiveTab = "0";
                    let tabStates = [];
                    for (let tab of item.Tabs) {
                        tabStates.push({ Status: 1 });

                    }
                    itemState.Tabs = tabStates;

                } else if (this.isTableItem(item)) {
                    itemState.AddBtn = 1;
                } else if (this.isDataSourceItem(item)) {
                    itemState.Text = "";
                    if (rawRowData) {
                        let value = rawRowData[item.Name];
                        if (value && typeof (value) == 'object')
                            itemState.Text = value.Text;
                    }
                }

                return itemState;
            },
            async buildTableRowState(tableItem, tableItemState, row, rawRow) {

                let childState = { DelBtn: 1 };
                for (let subItem of tableItem.Items) {
                    let subReadonly = tableItemState.Readonly ? true : subItem.Readonly;
                    childState[subItem.Id] = await this.buildItemState(subItem, row, rawRow);
                    childState[subItem.Id].Readonly = subReadonly;

                }

                return childState;

            },

            async initItemStates() {

                var formData = this.formData;
                var rawFormData = this.rawFormData;
                var states = {};

                for (let item of this.allItems) {

                    let itemState = await this.buildItemState(item, formData, rawFormData);
                    if (this.isTableItem(item)) {
                        var rows = formData[item.Name];
                        var rawRows = rawFormData[item.Name]
                        let childStates = [];
                        let index = 0;

                        for (let row of rows) {

                            let childState = await this.buildTableRowState(item, itemState, row, rawRows[index])
                            childStates.push(childState);
                            index++;

                        }
                        itemState.ChildStates = childStates;

                    }
                    states[item.Id] = itemState;

                }

                this.itemStates = states;
            },

            async initFormData() {

                var rawRowData = {};
                if (this.dataId || this.designData.DataSaveType == 2) {
                    var res = await $formDefApi.getData(this.formCode, this.dataId);
                    this.isAdd = !res.Data;
                    rawRowData = res.Data || {};
                }

                var rowData = this.getRowData(this.allItems, rawRowData);

                var tbItems = this.allItems.filter(m => this.isTableItem(m));
                for (var tbItem of tbItems) {
                    var rows = [];
                    var rawRows = rawRowData[tbItem.Name] || [];
                    for (var rawRow of rawRows) {
                        var subRowData = this.getRowData(tbItem.Items, rawRow);
                        rows.push(subRowData);
                    }
                    rowData[tbItem.Name] = rows;

                }

                this.formData = rowData;
                this.rawFormData = rawRowData;

            },

            async close(force) {

                this.forceClose = force || false;
                if (this.pageIsInDialog) {
                    this.$closeDialog();
                } else {
                    var result = await this.closeCallback();
                    if (result)
                        this.$closePage();
                }

            },
            calPageIsInDialog() {

                var parent = this.$parent;
                var name = parent.$options.componentName;

                while (parent && (!name || name !== 'Dialog')) {
                    parent = parent.$parent;
                    if (parent) {
                        name = parent.$options.componentName;
                    }
                }

                return !!parent

            },

            async initData() {

                this.$showLoading('加载中...')
                this.loading = true;
                await this.initDesignData();
                await this.initFormData();
                await this.initItemStates();
                await this.executeRuleFn(this.$consts.form.dfRuleFnNames[0]);
                this.loading = false;
                this.$hideLoading();
                this.$nextTick(function () {
                    this.bakFormData = this.getFormData();
                });

            },
            getFormData() {

                return this.$copyObj(this.formData);
            },

            async validateData() {

                let valid = true
                let elForms = [];

                function getElForms(parent) {

                    var children = parent.$children;
                    for (var child of children) {

                        let childName = child.$options.componentName;
                        if (childName == 'ElForm') {
                            elForms.push(child)
                        }
                        getElForms(child);

                    }

                }
                getElForms(this);

                for (var form of elForms) {
                    let vld = false;
                    try {
                        vld = await form.validate();

                    } catch {

                    }

                    valid = valid && vld;

                }
                return valid;

            },

            async executeBeforeSaveRule() {

                return await this.executeRuleFn(this.$consts.form.dfRuleFnNames[1]);

            },


            async saveData() {


                var valid = await this.validateData();

                if (!valid) return;

                this.saveLoading = true;
                var result = await this.executeBeforeSaveRule();
                if (result === false) {
                    this.saveLoading = false;
                    return;
                }

                var data = this.getFormData();
                var dto = { FormCode: this.formCode, Id: this.dataId, Data: JSON.stringify(data) };
                var res = await $formDefApi.saveData(dto);
                this.saveResult = res;

                result = await this.executeRuleFn(this.$consts.form.dfRuleFnNames[2]);

                this.saveLoading = false;

                if (result === false) {
                    return
                }

                if (res.IsSuccess) {
                    this.dispatch('FormList', 'list.reload', [this, data]);
                    this.$okTip(res.Msg)
                    if (this.pageIsInDialog) {
                        this.close(true);
                    }
                } else {
                    this.$errTip(res.Msg)

                }


            },
            setValueEmpty(obj, key) {
                var value = obj[key];
                if (Array.isArray(value))
                    value = [];
                else value = null;
                obj[key] = value;
            },

            async reloadItemOpts(data) {
                var item = data.item;
                var row0 = data.row;
                var isTopItem = false;

                var cascadeItems = this.cascadeItems[item.Id];

                if (!cascadeItems) return;

                if (!item.Parent) {
                    isTopItem = true;
                }

                for (var csdItem of cascadeItems) {
                    var parent = csdItem.Parent;
                    var states = [];
                    var rows;
                    if (parent) {
                        rows = this.formData[parent.Name];
                        states = this.itemStates[parent.Id].ChildStates;

                        if (!isTopItem) {

                            let state = states.find(m => m[item.Id].Id == data.state.Id);
                            states = [state]
                            rows = [row0];

                        }

                    } else {
                        rows = [this.formData];
                        states = [this.itemStates];
                    }
                    let index = 0;
                    for (var row of rows) {

                        this.setValueEmpty(row, csdItem.Name);
                        var query = {};
                        query[csdItem.CascadeField] = data.value;
                        var itemOpts = await this.getDataSourceItemOpts(csdItem, row, query);
                        states[index][csdItem.Id].Opts = itemOpts;
                        index++;

                    }

                }


                //var self = this;
                //setTimeout(function () {
                //    self.$forceUpdate();
                //}, 100)


            },


            /*执行脚本*/

            async executeRuleFn(name) {
                var ruleFn = this.ruleFn;
                if (!ruleFn) return;
                var fn = ruleFn(this.FORM)[name];
                if (!fn) return;
                var result = fn();
                if (result && result.then) {

                    try {
                        result = await result
                    } catch (e) {
                        console.error('executeRuleFn:', name, e);

                    }

                }

                return result;

            }


        },


        async created() {

            this.pageIsInDialog = this.calPageIsInDialog();
            if (this.pageIsInDialog) {
                this.$setDialogCloseCallback(this.closeCallback, this);
            }

            this.$on('item.valuechange', async (data) => {
                await this.reloadItemOpts(data)

            });

            var query = this.isNested ? this.args : this.$route.query;
            this.formCode = query.code || '';
            this.dataId = query.id || '';
            let mode = query.mode;
            if (mode == 'view' || mode == 'preview') {

            } else {
                mode = 'edit'
            }
            this.mode = mode;
            this.urlArgs = this.$copyObj(query);
            if (query.saveBtnTxt) {
                this.saveBtnTxt = query.saveBtnTxt
            }
            if (query.cancelBtnTxt) {
                this.cancelBtnTxt = query.cancelBtnTxt
            }

            if (!this.isNested) {
                await this.initData();
            }
        },



    }


</script>

<style scoped>
    .nct-work-area {
        background-color: #fff;
        padding: 0;
    }


    .form-container {
        height: 100%;
    }
</style>
