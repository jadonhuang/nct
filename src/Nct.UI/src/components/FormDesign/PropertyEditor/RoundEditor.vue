<template>
    <el-form-item label="按钮样式">
        <el-select v-model="model.Round" placeholder="请选择">
            <el-option label="默认" :value="0"> </el-option>
            <el-option label="圆角" :value="1"> </el-option>
            <el-option label="圆形" value="2"> </el-option>
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

