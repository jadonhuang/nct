<template>
    <el-form-item label="标签位置">

        <el-select v-model="model.TabPosition" placeholder="请选择">
            <el-option label="顶端" value="top"> </el-option>
            <el-option label="底部" value="bottom"> </el-option>
            <el-option label="右边" value="right"> </el-option>
            <el-option label="左边" value="left"> </el-option>
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

