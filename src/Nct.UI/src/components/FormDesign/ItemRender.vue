<template>


    <div :key="item.Id" :type="item.Type" :class="{'item-selected':designer.itemIsSelected(item),'item-area':true,'item-has-error':designer.itemErrInfo[item.Id],'align-top':isFormTileAlignTop(parent)}" @click.stop="designer.setCurItem(item)" :style="{'width':getItemAreaWidth(item,parent)}">


        <div class="item-op-holder">

            <el-button icon="el-icon-copy-document" circle size="mini" title="复制" @click="designer.copyItem(item)"></el-button>


            <el-popconfirm icon="el-icon-info"
                           icon-color="red"
                           v-on:confirm="designer.delItem(item)"
                           title="确定删除吗？">

                <el-button icon="el-icon-delete" circle size="mini" slot="reference"></el-button>

            </el-popconfirm>

        </div>

        <div class="item-title-holder" v-if="item.TitleVisible!==false">
            <div class="item-title" :style="getDesignItemTitleStyle(item,parent)">
                <label :style="{visibility:item.Required?'visible':'hidden'}" class="item-required">*</label>
                <label>{{item.Title}}</label>
            </div>
            <div class="item-show-state">
                <img src="../../assets/img/eye-close.svg" v-show="!item.Visible" />
            </div>
        </div>


        <!--单选-->
        <div class="item-detail" v-if="item.Type=='Radio'">
            <el-radio-group :value="item.DefaultValue">
                <el-radio :label="opt.value" :key="opt.value" v-for="opt in designer.getItemBindData(item)">{{opt.name}}</el-radio>
            </el-radio-group>

        </div>

        <!--label-->
        <div class="item-detail label" v-else-if="item.Type=='Label'" :style="designer.getItemTextStyle(item)">

            <label v-html="formatTitle(item.Text)"></label>

            <img src="../../assets/img/eye-close.svg" v-show="!item.Visible" />

        </div>

        <!--多选-->
        <div class="item-detail" v-else-if="item.Type=='CheckBox'">

            <el-checkbox-group :value="item.DefaultValue">
                <el-checkbox :label="opt.value" :key="opt.value" v-for="opt in designer.getItemBindData(item)">{{opt.name}}</el-checkbox>
            </el-checkbox-group>

        </div>


        <!--分隔线-->
        <div class="item-detail" v-else-if="item.Type=='Divider'">

            <el-divider :content-position="item.TextAlign"><span :style="designer.getItemTextStyle(item)">{{item.Text}}</span></el-divider>

        </div>

        <!--开关-->
        <div class="item-detail" v-else-if="item.Type=='Switch'">

            <el-switch disabled :active-value="1"
                       :inactive-value="0" :value="item.DefaultValue" :active-text="item.ActiveText" :inactive-text="item.InActiveText"> </el-switch>

        </div>


        <!--文本框-->
        <div class="item-detail" v-else-if="item.Type=='Text'">

            <el-input size="small" :readonly="true" :placeholder="item.Tip" :value="item.DefaultValue">

            </el-input>

        </div>


        <!--数字-->
        <div class="item-detail" v-else-if="item.Type=='Number'">

            <el-input size="small" :readonly="true" :placeholder="item.Tip" :value="item.DefaultValue" :class="{'has-suffix':item.Suffix}">
                <template slot="append" v-if="item.Suffix">
                    {{item.Suffix}}
                </template>
            </el-input>

        </div>

        <!--文本区-->
        <div class="item-detail" v-else-if="item.Type=='TextArea'">

            <div class="item-textarea el-textarea el-input--suffix">

                <textarea readonly="readonly" autocomplete="off" :rows="item.Rows" :placeholder="item.Tip" :value="item.DefaultValue" class="el-textarea__inner"></textarea>
                <div style="color: #909399;position:absolute;bottom:2px;right:20px;" v-if="item.maxlength>0" slot="suffix">0/{{item.maxlength}}</div>
            </div>

        </div>

        <!--图片上传-->
        <div class="item-detail" v-else-if="item.Type=='Image'">

            <el-upload list-type="picture-card" :disabled="true" action="/">
                <i class="el-icon-plus">
                </i>
            </el-upload>

        </div>

        <!--文件上传-->
        <div class="item-detail" v-else-if="item.Type=='Upload'">
            <div class="add-button">
                <div class="add-button-inner">
                    <i class="el-icon-circle-plus"></i>
                    <span>添加文件</span>
                </div>
            </div>

        </div>

        <!--下拉-->
        <div class="item-detail" v-else-if="item.Type=='Select'">


            <div class="el-select el-select--small" style="width: 100%;">
                <div class="el-input el-input--small el-input--suffix">
                    <input type="text" readonly="readonly" autocomplete="off" :placeholder="item.Tip" class="el-input__inner">
                    <span class="el-input__suffix">
                        <span class="el-input__suffix-inner">
                            <i class="el-select__caret el-input__icon el-icon-arrow-up"></i>
                        </span>
                    </span>
                </div>
            </div>


        </div>


        <!--日期-->
        <div class="item-detail" v-else-if="item.Type=='DatePicker'">

            <div class="el-date-editor el-input el-input--small el-input--prefix el-input--suffix el-date-editor--date" style="width: 100%;">
                <input type="text" autocomplete="off" :placeholder="item.Tip" readonly="readonly" class="el-input__inner">
                <span class="el-input__prefix">
                    <i class="el-input__icon el-icon-date"></i>
                </span>
            </div>


        </div>


        <!--数据选择器-->
        <div class="item-detail" v-else-if="item.Type=='DataSelect'">

            <el-input :readonly="true" size="small" :placeholder="item.Tip">
                <el-button slot="append" icon="el-icon-search"></el-button>
            </el-input>
        </div>


        <!--定位控件-->
        <div class="item-detail" v-else-if="item.Type=='Location'">

            <el-input :readonly="true" size="small" :placeholder="item.Tip">
                <el-button slot="append" icon="el-icon-location"></el-button>
            </el-input>
        </div>

        <div class="item-detail" v-else-if="item.Type=='RichText'">

            <UEditor :key="item.Id+item.Toolbar+item.Height" :config="getEditorConfig(item)"></UEditor>
            <div class="editor-mask"></div>
        </div>

        <!--表格-->
        <div class="item-detail" v-else-if="item.Type=='Table'">

            <draggable :class="{'drag-holder-table':true,'table-layout-drag':item.Layout==1,'empty':item.Items.length==0}" :set-data="designer.setData"
                       v-bind="designer.dragOptionsDest"
                       v-model="item.Items">

                <ItemRender :item="subItem" :key="index" :parent="item" :designData="designData" :designer="designer" v-for="(subItem,index) in item.Items" />

            </draggable>

        </div>

        <!--选项卡-->
        <div class="item-detail" v-else-if="item.Type=='Tab'">

            <el-tabs class="tab-holder" v-model="activeTabIndex" :type="getTabType(item)" :tab-position="item.TabPosition">
                <el-tab-pane :label="tab.Title" :key="index" :name="String(index)" v-for="(tab,index) in item.Tabs">
                    <span slot="label"><i :class="tab.Icon"></i> {{tab.Title}}</span>

                    <draggable class="drag-holder-table no-border" :class="{empty:tab.Items.length==0}" :set-data="designer.setData"
                               v-bind="designer.dragOptionsDest"
                               v-model="tab.Items">

                        <ItemRender :item="subItem" :key="index2" :designData="designData" :designer="designer" v-for="(subItem,index2) in tab.Items" />

                    </draggable>


                </el-tab-pane>

            </el-tabs>

        </div>

        <!--按钮-->
        <div class="item-detail" v-else-if="item.Type=='Button'" :style="designer.getItemTextStyle(item)" style="display:block">
            <el-button :type="item.BtnType" :round="item.Round==1" :plain="item.Plain" :disabled="item.Readonly" :size="item.Size" :circle="item.Round==2" :icon="item.Icon">
                <font :style="designer.getItemTextStyle(item)" v-if="item.Text">{{item.Text}}</font>
            </el-button>
        </div>



        <label class="item-error-info">
            <el-popover placement="top-start"
                        trigger="hover">
                <i class="el-icon-warning" slot="reference"></i>

                <span v-html="designer.itemErrInfo[item.Id]"></span>
            </el-popover>

        </label>


    </div>



</template>

<script>

    import draggable from 'vuedraggable'

    export default {
        name: 'ItemRender',
        inject: ['designer'],
        components: {
            draggable
        },

        props: ['item', "parent", "designData"],
        data() {

            return {
                activeTabIndex: "0"

            }

        },
        methods: {

            getDesignItemTitleStyle(item, parent) {
                parent = parent || {};
                let formTheme = this.designData.Theme;
                let css = this.designer.getItemTitleStyle(item, formTheme);
                if (item.TitleVisible !== false && parent && parent.Layout != 1) {

                    //设计器展示有用
                    css.width = formTheme.TitleWidth + 'px';

                    if (formTheme.TitlePosition != 'top')
                        css['text-align'] = formTheme.TitlePosition;

                }

                return css;

            },


            setActiveTabIndex(index) {

                this.activeTabIndex = String(index);
            },
            getTabType(item) {

                if (item.Style != 'simple') return item.Style;
                return null;

            },
            getEditorConfig(item) {

                var config = {
                    readonly: false, toolbar: item.Toolbar, height: item.Height
                }
                return config;
            },

            formatTitle(title) {

                if (!title) return title;
                return title.replaceAll('\\n', '<br/>').replaceAll('\n', '<br/>');

            },


            isFormTileAlignTop(parent) {

                if (parent && parent.Layout == 1) return false;
                return this.designData.Theme.TitlePosition == 'top';

            },


            getItemWidth(item) {
                var p = item.Span * 100 / 24;
                return p;

            },


            getItemAreaWidth(item, parent) {

                if (parent && parent.Layout == 1) {
                    var total = 0;
                    for (var subItem of parent.Items) {
                        total += subItem.Span;
                    }

                    var itemWidth = (item.Span * 100 / total) + '%';
                    return itemWidth;
                }

                if (item.Span == 100)
                    return 'auto';

                var width = this.getItemWidth(item);
                return width + "%";


            },


        },

    }



</script>



<style scoped>


    .no-border {
        border-width: 0 !important;
    }

    .tab-holder {
        width: 100%;
    }

    .table-layout-drag .item-area {
        box-sizing: border-box;
        border: 1px solid #eaeaea;
        border-left: 0;
        border-top: 0;
    }

    .table-layout-drag .item-area {
        flex-wrap: wrap;
    }

        .table-layout-drag .item-area:last-child {
            border-right: 0;
        }

    .table-layout-drag .item-title-holder {
        height: 40px;
        box-sizing: border-box;
        width: 100%;
        border-bottom: 1px solid #eaeaea;
        background: rgb(250, 250, 250);
    }

    .table-layout-drag .item-detail {
        width: 100%;
        min-height: 42px;
        padding-bottom: 5px;
        padding-top: 5px;
    }


    .table-layout-drag .item-img {
        display: none;
    }



    /deep/ .has-suffix .el-input-group__append {
        background-color: #fff;
        padding: 0 8px;
        font-size: 12px;
    }

    /deep/ .has-suffix > .el-input__inner {
        border-right: 0;
    }


    .item-textarea {
        padding-bottom: 2px;
    }


    .item-show-state {
        width: 16px;
        height: 16px;
    }


    .drag-holder-table {
        min-height: 150px;
        width: 100%;
        border: solid 1px #eaeaea;
        background-color: #fff;
        display: flex;
        flex-wrap: wrap;
        flex-direction: row;
        align-items: flex-start;
        align-content: flex-start;
    }



    .item-op-holder {
        width: auto;
        height: auto;
        position: absolute;
        right: 8px;
        top: -15px;
        z-index: 1001;
        background-color: transparent;
        display: none;
        min-width: 60px;
    }

    .item-area:hover > .item-op-holder {
        display: block;
    }



    .item-selected > .item-op-holder {
        display: block;
    }


    .item-selected {
        background-color: rgba(33, 150, 243, 0.15);
        box-sizing: border-box;
        /*    border-left: 2px solid #409EFF;*/
    }



    .item-area {
        display: flex;
        border: solid 0px #C0C4CC;
        box-sizing: border-box;
        min-height: 40px;
        position: relative;
    }

        .item-area:hover {
            background-color: rgb(248, 248, 248);
        }


    .item-detail {
        padding: 5px 1px 5px 5px;
        flex: 1;
        align-items: center;
        display: flex;
        flex-wrap: wrap;
        border: solid 0px red;
        box-sizing: border-box;
        color: #606266;
        position: relative;
    }


    /deep/ .item-detail .el-upload--picture-card {
        width: 100px;
        height: 100px;
        line-height: 110px;
    }

    .item-title-holder {
        display: flex;
        align-items: center;
    }


    .item-img {
        margin-left: 5px;
        width: 14px;
        height: 14px;
        display: none;
    }

    .item-title {
        padding: 5px 1px 5px 5px;
        text-align: left;
        color: #606266;
    }


    .item-required {
        padding-top: 3px;
        color: #F56C6C !important;
        text-decoration: none !important;
    }

    .item-detail .el-button {
        font-size: 12px;
    }

    .item-error-info {
        color: red;
        align-items: center;
        display: flex;
        visibility: hidden;
        font-size: 14px;
        width: 16px;
    }

    .item-has-error {
    }


        .item-has-error > .item-error-info {
            visibility: visible;
        }


    /deep/ .item-detail .el-input .el-input__inner {
        cursor: default;
    }

    .item-textarea textarea {
        cursor: default;
    }


    /deep/ .item-detail .el-input .el-input__inner:focus {
        border-color: #C0C4CC;
    }


    /deep/ .item-detail .el-textarea__inner:focus {
        border-color: #C0C4CC;
    }

    .item-area.align-top {
        flex-wrap: wrap;
    }

        .item-area.align-top .item-title-holder {
            width: 100%;
        }


    .item-detail.label {
        display: block;
        padding: 10px;
    }

    div[type="Label"] > .item-area {
        min-height: auto;
    }


    .drag-holder-table.empty {
        position: relative;
    }

        .drag-holder-table.empty::before {
            content: '从左侧拖入添加控件';
            text-align: center;
            width: 100%;
            position: absolute;
            top: 65px;
        }




    /deep/ .item-detail .edui-default .edui-toolbar .edui-state-disabled .edui-icon {
        opacity: 0.8 !important;
        filter: alpha(opacity = 80) !important;
    }

    /deep/ .item-detail .edui-toolbar .edui-state-disabled .edui-combox-body {
        background-color: #F0F0EE;
        opacity: 0.8 !important;
    }

    .editor-mask {
        width: 100%;
        height: 100%;
        position: absolute;
        left: 0;
        bottom: 0;
        z-index: 1000;
    }
</style>
