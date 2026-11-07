
<template>
    <el-form  size="medium" :label-position="nctForm.designData.Theme.TitlePosition" :model="model" @validate="validateEvent">
        <template v-for="(group,index) in groups">
            <el-row :key="index" class="group-row" :class="{'full-fill':group.fullFill,'is-readonly':nctForm.isReadonly}">
                <template v-for="item in group.items">
                    <el-col :key="item.Id" :span="item.Span" v-if="state[item.Id].Visible" :class="'item-'+item.Type">
                        <form-item :prop="item.Name" :rules="item.$Rules" :label-width='getLabelWidth(item)' class="item-group-form-item" :parentModel="parentModel">
                            <label slot="label" v-if="itemTitleVisible(item)" :style="getItemTitleStyle(item,nctForm.designData.Theme)">{{item.Title}}</label>
                            <component v-on="item.$Events" :is="getItemRenderName(item)" v-if="isContainerItem(item)" :state="state" v-model="model" :prop="item" />
                            <component v-on="item.$Events" :is="getItemRenderName(item)" v-else :state="state[item.Id]"  v-model="model[item.Name]" :prop="item" />

                        </form-item>

                    </el-col>
                </template>
            </el-row>
        </template>
    </el-form>
</template>

<script>
    import form from '@/mixins/form.js'
    export default {
        name: 'ItemGroup',
        componentName: 'ItemGroup',
        mixins: [form],
        inject: ['nctForm'],
        props: {
            state: Object,
            items: Array,
            model: Object,
            parentModel: Object
          
        },
        computed: {
            groups() {

                let items = this.items || [];
                let itemGroups = [];
                if (items.length == 0) return itemGroups;

                let itemGroup = { items: [], fullFill: false };
                let groupWidth = 0;
                let lastItem = items[items.length - 1];
                for (let item of items) {
                    groupWidth += item.Span;
                    if (groupWidth <= 24) {
                        itemGroup.items.push(item);
                        if (item.FullFill) itemGroup.fullFill = true;

                    } else {
                        itemGroups.push(itemGroup)
                        itemGroup = { items: [item], fullFill: item.FullFill || false };
                        groupWidth = item.Span;
                    }

                    if (item.Id == lastItem.Id) {
                        itemGroups.push(itemGroup);

                    }

                }

                return itemGroups;
            },
          
        },

        methods: {

            validateEvent(prop, passed, err) {
                this.$emit('validate', prop, passed, err);
            },
            getItemHolderClass(item) {

                let classes = [];
                if (this.isTabItem(item) && item.FullFill) {
                    classes.push('full-fill');
                }
                return classes;
            },

            itemTitleVisible(item) {

                return item.TitleVisible!==false;

            },
            getLabelWidth(item) {

                if (!this.itemTitleVisible(item)) return '0px';
                return this.nctForm.designData.Theme.TitleWidth + 'px'

            },
        },

    
     
    };
</script>

<style scoped>

    .item-group-form-item {
        margin-bottom: 22px;
    }
    /deep/ .el-form-item__content {
        margin-left: 0;
    }

    /deep/ .el-form-item__label {
        padding-left: 5px;
    }

     .el-form {
        display: flex;
        flex-direction: column;
        height: 100%;
    }

    .group-row {
        display: flex;
        flex-wrap: wrap;
        flex-direction: row;
        box-sizing: border-box;
    }

    /deep/ .group-row:last-child > .item-Tab:last-child > .el-form-item {
        margin-bottom: 0 !important;
    }



    .is-readonly {
        border-top: 1px solid #ebeef5;
        border-left: 1px solid #ebeef5;
        border-right: 1px solid #ebeef5;
    }

   /deep/ .is-readonly .el-form-item__label {
        padding-left: 6px;
        padding-right: 6px;
        flex-shrink: 0;
        border-left: 1px solid #ebeef5;
        border-right: 1px solid #ebeef5;
        background: #fafafa;
        padding-top: 10px;
        min-height: 40px;
        line-height: 22px;
        height: 100%;
        
    }

  .is-readonly:last-child {
        border-bottom: 1px solid #ebeef5;
    }


    .full-fill> .el-col > .el-form-item {
        width: 100%;
        display: flex;
    }

    /deep/ .full-fill > .el-col > .el-form-item > .el-form-item__content {
        flex: 1 ;
        margin-left: 0 !important;
    }

    .full-fill > .el-col {
        display: flex;
    }

    .full-fill {
        flex: 1;
    }

   

 
    .is-readonly > .el-col > .el-form-item {
        margin-bottom: 0 !important;
        height: 100%;
    }

     /deep/ .is-readonly > .el-col > .el-form-item > .el-form-item__content {
        padding: 10px;
        overflow: auto;
        line-height: 22px;
    }

    /deep/ .is-readonly > .el-col:first-child > .el-form-item > .el-form-item__label {
        border-left: none;
    }

    /deep/ .is-readonly .el-input__inner {
        background-color: transparent !important;
        border: 0;
        color: #606266;
        padding:0;
        height:auto;
        line-height:1em;
    }

    /deep/ .is-readonly .el-input {
      
        height: auto;
        line-height: 1em;
    }

    /deep/ .is-readonly .el-input__suffix {
       display:none;
    }

    /deep/ .is-readonly .el-form-item__content {
        height: auto;
        line-height: 1em;
    }


   


   

</style>
