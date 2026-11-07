<template>
    <el-form-item label="按钮尺寸">

        <el-select v-model="model.Size" placeholder="请选择">
            <el-option label="默认" value="large"> </el-option>
            <el-option label="中等" value="medium"> </el-option>
            <el-option label="小型" value="small"> </el-option>
            <el-option label="超小" value="mini"> </el-option>

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

