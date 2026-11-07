<template>
    <section style="height:100%" v-if="items.length>0">
        <div class="nct-search" v-if="type=='top'">
            <template v-for="(item,index) in simpleItems">

                <el-col :span="item.span" :key="index" v-if="itemIsShow(index)">
                    <label> {{item.title}}：</label>

                    <el-input v-model="params[item.field]" size="medium" clearable v-if="item.type==2" placeholder="请输入"></el-input>

                    <InputNumberRange v-model="params[item.field]" v-else-if="item.type==3"></InputNumberRange>


                    <el-date-picker v-model="params[item.field]" size="medium" type="daterange" v-else-if="item.type==4"
                                    range-separator="至"
                                    start-placeholder="开始日期"
                                    end-placeholder="结束日期" value-format="yyyy-MM-dd">
                    </el-date-picker>



                    <template v-else-if="item.type==5||item.type==6">

                        <el-select v-model="params[item.field]" v-if="item.type==5" class="w100" size="medium" :key="item.field"
                                   :multiple="item.type==6"
                                   collapse-tags clearable filterable
                                   placeholder="请选择">
                            <el-option v-for="item in item.opts"
                                       :key="item.value"
                                       :label="item.name"
                                       :value="item.value">
                            </el-option>
                        </el-select>


                        <el-cascader placeholder="请选择" v-else v-model="params[item.field]" collapse-tags class="w100" size="medium" :key="item.field"
                                     :options="item.opts" :props="getProps(item)"
                                     filterable clearable></el-cascader>
                    </template>

                    <template v-else-if="item.type==7">

                        <el-radio-group v-model="params[item.field]" :key="item.field">
                            <el-radio :label="opt.value" :key="opt.value" v-for="(opt) in item.opts">{{opt.name}}</el-radio>
                        </el-radio-group>


                    </template>

                    <template v-else-if="item.type==8">

                        <el-checkbox-group v-model="params[item.field]" :key="item.field">
                            <el-checkbox :label="opt.value" :key="opt.value" v-for="(opt) in item.opts">{{opt.name}}</el-checkbox>
                        </el-checkbox-group>


                    </template>


                </el-col>


            </template>


            <div class="search-btn-area" v-if="simpleItems.length>0">
                <el-button icon="el-icon-search" type="primary" size="small" @click="doSearch">查询</el-button>
                <el-button icon="el-icon-delete" type="primary" plain @click="reset" size="small" style="margin-right:10px;">清空</el-button>

                <template v-if="collapsIndex>0">
                    <el-link type="primary" v-if="isCollapsed" size="mini" :underline="false" @click="isCollapsed=false" icon="el-icon-arrow-down">展开</el-link>
                    <el-link type="primary" v-else :underline="false" size="mini" @click="isCollapsed=true" icon="el-icon-arrow-up">折叠</el-link>
                </template>

            </div>



        </div>


        <el-card class="tree-card" shadow="never" :body-style="{ padding: '0px' }" v-else-if="type=='tree'">
            <div slot="header">
                <span>{{treeItem.title}}</span>
            </div>

            <div class="tree-cnt">
                <div class="search-holder" v-if="!formList.FORM.urlArgs.hideTreeSearch">
                    <el-input placeholder="搜索"  v-model="treeKey" size="mini" clearable>
                        <i slot="prefix" class="el-input__icon el-icon-search"></i>
                    </el-input>
                </div>
                <div class="tree-parent">
                    <el-tree :data="treeItem.opts" highlight-current node-key="value" :filter-node-method="filterTreeNodes" :props="defaultProps" @node-click="handleNodeClick" class="tree" ref="tree"></el-tree>
                
                </div>
            </div>
        </el-card>

        <el-tabs type="border-card" class="query-tab" :key="item.field" v-for="(item) in tabItems" @tab-click="tabClick" v-else-if="type=='tab'">
            <el-tab-pane :label="opt.name" :name="opt.value" :field="item.field" :key="opt.value" v-for="opt in item.opts"></el-tab-pane>
        </el-tabs>

    </section>
</template>



<script>


    export default {
        inject: ['formList'],
        props: {

            items: Array,
            params: Object,
            type: String,



        },
        data() {
            return {
                isCollapsed: true,
                defaultProps: {
                    children: 'children',
                    label: 'name'
                },
                treeKey:''


            }
        },
        computed: {
            simpleItems() {

                return this.items.filter(m => m.type < 9);
            },
            tabItems() {

                return this.items.filter(m => m.type == 9);
            },
            treeItem() {

                return this.items.find(m => m.type == 10);
            },
            collapsIndex() {
                var startIndex = -1;
                var span = 0;
                var index = 0;
                for (var item of this.simpleItems) {

                    span += item.span;
                    if (span >= 24 && item.span >= 6) {
                        startIndex = index;
                        break;

                    }
                    if (span > 24) {
                        startIndex = index;
                        break;

                    }
                    index++;
                }

                if (startIndex == 0)
                    startIndex == -1;

                return startIndex;

            }

        },
        watch: {
            treeKey: function (value) {

                this.$refs.tree.filter(value);
            },

        },
        async created() {
       
           
        },

        methods: {
            filterTreeNodes(value, data) {
                if (!value) return true;
                return data.name.indexOf(value) !== -1;
            },
            itemIsShow(index) {

                var cpsIndex = this.collapsIndex;
                if (cpsIndex == -1)
                    return true;

                return !this.isCollapsed || index < cpsIndex;

            },

            handleNodeClick(data) {
                this.params[this.treeItem.field] = data.value;
                this.doSearch();
            },
           
            tabClick(tab) {

                var field = tab.$attrs.field;
                var value = tab.$options.propsData.name;
                this.params[field] = value;
                this.doSearch()

            },

            doSearch() {
                this.formList.page.pageIndex = 1;
                this.formList.getList();

            },

            setQueryTreeEmpty() {
                if (this.$refs.tree) {
                    this.$refs.tree.setCurrentKey('');
                }
            },

            reset() {
                this.$emptyObject(this.params);
                this.formList.setQueryTreeEmpty()
                this.doSearch()
            },



            getProps(item) {

                return {
                    multiple: item.type == 6,
                    'checkStrictly': item.strict,
                    'emitPath': false, 'label': 'name',
                    'value': 'value', 'children': 'children', 'expandTrigger': 'click'
                }

            },
        }




    }


</script>

<style scoped>
    .nct-search {
      display:flex;
      flex-wrap:wrap;
      flex:1 1 0%;
    
    }
        .nct-search > .el-col {
            padding: 15px 10px 0px 0;
        }

    .tree-cnt {
       display:flex;
       flex-direction:column;
       height:100%;
    
    }
    .search-holder {
       padding:8px 8px 5px 8px;

    
    }
    .tree-parent {
        flex: 1;
        overflow-y: auto;
        overflow-x: hidden;
        border: solid 0px blue;

    }
    .tree {
        margin-top: 5px;
    }
    .tree-card {
        width: 100%;
        height: 100%;
        box-sizing: border-box;
    }

    .search-btn-area {
    
       padding-top:15px;
    }
    .query-tab {
        border-bottom-width: 0;
        border-left-color: #ebeef5;
        border-right-color: #ebeef5;
        box-shadow: none;
    }

    /deep/ .query-tab .el-tabs__header {
        border-bottom-width: 0;
    }

    /deep/ .query-tab .el-tabs__content {
        display: none;
    }


    /deep/ .tree .el-tree-node__label {
        font-size: 12px;
    }

    /deep/ .el-link--inner {
        font-size: 12px;
    }

    /deep/ .el-range-separator {
        font-size: 12px;
    }

    .search-btn-area {
        /*width: 260px;*/
    }


</style>
