<template>

    <el-container class="root-container is-vertical">
        <el-container v-if="!preLoading" :class="{'form-container':true, 'nct-hastree-container':hasTreeItem,'nct-hasnotree-container':!hasTreeItem,'nested-list':isNested}">
            <el-aside width="200px" v-if="hasTreeItem">
                <QueryRender :params="params" :items="queryItems" type="tree" ref="queryTree"></QueryRender>
            </el-aside>

            <el-main class="nct-work-area">
                <div>
                    <div class="nct-cmd-area">

                        <QueryRender :params="params" :items="queryItems" v-if="isSearchShow" type="top"></QueryRender>

                        <div class="nct-toolbar" v-if="!isReadonly&&topShowBtns.length>0">
                            <template v-for="(btn,index) in topShowBtns">
                                <el-button :icon="btn.btn.Icon" :key="index" :type="btn.btn.Type" @click="invokeBtnClick(btn)" :disabled="!btn.state.enabled" size="small">{{btn.btn.Name}}</el-button>
                            </template>

                            <el-dropdown v-if="topShowInMoreBtns.length>0" @command="invokeDropdownBtnClick">
                                <el-button size="small" style="margin-left:10px;">
                                    更多<i class="el-icon-arrow-down el-icon--right"></i>
                                </el-button>
                                <el-dropdown-menu slot="dropdown">
                                    <el-dropdown-item :key="index" :command="{btn,row:null}" :disabled="!btn.state.enabled" v-for="(btn,index) in  topShowInMoreBtns">

                                        <i v-if="btn.btn.Icon" :class="btn.btn.Icon"></i> {{btn.btn.Name}}

                                    </el-dropdown-item>
                                </el-dropdown-menu>
                            </el-dropdown>

                            <div class="right-tool" v-if="!FORM.urlArgs.hideRefreshBtn||!FORM.urlArgs.hideSearchBtn">
                                <el-button icon="el-icon-refresh" size="small" @click="refreshChange" v-if="!FORM.urlArgs.hideRefreshBtn" circle></el-button>
                                <el-button icon="el-icon-search" v-if="queryItems.length>0&&!FORM.urlArgs.hideSearchBtn" size="small" circle @click="isSearchShow=!isSearchShow"></el-button>

                            </div>
                        </div>

                    </div>
                    <QueryRender :params="params" :items="queryItems" type="tab"></QueryRender>
                </div>



                <el-table :data="data" border element-loading-text="拼命加载中" row-key="Id" size="small" ref="table" height="50" v-loading="loading" class="nct-list-table"
                          highlight-current-row :tree-props="treePros" :lazy="isLazyLoad"
                          :load="loadChildren" :show-summary="showSummary" :summary-method="getSummaries"
                          @selection-change="selectionChange" @sort-change="sortChange" @row-dblclick="rowDblclick" :default-sort="defaultSort" @row-click="rowClick">


                    <el-table-column type="selection" align="center" v-if="hasBatchOpBtn||isMultiSelect"
                                     width="50">
                    </el-table-column>

                    <el-table-column width="50" type="index" label="序号">
                        <template slot-scope="scope">
                            <label>{{getRowIndex(scope.$index)}}</label>
                        </template>
                    </el-table-column>


                    <el-table-column type="expand" class-name="expand-cell" v-if="tableType==2">
                        <template slot-scope="scope">

                            <FormList :isNested="true" :key="getIdValue(scope.row)" @listDataChange="listDataChange" :args="getNestedFormArgs(scope.row)" :ref="getIdValue(scope.row)"></FormList>

                        </template>
                    </el-table-column>


                    <template v-for="(col,index) in listCols">

                        <el-table-column :prop="col.Field" :key="index" :sortable="col.Sort!='no'?'custom':false" :align="col.Align"
                                         :label="col.Title"
                                         :min-width="col.Width" v-if="calColVisible(col)">

                            <template slot-scope="scope">
                                <template v-if="col.Formatter">

                                    <el-progress :percentage="scope.row[col.Field]" v-if="col.Formatter.Type=='progress'" :color="col.Formatter.Config.Color" :show-text="col.Formatter.Config.ShowText" text-inside :stroke-width="15"></el-progress>

                                    <template v-else-if="col.Formatter.Type=='tag'">
                                        <el-tag :style="getColTagColorStyle(scope.row,col)" v-if="col.Formatter.Config.Color=='auto'">
                                            {{getFieldDisplayText(scope.row,col)}}
                                        </el-tag>
                                        <el-tag :type="col.Formatter.Config.Color" v-else
                                                :effect="col.Formatter.Config.Effect">
                                            {{getFieldDisplayText(scope.row,col)}}
                                        </el-tag>

                                    </template>

                                    <template v-else-if="col.Formatter.Type=='img'">

                                        <el-image fit="contain"
                                                  :src="img.url"
                                                  :key="imgIdx"
                                                  :style="{width:img.size+'px',height:img.size+'px'}"
                                                  :preview-src-list="img.previewList" v-for="(img,imgIdx) in getImgList(scope.row,col)">
                                        </el-image>

                                    </template>

                                    <template v-else-if="col.Formatter.Type=='switch'">

                                        <SwitchWrap :value="getItemRealValue(scope.row[col.Field])" v-on="colEvent[col.Field]" :state="scope.row"
                                                    :active-color="col.Formatter.Config.ActiveColor"
                                                    :inactive-color="col.Formatter.Config.InactiveColor"
                                                    :active-text="col.Formatter.Config.ActiveText"
                                                    :disabled="col.Formatter.Config.Disabled"
                                                    :inactive-text="col.Formatter.Config.InactiveText">
                                        </SwitchWrap>

                                    </template>

                                    <template v-else-if="col.Formatter.Type=='code'">

                                        <div v-html="evalListFormatCode(scope.row,col)"></div>

                                    </template>


                                </template>
                                <template v-else>

                                    {{getFieldDisplayText(scope.row,col)}}

                                </template>

                            </template>

                        </el-table-column>
                    </template>

                    <el-table-column label="操作" :width="actionColWidth" align="center" v-if="actionColWidth>0&&!isReadonly&&hasRowBtn">
                        <template slot-scope="scope">

                            <el-link :type="btn.btn.Type" :underline="false" @click="invokeBtnClick(btn,scope.row)" :disabled="!btn.state.enabled" :icon="btn.btn.Icon" :key="btn.btn.Id" v-for="(btn) in getRowShowBtns(scope.row)">{{btn.btn.Name}}</el-link>
                            <el-dropdown v-if="getRowShowInMoreBtns(scope.row).length>0" @command="invokeDropdownBtnClick">
                                <el-link type="primary" :underline="false" icon="el-icon-arrow-down el-icon--right">更多</el-link>

                                <el-dropdown-menu slot="dropdown">
                                    <el-dropdown-item :command="{btn,row:scope.row}" :disabled="!btn.state.enabled" :key="btn.Id" v-for="(btn) in  getRowShowInMoreBtns(scope.row)">
                                        <i v-if="btn.btn.Icon" :class="btn.btn.Icon"></i> {{btn.btn.Name}}
                                    </el-dropdown-item>
                                </el-dropdown-menu>
                            </el-dropdown>

                        </template>
                    </el-table-column>


                    <el-table-column label="操作" width="100" align="center" v-else-if="isSelect">
                        <template slot-scope="scope">
                            <el-link type="primary" :underline="false" @click="selectRow(scope.row)" icon="el-icon-check">选择</el-link>
                        </template>
                    </el-table-column>

                </el-table>

                <el-pagination @size-change="sizeChange" v-if="!noPaging" :hide-on-single-page="isNested"
                               @current-change="currentChange"
                               :current-page="page.pageIndex"
                               :page-sizes="pageSizes"
                               :page-size="page.pageSize"
                               layout="total, sizes, prev, pager, next, jumper"
                               :total="page.total">
                </el-pagination>
                <div style="display:none" v-html="style"></div>
            </el-main>

        </el-container>


        <div v-else class="list-loading" v-loading="true"></div> <!--直接把loading加到根节点页面会闪烁-->

        <div class="el-dialog__footer" v-if="isMultiSelect&&!preLoading">
            <el-button type="primary" size="small" @click="doSelectRows" icon="el-icon-circle-check" :disabled="selectRows.length==0">确定</el-button>
            <el-button size="small" icon="el-icon-circle-close" @click="close">取消</el-button>
        </div>


    </el-container>





</template>
<script>


    import FormList from './List.vue';
    import form from '@/mixins/form.js'
    import crud from '@/mixins/crud.js'
    import request from '@/api/request.js';
    import emitter from '@/mixins/emitter';
    export default {
        props: {
            isNested: { type: Boolean, default: false },
            args: Object

        },
        path: '/form/list',
        name: 'FormList',
        componentName: 'FormList',
        mixins: [emitter, form, crud({ api: window.$formDefApi, list: 'getPageData' })],
        provide() {
            return {
                formList: this
            };
        },
        data() {
            return {
                designData: {},
                formDef: {},
                allItems: [],
                preLoading: true,
                formCode: null,
                isSearchShow: true,
                queryItems: [],
                routeQuery: {},
                selectRows: [],
                mode: 'edit',
                pageId: this.$newId(),
                defaultSort: { prop: 'CreateTime', order: 'descending' },
                css: '',
                curBtnRow: null,
                expandedData: {},
                colEvent: {},
                summaryData: []



            }
        },
        computed: {
            pageSizes() {

                let sizes = [10, 20, 30, 40, 50, 100];
                if (this.designData && this.designData.ListConfig.List.Page.Enabled) {
                    let pageSize = this.designData.ListConfig.List.Page.PageSize;
                    if (sizes.findIndex(m => m == pageSize) == -1) {
                        sizes.push(pageSize);
                        sizes.sort((a, b) => a - b);

                    }

                }
                return sizes;

            },

            style() {
                return `<style>${this.css}</style>`
            },
            treePros() {
                if (this.tableType == 3)
                    return { children: 'children', hasChildren: 'haschildren' }
                return {};

            },
            listCols() {
                let cols = [];
                for (var col of this.designData.ListConfig.List.Cols) {

                    const item = this.allItems.find(m => !!m.Field && m.Field.toLowerCase() == col.Field.toLowerCase());
                    col.Item = item;
                    cols.push(col);


                }

                return cols;

            },
            showSummary() {

                return this.designData.ListConfig.List.Summary && this.designData.ListConfig.List.Summary.Enabled;

            },

            isLazyLoad() {

                return this.tableType == 3 && this.designData.ListConfig.List.Tree.LazyLoad;

            },

            tableType() {

                var listConfig = this.designData.ListConfig.List;
                var type = listConfig.Type;
                if (type == 1) return 1;
                if (type == 3 && listConfig.Tree.ParentField) return 3;

                if (type == 2 && listConfig.SubForm.FormCode && listConfig.SubForm.RefField)
                    return 2;

                return 1;

            },

            actionColWidth() {
                var w = this.designData.ListConfig.List.ActionCol.Width;
                return this.$isEmptyStr(w) ? 180 : w;


            },

            isReadonly() {
                return this.mode == 'view' || this.isSelect || this.isMultiSelect;
            },
            isSelect() {
                return this.mode == 'select';
            },

            isMultiSelect() {
                return this.mode == 'multiselect';
            },

            noPaging() {
                return this.routeQuery.page === 'N' || this.tableType == 3 || !this.designData.ListConfig.List.Page.Enabled;
            },
            FORM() {
                var state = this.$store.state;
                var obj = { canDo: this.$canDo, hasRole: this.$hasRole, selectRows: this.selectRows, user: state.user, formCode: this.formCode, queryArgs: this.params, formDef: this.formDef, urlArgs: this.routeQuery, token: state.token.AccessToken };
                obj.showDialog = this.$showDialog;
                obj.showDialogWithValue = this.$showDialogWithValue;
                obj.$this = this;


                obj.ajax = function (url, params, httpMethod) {

                    let isPost = httpMethod && httpMethod.toLowerCase() == 'post';
                    if (isPost)
                        return request.post(url, params);

                    return request.get(url, params);

                }

                obj.addListener = function (colName, eventName, event) {
                    let eventObj = this.$this.colEvent[colName] || {};
                    eventObj[eventName] = event;
                    Vue.set(this.$this.colEvent, colName, eventObj);

                }

                obj.setColDisabled = function (colName, state) {

                    let col = this.$this.listCols.find(m => m.Field == colName);
                    if (col && col.Formatter && col.Formatter.Type == 'switch') {
                        col.Formatter.Config.Disabled = state
                    }

                }

                return obj;
            },

            hasTreeItem() {

                return this.queryItems.find(m => m.type == 10) != null;
            },

            hasBatchOpBtn() {

                return this.designData.ListConfig.List.TopBtns.findIndex(m => m.IsShow && !!m.Url && m.Url.indexOf('[row.') != -1) >= 0;

            },
            hasRowBtn() {

                return this.designData.ListConfig.List.RowBtns.find(m => m.IsShow) != null;
            },

            topShowBtns() {

                var btns = this.designData.ListConfig.List.TopBtns.filter(m => !m.ShowInMore && m.IsShow);
                return this.buildBtns(btns, null)

            },
            topShowInMoreBtns() {

                var btns = this.designData.ListConfig.List.TopBtns.filter(m => m.ShowInMore && m.IsShow);
                return this.buildBtns(btns, null)

            }

        },
        watch: {

        },
        methods: {
            close() {

                this.$closeDialog();

            },
            getSummaries(param) {

                let summary = this.designData.ListConfig.List.Summary;
                if (this.showSummary && summary && summary.Enabled && summary.Title && this.summaryData) {

                    this.summaryData[0] = summary.Title;

                }

                return this.summaryData;

            },

            getColTagColorStyle(row, col) {

                let value = row[col.Field];
                var style = {};

                if (!this.$isEmptyStr(value) && col.dicValues) {

                    let dicValue = col.dicValues.find(m => m.Value.toLowerCase() == String(value).toLowerCase());
                    if (dicValue) {
                        style.color = '#fff';
                        style['background-color'] = dicValue.Color;
                        style['border-color'] = dicValue.Color;
                    }
                }
                return style;
            },
            rowDblclick(row, column, event) {

                if (this.isSelect) {

                    this.selectRow(row);
                }
            },
            listDataChange() {
                var self = this;
                setTimeout(function () {
                    self.$refs.table.doLayout();
                }, 10)

            },

            getNestedFormArgs(parentRow) {

                var args = {};
                args.code = this.designData.ListConfig.List.SubForm.FormCode;
                var refField = this.designData.ListConfig.List.SubForm.RefField;
                var queryJson = {};
                queryJson[refField] = this.getIdValue(parentRow);
                args.q = JSON.stringify(queryJson);
                args.parentRow = parentRow;
                args.mode = this.mode;
                return args;

            },
            rowClick(row, column, event) {
                if (!this.hasBatchOpBtn) {
                    this.selectRows = [row];

                }
            },
            getImgList(row, col) {

                var imgIds = row[col.Field];
                if (!imgIds) return [];

                var list = [];
                var config = col.Formatter.Config;
                var num = config.Num;
                var size = config.Size || 80;
                var idArray = imgIds.split(',');
                var index = 0;
                var previewList = [];
                for (var id of idArray) {
                    index++;
                    if (num > 0 && index > num) break;
                    var url = this.$appUrl + '/api/annex/download/' + id;
                    let img = { url: url, size: size }
                    list.push(img);
                    previewList.push(url);
                }

                for (let img of list) {

                    img.previewList = previewList;
                }

                return list;
            },
            evalJsCode(code, args) {
                args = args || {};
                let FORM = this.FORM;
                let row = args.row || {};
                let codeStr = `(function () {
                      ${code}
                   }

                   )()`

                var result = eval(codeStr);
                return result;
            },

            evalListFormatCode(row, col) {

                let code = col.Formatter.Config.Code;
                if (!code)
                    return this.getFieldDisplayText(row, col);

                try {

                    let FORM = this.FORM;
                    let codeStr = `(function () {
                        ${code}
                     }

                     )()`

                    var result = eval(codeStr);
                    return result;


                } catch (e) {

                    console.error('evalListFormatCode:', col, e);
                }

                return this.getFieldDisplayText(row, col);

            },
            calColVisible(col) {
                let visible = false;
                if (col.Width > 0) {
                    visible = true;
                    let cdt = col.Condition;
                    if (!this.$isEmptyStr(cdt)) {

                        try {
                            let result = this.evalJsCode(cdt);
                            visible = result === true;
                        } catch (e) {
                            console.error('calColVisible:', col, e);

                        }
                    }

                }

                return visible;


            },

            sortChange(obj) {

                var order = obj.order;
                var column = obj.prop;
                var sort = '';
                if (order == 'descending')
                    sort = '-';
                else if (order == 'ascending')
                    sort = '+';
                if (sort)
                    this.orderBy = column + sort;
                else
                    this.orderBy = '';

                this.searchChange();


            },

            doSelectRows() {

                this.$closeDialog(this.$copyObj(this.selectRows));

            },

            selectRow(row) {

                this.$closeDialog(this.$copyObj(row));

            },

            setQueryTreeEmpty() {
                if (this.$refs.queryTree) {
                    this.$refs.queryTree.setQueryTreeEmpty();

                }

            },

            getFieldDisplayText(row, col) {

                const field = col.Field;
                var value = row[field];
                if (value && typeof (value) === 'object') {
                    var txt = value.Text;
                    if (txt) {
                        if (Array.isArray(txt))
                            value = txt.join(', ')
                        else
                            value = txt;
                    }
                    else
                        value = value.Value
                }
                if (value) {
                    if (col.Item) {
                        if (col.Item.Type == 'DatePicker') {
                            if (col.Item.DateFormat == 2)
                                value = this.$formatDate(value, 'yyyy-MM-dd HH:mm:ss')
                            else if (col.Item.DateFormat == 1)
                                value = this.$formatDate(value, 'yyyy-MM-dd')
                        } else if (col.Item.Type == 'Location') {

                            try {
                                var obj = JSON.parse(value);
                                return obj.title || obj.address;

                            } catch (e) { }


                        }

                    }
                }

                return value;

            },
            selectionChange(selection) {

                this.selectRows = selection;

            },


            getIdValue(row) {

                return row["Id"] || row["id"];
            },


            openDialog(title, url, confirm) {
                let queryArgs = this.$getUrlQuery(url);
                title = queryArgs.t || title;
                let width = queryArgs.w || this.$consts.diaWidth;
                let height = queryArgs.h || this.$consts.diaHeight;
                let opts = { url: url, title: title, width: width, height: height };
                if (confirm) {
                    opts.beforeClose = confirm;
                }
                this.$showDialog(opts);

            },

            handleBtnAction(btn, args) {
                let target = args.target;
                let url = args.url
                if (target == 'openurl') {

                    window.open(url);

                } else if (target == 'redirect') {
                    url = this.$buildAbsUrl(this.$appUrl, url);
                    window.redirect(url);

                } else if (target == 'opentab') {

                    let queryArgs = this.$getUrlQuery(url);
                    let title = queryArgs.t || btn.Name;
                    this.$store.commit('addMenu', { path: url, name: title });


                }
                else if (target == 'opendialog') {

                    this.openDialog(btn.Name, url);
                }


            },
            async invokeDropdownBtnClick(cmd) {

                await this.invokeBtnClick(cmd.btn, cmd.row)

            },

            async invokeBtnClick(btn, row) {
                this.curBtnRow = row;
                let state = btn.state;
                btn = btn.btn;
                let cdt = btn.Condition;
                if (cdt) {

                    try {
                        let result = this.evalJsCode(cdt, { clicked: true, row });
                        if (typeof result === 'object' && result.cancel === true) {
                            return;
                        }

                    } catch (e) {

                        console.error("按钮条件错误", btn, e);

                    }

                }


                if (btn.Id == 'view') {
                    let id = this.getIdValue(row);
                    let url = state.url || `/form/view?code=${this.formCode}&id=${id}&mode=view`;
                    let title = `${this.formDef.Name} - 查看`
                    this.openDialog(title, url);

                }
                else if (btn.Id == 'add') {
                    let url = state.url || `/form/view?code=${this.formCode}`;
                    let title = `${this.formDef.Name} - 添加`
                    this.openDialog(title, url);

                } else if (btn.Id == 'edit') {
                    let id = this.getIdValue(row);
                    let url = state.url || `/form/view?code=${this.formCode}&id=${id}`;
                    let title = `${this.formDef.Name} - 编辑`
                    this.openDialog(title, url);
                }
                else if (btn.Id == 'import') {

                    let importApi = this.designData.ExportConfig.ImportApi;
                    if (importApi) importApi = this.$urlEncode(importApi);
                    let url = state.url || `/form/import?code=${this.formCode}&importApi=${importApi}`;
                    let title = `${this.formDef.Name} - 导入数据`
                    let opts = { url: url, title: title, width: 450, resizeable: false };
                    this.$showDialog(opts);
                }
                else if (btn.Id == 'export') {

                    let exportApi = this.designData.ExportConfig.ExportApi;
                    let url = state.url || exportApi || `/formdef/ExportData`;
                    let dto = this.processParams(this.params);

                    dto.queryJson = JSON.parse(dto.queryJson);
                    dto.queryJson.page = 1;
                    dto.queryJson.count = 10000;
                    dto.queryJson = JSON.stringify(dto.queryJson);

                    const loading = this.$showLoading();
                    let res = await request.post(url, dto);
                    loading.close();
                    if (res.IsSuccess) {
                        this.$okTip(res.Msg || '导出成功');
                        window.location = this.$appUrl + res.Data;

                    } else {

                        this.$errTip(res.Msg);

                    }

                }
                else {

                    let target = btn.Target;
                    let url = state.url;

                    if (!url) {
                        if (btn.Id == 'delete') {
                            var id = this.getIdValue(row);
                            url = `/formdef/deletedata?FormCode=${this.formCode}&Ids=${id}`;
                        }
                        else if (btn.Id == 'batchdelete') {
                            let ids = this.selectRows.map(function (m) { return this.getIdValue(m) }).join(',');
                            url = `/formdef/deletedata?FormCode=${this.formCode}&Ids=${ids}`;
                        }
                    }

                    if (!url) {
                        this.$errTip('按钮地址为空！');
                        return;
                    }

                    if (state.argType > 0 && this.selectRows.length == 0 && !row) {

                        this.$errTip("请选择要操作的数据！")
                        return;
                    }


                    if (state.argType == 1 && this.selectRows.length > 1) {

                        this.$errTip("只能选择一条数据！")
                        return;
                    }


                    if (target == 'api') {

                        let queryArgs = this.$getUrlQuery(url);
                        let defTip = '确认操作吗？';
                        if (btn.Id == 'delete') {

                            defTip = '确认删除此条数据吗？'

                        } else if (btn.Id == 'batchdelete') {

                            defTip = '确认删除所选数据吗？'
                        }


                        let tip = queryArgs.tip;

                        if (tip === undefined || tip) {
                            tip = tip || defTip;
                            const confirmed = await this.$confirmTip(tip);
                            if (!confirmed) return;
                        }

                        let args = {};

                        if (btn.Method == 'post' && btn.ArgTran == 'body') {
                            let markIndex = url.indexOf('?');
                            if (markIndex > 0)
                                url = url.substring(0, markIndex);
                            args = queryArgs;

                        }
                        const loading = this.$showLoading();

                        let res
                        if (btn.Method == 'post') {
                            for (var key in args) {
                                args[key] = this.$urlDecode(args[key]);
                            }
                            res = await request.post(url, args);
                        }
                        else
                            res = await request.get(url, args);

                        loading.close();
                        if (res.IsSuccess) {

                            if (res.Data && res.Data.Action && res.Data.Url) {
                                this.handleBtnAction(btn, { target: btn.Action, url: btn.Url });
                            } else {
                                if (queryArgs.reload != '0') {
                                    this.refreshData();
                                }
                                this.$okTip(res.Msg || '操作成功');
                            }

                        } else {

                            this.$errTip(res.Msg);

                        }

                    } else {

                        this.handleBtnAction(btn, { target, url })
                    }


                }

            },
            getRowShowBtns(row) {

                let btns = this.designData.ListConfig.List.RowBtns.filter(m => !m.ShowInMore && m.IsShow);
                return this.buildBtns(btns, row)
            },

            getRowShowInMoreBtns(row) {

                let btns = this.designData.ListConfig.List.RowBtns.filter(m => m.ShowInMore && m.IsShow);
                return this.buildBtns(btns, row)
            },

            buildBtns(btns, row) {

                var FORM = this.FORM;
                var stateBtns = [];

                if (!row)
                    row = this.selectRows[0] || {};

                for (let btn of btns) {
                    let state = { visible: true, enabled: true };
                    let cdt = btn.Condition;
                    if (cdt) {
                        try {
                            let result = this.evalJsCode(cdt, { row: row });
                            if (typeof result === 'object') {
                                state.visible = result.visible;
                                state.enabled = result.enabled;
                            } else {

                                state.enabled = result === true;
                            }


                        } catch (e) {

                            console.error("按钮条件错误", btn, e);

                        }

                    }

                    state.argType = 0;

                    if (state.visible && state.enabled && btn.Url) {

                        let url = btn.Url;
                        if (url.indexOf('[row.') != -1)
                            state.argType = 2;
                        else if (url.indexOf('{row.') != -1)
                            state.argType = 1;

                     
                        if (row) {
                            let self = this;
                            for (let key in row) {
                                url = url.replace(`{row.${key}}`, this.$urlEncode(this.getItemRealValue(row[key])));
                                let values = this.selectRows.map(function (m) { return self.getItemRealValue(m[key]) }).join(',');
                                url = url.replace(`[row.${key}]`, this.$urlEncode(values));
                            }
                        }

                        if (this.isNested) {
                            let parentRow = this.args.parentRow;
                            for (let key in parentRow) {
                                url = url.replace(`{parentRow.${key}}`, this.$urlEncode(this.getItemRealValue(parentRow[key])));

                            }
                        }


                        url = url.replace(`{formCode}`, FORM.formCode);
                        url = url.replace(`{formName}`, FORM.formDef.Name);
                        url = url.replace(`{token}`, FORM.token);
                        url = url.replace(`{query}`, this.$urlEncode(JSON.stringify(FORM.queryArgs)));

                        for (let key in FORM.user) {
                            url = url.replace(`{user.${key}}`, this.$urlEncode(FORM.user[key]))

                        }

                        for (let key in FORM.queryArgs) {
                            url = url.replace(`{query.${key}}`, this.$urlEncode(FORM.queryArgs[key] || ''))

                        }
                        for (let key in FORM.routeQuery) {
                            url = url.replace(`{url.${key}}`, this.$urlEncode(FORM.routeQuery[key]))

                        }

                        state.url = url;
                    }

                    if (state.visible) {

                        stateBtns.push({ state: state, btn: btn });

                    }

                }

                return stateBtns;

            },

            itemTypeToQueryType(itemType) {

                if (itemType == 'Text' || itemType == 'RichText')
                    return 2;

                if (itemType == 'Number')
                    return 3;

                if (itemType == 'DateTime')
                    return 4;

                if (itemType == 'Select')
                    return 5;

                if (itemType == 'Radio')
                    return 7;

                if (itemType == 'CheckBox')
                    return 8;


                return -1;

            },

            async buildQueryItem(designData) {

                var queryItems = [];
                var items = designData.Items;
                var cdts = designData.ListConfig.Query.Cdts;
                var params = this.tranQueryJsonToParam(this.queryJson);
                var queryParams = {};
                for (var cdt of cdts) {

                    var queryItem = {};
                    var renderType = cdt.RenderType;
                    var item = items.find(m => m.Field == cdt.Field);
                    if (renderType == 1 && item) {

                        renderType = this.itemTypeToQueryType(item.Type);
                        if (renderType == -1) { continue; }


                    }

                    if (renderType >= 5) {

                        if (!item || !this.isDataSourceItem(item)) continue;

                        await this.getItemDsData(item);
                        var opts = await this.getItemBindData(item);
                        if (renderType == 9 || renderType == 10)
                            opts.splice(0, 0, { name: '全部', value: '' })

                        queryItem.opts = opts;

                        if (item.DataSourceType == 2 && item.Type == 'Select' || item.RenderType == 2) {
                            queryItem.isTree = true;
                            queryItem.strict = item.CheckStrictly;

                        }

                    }

                    let field = cdt.Field;
                    queryItem.type = renderType;
                    queryItem.span = cdt.Span;
                    queryItem.title = cdt.Title;
                    queryItem.field = field;


                    if (!this.$isEmptyStr(cdt.Condition)) {

                        try {

                            let result = this.evalJsCode(cdt.Condition, queryItem);
                            if (!result) continue;
                        } catch (e) {
                            console.error('calQueryVisible:', cdt.Title, e);
                            continue
                        }

                    }


                    queryItems.push(queryItem);

                    if (params[field] !== undefined) {
                        queryParams[field] = params[field];
                        delete this.queryJson[field];

                    } else {
                        queryParams[field] = null;

                    }


                }

                this.queryItems = queryItems;
                this.params = queryParams;


            },

            resetDefaultData() {
                this.orderBy = '';
                this.data = [];
                this.defaultSort = { prop: 'CreateTime', order: 'descending' };
                this.page.pageSize = 10;
                this.page.pageIndex = 1;
                this.isSearchShow = true;
                this.params = {};
                this.pageId = this.$newId();

            },
            buildDefaultSort() {

                let sortCol = this.designData.ListConfig.List.Cols.find(m => m.Sort == 'asc' || m.Sort == 'desc');
                if (sortCol) {
                    this.orderBy = sortCol.Field + (sortCol.Sort == 'asc' ? '+' : '-');
                    let order = sortCol.Sort == 'asc' ? 'ascending ' : 'descending'
                    this.defaultSort = { prop: sortCol.Field, order: order };
                }

            },

            filterCmdBtns(designData) {

                var rowBtns = designData.ListConfig.List.RowBtns.filter(m => !m.Roles || m.Roles.length == 0 || this.$hasRole(m.Roles));
                var topBtns = designData.ListConfig.List.TopBtns.filter(m => !m.Roles || m.Roles.length == 0 || this.$hasRole(m.Roles));
                designData.ListConfig.List.RowBtns = rowBtns;
                designData.ListConfig.List.TopBtns = topBtns;

            },
            async loadListColDicValues() {

                for (var col of this.listCols) {
                    var item = col.Item;
                    if (!item || item.DataSourceType != 1 || !this.isDataSourceItem(item) || !col.Formatter || col.Formatter.Type != 'tag' || col.Formatter.Config.Color != 'auto') continue;

                    const values = await this.getItemDsData(item);
                    col.dicValues = values;


                }


            },


            async initDesignData() {
                let res = await $formDefApi.getByCode(this.formCode);
                this.formDef = res.Data;
                let designData = JSON.parse(res.Data.Data);
                this.allItems = this.getAllItems(designData.Items);

                this.filterCmdBtns(designData);
                this.designData = designData;
                this.buildDefaultSort();
                await this.buildQueryItem(this.designData);
                await this.loadListColDicValues();

            },

            processParams(params) {

                let query = this.$copyObj(this.queryJson || {});
                let param = {
                    page: this.page.pageIndex,
                    count: this.page.pageSize,
                }
                let queryMulti = {};
                let combine = [];
                for (var key in params) {

                    var queryItem = this.queryItems.find(m => m.field == key);
                    var value = params[key];
                    var cdtKey = key + '{}';
                    if (this.$isNull(value) || this.$isEmptyStr(value)) continue;

                    var itemDef = this.designData.Items.find(m => m.Field == key);

                    if (Array.isArray(value) && (queryItem.type == 3 || queryItem.type == 4)) {

                        var value2 = value[1];
                        value = value[0];

                        if (this.$isNotNull(value) && this.$isNotNull(value2)) {

                            query[key + '&{}'] = `>=${value},<=${value2}`;

                        }
                        else if (this.$isNotNull(value)) {

                            query[cdtKey] = `>=${value}`;
                        }

                        else if (this.$isNotNull(value2)) {

                            query[cdtKey] = `<=${value2}`;

                        }


                    } else {

                        if (queryItem.type == 2) {
                            query[key + '$'] = `%${value}%`;

                        } else if (queryItem.type == 6 || queryItem.type == 8) {

                            if (value.length > 0) {
                                let index = 1;
                                let qKeys = [];
                                for (var v of value) {
                                    let qKey = `${key}${index}{}`;
                                    qKeys.push(qKey);
                                    queryMulti[qKey] = `charindex(',${v},',','+${key}+',')>0`;
                                    index++;
                                }
                                combine.push('(' + qKeys.join('|') + ')');
                            }
                        }
                        else if (queryItem.type == 10 && (itemDef && (itemDef.Type == 'CheckBox' || (itemDef.Type == 'Select' && itemDef.MultiSelect)))) {

                            query[cdtKey] = `charindex(',${value},',','+cast(${key} as varchar)+',')>0`
                        }

                        else {

                            query[cdtKey] = value;

                        }

                    }

                }

                if (combine.length > 0) {

                    for (let k in query) {
                        combine.push(k);
                    }
                    query['@combine'] = combine.join('&');
                    for (let k in queryMulti) {

                        query[k] = queryMulti[k];

                    }
                }


                var orderBy = this.orderBy || 'CreateTime-'
                query['@order'] = orderBy;
                param["Table"] = query;
                return { formCode: this.formCode, queryJson: JSON.stringify(param) };

            },


            processData(data) {

                if (this.tableType == 3 && !this.isLazyLoad) {
                    var pidField = this.designData.ListConfig.List.Tree.ParentField;
                    var rootNodes = this.buildNodesToTree(data.Rows, 'Id', pidField);
                    this.removeEmptyChild(rootNodes);
                    return rootNodes;
                }

                return data.Rows;

            },

            async getChildren(pid) {

                let pidField = this.designData.ListConfig.List.Tree.ParentField;
                let dto = { formCode: this.formCode, }
                var queryJson = { page: 1, count: 1000, Table: { "@order": this.orderBy } }
                queryJson.Table[pidField] = pid;
                dto.queryJson = JSON.stringify(queryJson);

                let res = await $formDefApi.getPageData(dto);
                if (res.IsSuccess) {
                    return res.Data.Rows;

                } else {

                    this.$errTip(res.Msg);
                    return [];
                }

            },

            async loadChildren(data, treeNode, resolve) {
                let pid = this.getIdValue(data);
                let rows = await this.getChildren(pid);
                resolve(rows);
                this.expandedData[pid] = { data, treeNode, resolve };

            },

            async getList() {

                this.loading = true;
                let queryParams = this.processParams(this.params);
                let res = await $formDefApi.getPageData(queryParams);
                if (res.IsSuccess) {
                    this.page.total = res.Data.Total;
                    this.data = this.processData(res.Data);
                    this.$emit('listDataChange', this.data);
                    let smData = res.Data.Data;
                    if (this.showSummary && smData && smData.length == 1) {
                        let summaryData = [''];
                        if (this.tableType == 2) summaryData.push('')

                        smData = smData[0];
                        for (let col of this.listCols) {
                            let visible = this.calColVisible(col);
                            if (!visible) continue;
                            let value = smData[col.Field];
                            summaryData.push(value);


                        }
                        this.summaryData = summaryData;
                    }

                } else {

                    this.$errTip(res.Msg);
                }
                this.loading = false;



            },

            refreshData() {
                let treeConfig = this.designData.ListConfig.List.Tree;
                if (this.isLazyLoad && this.curBtnRow) {

                    let pid = this.curBtnRow[treeConfig.ParentField];
                    let expandedRow = this.expandedData[pid];
                    if (!expandedRow) {
                        expandedRow = this.expandedData[this.getIdValue(this.curBtnRow)];
                    }

                    if (expandedRow) {
                        const { data, treeNode, resolve } = expandedRow;
                        this.loadChildren(data, treeNode, resolve);

                    } else {
                        this.getList();
                    }
                }
                else {
                    this.getList();
                }

            },

            tranQueryJsonToParam(queryJson) {
                let params = {};
                for (let key in queryJson) {

                    let value = queryJson[key];
                    let paramKey = key.replace('{}', '').replace('&', '').replace('$', '');

                    if (!Array.isArray(value) && !this.$isEmptyStr(value)) {
                        value = String(value);
                        value = value.replaceAll('%', '');
                        if (value.indexOf('>') >= 0 || value.indexOf('<') >= 0) {
                            value = value.replaceAll('>', '').replaceAll('<', '').replaceAll('=', '');
                            let array = value.split(',');
                            if (array.length == 1)
                                array.push(null)

                            value = array;
                        }


                    }
                    params[paramKey] = value;
                }
                return params;

            },


            async loadAll() {

                this.preLoading = true;
                var query = this.$copyObj(this.isNested ? this.args : this.$route.query);
                this.formCode = query.code || '';
                this.mode = query.mode || 'edit';
                var queryJson = query.q;
                if (queryJson) {
                    this.queryJson = JSON.parse(queryJson);
                }
                this.routeQuery = query;


                await this.initDesignData();

                var pageSize = query.pageSize;
                if (pageSize) {
                    this.page.pageSize = parseInt(pageSize) || 10;
                } else {
                    this.page.pageSize = this.designData.ListConfig.List.Page.PageSize;

                }

                if (this.noPaging) {

                    this.page.pageSize = 1000;
                }

                await this.getList();
                this.preLoading = false;


            },
            executeRuleFn(name) {
                var ruleFn = this.ruleFn;
                if (!ruleFn) return;
                var fn = ruleFn(this.FORM)[name];
                if (!fn) return;
                var result = fn();
                return result;

            }
        },


        async created() {


            this.$on('list.reload', () => {
                this.refreshData();

            });

            await this.loadAll();

            const funName = 'listInit';
            this.ruleFn = this.tryCompileJs(this.formDef.ClientRule, [funName]);
            this.executeRuleFn(funName);



        },

        components: { FormList }


    }


</script>

<style scoped>

    .list-loading {
        height: 100%;
        box-sizing: border-box;
    }
   
    .root-container {
        height: 100%;
        font-size:12px !important;
    }

    .form-container {
        height: 0 !important;
    }

    .nct-cmd-area {
        padding: 0px 15px 15px 15px
    }

    .nct-toolbar {
        padding-top: 15px;
        padding-bottom: 0px;
    }

    .nct-cmd-area:empty {
        display: none;
    }

    .nested-list .nct-work-area {
        padding-left: 0;
    }

    /deep/ .nested-list .nct-list-table .el-table__body-wrapper {
        height: auto !important;
    }
</style>
