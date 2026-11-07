<template>
    <el-form-item label="按钮类型">

        <el-select v-model="model.BtnType" placeholder="请选择">
            <el-option label="默认按钮" value="default"> </el-option>
            <el-option label="主要按钮" value="primary"> </el-option>
            <el-option label="成功按钮" value="success"> </el-option>
            <el-option label="信息按钮" value="info"> </el-option>
            <el-option label="警告按钮" value="warning"> </el-option>
            <el-option label="危险按钮" value="danger"> </el-option>
            <el-option label="文字按钮" value="text"> </el-option>
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

        },
        methods: {

        }


    }</script>

