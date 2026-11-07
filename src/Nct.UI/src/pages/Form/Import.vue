<template>

    <div style="text-align:center">

        <div style="padding:15px" @drop="preventDrop">
            <el-upload drag
                       :limit="1"
                       :headers="headers"
                       ref="upload"
                       :action="uploadUrl"
                       :on-success="handleSuccess"
                       :on-remove="removeFile"
                       :before-upload="beforeUpload"
                       accept=".xlsx">
                <i class="el-icon-upload"></i>
                <div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
                <div class="el-upload__tip" slot="tip">{{tip}} <a :href="templateUrl" target="_blank">下载导入模板</a></div>
            </el-upload>
        </div>

        <div class="el-dialog__footer">
            <el-button type="primary" @click="doImport" size="small" :loading="loading">确 定</el-button>
            <el-button @click="close" size="small">取 消</el-button>

        </div>
    </div>
</template>
<script>
    import { appUrl } from '@/api';
    import emitter from '@/mixins/emitter';
    import request from '@/api/request.js';

    export default {
        mixins: [emitter],
        data() {

            return {
                loading: false,
                fileInfo: null,
            }
        },
        computed: {
            headers() {
                var token = this.$store.state.token;
                return { Authorization: `${token.TokenType} ${token.AccessToken}` }

            },
            tip() {
                return this.$route.query.tip || '只能上传xlsx文件';
            },
            uploadUrl() {

                return appUrl + '/api/annex/uploadfiles?isLocal=true'
            },
            templateUrl() {
                var template = this.$route.query.template;
                if (template) {
                    return appUrl + this.$urlDecode(template);

                }
                return appUrl + '/api/FormDef/ImportTemplate/' + this.$route.query.code + '?token=' + this.$urlEncode(this.$store.state.token.AccessToken);
            }
        },
        methods: {

            close() {
                this.$closeDialog();
            },
            beforeUpload(file) {

                var ext = file.name.substring(file.name.lastIndexOf('.') + 1);
                if (ext.toLowerCase() !== 'xlsx') {
                    this.$errTip(`请选择.xlsx文件`);
                    return false;
                }

            },
            preventDrop(event) {
                event.preventDefault()
                event.stopPropagation()
            },

            removeFile: function () {

                this.fileInfo = null;

            },

            handleSuccess(res, file, fileList) {

                if (res.IsSuccess) {
                    this.fileInfo = res.Data[0]
                } else {
                    fileList.splice(0, 1);
                    this.$errTip(res.Msg);
                }
            },
            doImport() {

                if (!this.fileInfo) {
                    this.$errTip('请上传文件');
                    return;
                }

                let url = this.$route.query.api;
                if (url) {
                    url = this.$urlDecode(url);

                } else {
                    url = '/formdef/ImportData';
                }
                var dto = { FileName: this.fileInfo.Path, FormCode: this.$route.query.code }
                const loading = this.$showLoading('导入中...');
                request.post(url, dto).then(res => {
                    this.loading = false;
                    loading.close();
                    if (res.IsSuccess) {
                        this.dispatch('FormList', 'list.reload', [this]);
                        var msg = res.Msg || `导入成功`;
                        this.$okTip(msg)
                        this.close();
                    } else {

                        this.$errTip(res.Msg);
                    }

                })

            }

        },
        async created() {



        },
        components: {}


    }
</script>



