
import draggable from 'vuedraggable'
import consts from '@/utils/consts'
import form from '@/mixins/form.js'
import util from '@/utils/util.js'

import TableSelect from '@/pages/DbTable/Select.vue';
import DicCreate from '@/pages/DicValue/Create.vue';

import TableCreate from '@/pages/DbTable/Create.vue';
import TableEdit from '@/pages/DbTable/Edit.vue';

import DataSourceCreate from '@/pages/DataSource/Create.vue';
import DataSourceEdit from '@/pages/DataSource/Edit.vue';

let commonItemProps = consts.form.commonItemProps
let itemDefs = consts.form.itemDefs;

export default {
    name: 'FormDesign',
    componentName: 'FormDesign',
    provide() {
        return {
            designer: this
        };
    },
    mixins: [form],
    components: {
        draggable, TableSelect, DicCreate, TableEdit, TableCreate, DataSourceCreate, DataSourceEdit
    },
    data() {
        var tableValidate = (rule, value, callback) => {

            if (this.needSelectTable && this.designData.DbTables.length == 0) {
                callback(new Error('请添加数据表'));

            } else {
                callback();
            }

        }
        return {
            publicPath: process.env.BASE_URL,
            propEditor: consts.form.propEditor,
            itemPropTabId: 'itemprop',
            curTabId: 'first',
            curMenu: 'listbase',
            curRuleMenu: 'clientrule',
            autoCreateTable: 0,
            selected: false,
            itemDefs: this.$copyObj(consts.form.itemDefs),
            curItem: null,
            designData: this.$copyObj(consts.form.defaultDesignData),
            saveType: 1,
            model: { Code: '', Name: '', Cate: null, Descpt: '', ClientRule: '', ServerRule: '' },
            rules: {
                Name: [
                    { required: true, message: '请输入表单名称', trigger: 'blur' },

                ],

                Code: [
                    { required: true, message: '请输入表单编码', trigger: 'blur' },
                    {
                        validator: function (rule, value, callback) {
                            var valid = util.isValidId(value)
                            if (!valid) {
                                callback(new Error('表单编码格式不正确'));

                            } else {
                                callback();
                            }

                        }
                    }

                ],
                Tables: [

                    {
                        validator: tableValidate
                    }

                ]

            },

            itemErrInfo: {},
            sortables: {},
            editTableIndex: -1,
            loading: false,
            saving: false,
           


        }


    },


    watch: {


        "autoCreateTable"(value) {

            if (value == 1)
                this.designData.DataSaveType = 1;

        },
        "curItem.Table": async function (value, oldValue) {

            await this.getTableColumns(value);

            if (this.curItemIsTable) {

                if (!this.curItem.Name)
                    this.curItem.Name = value;

                for (var tbItem of this.curItem.Items) {

                    this.resetItemTable(tbItem, value, oldValue);

                }

            }


        },

        "curItem.ValueField": function (value, oldValue) {

            this.checkItemDefaultValue(this.curItem)

        },

        "curItem.DataSourceCode": async function (value) {


            if (this.curItem.DataSourceType == 2) {
                await this.getDataSourceColumns(value, false);
            }
            else if (this.curItem.DataSourceType == 3) {
                await this.getFormColumns(value, false);
            }

            this.checkCurItemTextAndValueField();

            if (this.curItem.DataSourceType <= 2) {
                await this.getItemDsData(this.curItem);
                this.checkItemDefaultValue(this.curItem);
            }


        },


        "designData.ListConfig.DataSource.Enabled": async function (value) {
            if (value)
                await this.getDataSources();
            else {
                var cols = this.getItemFields();
                this.checkListConfigFields(cols);

            }

        },
        "designData.ListConfig.DataSource.Code": async function (value) {

            var cols = await this.getDataSourceColumns(value, false);
            this.checkListConfigFields(cols);


        },

    },

    computed: {
        items() {

            return this.designData.Items || [];
        },

        allItems() {

            return this.getAllItems(this.items);

        },


        $this() {

            return this;

        },

        dataSaveType() {
            return this.designData.DataSaveType || 1;

        },
        curItemIsTable() {
            return this.curItem && this.curItem.Id && this.curItem.Type == 'Table';

        },



        needSelectTable() {

            return this.dataSaveType != 2 && (this.saveType == 2 || (this.saveType == 1 && this.autoCreateTable == 0));

        },



        dragOptions() {
            return {

                group: { name: 'item', pull: 'clone', put: false },
                sort: false,
            };
        },

        dragOptionsDest() {
            return {
                animation: 150,
                group: { name: 'item', pull: false },
                ghostClass: "ghost",


            };
        },



        showAutoAddItemBtn() {

            return this.saveType == 1 && this.autoCreateTable == 0 && this.items.length == 0 && this.dataSaveType != 2
        },

        curItemSwitchItems() {

            var items = [];
            var types = this.$consts.form.itemSwitchDefs[this.curItem.Type];
            if (!types)
                return items;

            for (var t of types) {

                var name = this.getItemTypeName(t);
                var obj = { name: name, value: t };
                items.push(obj);

            }

            return items;


        },



        curItemIsEmpty() {

            return !(this.curItem && this.curItem.Id)

        },


    },

    methods: {
        getItemIcon(item) {

            return `${this.publicPath}img/form/${item.Type.toLowerCase()}.svg`;
        },
        resetItemTable(item, newTable, oldTable) {

            if (item.Table == newTable || item.Table != oldTable) return;

            if (!newTable) {

                if (item.Table != undefined)
                    item.Table = '';

            } else {
                item.Table = newTable;

            }

            if (item.Field != undefined)
                item.Field = '';

        },

        resetItemsTable(newTable, oldTable) {

            for (var item of this.allItems) {

                this.resetItemTable(item, newTable, oldTable);

                if (this.isTableItem(item)) {

                    for (var tbItem of item.Items) {

                        this.resetItemTable(tbItem, newTable, oldTable);

                    }

                }

            }


        },

        getItemFields() {

            var fields = [];
            for (item of this.allItems) {
                if (item.Field) {

                    fields.push({ Value: item.Field, Name: item.Title });

                }


            }

            return fields;

        },

        checkListConfigFields(cols) {

            var cdts = this.designData.ListConfig.Query.Cdts;
            for (var cdt of cdts) {
                let index = cols.findIndex(m => m.Value == cdt.Field);
                if (index == -1)
                    cdt.Field = '';

            }
            var listCols = this.designData.ListConfig.List.Cols;
            for (var col of listCols) {
                let index = cols.findIndex(m => m.Value == col.Field);
                if (index == -1)
                    col.Field = '';

            }

        },



        checkCurItemTextAndValueField() {

            var cols = this.getItemDsColumns(this.curItem);

            if (this.curItem.TextField !== undefined) {

                let index = cols.findIndex(m => m.Value == this.curItem.TextField);
                if (index == -1)
                    this.curItem.TextField = '';

                index = cols.findIndex(m => m.Value == this.curItem.ValueField);
                if (index == -1)
                    this.curItem.ValueField = '';
            }
            if (this.curItem.CascadeField !== undefined) {

                let index = cols.findIndex(m => m.Value == this.curItem.CascadeField);
                if (index == -1)
                    this.curItem.CascadeField = '';

            }


        },



        tableSelected(model) {

            var name = model.Name;
            if (this.editTableIndex == -1) {
                var index = this.designData.DbTables.findIndex(m => m.Name == name);
                if (index != -1) {
                    this.$errTip("该数据表已添加")
                    return;

                }
                this.designData.DbTables.push(model);
                return;
            }

            var table = this.designData.DbTables[this.editTableIndex];
            if (table.Name != name) {
                this.resetItemsTable(name, table.Name);

            }
            table.Name = name;
            table.RefField = model.RefField;

        },
        delSelectedTable(index) {
            var tb = this.designData.DbTables[index];
            this.designData.DbTables.splice(index, 1);
            this.resetItemsTable(null, tb.Name);

        },

        showSelectTableDialog(tbIndex) {
            this.editTableIndex = tbIndex;
            var isSubTable = tbIndex > 0 || (tbIndex == -1 && this.designData.DbTables.length > 0)
            this.$refs.tableSelect.show(isSubTable);

        },
        showDicCreator(code) {

            this.$refs.dicCreator.show(code);
        },

        showDsCreator(code) {
            if (!code) {

                this.$refs.dsCreator.show();
            }
            else {

                var ds = this.dataSources.find(m => m.Code == code);
                this.$refs.dsEditor.show(ds.Id);

            }
        },

        async showTableEditor(name) {

            var tb = await this.getTable(name);
            this.$refs.tbEditor.show(tb.Id);
        },

        async dicChange(dic) {

            this.curItem.DataSourceCode = dic.Code;
            await this.getDicList(true);
            await this.getItemDsData(this.curItem, true);
            this.checkItemDefaultValue(this.curItem);


        },
        async tbChange(tb) {

            this.curItem.Table = tb.Name;
            await this.getTableColumns(tb.Name, true);

        },

        async dsChange(ds) {

            var cols = await this.getDataSourceColumns(ds.Code, true);
            if (this.curTabId == 'second') {

                await this.getItemDsData(this.curItem, true);
                if (this.curItem.DataSourceCode == ds.Code) {

                    this.checkCurItemTextAndValueField();
                    this.checkItemDefaultValue(this.curItem);

                } else {
                    await this.getDataSources(true);
                    this.curItem.DataSourceCode = ds.Code;

                }

            } else {

                if (this.designData.ListConfig.DataSource.Code == ds.Code) {
                    this.checkListConfigFields(cols);
                } else {
                    await this.getDataSources(true);
                    this.designData.ListConfig.DataSource.Code = ds.Code;
                    //监听change会获取列和列验证
                }


            }

        },


        checkItemDefaultValue(item) {

            if (item.DefaultValue === undefined || item.DefaultValue == '' || item.DefaultValue == []) return;

            var datas = this.getItemBindData(item);

            var value = item.DefaultValue;
            if (Array.isArray(value)) {
                var values = [];
                for (var v of value) {

                    let index = datas.findIndex(m => m.value == v);
                    if (index != -1) {
                        values.push(v);

                    }
                }

                item.DefaultValue = values;


            } else {

                let index = datas.findIndex(m => m.value == value);
                if (index == -1) {
                    item.DefaultValue = '';
                }
            }

        },


        hasPropEditor(item, prop) {

            return item[prop] !== undefined;

        },


        getCurItemInItems(item) {

            var items = [];
            var parent = this.getItemParent(item);
            if (parent) {
                items = parent.Items;

            } else {
                items = this.items;
            }

            return items;
        },


        isFormTileAlignTop(parent) {

            if (parent && parent.Layout == 1) return false;
            return this.designData.FormConfig.Theme.TitlePosition == 'top';

        },






        bindSortable() {

            if (this.curTabId != 'third') return;

            for (var key in this.sortables) {
                var obj = this.sortables[key];
                obj.destroy();
                delete this.sortables[key];
            }

            if (this.formScheme.listConfig) {
                if (this.curMenu == 'listpage') {

                    this.bindTableRowDrop('query-table', this.formScheme.listConfig.querySearchInfo.List)
                    this.bindTableRowDrop('list-table', this.formScheme.listConfig.queryTableInfo.TableList)
                } else if (this.curMenu == 'export') {
                    this.bindTableRowDrop('export-table', this.formScheme.exportConfig.ExcelCfg)
                }
            }

        },


        switchItem(type) {

            var itemDef = this.getItemDefByType(type);
            var curItem = this.curItem;
            for (let key in curItem) {

                if (itemDef[key] === undefined) {
                    delete curItem[key];

                }

            }

            for (let key in itemDef) {

                if (curItem[key] === undefined) {

                    Vue.set(curItem, key, itemDef[key]);

                }

            }
            curItem.Type = itemDef.Type;

        },

        getItemDefByType(type) {

            for (var key in itemDefs) {

                for (var item of itemDefs[key]) {

                    if (item.Type == type) {

                        var copyItem = this.$copyObj(item);

                        for (let key in commonItemProps) {
                            if (copyItem[key] === undefined) {
                                var value = commonItemProps[key];
                                if (value instanceof Object) {
                                    value = this.$copyObj(commonItemProps[key]);
                                }
                                copyItem[key] = value;
                            }

                        }

                        for (let key in copyItem) {
                            if (copyItem[key] == 'undefined')
                                delete copyItem[key];
                        }

                        return copyItem;

                    }


                }

            }

            return null;


        },

        getItemFromField(field) {

            var type = field.FieldType;
            var len = field.Length;
            var name = field.Name;
            var title = field.Remark || name;

            var item = null;

            if (type == 'datetime') {

                item = this.getItemDefByType('DatePicker');

            }
            else if (type == 'img') {

                item = this.getItemDefByType('Image');

            }
            else if (type == 'file') {

                item = this.getItemDefByType('Upload');

            }

            else if (type == 'str') {

                if (len <= 100)
                    item = this.getItemDefByType('Text');
                else
                    item = this.getItemDefByType('TextArea');

                item.MaxLen = len;

            }
            else if (type == 'money' || type == 'int' || type == 'float') {

                item = this.getItemDefByType('Number');
                if (type == 'money') {
                    item.Precision = 2

                }

            }

            if (item) {
                item.Title = title;
                item.Field = item.Name = name;
                item.Id = this.$newId();
            }

            return item;



        },

        async autoAddItem() {

            var tables = this.designData.DbTables;
            if (tables.length == 0) {

                this.error('请先添加数据表');
                this.curTabId = 'first';
                return;

            }

            var items = [];

            var self = this;

            var gridTable = {};

            for (var table of tables) {

                var tbName = table.Name;

                var isMainTable = !table.RefField;

                if (table.RefField) {
                    var grid = self.getItemDefByType('Table');
                    grid.Table = tbName;
                    grid.Title = tbName;
                    grid.Name = tbName;
                    grid.Id = this.$newId();
                    gridTable[tbName] = grid;
                }
                var cols = await this.getTableColumns(tbName, false);

                for (var col of cols) {

                    var item = self.getItemFromField(col);
                    if (!item) continue;

                    item.Table = tbName;
                    if (isMainTable) {
                        items.push(item);

                    } else {

                        gridTable[tbName].Items.push(item);

                    }

                }


            }

            for (var key in gridTable) {

                items.push(gridTable[key]);

            }
            this.designData.Items = items;
            this.setFirstItemSelected();


        },

        setCurItemEmpty() {
            this.curItem = {};

        },

        setFirstItemSelected() {

            var items = this.items;
            if (items.length > 0) {

                this.setCurItem(items[0]);

                return true;
            }
            return false;
        },



        showMenu(menu) {

            this.curMenu = menu;
        },

        showRuleMenu(menu) {

            this.curRuleMenu = menu;
        },



        getItemParent(item0) {

            for (var item of this.items) {

                if (item.Id == item0.Id) {

                    return null;
                }
                if (this.isTableItem(item)) {

                    for (const tbItem of item.Items) {
                        if (tbItem.Id == item0.Id)
                            return item;

                    }

                } else if (this.isTabItem(item)) {

                    for (const tab of item.Tabs) {

                        for (const subItem of tab.Items) {
                            if (subItem.Id == item0.Id)
                                return tab;
                            if (this.isTableItem(subItem)) {

                                for (const subItem2 of subItem.Items) {
                                    if (subItem2.Id == item0.Id)
                                        return subItem;

                                }

                            }

                        }

                    }



                }

            }


            return null;


        },


        setCurItem(item) {

            this.curItem = item;
            if (item.DataSourceCode != undefined) {

                if (item.Type == "DataSelect") {
                    this.getFormDefs();

                } else {
                    this.getDicList();
                    this.getDataSources();
                }

            }
            this.itemPropTabId = 'itemprop';

        },

        error(msg) {
            this.$errTip(msg);
        },


        hasCanEditItems() {

            var items = this.items;
            if (items.length == 0) return false;
            return true;

        },

        getItemTypeName: function (type) {

            for (var key in itemDefs) {

                for (var itemDef of itemDefs[key]) {

                    if (type == itemDef.Type) {
                        return itemDef.Title;

                    }
                }

            }

            return '';
        },

        getItemCateName: function (item) {

            for (var key in itemDefs) {

                for (var itemDef of itemDefs[key]) {

                    if (itemDef.Type == item.Type) {
                        return key;

                    }
                }

            }

            return '';
        },

        copyItem: function (item) {

            var items = this.getCurItemInItems(item);
            var newItem = this.$copyObj(item);
            newItem.Id = this.$newId();
            var ref = item.Name;
            if (ref) {
                var index = 1;
                for (var _item of items) {
                    if (_item.Name && _item.Name.indexOf(ref) == 0) {

                        index++;
                    }


                }
                newItem.Name = ref + index;
            }

            items.push(newItem);

        },


        delItem: function (item) {

            var items = this.getCurItemInItems(item);
            var index = items.findIndex(m => m.Id == item.Id);
            items.splice(index, 1);

            var self = this;
            setTimeout(function () {
                if (self.setFirstItemSelected())
                    return;
                self.setCurItemEmpty();

            }, 10)


        },



        itemIsSelected: function (item) {

            return this.curItem && this.curItem.Id == item.Id;
        },
        setData(dataTransfer) {

            dataTransfer.setData('Text', '');
        },


        addItem(item, isDrag) {


            item.Id = this.$newId();

            let parent = null;

            if (isDrag) {

                parent = this.getItemParent(item);

            }


            if (this.dataSaveType == 2 && this.isTableItem(item)) {

                this.delItem(item, parent)
                this.error('字典保存方式下不能添加表格控件')
                return;
            }


            if (this.isTableItem(parent) && (this.isTableItem(item) || this.isTabItem(item))) {


                this.delItem(item, parent)
                this.error('表格控件里不能添加此类控件')
                return;

            }


            if (parent && this.isTabItem(item)) {

                this.delItem(item, parent)
                this.error('标签控件里不能添加标签控件')
                return;

            }


            if (!this.isTableItem(parent) && item.Table !== undefined && !this.isTableItem(item) && this.designData.DbTables.length > 0) {

                item.Table = this.designData.DbTables[0].Name
            }
            if (item.Table === undefined) {

                item.Name =  this.$newId();
            }

            if (this.isTabItem(item)) {

                item.Title = '';

            }

            if (this.isTableItem(parent) && item.Table !== undefined) {

                item.Table = parent.Table;

            }

            var items = parent ? parent.Items : this.items;

            if (!isDrag) {

                items.push(item);
            }

            var index = items.findIndex(m => m.Id == item.Id);

            if (index > 0) {

                item.Span = items[index - 1].Span;
            }

            this.setCurItem(item);


        },

        cloneDraggingItem(item) {

            return this.$copyObj(item);

        },

        addItemByClick(itemDef) {

            var item = this.getItemDefByType(itemDef.Type);
            this.addItem(item, false);

        },



        addItemByDrag(evt) {

            var isToHolder = evt.to.className.indexOf('drag-holder') != -1;

            if (!isToHolder) {

                return;
            }

            var item = evt.item._underlying_vm_;

            if (!item) return;

            var itemDef = this.getItemDefByType(item.Type);

            for (let key in item) {

                if (item[key] == 'undefined') {

                    delete item[key]

                }

            }

            for (let key in itemDef) {

                if (item[key] === undefined) {

                    Vue.set(item, key, itemDef[key]);

                }

            }

            this.addItem(item, true);


        },




        initSelectAndRadioDsData() {

            var items = this.allItems
            for (var item of items) {
                if (this.isRadioOrCheckBoxItem(item)) {
                    this.getItemDsData(item);
                }
                else if (this.isTableItem(item)) {

                    for (var tbItem of item.Items) {

                        if (this.isRadioOrCheckBoxItem(tbItem))
                            this.getItemDsData(tbItem);

                    }

                }

            }


        },


        initDesignData(data) {


            this.model = data;
            var designData = JSON.parse(data.Data);
            this.designData = designData;
            this.saveBakDesignData();
            this.initSelectAndRadioDsData();
            this.setFirstItemSelected();


        },



        getDesignData: function (id) {

            $formDefApi.getFullModel(id).then(res => {
                this.loading = false;
                if (res.IsSuccess) {
                    this.initDesignData(res.Data);

                } else {
                    this.error(res.Msg);
                }

            });

        },

        async previewListPage() {

            var changed = this.isDesignDataChanged()
            if (changed) {
                var confirm = await this.$confirmTip('有未保存的设计数据, 预览效果不是最新的，确定预览?');
                if (!confirm)
                    return;

            }

            let url = '/form/list?code=' + this.model.Code + '&noCache=Y';
            let title = `预览列表 - ${this.model.Name}`;
            this.$openPage(url, title);


        },

        previewFormPage() {

            var formDef = this.$copyObj(this.model);
            formDef.Data = JSON.stringify(this.designData);
            window["$formDef"] = formDef;
            let url = `/form/view?mode=preview`;
            let title = `预览表单`
            let opts = { url: url, title: title, width: consts.diaWidth, height: consts.diaHeight };
            this.$showDialog(opts);

        },



        validateItem(item, ids, fields, needTable, tables) {

            var msg = '';
            if (!item.Name) {
                msg += '控件Id不能为空<br/>';

            } else {
                if (ids[item.Name]) {
                    msg += '控件Id重复<br/>';


                } else {
                    ids[item.Name] = 1;
                }

                if (!this.$isValidId(item.Name)) {
                    msg += '控件Id格式不正确<br/>';

                }
            }

            if (needTable && item.Table !== undefined) {
                if (!item.Table) {
                    msg += '数据表不能为空<br/>';

                }
                else if (this.isTableItem(item)) {

                    if (tables[item.Table]) {

                        msg += '数据表重复<br/>';
                    } else {
                        tables[item.Table] = 1;
                    }
                }

            }
            if (this.isTableItem(item) && item.Items.length == 0) {

                msg += '表格子控件不能为空<br/>';
            }

            if (item.Field) {
                if (fields[item.Field]) {
                    msg += '字段重复<br/>';
                } else {
                    fields[item.Field] = 1;
                }
            }


            if (item.DataSourceCode !== undefined && !item.DataSourceCode) {

                msg += '数据源字典编码不能为空<br/>';
            }

            if (item.DataSourceType > 1 && item.TextField !== undefined && !item.TextField) {

                msg += '显示字段不能为空<br/>';
            }

            if (item.DataSourceType > 1 && item.ValueField !== undefined && !item.ValueField) {

                msg += '值字段不能为空<br/>';
            }


            if (item.CascadeParent && !item.CascadeField) {

                msg += '级联字段不能为空<br/>';
            }

            return msg;

        },

        correctDesignData() {


            var fieldIsName = this.autoCreateTable == 1 || this.dataSaveType == 2;

            for (var item of this.allItems) {

                if (this.isTableItem(item)) {

                    for (const tbItem of item.Items) {

                        if (tbItem.Field !== undefined) {

                            if (fieldIsName)
                                tbItem.Field = tbItem.Name;
                            tbItem.Table = item.Table;

                        }

                    }

                }
                else {

                    if (fieldIsName && item.Field !== undefined)
                        item.Field = item.Name;

                }


            }


        },

        validateDesignData() {

            var itemErrInfo = {};
            var ids = {};
            var fields = {};
            var tables = {};

            var valid = true;
            var msg;
            var needTable = this.needSelectTable;
            var self = this;

            function validateTableItem(item) {

                let ids2 = {};
                let fields2 = {};
                let pass = true
                for (let tbItem of item.Items) {

                    msg = self.validateItem(tbItem, ids2, fields2, needTable, tables);
                    itemErrInfo[tbItem.Id] = msg;
                    if (msg) {
                        valid = false;
                        pass = false;
                    }
                }
                return pass;
            }

            for (var item of this.items) {

                msg = this.validateItem(item, ids, fields, needTable, tables);
                itemErrInfo[item.Id] = msg;
                if (msg)
                    valid = false;

                if (this.isTableItem(item)) {

                    validateTableItem(item);

                } else if (this.isTabItem(item)) {

                    let invalidIndex = -1;
                    let tabIndex = 0;
                    for (const tab of item.Tabs) {

                        for (const tabItem of tab.Items) {

                            if (this.isTableItem(tabItem)) {

                                let isValid = validateTableItem(tabItem);
                                if (!isValid && invalidIndex == -1) {

                                    invalidIndex = tabIndex;

                                }

                            } else {
                                msg = this.validateItem(tabItem, ids, fields, needTable, tables);
                                itemErrInfo[tabItem.Id] = msg;
                                if (msg) {
                                    valid = false;
                                    if (invalidIndex == -1) {
                                        invalidIndex = tabIndex;

                                    }
                                }


                            }


                        }
                        tabIndex++;

                    }

                    if (invalidIndex >= 0) {

                        this.$refs[item.Id][0].setActiveTabIndex(invalidIndex);
                       
                        
                    }

                }




            }

            this.itemErrInfo = itemErrInfo;


            if (!valid) {

                this.curTabId = 'second';
                this.error("控件信息有错误");
                return false;
            }

            var designData = this.designData;

            if (designData.ListConfig.DataSource.Enabled && !designData.ListConfig.DataSource.Code) {

                this.error("请选择自定义数据源");
                this.curTabId = 'third';
                this.curMenu = 'listbase';
                return false;

            }


            for (let f of designData.ListConfig.Query.Cdts) {

                if (!f.Field) {

                    this.error("搜索项名称不能为空");
                    this.curTabId = 'third';
                    this.curMenu = 'listquery';
                    return false;
                }

            }

            //if (designData.ListConfig.List.Cols.length == 0) {


            //    this.error("请设置表单列表列");
            //    this.curTabId = 'third';
            //    this.curMenu = 'listtable';
            //    return false;

            //}



            for (let f of designData.ListConfig.List.Cols) {

                if (!f.Field) {
                    this.error("表单列表列字段不能为空");
                    this.curTabId = 'third';
                    this.curMenu = 'listtable';
                    return false;
                }

            }





            return valid;

        },





        getNewDesignData() {

            var designData = this.designData;
            var data = this.$copyObj(this.model);
            data.AutoCreateTable = this.autoCreateTable;
            data.Data = JSON.stringify(designData);
            return data;
        },

        saveDesignData: function (close) {

            if (this.saving) {
                return;
            }
            this.saving = true;


            this.$refs.infoform.validate((valid) => {

                if (!valid) {

                    this.curTabId = 'first'
                    return;
                }

                this.correctDesignData();
                valid = this.validateDesignData();
                if (!valid) return;

                var dto = this.getNewDesignData();

                const loading = this.$showLoading('保存中...');

                if (this.saveType == 1) {
                    dto.Id = 0;
                    $formDefApi.create(dto).then(res => {
                        loading.close();
                        if (res.IsSuccess) {
                            this.model.Id = res.Data.Id;
                            if (this.autoCreateTable) {
                                this.autoCreateTable = 0;
                                this.designData = JSON.parse(res.Data.Code);
                                this.setFirstItemSelected();
                            }
                            this.$okTip("保存成功");
                            this.$closePage();
                           
                            if (!close) {
                                let path = '/formdef/design?id=' + res.Data.Id;
                                let name = `编辑表单 - ${this.model.Name}`
                                this.$openPage(path, name);
                                this.$eventHub.$emit('formdef-refresh');
                            }

                        } else {

                            this.$errTip(res.Msg);
                        }

                    })


                } else {
                    $formDefApi.update(dto).then(res => {
                        loading.close();
                        if (res.IsSuccess) {
                            this.$okTip("保存成功");

                            if (close) {
                                this.$closePage();
                                this.$refreshPage('/formdef')
                            } else {
                                this.saveBakDesignData();
                                this.$eventHub.$emit('formdef-refresh');
                            }

                        } else {

                            this.$errTip(res.Msg);
                        }

                    })

                }



            });


            this.saving = false;

        },


        bindTableRowDrop(selector, tableData) {

            var self = this;
            const el = document.querySelector(`.${selector} .el-table__body-wrapper tbody`)
            if (!el) {
                setTimeout(function () {

                    self.bindTableRowDrop(selector, tableData);

                }, 500)

                return;
            }
            this.sortables[selector] = new Sortable(el, {
                animation: 150,
                setData: self.setData,
                handle: '.selection',
                onEnd: function (evt) {
                    var list = tableData;
                    const oldItem = list[evt.oldIndex]
                    list.splice(evt.oldIndex, 1)
                    list.splice(evt.newIndex, 0, oldItem)
                    self.editCellId = '';

                }

            });



        },
 
        isDesignDataChanged() {

            var bakData = '';
            if (this.designDataBak) {
                bakData = JSON.stringify(this.designDataBak);

            }
            var currentData = JSON.stringify(this.getNewDesignData());
            var changed = currentData != bakData;

            return changed;

        },

        async confirmCloseCurPage() {

            var changed = this.isDesignDataChanged()
            if (changed) {
                const confirm = await this.$confirmTip('有未保存的设计数据, 是否关闭?');
                if (confirm) {
                    this.$closePage();
                    
                }

            } else {

                this.$closePage();
            }


        },
        saveBakDesignData() {

            this.designDataBak = this.getNewDesignData();

        },
        loadData() {
            var id = this.$route.query.id;
            if (id) {
                this.curTabId = "second";
                this.loading = true;
                this.saveType = 2;
                this.getDesignData(id)

            }


        }

    },

    mounted: function () {

        this.loadData();
        this.$setTabCloseCallback(this.confirmCloseCurPage);


    }

};






