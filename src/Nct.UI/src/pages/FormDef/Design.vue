<template>
    <el-container style="height: 100%">
        <el-main style="padding:0" class="main">
            <el-tabs v-model="curTabId" class="main-tab" type="border-card">
                <el-tab-pane label="基本信息" name="first" style="padding:20px;">
                    <el-form ref="infoform" label-width="100px" :model="model" :rules="rules" size="medium">
                        <el-form-item label="表单名称" prop="Name">
                            <el-input v-model="model.Name" placeholder="请输入表单名称"></el-input>
                        </el-form-item>
                        <el-form-item label="表单编码" prop="Code">
                            <el-input v-model="model.Code" :disabled="saveType==2" placeholder="只能输入数字、字母、下划线">

                            </el-input>
                        </el-form-item>


                        <el-form-item label="备注信息">
                            <el-input v-model="model.Descpt"></el-input>

                        </el-form-item>


                        <el-form-item label="数据表设置" prop="Descpt" :rules="rules.Tables" :class="{'is-required':needSelectTable}">
                            <div style="height:45px;">

                                <el-radio-group v-if="saveType==1" v-model="autoCreateTable" size="small" style="margin-right:20px;">
                                    <el-radio-button :label="0">使用已有表</el-radio-button>
                                    <el-radio-button :label="1">自动创建表</el-radio-button>
                                </el-radio-group>


                                <template v-if="needSelectTable">

                                    <el-link style="margin-right:20px;" type="primary" icon="el-icon-circle-check" :underline="false" @click="showSelectTableDialog(-1)">选择数据表</el-link>

                                </template>
                                <el-tooltip v-else content="将尝试使用表单编码作为表名" placement="top-start">

                                    <el-link icon="el-icon-question"></el-link>
                                </el-tooltip>


                            </div>
                            <TableSelect ref="tableSelect" @change="tableSelected"></TableSelect>
                            <el-table height="200" size="small" border :data="designData.DbTables" class="table"
                                      style="width: 100%">
                                <el-table-column label="表名">
                                    <template slot-scope="scope">

                                        <el-tag v-if="scope.row.RefField" type="success" size="small">从表</el-tag>
                                        <el-tag v-else size="small">主表</el-tag> &nbsp;&nbsp;
                                        {{scope.row.Name}}

                                    </template>
                                </el-table-column>

                                <el-table-column label="关联字段">
                                    <template slot-scope="scope">
                                        {{scope.row.RefField}}

                                    </template>
                                </el-table-column>

                                <el-table-column label="操作" width="220" align="center">
                                    <template slot-scope="scope">
                                        <el-link type="primary" @click="showSelectTableDialog(scope.$index)" icon="el-icon-edit" :underline="false">编辑</el-link>

                                        <el-popconfirm icon="el-icon-info"
                                                       icon-color="red"
                                                       v-on:confirm="delSelectedTable(scope.$index)"
                                                       title="确定删除吗？">

                                            <el-link type="primary" slot="reference" icon="el-icon-delete" v-if="scope.row.RefField" :underline="false">删除</el-link>

                                        </el-popconfirm>



                                    </template>
                                </el-table-column>

                            </el-table>


                        </el-form-item>

                        <el-form-item label="数据保存方式" v-if="autoCreateTable==0&&saveType==1">
                            <el-radio-group :disabled="saveType==2" v-model="designData.DataSaveType" size="small">
                                <el-radio :label="1">正常方式</el-radio>
                                <el-radio :label="2">字典方式</el-radio>
                            </el-radio-group>

                            <el-tooltip content="一个控件一行数据，控件Id将作为Id值，控件值作为Value值，数据表必须有Id,Value字段" placement="top-start">
                                <el-link icon="el-icon-question" style="margin-left:10px;"></el-link>
                            </el-tooltip>
                        </el-form-item>

                    </el-form>
                </el-tab-pane>
                <el-tab-pane label="表单设计" name="second">

                    <el-container class="work-area">
                        <el-aside class="ctl-outer-holder">

                            <el-scrollbar>

                                <div class="ctl-inner-holder">
                                    <template v-for="(ctls,key,index) in itemDefs">
                                        <div :key="index+'c'" class="ctl-cate-title"> {{key}}</div>

                                        <draggable :key="index+'t'" :list="ctls" :set-data="setData" v-bind="dragOptions" class="ctl-dragger" @end="addItemByDrag" :clone="cloneDraggingItem">
                                            <div v-for="(ctl) in ctls" :key="ctl.Type" class="ctl-def" @dblclick="addItemByClick(ctl)">
                                                <img :src="getItemIcon(ctl)" />
                                                <span class="ctl-title">
                                                    {{ ctl.Title }}
                                                </span>
                                            </div>

                                        </draggable>

                                    </template>
                                </div>
                            </el-scrollbar>


                        </el-aside>


                        <el-main class="drag-workarea">

                            <div style="background-color:#fff;height:100%;" v-if="loading">
                                <div style="padding:20px;">
                                    <el-skeleton :rows="5" animated :count="2"></el-skeleton>
                                </div>
                            </div>


                            <template v-else>

                                <draggable :class="{'drag-holder':true}" :set-data="setData"
                                           v-bind="dragOptionsDest"
                                           :list="items">

                                    <ItemRender :item="item" :ref="item.Id" :key="index" v-for="(item,index) in items" :designData="designData" />

                                </draggable>

                                <div class="no-items" v-if="!hasCanEditItems()">
                                    <label style="position:absolute;bottom:-75px;left:75px;color:#333">从左侧拖入或双击添加控件</label>
                                    <el-button type="text" v-if="showAutoAddItemBtn" icon="el-icon-success" style="position:absolute;bottom:35px;left:60px;" @click="autoAddItem">根据数据表一键创建控件</el-button>
                                </div>

                            </template>

                        </el-main>

                        <el-aside class="item-prop-holder" width="270px">

                            <el-card class="item-prop-card" shadow="never">

                                <div slot="header">
                                    <div class="prop-tab">

                                        <div :class="{'prop-tab-title':true,'active':itemPropTabId=='itemprop'}" @click="itemPropTabId='itemprop'">控件属性</div>

                                        <div :class="{'prop-tab-title':true,'active':itemPropTabId=='formprop'}" @click="itemPropTabId='formprop'">表单属性</div>

                                    </div>

                                    <div v-if="!curItemIsEmpty&&itemPropTabId=='itemprop'" class="item-prop-head">
                                        <img :src="getItemIcon(curItem)" />
                                        <span style="margin-left:5px;"> {{getItemTypeName(curItem.Type)}}</span>

                                        <div style="flex:1; text-align:right">

                                            <el-dropdown v-if="curItemSwitchItems.length>0" @command="switchItem">

                                                <i class="fa fa-exchange" style="cursor: pointer; color: #409eff;">切换</i>

                                                <el-dropdown-menu slot="dropdown">
                                                    <el-dropdown-item :key="item.Type" v-for="item in curItemSwitchItems" :command="item.value">{{item.name}}</el-dropdown-item>
                                                </el-dropdown-menu>
                                            </el-dropdown>

                                        </div>

                                    </div>

                                </div>
                                <template v-if="itemPropTabId=='itemprop'">


                                    <el-form ref="form" v-if="!curItemIsEmpty" :model="curItem" class="edit-form" size="small" label-width="80px" label-position="left">

                                        <template v-for="(editor,prop) in propEditor">

                                            <component v-if="hasPropEditor(curItem,prop)" :key="editor" :is="editor" :designer="$this" :model="curItem"></component>

                                        </template>

                                    </el-form>

                                </template>
                                <template v-else-if="itemPropTabId=='formprop'">

                                    <FormThemeEditor :model="designData.Theme"></FormThemeEditor>

                                </template>

                            </el-card>

                        </el-aside>

                    </el-container>

                </el-tab-pane>


                <el-tab-pane label="列表视图" name="third">

                    <el-container class="work-area">
                        <el-aside class="form-set-left">

                            <div :class="{'left-menu':true ,'left-menu-select':curMenu=='listbase'}" @click="showMenu('listbase')">
                                <div class="title"> <i class="el-icon-setting"></i> 基础设置</div>
                                <div class="descpt">设置数据源、类型等 </div>

                            </div>

                            <div :class="{'left-menu':true ,'left-menu-select':curMenu=='listquery'}" @click="showMenu('listquery')">
                                <div class="title"> <i class="el-icon-search"></i> 查询条件设置</div>
                                <div class="descpt">设置列表查询条件 </div>

                            </div>

                            <div :class="{'left-menu':true ,'left-menu-select':curMenu=='listtable'}" @click="showMenu('listtable')">
                                <div class="title"> <i class="el-icon-s-grid"></i> 字段、按钮设置</div>
                                <div class="descpt">设置列表字段和按钮</div>

                            </div>


                            <div :class="{'left-menu':true ,'left-menu-select':curMenu=='export'}" @click="showMenu('export')">
                                <div class="title"> <i class="fa fa-file-excel-o"></i> 导入导出设置</div>
                                <div class="descpt">设置导入、导出列和接口等  </div>

                            </div>

                        </el-aside>
                        <el-main>

                            <ListPageConfig :designData="designData"></ListPageConfig>

                        </el-main>
                    </el-container>

                </el-tab-pane>

                <el-tab-pane label="代码脚本" name="fourth">

                    <el-container class="work-area">
                        <el-aside class="form-set-left">

                            <div :class="{'left-menu':true ,'left-menu-select':curRuleMenu=='clientrule'}" style="border-top:0" @click="showRuleMenu('clientrule')">
                                <div class="title"> <i class="el-icon-notebook-2"></i> 静态规则设置</div>
                                <div class="descpt">设置表单前端的js代码逻辑</div>
                            </div>


                            <div :class="{'left-menu':true ,'left-menu-select':curRuleMenu=='serverrule'}" @click="showRuleMenu('serverrule')">

                                <div class="title"> <i class="el-icon-cpu"></i> 动态规则设置</div>
                                <div class="descpt">设置表单后端保存删除等触发的服务端逻辑</div>
                            </div>

                        </el-aside>
                        <el-main>

                            <RuleConfig :designData="designData"></RuleConfig>

                        </el-main>
                    </el-container>

                </el-tab-pane>



            </el-tabs>
        </el-main>
        <el-footer class="footer">

            <el-button-group style="float:left;margin-right:2px;">
                <el-button icon="el-icon-view" @click="previewListPage" size="mini" v-if="saveType==2">预览列表</el-button>
                <el-button icon="el-icon-view" @click="previewFormPage" size="mini">预览单页</el-button>

            </el-button-group>

            <el-button-group style="float:right;">
                <el-button type="warning" icon="el-icon-circle-close" size="mini" @click="confirmCloseCurPage">取消</el-button>
                <el-button type="primary" icon="el-icon-circle-check" size="mini" @click="saveDesignData(false)">保存</el-button>
                <el-button type="success" icon="el-icon-circle-check" size="mini" @click="saveDesignData(true)">保存并关闭</el-button>

            </el-button-group>
        </el-footer>

        <TableEdit ref="tbEditor" @change="tbChange"></TableEdit>

        <DicCreate @change="dicChange" ref="dicCreator"></DicCreate>

        <DataSourceCreate ref="dsCreator" @change="dsChange"></DataSourceCreate>
        <DataSourceEdit ref="dsEditor" @change="dsChange"></DataSourceEdit>



    </el-container>



</template>




<script src="./Design.js"></script>
<style scoped src="./Design.css"></style>
