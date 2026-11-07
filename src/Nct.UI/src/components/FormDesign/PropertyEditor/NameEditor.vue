<template>
    <el-form-item label="控件Id">
        <el-input v-model="model.Name" :placeholder="placeholder"></el-input>
    </el-form-item>


</template>

<script>
    export default {
        props: {
            designer: Object,
            model: Object

        },
        computed: {

            placeholder() {

                var df = '请输入';

                if (this.model.Field === undefined) return df;

                if (this.designer.needSelectTable) return df;

                if (this.designer.isTableItem(this.model)) return df;

                if (this.designer.dataSaveType == 2) return df;

                return '控件Id将作为字段名';

            },

        },
        methods: {

        }


    }</script>

