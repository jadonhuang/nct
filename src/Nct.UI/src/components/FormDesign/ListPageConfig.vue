<template>

    <div class="listpage-holder">


        <!--基本信息设置-->
        <template v-if="designer.curMenu=='listbase'">


            <el-form label-width="140px" size="medium">


                <el-form-item label="数据源类型：">
                    <el-radio-group v-model="designData.ListConfig.DataSource.Enabled">
                        <el-radio-button :label="false" border>默认数据源</el-radio-button>
                        <el-radio-button :label="true" border>自定义数据源</el-radio-button>
                    </el-radio-group>
                </el-form-item>



                <el-form-item label="选择数据源：" v-if="designData.ListConfig.DataSource.Enabled">
                    <el-select placeholder="请选择" v-model="designData.ListConfig.DataSource.Code" filterable style="width:250px;">

                        <el-option v-for="item in designer.dataSources"
                                   :key="item.Code"
                                   :label="item.Name"
                                   :value="item.Code">
                        </el-option>

                    </el-select>
                    <el-link style="margin-left:5px;" type="primary" :underline="false" @click="designer.showDsCreator()" icon="el-icon-circle-plus-outline">添加数据源</el-link>
                    <el-link style="margin-left:5px;" v-if="designData.ListConfig.DataSource.Code" type="primary" @click="designer.showDsCreator(designData.ListConfig.DataSource.Code)" :underline="false" icon="el-icon-edit">编辑数据源</el-link>
                </el-form-item>


                <el-form-item label="列表类型：">
                    <el-radio-group v-model="designData.ListConfig.List.Type">
                        <el-radio-button :label="1" border>普通列表</el-radio-button>
                        <el-radio-button :label="2" border>明细列表</el-radio-button>
                        <el-radio-button :label="3" border>树形列表</el-radio-button>
                    </el-radio-group>
                </el-form-item>


                <el-row v-if="designData.ListConfig.List.Type==2">

                    <el-col :span="8">
                        <el-form-item label="子表单编号：">
                            <el-input v-model="designData.ListConfig.List.SubForm.FormCode"></el-input>
                        </el-form-item>

                    </el-col>
                    <el-col :span="16">
                        <el-form-item label="关联字段：">
                            <el-input v-model="designData.ListConfig.List.SubForm.RefField" style="width:200px" ></el-input>
                        </el-form-item>

                    </el-col>
                </el-row>

                <template v-else-if="designData.ListConfig.List.Type==3">
                    <el-row v-if="designData.ListConfig.List.Page">

                        <el-col :span="8">

                            <el-form-item label="父级字段：">
                                <el-input v-model="designData.ListConfig.List.Tree.ParentField"></el-input>
                            </el-form-item>

                        </el-col>
                        <el-col :span="16">
                            <el-form-item label="手动加载子级：">

                                <el-switch v-model="designData.ListConfig.List.Tree.LazyLoad"
                                           active-text="是"
                                           inactive-text="否">
                                </el-switch>


                            </el-form-item>

                        </el-col>
                    </el-row>

                </template>

                <el-form-item label="操作列宽度：">

                    <el-input-number v-model="designData.ListConfig.List.ActionCol.Width" controls-position="right" :min="0"></el-input-number>

                </el-form-item>

                <el-row v-if="designData.ListConfig.List.Page">

                    <el-col :span="8">
                        <el-form-item label="显示分页：">

                            <el-switch v-model="designData.ListConfig.List.Page.Enabled"
                                       active-text="是"
                                       inactive-text="否">
                            </el-switch>

                        </el-form-item>

                    </el-col>
                    <el-col :span="16">
                        <el-form-item label="每页条数：" v-if="designData.ListConfig.List.Page.Enabled">

                            <el-input-number v-model="designData.ListConfig.List.Page.PageSize" controls-position="right" :min="0"></el-input-number>
                        </el-form-item>

                    </el-col>
                </el-row>


                <el-row v-if="designData.ListConfig.List.Summary">

                    <el-col :span="8">
                        <el-form-item label="显示合计行：">

                            <el-switch v-model="designData.ListConfig.List.Summary.Enabled"
                                       active-text="是"
                                       inactive-text="否">
                            </el-switch>

                        </el-form-item>

                    </el-col>
                    <el-col :span="16">
                        <el-form-item label="合计文本：" v-if="designData.ListConfig.List.Summary.Enabled">

                            <el-input v-model="designData.ListConfig.List.Summary.Title" style="width: 200px" ></el-input>
                        </el-form-item>

                    </el-col>
                </el-row>


                <el-form-item label="数据权限：">
                    <el-radio-group v-model="designData.ListConfig.DataAuth">
                        <el-radio-button :label="0" border>无数据权限</el-radio-button>
                        <el-radio-button :label="1" border>所属部门</el-radio-button>
                        <el-radio-button :label="2" border>所属部门及下级部门</el-radio-button>
                        <el-radio-button :label="3" border>仅自己创建的数据</el-radio-button>
                    </el-radio-group>
                </el-form-item>

            </el-form>

        </template>


        <!--搜索设置-->
        <div v-else-if="designer.curMenu=='listquery'">

            <el-table title="拖动行第一列排序" key="querytb" height="400px" size="mini" border :data="designData.ListConfig.Query.Cdts"
                      style="width: 100%" @select="setSelectQueryCdts" @select-all="setSelectQueryCdts" class="query-table table" row-key="Id">

                <el-table-column type="selection" class-name="selection" width="55" align="center">
                </el-table-column>


                <el-table-column label="搜索项" width="150" class-name="edit-cell">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,1,'query')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,1,'query')">

                                <el-select placeholder="请选择" v-model="scope.row.Field" @change="setCellText(scope.row)" filterable size="mini" v-if="!isListUseDs">

                                    <el-option v-for="item in hasFieldItems"
                                               :key="item.Id"
                                               :label="item.Title"
                                               :value="item.Field">
                                    </el-option>

                                </el-select>

                                <el-select v-else placeholder="请选择" @change="setCellText(scope.row)" v-model="scope.row.Field" filterable size="mini">

                                    <el-option v-for="item in designer.dsColumns[designData.ListConfig.DataSource.Code]"
                                               :key="item.Value"
                                               :label="item.Name"
                                               :value="item.Value">
                                    </el-option>

                                </el-select>


                            </div>
                            <div v-else class="cell-text">
                                {{displayFieldCellValue(scope.row)}}
                            </div>
                        </div>
                    </template>
                </el-table-column>

                <el-table-column label="展示名" width="150" class-name="edit-cell">
                    <template slot-scope="scope">

                        <div @click="setCellEdit(scope.$index,2,'query')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,2,'query')">

                                <el-input v-model="scope.row.Title" size="mini"></el-input>

                            </div>
                            <div v-else class="cell-text">
                                {{scope.row.Title}}
                            </div>
                        </div>

                    </template>
                </el-table-column>

                <el-table-column label="所占比例" width="150" class-name="edit-cell">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,6,'query')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,6,'query')">
                                <el-select placeholder="请选择" v-model="scope.row.Span" size="mini">
                                    <el-option :key="index" :label="item.label" :value="item.value" v-for="(item,index) in consts.form.spanOpts"> </el-option>

                                </el-select>
                            </div>
                            <div v-else class="cell-text">
                                {{displayQuerySpanValue(scope.row)}}
                            </div>
                        </div>

                    </template>
                </el-table-column>


                <el-table-column label="展现形式" class-name="edit-cell" width="150">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,4,'query')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,4,'query')">
                                <el-select placeholder="请选择" v-model="scope.row.RenderType" size="mini">

                                    <el-option v-for="item in $consts.form.queryRenderTypes"
                                               :key="item.value"
                                               :label="item.label"
                                               :value="item.value">
                                    </el-option>

                                </el-select>
                            </div>
                            <div v-else class="cell-text">
                                {{displayRenderTypeValue(scope.row)}}
                            </div>
                        </div>

                    </template>
                </el-table-column>

                <el-table-column label="前置条件" class-name="edit-cell">
                    <template slot-scope="scope">

                        <div @click="setCellEdit(scope.$index,5,'query')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,5,'query')">

                                <el-input v-model="scope.row.Condition" size="mini"></el-input>

                            </div>
                            <div v-else class="cell-text">
                                {{scope.row.Condition}}
                            </div>
                        </div>

                    </template>
                </el-table-column>


            </el-table>

            <div class="list-btn">
                <el-button-group>
                    <el-button icon="el-icon-circle-plus-outline" size="mini" @click="addQueryCdts">新增</el-button>
                    <el-button icon="el-icon-delete" size="mini" @click="delQueryCdts">删除</el-button>
                </el-button-group>

            </div>
        </div>

        <!--列表设置-->
        <div v-else-if="designer.curMenu=='listtable'">

            <el-alert title="右键单击选中/取消选中按钮，左键单击编辑按钮" type="warning" :closable="false" style="margin-bottom:10px;">
            </el-alert>
           

            <!--表头按钮-->
            <div class="list-btn2" style="border-bottom:0;">
                <div style="float:left;height:30px;line-height:30px">
                    顶部按钮
                </div>

                <div class="btn-group">
                    <div v-for="btn in tableButtons" :key="btn.Id" @contextmenu.prevent="btn.IsShow=!btn.IsShow" @click="editBtn(btn,designData.ListConfig.List.TopBtns,true)" :class="{'lbtn':true,'active':btn.IsShow}"><i :class="['btn-icon',btn.Icon]"></i>{{btn.Name}}</div>
                    <el-button icon="el-icon-plus" class="add-btn" title="添加顶部按钮" @click="addBtn(designData.ListConfig.List.TopBtns,true)"></el-button>
                </div>

            </div>


            <el-table title="拖动行第一列排序" size="mini" border :data="designData.ListConfig.List.Cols" @select="setSelectListCols" @select-all="setSelectListCols"
                      class="list-table table" row-key="Id" height="300px">


                <el-table-column type="selection" class-name="selection" align="center"
                                 width="55">
                </el-table-column>

                <el-table-column label="列字段" width="150" class-name="edit-cell" column-key="field">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,1,'lstcol')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,1,'lstcol')">

                                <el-select placeholder="请选择" v-model="scope.row.Field" @change="setCellText(scope.row)" filterable size="mini" v-if="!isListUseDs">

                                    <el-option v-for="item in hasFieldItems"
                                               :key="item.Id"
                                               :label="item.Title"
                                               :value="item.Field">
                                    </el-option>

                                </el-select>

                                <el-select v-else placeholder="请选择" @change="setCellText(scope.row)" v-model="scope.row.Field" filterable size="mini">

                                    <el-option v-for="item in designer.dsColumns[designData.ListConfig.DataSource.Code]"
                                               :key="item.Value"
                                               :label="item.Name"
                                               :value="item.Value">
                                    </el-option>

                                </el-select>


                            </div>
                            <div v-else class="cell-text">
                                {{displayFieldCellValue(scope.row)}}
                            </div>
                        </div>
                    </template>
                </el-table-column>

                <el-table-column label="标题" width="150" class-name="edit-cell">
                    <template slot-scope="scope">

                        <div @click="setCellEdit(scope.$index,2,'lstcol')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,2,'lstcol')">

                                <el-input v-model="scope.row.Title" size="mini"></el-input>

                            </div>
                            <div v-else class="cell-text">
                                {{scope.row.Title}}
                            </div>
                        </div>


                    </template>
                </el-table-column>
                <el-table-column label="对齐" width="80" class-name="edit-cell" align="center">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,3,'lstcol')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,3,'lstcol')">
                                <el-select placeholder="请选择" v-model="scope.row.Align" size="mini">

                                    <el-option v-for="item in $consts.form.alignTypes"
                                               :key="item.value"
                                               :label="item.label"
                                               :value="item.value">
                                    </el-option>

                                </el-select>
                            </div>
                            <div v-else class="cell-text">
                                {{displayAlignTypeValue(scope.row)}}
                            </div>
                        </div>

                    </template>
                </el-table-column>
                <el-table-column label="宽度" width="80" class-name="edit-cell" align="center">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,4,'lstcol')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,4,'lstcol')">

                                <el-input-number v-model="scope.row.Width" :min="0" :controls="false" size="mini"></el-input-number>

                            </div>
                            <div v-else class="cell-text">
                                {{scope.row.Width}}
                            </div>
                        </div>

                    </template>
                </el-table-column>


                <el-table-column label="数据排序" width="90" class-name="edit-cell" align="center">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,5,'lstcol')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,5,'lstcol')">
                                <el-select placeholder="请选择" v-model="scope.row.Sort" size="mini">

                                    <el-option v-for="item in $consts.form.sortTypes"
                                               :key="item.value"
                                               :label="item.label"
                                               :value="item.value">
                                    </el-option>

                                </el-select>
                            </div>
                            <div v-else class="cell-text">
                                {{displaySortTypeValue(scope.row)}}
                            </div>
                        </div>

                    </template>
                </el-table-column>


                <el-table-column label="合计方式" width="90" class-name="edit-cell" align="center" v-if="designData.ListConfig.List.Summary&&designData.ListConfig.List.Summary.Enabled">
                    <template slot-scope="scope">
                        <div @click="setCellEdit(scope.$index,8,'lstcol')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,8,'lstcol')">
                                <el-select placeholder="请选择" v-model="scope.row.SummaryType" size="mini">

                                    <el-option v-for="item in $consts.form.summaryTypes"
                                               :key="item.value"
                                               :label="item.label"
                                               :value="item.value">
                                    </el-option>

                                </el-select>
                            </div>
                            <div v-else class="cell-text">
                                {{displaySummaryTypeValue(scope.row)}}
                            </div>
                        </div>

                    </template>
                </el-table-column>


                <el-table-column label="格式化" width="90" class-name="edit-cell" align="center">
                    <template slot-scope="scope">
                        <div @click="showFieldFmtEditor(scope.row)" class="cell-text" style="height:28px;line-height:28px;">

                            {{ displayFormatterCellValue(scope.row)}}
                        </div>

                    </template>
                </el-table-column>

                <el-table-column label="前置条件" class-name="edit-cell">
                    <template slot-scope="scope">

                        <div @click="setCellEdit(scope.$index,7,'lstcol')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,7,'lstcol')">

                                <el-input v-model="scope.row.Condition" size="mini"></el-input>

                            </div>
                            <div v-else class="cell-text">
                                {{scope.row.Condition}}
                            </div>
                        </div>


                    </template>
                </el-table-column>


            </el-table>

            <div class="list-btn">
                <el-button-group>
                    <el-button icon="el-icon-circle-plus-outline" @click="addListCols" size="mini">新增</el-button>
                    <el-button icon="el-icon-delete" @click="delListCols" size="mini">删除</el-button>
                </el-button-group>

            </div>
            <div class="list-btn2 no-top-border">
                <div style="float:left;height:30px;line-height:30px">
                    行内按钮
                </div>



                <div class="btn-group">
                    <div v-for="btn in tableRowButtons" :key="btn.Id" @click="editBtn(btn,designData.ListConfig.List.RowBtns)" @contextmenu.prevent="btn.IsShow=!btn.IsShow" :class="{'lbtn':true,'active':btn.IsShow}"><i v-if="btn.Icon" :class="['btn-icon',btn.Icon]"></i>{{btn.Name}}</div>
                    <el-button icon="el-icon-plus" class="add-btn" title="添加行内按钮" @click="addBtn(designData.ListConfig.List.RowBtns)"></el-button>
                </div>

            </div>

        </div>



        <div v-else-if="designer.curMenu=='export'">
            <el-form label-width="120px" size="medium">

                <el-row>
                    <el-col :span="12">

                        <el-form-item label="导入接口地址：">
                            <el-input v-model="designData.ExportConfig.ImportApi" placeholder="自定义导入接口地址如：/myapi/import"></el-input>
                        </el-form-item>
                    </el-col>


                    <el-col :span="12">

                        <el-form-item label="导出接口地址：">
                            <el-input v-model="designData.ExportConfig.ExportApi" placeholder="自定义导出接口地址如：/myapi/export"></el-input>
                        </el-form-item>
                    </el-col>

                </el-row>



                <el-row>

                    <el-col :span="24">

                        <el-form-item label="导入方式：">
                            <el-radio-group v-model="designData.ExportConfig.ImportMode">
                                <el-radio :label="1">只新增数据</el-radio>
                                <el-radio :label="2">只编辑已有数据</el-radio>
                                <el-radio :label="3">新增和编辑数据</el-radio>
                            </el-radio-group>
                        </el-form-item>


                    </el-col>

                </el-row>

            </el-form>

            <el-alert title="勾选需要导入、导出的列，导入方式如果为只编辑数据则必须设置唯一列" type="warning" :closable="false" style="margin-bottom:10px;">
            </el-alert>


            <el-table size="mini" border key="exporttb" :data="exportCols" @select="setSelectExportCols" @select-all="setSelectExportCols" ref="exportTable"
                      class="list-table table" row-key="Id" height="400px">


                <el-table-column type="selection" class-name="selection" align="center"
                                 width="55">
                </el-table-column>

                <el-table-column label="导出列" mini-width="120" class-name="edit-cell">

                    <template slot-scope="scope">
                        <div class="cell-text">
                            {{scope.row.Title}}
                        </div>
                    </template>
                </el-table-column>

                <el-table-column label="是否唯一" mini-width="150" class-name="edit-cell" align="center">
                    <template slot-scope="scope">

                        <div @click="setCellEdit(scope.$index,1,'export')" class="edit-cell-holder">
                            <div v-if="isCellEdit(scope.$index,1,'export')">
                                <el-switch v-model="scope.row.Unique" :active-value="true" :inactive-value="false" @change="saveExportCols">
                                </el-switch>

                            </div>
                            <div v-else class="cell-text">
                                {{scope.row.Unique?'是':'否'}}
                            </div>
                        </div>


                    </template>
                </el-table-column>



            </el-table>



        </div>



        <ButtonEditor ref="btnEditor"></ButtonEditor>
        <FieldFormatEditor ref="fieldFmtEditor"></FieldFormatEditor>



    </div>


</template>

<script>

    import Sortable from "sortablejs";

    export default {
        inject: ['designer'],
        components: {

        },
        props: ["designData"],
        data() {
            return {
                selectedQueryCdts: [],
                selectedListCols: [],
                selectedExportCols: [],
                exportCols: [],
                editCellId: '',
                sortables: {},

            }
        },
        watch: {
            "designer.curMenu": function (value) {
                if (value == 'export')
                    this.initExportCols();
                else if (value == 'listquery' || value == 'listtable') {
                    let selector = value == 'listquery' ? 'query-table' : 'list-table'
                    let data = selector == 'query-table' ? this.designData.ListConfig.Query.Cdts : this.designData.ListConfig.List.Cols
                    this.$nextTick(function () {
                        this.bindTableRowDrop(selector, data);

                    })

                }


            },
            "designer.curTabId": function (value) {
                if (value == 'third' && this.designer.dataSaveType!=2) {

                    this.initTableListDefaultSet();
                }
            },

        },
        computed: {
            items() {
                return this.designer.allItems;
            },
            dsColumns() {
                return this.designData.dsColumns;


            },

            isListUseDs() {

                return this.designData.ListConfig && this.designData.ListConfig.DataSource.Enabled === true

            },
            hasFieldItems() {
                var items = [];
                if (this.designer.dataSaveType == 2) {

                    items.push({ Id: "id", Title: "Id", Field: "Id" });
                    items.push({ Id: "value", Title: "值", Field: "Value" });
                    return items;
                }

                var allItems = this.items;
              
                if (this.designer.autoCreateTable) {

                    var validItems = allItems.filter(m => !!m.Name);
                    for (var item of validItems) {
                        items.push({ Id: item.Id, Title: item.Title, Field: item.Name });
                    }

                } else {

                    var mainTable = this.designData.DbTables.length > 0 ? this.designData.DbTables[0].Name : "$";
                    items = allItems.filter(m => !!m.Field && m.Table == mainTable);
                }


                items.push({ Id: "id", Title: "Id", Field: "Id" });
                items.push({ Id: "createtime", Title: "创建时间", Field: "CreateTime" });
                return items;

            },
            tableButtons() {

                return this.designData.ListConfig.List.TopBtns;
            },
            tableRowButtons() {

                return this.designData.ListConfig.List.RowBtns;
            },

        },
        methods: {

            initTableListDefaultSet() {

                var items = this.items;

                if (this.designer.saveType == 1 && items.length > 0 && !this.isListUseDs) {
                    var queryCdts = this.designData.ListConfig.Query.Cdts
                    var lstCols = this.designData.ListConfig.List.Cols;
                    if (lstCols.length > 0) return;
                    for (var item of items) {
                        if (item.Field) {
                            queryCdts.push({ "Id": this.$newId(), "Field": item.Field, "Title": item.Title, "Span": 6, "RenderType": 1, "Condition":"" });
                            lstCols.push({
                                "Id": this.$newId(),
                                "Title": item.Title,
                                "Align": "left",
                                "Width": 100,
                                "Sort": "",
                                "Field": item.Field,
                                "Format": "",
                                "Condition": ""

                            })
                        }
                      

                    }         

                }

            },
            initExportCols() {
                var cols = [];
                var selectedCols = [];
                for (var col of this.designData.ListConfig.List.Cols) {

                    var newCol = { Id: col.Id, Title: col.Title || col.Field, Field: col.Field, Unique: false }

                    var exportCol = this.designData.ExportConfig.Cols.find(m => m.Field == col.Field);
                    if (exportCol) {
                        newCol.Unique = exportCol.Unique;
                        selectedCols.push(newCol);
                    }

                    cols.push(newCol)

                }

                this.exportCols = cols;

                this.$nextTick(function () {
                    selectedCols.forEach(row => {
                        this.$refs.exportTable.toggleRowSelection(row);
                    });
                });

            },

            showFieldFmtEditor(row) {
                this.$refs.fieldFmtEditor.show(row);

            },

            setCellText(row) {

                var text = this.displayFieldCellValue(row);
                if (text && text != row.field)
                    row.Title = text;

            },



            displayFieldCellValue(row) {

                if (this.designData.ListConfig.DataSource.Enabled) {


                    var fields = this.designer.dsColumns[this.designData.ListConfig.DataSource.Code];
                    if (!fields) return row.Field;

                    var field = fields.find(m => m.Value == row.Field)
                    if (field) return field.Name || field.Value

                  
                }
                var items = this.hasFieldItems;
                var item = items.find(m => m.Field == row.Field);
                if (item) return item.Title || item.Field;

                return '';


            },


            setCellEdit(rowIndex, colIndex, tbId) {

                var id = `${tbId}_${rowIndex}_${colIndex}`;
                this.editCellId = id;

            },

            isCellEdit(rowIndex, colIndex, tbId) {
                var id = `${tbId}_${rowIndex}_${colIndex}`;
                return this.editCellId == id;

            },

            addQueryCdts() {


                var queryList = this.designData.ListConfig.Query.Cdts

                var item = { "Id": this.$newId(), "Field": "", "Title": "", "Span": 6, "RenderType": 1, "Condition":"" }


                queryList.push(item);

            },

            displayRenderTypeValue(row) {


                var item = this.$consts.form.queryRenderTypes.find(m => m.value == row.RenderType);
                if (item) return item.label;

                return '';


            },


            delQueryCdts() {

                var list = this.designData.ListConfig.Query.Cdts;

                for (var cdt of this.selectedQueryCdts) {
                    var index = list.findIndex(m => m.Id == cdt.Id);
                    if (index != -1)
                        list.splice(index, 1);

                }


            },

            setSelectQueryCdts(selection, row) {

                this.selectedQueryCdts = selection;

            },

          

            displayQuerySpanValue(row) {
                var span = row.Span;
                var opt = this.$consts.form.spanOpts.find(m => m.value == span);
                if (opt) return opt.label;
                return '';

            },


            displayFormatterCellValue(row) {

                var formatter = row.Formatter;
                if (!formatter) {
                    return '';
                }
                if (formatter.Type == 'code') {
                    if (formatter.Config.Code)
                        return '自定义'
                    return '';
                }

                if (formatter.Type == 'img')
                    return '图片'
                if (formatter.Type == 'tag')
                    return '标签'

                if (formatter.Type == 'progress')
                    return '进度条'

                if (formatter.Type == 'switch')
                    return '开关'

                return '';


            },



            delListCols() {

                var list = this.designData.ListConfig.List.Cols;
                for (var obj of this.selectedListCols) {
                    var index = list.findIndex(m => m.Id == obj.Id);
                    if (index != -1)
                        list.splice(index, 1);

                }


            },

            setSelectListCols(selection, row) {

                this.selectedListCols = selection;
                this.saveExportCols();
            },


            setSelectExportCols(selection, row) {

                this.selectedExportCols = selection;
                this.saveExportCols();
            },

            saveExportCols() {

                var cols = this.selectedExportCols.map(function (col) { return { Field: col.Field, Unique: col.Unique || false } });
                this.designData.ExportConfig.Cols = cols;
            },


            displayAlignTypeValue(row) {

                var item = this.$consts.form.alignTypes.find(m => m.value == row.Align);
                if (item) return item.label;
                return '';

            },
            displaySortTypeValue(row) {

                var item = this.$consts.form.sortTypes.find(m => m.value == row.Sort);
                if (item) return item.label;

                return '';


            },
            displaySummaryTypeValue(row) {

                var item = this.$consts.form.summaryTypes.find(m => m.value == row.SummaryType);
                if (item) return item.label;

                return '';


            },
            addListCols() {

                var list = this.designData.ListConfig.List.Cols;
                var item = {
                    "Id": this.$newId(),
                    "Title": "",
                    "Align": "left",
                    "Width": 100,
                    "Sort": "",
                    "Field": "",
                    "Format": "",
                    "Condition": "",
                    "SummaryType":""

                }

                list.push(item);

            },



            editBtn: function (btn, btns, isTop) {

                this.$refs.btnEditor.show(btn, btns, isTop);

            },

            addBtn: function (btns, isTop) {

                this.$refs.btnEditor.show(null, btns, isTop);

            },

            bindTableRowDrop(selector, tableData) {

                var self = this;
                const el = document.querySelector(`.${selector} .el-table__body-wrapper tbody`)
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




        },

    }



</script>



<style scoped>
    * {
        box-sizing: border-box;
    }

    .no-top-border {
        border-top-width: 0 !important;
    }

    .listpage-holder {
        height: 100%;
        overflow: auto;
        box-sizing: border-box;
        display: flex;
        flex-direction: column;
        align-content: flex-start;
    }


    .list-left {
        border: solid 1px #eaeaea;
        overflow: hidden;
        width: 220px;
        border-right-width: 0;
    }

    .list-holder {
        box-sizing: border-box;
    }

        .list-holder .el-main {
            padding: 0;
        }

    .tree-title {
        padding-left: 8px;
        border-bottom: solid 1px #eaeaea;
        font-size: 14px;
        color: #000;
        height: 37px;
        line-height: 37px;
    }


    .list-btn {
        padding: 3px;
        border: solid 1px #eaeaea;
        border-top: 0;
        background-color: #f5f5f9;
    }

    .list-btn2 {
        border: solid 1px #eaeaea;
        text-align: right;
        padding: 5px;
        padding-right: 5px;
    }


    .add-btn {
        margin-left: 8px;
        padding: 8px 10px;
        height: 30px;
    }




    /deep/ .list-btn .el-button, .list-btn2 .el-button {
        font-size: 12px;
    }



    .tree-item-title {
        padding: 5px 10px 0px 10px;
    }

    .tree-item-holder {
        padding: 5px 10px 5px 10px;
    }


    .list-holder {
        min-height: 300px;
        flex: 1;
    }

    .tree-left-holderrrr {
        max-height: calc(100vh - 465px);
    }

    .btn-icon {
        margin-right: 5px;
        font-size: 12px;
    }


    .btn-group {
        display: inline-block;
        text-align: right;
        box-sizing: border-box;
        border: solid 0px red;
    }


    .lbtn {
        display: inline-block;
        font-weight: 400;
        text-align: center;
        white-space: nowrap;
        cursor: pointer;
        border: 1px solid #DCDFE6;
        padding: 5px 10px;
        line-height: 1.5;
        border-radius: 3px;
        color: #373a3c;
        background-color: #fff;
        margin-left: -1px;
        box-sizing: border-box;
    }

        .lbtn.active {
            box-sizing: border-box;
            border: 1px solid #ff5d5b;
            z-index: 1;
            background: url(../../assets/img/selected_red.png) right top no-repeat;
        }

    .btn-group > .lbtn:first-child:not(:last-child) {
        border-top-right-radius: 0;
        border-bottom-right-radius: 0;
    }

    .btn-group > .lbtn:not(:first-child):not(:last-child) {
        border-radius: 0;
    }

    .btn-group > .lbtn:first-child {
        margin-left: 0;
    }

    .btn-group > .lbtn:last-child:not(:first-child) {
        border-top-left-radius: 0;
        border-bottom-left-radius: 0;
    }



    .listpage-holder .el-main {
        display: flex;
        flex-direction: column;
    }

    .list-table {
        flex: 1;
    }

    .select-add-inner {
        text-align: center;
        border-bottom: 1px solid #EBEEF5;
        padding: 5px;
        padding-bottom: 8px;
    }
</style>
