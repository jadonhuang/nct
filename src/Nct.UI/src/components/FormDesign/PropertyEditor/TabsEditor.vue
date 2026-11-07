<template>
    <el-form-item label="标签项">

            <table>
            <tr>
                <th>
                    标题
                </th>
                <th>
                    图标
                </th>
                <th>

                </th>

            </tr>
            <tr :key="index" v-for="(p,index) in model.Tabs">
                <td>
                    <el-input v-model="p.Title" style="width:85px"></el-input>
                </td>
                <td>
                    <IconSelect v-model="p.Icon" style="width:60px"></IconSelect>
                </td>
                <td style="width:25px;text-align:center;">
                    <el-button type="danger" icon="el-icon-minus" style="padding:2px;" size="mini" circle @click="delTab(index)"></el-button>
                </td>
            </tr>

        </table>
        <el-link type="primary" :underline="false" @click="addTab" icon="el-icon-plus">添加标签</el-link>

    </el-form-item>
</template>

<script>export default {
        props: {
            designer: Object,
            model: Object

        },
        computed: {

        },
        methods: {
           delTab(index){
             this.model.Tabs.splice(index,1);

           },
            addTab() {

                var tabNum = this.model.Tabs.length;
                var title = "标签";
                if (tabNum > 0)
                    title = title + (tabNum + 1);

                this.model.Tabs.push({ Title: title,Icon:"",Items:[]});

           }
        }


    }</script>

<style scoped>
    table  {
        border-collapse: collapse;
    }
        table th {
            background-color: #fafafa !important;
            text-align: left;
            padding-left: 10px;
            color: #606266;
        }

</style>

