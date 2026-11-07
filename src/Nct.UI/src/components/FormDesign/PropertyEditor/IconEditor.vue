
<template>
    <el-form-item label="图标">

      <IconSelect v-model="model.Icon"></IconSelect>

    </el-form-item>
</template>

<script>
    export default {
        props: {
            desiner: Object,
            model: Object

        },
        computed: {


        },
        methods: {

        }


    }</script>
