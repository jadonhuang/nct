<template>
    <div class="wrap">
        <div class="wrap-top" v-for="(item,index) in list"  :key="index">
            <div class="item-wrap">
                <div class="title_wrap" @click="expandItem(item)">
                    <div class="icon">
                        <i class="el-collapse-item__arrow el-icon-arrow-right" :class="{'is-active':item.expanded}"></i>           
                    </div>
                    <div class="left">
                        {{item.StepName}}
                    </div>
                    <div class="right">
                        {{$formatInterval(item.Duration)}}
                        <i class="el-icon-success success" v-if="item.Success"></i>
                        <i class="el-icon-warning error" v-if="!item.Success"></i>

                    </div>

                </div>

                <div class="content-wrap" v-if="item.expanded">

                    <div class="arg-wrap" v-if="item.Exception">
                        <el-alert :title="item.Exception" :closable="false" show-icon
                                  type="error">
                        </el-alert>
                    </div>
                    <div class="arg-wrap" v-if="!objIsEmpty(item.Inputs)">
                        <div class="title">输入</div>
                        <el-input type="textarea" readonly :rows="4" :value="formatObj(item.Inputs)"></el-input>
                    </div>

                    <div class="arg-wrap" v-if="!objIsEmpty(item.Outputs)">
                        <div class="title">输出</div>
                        <el-input type="textarea" readonly :rows="4" :value="formatObj(item.Outputs)"></el-input>

                    </div>

                    <div class="sub_wrap" v-if="item.SubResults&&item.SubResults.length>0">
                        <StepResult :list="item.SubResults"></StepResult>
                    </div>

                </div>

            </div>

          

        </div>
    </div>

</template>
<style scoped>
    .wrap {
        border-radius: 8px;

    }
    .wrap-top:not(:last-child) {
        margin-bottom: 10px;
    }

    .item-wrap {
        border: solid 0px red;
        background-color: #fff;
        border-radius: 8px;
        cursor: pointer;
    }
        .arg-wrap {
            padding: 10px;
        }

        .arg-wrap .title {
            padding-bottom: 2px;
            font-size:12px;
        }

    .title_wrap {
        display: flex;
        flex-direction: row;
        box-sizing: border-box;
        padding: 8px;
        border: solid 0px blue;
    }

        .title_wrap .left {
            flex: 1;
        }

        .title_wrap .right {
            width: 100px;
            text-align: right;
            padding-right: 0px;
        }
    .right i {
      font-size:14px;
    
    }
    .sub_wrap {
        padding-left: 10px;
    }

  
</style>
<script>

    export default {
        name: 'StepResult',
        props: {

            list: Array,

        },

        data() {
            return {


            }
        },
        watch: {

        },
        methods: {
            expandItem(item) {

                let expand = !item.expanded;
                Vue.set(item, 'expanded', expand);

            },
            objIsEmpty(obj) {
                if (!obj || Object.keys(obj).length == 0)
                    return true;
                return false;

            },
            formatObj(obj) {
                if (this.objIsEmpty(obj))
                    return '';

                return JSON.stringify(obj);

            }

        },
        components: {}


    }
</script>

