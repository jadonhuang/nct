<template>
    <el-form-item label="风格样式">

        <el-select v-model="model.Style" placeholder="请选择">
            <el-option label="默认" value="border-card"> </el-option>
            <el-option label="卡片" value="card"> </el-option>
            <el-option label="简洁" value="simple"> </el-option>
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

