
export default {
    data() {
        return {

            dicList: [],
            dataSources: [],
            dsColumns: {},
            dsData: {},
            tableColumns: {},
            formColumns: {},
            formDefs: [],
            tabels: {},


        }

    },
    methods: {



        getItemRenderName(item) {

            return item.Type + 'Item';

        },

        getItemTitleStyle(item, formTheme) {

        
            var css = {};

            var titleFont = item.TitleFont;

            if (!titleFont) return css;

            var formTitleFont = formTheme.TitleFont || {};

            if (titleFont.FontWeight) {
                css["font-weight"] = titleFont.FontWeight;

            }
            else if (formTitleFont.FontWeight) {
                css["font-weight"] = formTitleFont.FontWeight;

            }
            css["font-size"] = '12px';
            if (titleFont.FontSize) {
                css["font-size"] = titleFont.FontSize + 'px';

            } else if (formTitleFont.FontSize) {
                css["font-size"] = formTitleFont.FontSize + 'px';

            }

            if (titleFont.FontStyle) {
                css["font-style"] = titleFont.FontStyle;

            } else if (formTitleFont.FontStyle) {
                css["font-style"] = formTitleFont.FontStyle;

            }

            if (titleFont.Color) {
                css["color"] = titleFont.Color;

            } else if (formTitleFont.Color) {
                css["color"] = formTitleFont.Color;

            }


            if (titleFont.TextDecoration) {
                css["text-decoration"] = titleFont.TextDecoration;

            } else if (formTitleFont.TextDecoration) {
                css["text-decoration"] = formTitleFont.TextDecoration;

            }

            var textAlign = item.TextAlign;
            if (textAlign) {

                css['text-align'] = textAlign;            
            }
            return css;
        },


        getItemTextStyle(item) {

        
            var css = {};

            var textFont = item.TextFont;

            if (!textFont) return css;

            if (textFont.FontWeight) {
                css["font-weight"] = textFont.FontWeight;

            }
          
            css["font-size"] = '12px';
            if (textFont.FontSize) {
                css["font-size"] = textFont.FontSize + 'px';

            } 
            if (textFont.FontStyle) {
                css["font-style"] = textFont.FontStyle;

            } 
            if (textFont.Color) {
                css["color"] = textFont.Color;

            }

            if (textFont.TextDecoration) {
                css["text-decoration"] = textFont.TextDecoration;

            }

            var textAlign = item.TextAlign;
            if (textAlign) {

                css['text-align'] = textAlign;
            }

            return css;
        },


        isItem(item) {
            return item && item.Type;
        },


        isContainerItem(item) {
            return this.isTabItem(item);
        },

        isFileItem(item) {

            return this.isItem(item) && (item.Type == 'Upload' || item.Type == 'Image');

        },

        isInputTypeItem(item) {

            if (!this.isItem(item)) return false;
            const type = item.Type;
            return type == 'Text' || type == 'TextArea' || type == 'Number' || type == 'RichText';

        },
        isSelectTypeItem(item) {

            if (!this.isItem(item)) return false;
            const type = item.Type;
            return type == 'Select' || type == 'Radio' || type == 'CheckBox' || type == 'DataSelect' || type == 'DatePicker'||type=='Location';

        },


        isTableItem(item) {

            return this.isItem(item) && item.Type == 'Table';

        },
        isTabItem(item) {

            return this.isItem(item) && item.Type == 'Tab';

        },

        isDataSelectItem(item) {

            return this.isItem(item) && item.Type == 'DataSelect';

        },


        isRadioOrCheckBoxItem(item) {

            return this.isItem(item) && (item.Type == 'Radio' || item.Type == 'CheckBox')

        },
        isDataSourceItem(item) {

            return this.isItem(item) && (this.isRadioOrCheckBoxItem(item) || item.Type == 'Select' || item.Type == 'DataSelect');

        },

        getAllItems(items) {
            let allItems = [];
            for (var item of items) {
                if (this.isTabItem(item)) {
                    allItems.push(item)
                    for (var tab of item.Tabs) {

                        allItems = [...allItems, ...tab.Items];

                    }

                } else {

                    allItems.push(item);
                }


            }
            return allItems;

        },

        getItemDsColumns(item) {
            if (item.DataSourceType == 2)
                return this.dsColumns[item.DataSourceCode] || [];

            if (item.DataSourceType == 3)
                return this.formColumns[item.DataSourceCode] || []

            return [];
        },

        getItemDsDataKey(item, query) {
            var dataKey = item.DataSourceCode + '__' + item.DataSourceType;
            if (query) {
                for (var key in query) {

                    dataKey += key + '_' + query[key];
                }

            }
            return dataKey;

        },

        getItemBindData(item, query) {

            var dataKey = this.getItemDsDataKey(item, query);
            var dsData = this.dsData[dataKey];
            var txtField = item.TextField;
            var valField = item.ValueField;
            if (item.DataSourceType == 1) {
                txtField = "Name";
                valField = "Value"
            }

            var data = [];

            if (!valField || !txtField || !dsData) return data;

            if (item.DataSourceType == 2 && item.Type == 'Select' && item.RenderType == 2) {

                for (let row of dsData) {
                    row.name = row[txtField];
                    row.value = row[valField];
                }

                var parentField = item.ParentField;
                var rootNodes = this.buildNodesToTree(dsData,valField, parentField);
                this.removeEmptyChild(rootNodes);
                data = rootNodes;

            } else {
                for (let row of dsData) {
                    data.push({ name: row[txtField], value: String(row[valField]) })
                }

            }
            return data;
        },

        getTable: async function (name) {

            if (this.tabels[name])
                return this.tabels[name];

            var res = await $dbTableApi.getByName(name);
            this.tabels[name] = res.Data;
            return res.Data;


        },



        getFormDefs: async function (reload) {


            if (!reload && this.formDefs.length > 0)
                return this.formDefs;

            var res = await $formDefApi.getAllList();

            this.formDefs = res.Data || [];

            return this.formDefs;


        },
        getItemRealValue: function (value) {

            if (value && typeof (value) === 'object')
                return value.Value;

            return value;


        },


        removeEmptyChild: function (items,childKey) {
            childKey = childKey ||'children'
            for (var item of items) {
                if (item[childKey]) {
                    if (item[childKey].length == 0) {

                        delete item[childKey];

                    } else {

                        this.removeEmptyChild(item[childKey])

                    }

                }

            }

        },

        buildNodesToTree(nodes,idKey,pidKey) {

            var childMap = {};
            var parentMap = {};
            var rootNodes = [];
           
            for (var item of nodes) {

                let id = item[idKey];
                let pid = item[pidKey];
                if (parentMap[id]) continue;

                if (!childMap[pid]) {
                    childMap[pid] = [];

                }
                childMap[pid].push(item);
                parentMap[id] = 1;

                if (!childMap[id]) {

                    childMap[id] = [];
                }
                item.children = childMap[id]


            }

            for (var key in childMap) {
                if (!parentMap[key]) {
                    rootNodes.push(...childMap[key]);
                }
            }

            return rootNodes;

        },


        getDicList: async function (reload) {

            if (!reload && this.dicList.length > 0)
                return this.dicList;

            var res = await $dicApi.getTreeByParentCode('dic');

            var dicList = res.Data || [];
            if (dicList.length > 0) {
                this.removeEmptyChild(dicList,'Children');

            }

            this.dicList = dicList;
            return this.dicList

        },




        getDataSources: async function (reload) {

            if (!reload && this.dataSources.length > 0)
                return this.dataSources;

            var queryParams = { pageIndex: 1, pageSize: 1000, Query: {} };

            var res = await $dataSourceApi.getList(queryParams);

            this.dataSources = res.Data.Rows || [];

            return this.dataSources;

        },

        getDataSourceColumns: async function (dsCode, reload) {

            if (!dsCode) return [];

            if (!reload && this.dsColumns[dsCode] && this.dsColumns[dsCode].length > 0) {

                return this.dsColumns[dsCode];
            }

            var res = await $dataSourceApi.getColumns(dsCode);
            Vue.set(this.dsColumns, dsCode, res.Data || []);

            return this.dsColumns[dsCode];

        },

        getFormColumns: async function (code, reload) {

            if (!code) return [];

            if (!reload && this.formColumns[code] && this.formColumns[code].length > 0) {

                return this.formColumns[code];
            }

            var res = await $formDefApi.getByCode(code);

            var cols = [];

            var data = JSON.parse(res.Data.Data);
            cols.push({ Name: "Id", Value: "Id" });

            for (var col of data.ListConfig.List.Cols) {

                cols.push({ Name: col.Title, Value: col.Field })

            }

            Vue.set(this.formColumns, code, cols);

            return cols;

        },


        getTableColumns: async function (table, reload) {

            if (!table) return [];

            if (!reload && this.tableColumns[table] && this.tableColumns[table].length > 0) {
                return this.tableColumns[table];
            }

            var res = await $dbTableApi.getCustomFields(table);

            Vue.set(this.tableColumns, table, res.Data || []);

            return this.tableColumns[table];


        },


        getItemDsData: async function (item, query) {

            var dsCode = item.DataSourceCode;
            var dataKey = this.getItemDsDataKey(item, query);
            var data = this.dsData[dataKey];
            if (data) return data;

            if (!dsCode) {
                return [];
            }


            if (item.DataSourceType == 1) {

                let res = await $dicValueApi.getValuesByCode(dsCode);
                data = res.Data || [];

            }
            else if (item.DataSourceType == 2) {
                var dto = { Code: dsCode, Query: JSON.stringify(query || {}) };
                let res = await $dataSourceApi.getDataTable(dto)
                data = res.Data || [];

            }

            Vue.set(this.dsData, dataKey, data);
            return data;


        },


        getAnnexs: async function (ids) {

            var res = await $annexApi.getListByIds(ids);
            return res.Data;



        },


        tryCompileJs(js,names) {

            if (!js) return;

            var funNames = [];
            for (var name of names) {

                var pattern = `function\\s+${name}\\s*\\(`;
                var reg = new RegExp(pattern);
                var flag = reg.test(js);
                if (flag) funNames.push(name);

            }
            if (funNames.length > 0) {

                var objFun = '{' + funNames.join(',') + '}';

                var script = `(function() {
            return function (FORM) {
              ${js}
             return ${objFun}
            }
          }

          )()`;
                try {
                    var fn = eval(script);
                    return fn;
                } catch (e) {
                    console.error('±‡“ÎΩ≈±æ£∫', e);
                }

            }
        },





    }



}
