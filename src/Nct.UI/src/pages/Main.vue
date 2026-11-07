<template>
    <section class="nct-work-area">
        <div v-if="!loading">
            <el-row :gutter="10" v-if="dto.numDatas.length>0"  style="margin-bottom:10px;">
                <el-col :span="dto.span" :key="index" v-for="(item,index) in dto.numDatas">
                    <el-card class="box-card" shadow="never">
                        <div class="statis">
                            <div class="statis-title">{{item.title}}</div>

                            <span class="statis-img">

                                <i :class="[item.icon]"></i>

                            </span>

                            <span class="statis-data">{{item.num}}</span>
                        </div>

                    </el-card>


                </el-col>


            </el-row>
            <el-row :gutter="10">
                <el-col :span="12">
                    <el-card class="chart-card" shadow="never" :body-style="{padding: '10px'}">
                        <chart :options="dto.loginOptsLine"></chart>
                    </el-card>
                </el-col>
                <el-col :span="12">
                    <el-card class="chart-card" shadow="never" :body-style="{padding: '10px'}">
                        <chart :options="dto.loginOptsColumn"></chart>
                    </el-card>
                </el-col>



            </el-row>
        </div>


        <div class="loading-holder" v-else>
            <el-skeleton :rows="8" animated />
        </div>
    </section>

</template>

<script>

    import { Chart } from "highcharts-vue";

    export default {
        name:'mainpage',
        data() {

            return {
                loading: true,
                dto: {}

            }
        },
        methods: {
            async loadData() {


                let numDatas = (await $dataSourceApi.getDataTable({ Code: 'indexnumstatis' })).Data;
                let span = 0;
                if (numDatas.length > 0)
                    span = 24 / numDatas.length;

                let loginData = (await $logApi.getLoginStatisByDays(15)).Data;
                let loginOptsLine = this.getLoginOptsLine(loginData);
                let loginOptsColumn = this.getLoginOptColumn(loginData);

                this.loading = false;
                let dto = { numDatas, span, loginOptsLine, loginOptsColumn };
                this.dto = dto;

            },
            getLoginOptsLine(data) {

                let option = {
                    chart: {
                        type: 'spline'
                    },
                    title: {
                        text: '近半个月访问曲线图',
                        style: { "font-size": "14px" }
                    },

                    xAxis: {
                        categories: [],
                        crosshair: true
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: null
                        }
                    },

                    series: []
                }


                let chartData = [];
                let categories = [];
                for (let key in data) {
                    categories.push(key);
                    chartData.push(data[key]);

                }

                option.xAxis.categories = categories;
                option.series = [{ data: chartData, name: "访问次数" }];

                return option;

            },

            getLoginOptColumn(data) {

                let option = {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: '近半个月访问柱状图',
                        style: { "font-size": "14px" }
                    },

                    xAxis: {
                        categories: [],
                        crosshair: true
                    },
                    yAxis: {
                        min: 0,
                        title: {
                            text: null
                        }
                    },

                    series: []
                }


                let chartData = [];
                let categories = [];
                for (let key in data) {
                    categories.push(key);
                    chartData.push(data[key]);

                }

                option.xAxis.categories = categories;
                option.series = [{ data: chartData, name: "访问次数" }];

                return option;

            }

        },

        async created() {
            await this.loadData();
        },

        components: { Chart }


    }


</script>

<style scoped>


    .statis-title {
        font-size: 14px;
        padding-bottom: 4px;
    }

    .statis-data {
        font-size: 18px;
        font-weight: bold;
    }

    .statis-img {
        font-size: 30px;
        margin-right: 15px;
    }

    /deep/ .box-card .el-card__body {
        padding: 10px 20px;
    }

    .nct-work-area {
    }

    .loading-holder {
        background-color: #fff;
        padding: 10px;
    }
</style>


