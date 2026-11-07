<template>
    <el-form-item label="级联上级" v-if="model.DataSourceType>1">

        <el-select v-model="model.CascadeParent" placeholder="请选择" filterable clearable>
            <el-option v-for="item in cascadeItems"
                       :key="item.Id"
                       :label="item.Title"
                       :value="item.Id">
            </el-option>
        </el-select>


    </el-form-item>


</template>

<script>
    export default {
        props: {
            designer: Object,
            model: Object

        },
        computed: {
            cascadeItems() {

                var items = [];
                var model = this.model;
                var allItems = this.designer.allItems;
                var parent = this.designer.getItemParent(model);
                if (this.designer.isTableItem(parent)) {

                    for (let subItem of parent.Items) {
                        if (!this.isCanCascadeItem(subItem, model)) continue;
                        items.push({ Id: subItem.Id, Title: subItem.Title || subItem.Name })

                    }

                }

                for (let item of allItems) {
                    if (!this.isCanCascadeItem(item, model)) continue;
                    items.push({ Id: item.Id, Title: item.Title || item.Name })
                }


                return items;

            }
        },
        methods: {
            isCanCascadeItem(item, item0) {

                if (item.Id == item0.Id)
                    return false;

                if (item.Type == 'Text' || item.Type == 'Radio' || item.Type == 'CheckBox' || item.Type == 'DataSelect' || item.Type == 'Select')
                    return true;

                return false;


            },
        }


    }</script>

