<template>
    <el-dialog title="按钮设置" :visible.sync="isShow" :close-on-click-modal="false" width="600px" :destroy-on-close="true">
      
        <el-form ref="formatform" label-width="80px" size="small">
            <el-form-item label="格式类型">
                <el-radio-group v-model="formatData.Type">
                    <el-radio-button label="code">自定义</el-radio-button>
                    <el-radio-button label="img">图片</el-radio-button>
                    <el-radio-button label="progress">进度条</el-radio-button>
                    <el-radio-button label="tag">标签</el-radio-button>
                    <el-radio-button label="switch">开关</el-radio-button>
                </el-radio-group>
            </el-form-item>

            <el-form-item label="自定义代码" v-if="formatData.Type=='code'">
                <el-input placeholder="请输入格式化代码 如:return row.name" type="textarea" :rows="6"
                          v-model="formatData.Config.Code">
                </el-input>
            </el-form-item>

            <template v-else-if="formatData.Type=='img'">
                <el-form-item label="图片大小">
                    <el-input-number v-model="formatData.Config.Size" :min="0" :precision="0" controls-position="right">
                    </el-input-number> 0表示默认
                </el-form-item>

                <el-form-item label="图片数量">
                    <el-input-number v-model="formatData.Config.Num" :precision="0" controls-position="right">
                    </el-input-number> 0表示全部
                </el-form-item>

            </template>

            <template v-else-if="formatData.Type=='progress'">

                <el-form-item label="显示文字">
                    {{formatData.Config}}
                    <el-switch v-model="formatData.Config.ShowText">
                    </el-switch>
                </el-form-item>

                <el-form-item label="颜色">

                    <el-color-picker v-model="formatData.Config.Color"></el-color-picker>

                </el-form-item>


                <el-form-item label="示例">
                    <div style="width:200px;padding-top:8px">

                        <el-progress :percentage="30" :color="formatData.Config.Color" :show-text="formatData.Config.ShowText" text-inside :stroke-width="15"></el-progress>

                    </div>

                </el-form-item>




            </template>

            <template v-else-if="formatData.Type=='tag'">

                <el-form-item label="类型">

                    <el-radio-group v-model="formatData.Config.Color">
                        <el-radio-button label="">默认</el-radio-button>
                        <el-radio-button label="auto">自动</el-radio-button>
                        <el-radio-button label="success">成功</el-radio-button>
                        <el-radio-button label="info">信息</el-radio-button>
                        <el-radio-button label="warning">警告</el-radio-button>
                        <el-radio-button label="danger">危险</el-radio-button>
                    </el-radio-group>

                </el-form-item>


                <el-form-item label="主题">

                    <el-radio-group v-model="formatData.Config.Effect">
                        <el-radio-button label="light">浅色</el-radio-button>
                        <el-radio-button label="dark">深色</el-radio-button>
                        <el-radio-button label="plain">朴素</el-radio-button>
                    </el-radio-group>

                </el-form-item>


                <el-form-item label="示例">
                    <el-tag :type="formatData.Config.Color"
                            :effect="formatData.Config.Effect">
                        标签文字
                    </el-tag>
                </el-form-item>

            </template>

            <template v-else-if="formatData.Type=='switch'">

                <el-form-item label="文字(开/关)">

                    <el-input v-model="formatData.Config.ActiveText" class="short-input">
                    </el-input>&nbsp;
                    <el-input v-model="formatData.Config.InactiveText" class="short-input">

                    </el-input>

                </el-form-item>

                <el-form-item label="颜色(开/关)">

                    <el-color-picker v-model="formatData.Config.ActiveColor"></el-color-picker>&nbsp;
                    <el-color-picker v-model="formatData.Config.InactiveColor"></el-color-picker>


                </el-form-item>


                <el-form-item label="禁用">

                    <el-radio-group v-model="formatData.Config.Disabled">
                        <el-radio-button :label="true">是</el-radio-button>
                        <el-radio-button :label="false">否</el-radio-button>
                    </el-radio-group>


                </el-form-item>


                <el-form-item label="示例">

                    <el-switch :disabled="formatData.Config.Disabled"
                               :active-color="formatData.Config.ActiveColor"
                               :inactive-color="formatData.Config.InactiveColor"
                               :active-text="formatData.Config.ActiveText"
                               :inactive-text="formatData.Config.InactiveText">
                    </el-switch>


                </el-form-item>

            </template>

        </el-form>


        <div slot="footer">

            <el-button type="primary" @click="save" size="small" icon="el-icon-circle-check">确定</el-button>
            <el-button @click="isShow=false" size="small" icon="el-icon-circle-close">取消</el-button>
            <!--<el-button type="danger" @click="clear" size="small" icon="el-icon-delete">清除</el-button>-->


        </div>


    </el-dialog>
</template>

<script>


    var dfFormatData = {
        code: { Code: "" },
        img: { Size: 0, Num: 0 },
        progress: { ShowText: true, Color: "#409eff" },
        tag: { Color: "", Effect: "light" },
        switch: { ActiveText: "", InactiveText: "", ActiveColor: "#409EFF", InactiveColor: "#C0CCDA", Disabled: true }
    };


    export default {
        props: {


        },
        data() {
            return {
                formatData: {},
                formDataBak: {},
                model: {},
                isShow: false,
                oldType: null

            }
        },
        watch: {
            isShow(val) {
                if (!val) {
                    this.formDataBak = {};
                    this.formatData = {};
                }

            },
            'formatData.Type'(val, oldVal) {

                if (!oldVal || !val) return;
                let bakData = this.formDataBak[val];
                this.formDataBak[oldVal] = this.$copyObj(this.formatData.Config);

                if (bakData) {
                    this.formatData.Config = this.$copyObj(bakData);

                } else {
                    this.formatData.Config = this.$copyObj(dfFormatData[val]);

                }





            }
        },
        computed: {
        },
        methods: {

            show(row) {
                this.formDataBak = {};
                this.formatData = this.$copyObj(row.Formatter || { Type: "code", Config: dfFormatData.code });
                this.model = row;
                this.isShow = true;

            },
            clear() {
                this.isShow = false;
                delete this.model.Formatter;

            },
            save() {
                this.isShow = false;
                Vue.set(this.model, 'Formatter', this.formatData);

            },
        }


    }</script>

<style scoped>

    .short-input {
        width: 100px;
    }
</style>

