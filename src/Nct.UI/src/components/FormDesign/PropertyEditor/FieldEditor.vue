<template>


    <el-form-item label="绑定字段" v-if="isShow">

        <el-select v-model="model.Field" placeholder="请选择" filterable clearable @change="autoSetCurItemNameAndTitle">
            <el-option v-for="item in fittedColumns"
                       :key="item.Name"
                       :label="item.Name"
                       :value="item.Name">

                <span>{{getDisplayName(item)}}<i class="el-icon-star-on" v-if="!columnIsUsed(item.Name)" title="可选"></i></span>

            </el-option>
        </el-select>
    </el-form-item>



</template>

<script>
    export default {
        props: {
            designer: Object,
            model: Object,
        },
        computed: {
            isShow() {
                var flag = this.model.Field !== undefined && this.designer.needSelectTable;
                return flag;
            },
            parent() {

                var parent = this.designer.getItemParent(this.model);
                if (this.designer.isItem(parent)) return parent;
                return null;
            },
            fittedColumns() {
                var parent = this.parent;
                var table = this.model.Table;
                if (parent && parent.Table !== undefined) table = parent.Table;
                if (!table) return [];
                var fields = this.designer.tableColumns[table] || [];
                if (fields.length == 0) return [];


                var itemType = this.model.Type;

                var disableTypes = [];

                if (itemType == 'DateTime') {

                    disableTypes = ["money", "int", "float"]

                } else if (itemType == 'Number') {

                    disableTypes = ["datetime"]

                }
                else if (itemType == 'Switch') {

                    disableTypes = ["datetime", "money", "float", "str"]

                }
                if (disableTypes.length == 0)
                    return fields;

                fields = fields.filter(m => disableTypes.indexOf(m.FieldType) == -1)

                return fields;

            }


        },
        methods: {
            getDisplayName(col) {
                if (col.Remark)
                    return `${col.Name}(${col.Remark})`;
                return col.Name;

            },
            columnIsUsed(name) {
  
                var items = this.parent ? this.parent.Items : this.designer.allItems;
                var item = items.find(m => m.Field == name);
                if (item && item.Id != this.model.Id)
                    return true;

                return false;


            },

            autoSetCurItemNameAndTitle(value) {

                if (!value) return;

                if (this.designer.saveType == 1 || !this.model.Name) {
                    this.model.Name = value;
                }

                var field = this.fittedColumns.find(m => m.Name == value);
                if (!field) return;

                if (field.Remark && this.model.Title !== undefined) {

                    this.model.Title = field.Remark;

                }
                if (this.model.MaxLen !== undefined) {

                    this.model.MaxLen = field.Length;
                }

            },


        }


    }</script>

