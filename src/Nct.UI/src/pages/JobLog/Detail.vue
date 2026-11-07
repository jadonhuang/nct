<template>
    <div class="container" v-loading="loading">
        <div class="title_wrap">
            <div class="left">
                执行用时：{{$formatInterval(model.ElapsedTime)}}
            </div>
            <div class="right">
                <i class="el-icon-success success" v-if="model.IsSuccess"></i>
                <i class="el-icon-warning error" v-if="!model.IsSuccess"></i>

            </div>
        </div>
        <StepResult :list="results" v-if="model.LogType==1"></StepResult>

        <div class="title_wrap" v-else>
            <div class="left">
                {{model.Message}}
            </div>
        </div>

       

    </div>


</template>
<style scoped>


    .container {
        padding: 10px;
        background-color: #eaecf0;
        box-sizing: border-box;
        min-height: 100%;
        overflow: auto;
    }

    .title_wrap {
        display: flex;
        flex-direction: row;
        margin-bottom: 10px;
        background-color: #fff;
        padding: 8px;
        align-items: center;
        border-radius: 8px;
        max-height:365px;
        overflow-y:auto;
    }
        .title_wrap:nth-child(2){
            margin-bottom: 0px !important;
        }

        .title_wrap .left {
            padding-left: 10px;
            flex: 1;
        }

        .title_wrap .right {
            width: 100px;
            text-align: right;
            font-size: 14px;
        }
</style>
<script>
    import StepResult from './StepResult.vue'
    export default {

        props: {

            logId: Number,

        },


        data() {
            return {
                loading: false,
                results: [],
                model: {}


            }
        },
        watch: {

        },
        methods: {

        },

        async mounted() {
            this.loading = true;
            var id = this.logId || this.$route.query.id;
            var api = window.$jobLogApi;
            var data = await api.get(id);
            this.model = data.Data;
            if (data.Data.LogType == 1)
                this.results = JSON.parse(data.Data.Message);
            this.loading = false;
        },

        components: { StepResult }


    }
</script>

