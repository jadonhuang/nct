<template>
    <div>
        <el-upload :action="uploadUrl"
                   :limit="fileLimit"
                   :headers="headers"
                   :disabled="disabled||files.length>=limit"
                   list-type="picture-card"
                   :file-list="files"
                   :accept="accepts"
                   :multiple="multiple"
                   :on-success="handleSuccess"
                   :on-progress="handleProgress"
                   :on-exceed="handleExceed"
                   :class="getClass"
                   :before-upload="beforeUpload"
                   :auto-upload="true">

            <div class="upload-btn">
                <i class="el-icon-plus">
                </i>
                <div class="upload-tip">
                    {{tip}}
                </div>

            </div>


            <div slot="file" slot-scope="{file}" style="height:100%">

                <template v-if="file&&file.id">

                    <el-image class="el-upload-list__item-thumbnail" fit="contain" v-if="type=='Image'" ref="previewImg"
                              :src="file.url"
                              :preview-src-list="previewList">
                    </el-image>


                    <el-image :src="getFileTypeIcon(file.ext)" class="file-type-icon" v-else>                      
                        <img slot="error"  :src="getDefaultFileTypeIcon()"  class="def-file-type-icon" />
                    </el-image>


                    <div class="container">

                        <span class="info">
                            <span class="name">
                                <el-tooltip effect="dark" :content="file.name" placement="top">
                                    <label>
                                        {{file.name}}
                                    </label>
                                </el-tooltip>
                            </span>
                            <span class="size">({{getSizeName(file.size)}})</span>

                        </span>

                        <div class="actions" style="">
                            <span @click="previewFile(file)" v-if="canPreview(file)" title="预览">
                                <i class="el-icon-zoom-in"></i>
                            </span>
                            <template v-if="!disabled">
                                <span @click="downloadFile(file)" title="下载">
                                    <i class="el-icon-download"></i>
                                </span>
                                <span @click="editFile(file)" title="修改">
                                    <i class="el-icon-edit"></i>
                                </span>
                                <span @click="removeFile(file)" title="删除">
                                    <i class="el-icon-delete"></i>
                                </span>
                            </template>
                        </div>

                    </div>

                </template>

                <el-progress type="circle" :width="90" :percentage="getFilePercent(file)" style="width:90px" v-else></el-progress>

            </div>
        </el-upload>

    </div>
</template>
<script>

    import { appUrl } from '@/api';
    import formMixin from '@/mixins/form.js'
    import emitter from '@/mixins/emitter';


    export default {
        mixins: [formMixin, emitter],
        props: {
            type: String,
            accept: String,
            value: String,
            limit: Number,
            disabled: Boolean,
            data: {}

        },
        data() {
            return {
                publicPath: process.env.BASE_URL,
                previewIndex: 0,
                idClass: 'id-' + this.$newId(),
                fileList: this.data || [],
                filePercent: {},
                editFileId: ''
            }
        },
        watch: {
            value(value) {
                if (value) {
                    var ids = value.split(',')
                    var self = this;
                    var isSame = ids.every(function (value, index, arr) {

                        return self.fileList.findIndex(m => m.Id == value) >= 0;

                    })

                    if (isSame) return;


                    this.getAnnexs(ids).then(files => {
                        this.fileList = files;
                    });
                } else {
                    this.fileList = [];
                }
            }
        },
        methods: {

            beforeUpload(file) {

                if (this.accepts === '*') return true;
                var ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();          
                var allowExts = this.accepts.split(',');
                var allow = allowExts.some(m => m.toLowerCase() == ext);
                if (!allow) {
                    this.$errTip(`不允许的文件类型：${ext}`);
                }
                return allow;


            },
            canPreview(file) {

                if(this.type=='Image') return true;

                let ext = file.ext;
                if (ext == '.doc' || ext == '.docx' || ext == '.pdf' || ext == '.xls' || ext == '.xlsx' || ext == '.ppt' || ext == '.pptx')
                    return true;

                return false;
            },
            editFile(file) {

                var selector = `.${this.idClass} .el-upload__input`
                var el = document.querySelector(selector);
                el.click();
                this.editFileId = file.id;


            },

            getFileTypeIcon(ext) {
                ext = ext.substr(1);
                return `${this.publicPath}img/filetype/${ext}.png`;


            },

            getDefaultFileTypeIcon() {

                return `${this.publicPath}img/filetype/file.png`;

            },

            getSizeName(size) {
                if (size >= 1024 * 1024) {

                    size = (size /(1024 * 1024));
                    return size.toFixed(1) + 'M';

                }

                if (size >= 1024) {

                    size = (size / 1024);
                    return size.toFixed(1) + 'K';
                }

                return size + 'B';


            },
            previewFile(file) {

                if (this.type == 'Image') {
                    var index = this.fileList.findIndex(m => m.Id == file.id);
                    var selector = `.${this.idClass} .el-image__preview`
                    var els = document.querySelectorAll(selector);
                    var el = els[index];
                    el.click();
                } else {
                    var annFile = this.fileList.find(m => m.Id == file.id)
                    //var fileUrl = encodeURIComponent("http://test.uat.dvp315.cn/download/1.docx");
                    var fileUrl = encodeURIComponent(appUrl + '/api/annex/download/' + file.id);
                    var url = `/form/filepreview?url=${fileUrl}&ext=${file.ext}`;
                    var title = `${annFile.Name}`
                    var opts = { url: url, title: title, width:1080, height:700, resizeable: false, headerVisible: true };
                    this.$showDialog(opts);
                 
                }

            },

            removeFile(file) {

                this.fileList = this.fileList.filter(m => m.Id != file.id);
                var ids = this.fileList.map(m => m.Id).join(',');
                this.$emit('input', ids)
                this.dispatch('ElFormItem', 'el.form.change', ids);

            },

            downloadFile(file) {

                window.location = file.url;

            },

            handleProgress(event, file, fileList) {
                if (!file) return;
                Vue.set(this.filePercent, file.uid, file.percentage);

            },

            handleExceed(file) {

                this.$errTip(`文件数超过限制：${this.fileLimit}`);

            },

            handleSuccess(res, file, fileList) {

                if (res.IsSuccess) {

                    if (this.editFileId) {

                        let file = this.fileList.find(m => m.Id == this.editFileId);
                        Object.assign(file, res.Data[0]);

                    } else {

                        for (var f of res.Data) {
                            this.fileList.push(f);
                        }

                    }
                    var ids = this.fileList.map(m => m.Id).join(',');
                    this.$emit('input', ids)


                } else {

                    var index = fileList.findIndex(m => m.uid == file.uid);
                    fileList.splice(index, 1);
                    this.$errTip(res.Msg);
                }
            },
            getFilePercent(file) {
                var percent = this.filePercent[file.uid] || 0;
                percent = parseInt(percent.toFixed(0));
                return percent;
            },


        },
        computed: {
            multiple() {

                return this.fileLimit > 1 && !this.editFileId;

            },
            getClass() {

                var cls = {};
                cls[this.idClass] = true;

                if (this.type == 'Image')
                    cls['img-type'] = true;
                else
                    cls['file-type'] = true;

                if (this.files.length >= this.limit) {

                    cls['file-limit'] = true;
                }

                return cls;
            },

            headers() {
                var token = this.$store.state.token;
                return { Authorization: `${token.TokenType} ${token.AccessToken}` }

            },
            previewList() {
                return this.files.map(m => m.url);
            },
            accepts() {
                return this.accept || "*";
            },
            tip() {
                if (this.type == 'Image') {

                    return '点击添加图片'

                }

                if (this.accept) {
                    return `点击添加文件，允许类型(${accept})`
                }


                return `点击添加文件`


            },
            files() {
                var files = [];
                for (var file of this.fileList) {
                    var url = appUrl + '/api/annex/download/' + file.Id + '?t=' + new Date().getTime();
                    files.push({ name: file.Name, size: file.Size, url: url, ext: file.Extension, id: file.Id })
                }
                return files;
            },

            uploadUrl() {

                return appUrl + '/api/annex/uploadfiles?fileId=' + this.editFileId;
            },
            fileLimit() {

                var limit = this.limit > 0 ? this.limit : 100;
                if (this.editFileId) {
                    limit++;
                }
                return limit;
            },

        },

        mounted() {
            var selector = `.${this.idClass} .el-upload`
            var el = document.querySelector(selector);
            el.addEventListener('click', function (ev) {

                var ex = ev.explicitOriginalTarget;
                if (ex.className == 'el-icon-edit') return;

                this.editFileId = '';

            });

        }


    }


</script>

<style scoped>

    .upload-tip {
        padding: 5px;
    }

    .upload-btn {
        line-height: normal;
        display: flex;
        flex-direction: column;
        vertical-align: central;
        align-items: center;
        height: 100%;
        justify-content: center;
    }

    .file-type-icon {
        width: 40px;
        height: 40px;
        margin-left: 53px;
        margin-top: 30px;
    }

    .def-file-type-icon {
        width: 40px;
        height: 40px;

    }



    .container {
        position: absolute;
        width: 100%;
        height: 100%;
        left: 0;
        top: 0;
        cursor: default;
        color: #fff;
        opacity: 0;
        background-color: rgba(0,0,0,.3);
        transition: opacity .3s;
    }

    .file-type .container {
        opacity: 1;
        background-color: transparent;
    }

    .container:hover {
        opacity: 1;
    }

    .info {
        position: absolute;
        top: 0;
        left: 0;
        display: flex;
        background-color: rgba(0,0,0,.2);
        flex: auto;
        overflow: hidden;
        white-space: nowrap;
        width: 100%;
    }

    .name {
        padding-left: 3px;
        margin-right: 2px;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
        flex: 1;
    }

    .size {
        overflow: hidden;
        display: inline-block;
        box-sizing: border-box;
        padding-right: 3px;
    }

    .actions {
        position: absolute;
        bottom: 0;
        left: 0;
        right: 0;
        background-color: rgba(0,0,0,.2);
        text-align: center;
    }

        .actions span {
            padding: 5px;
            font-size: 14px;
            cursor: pointer;
        }


    /deep/ .file-type .el-upload--picture-card {
        height: 100px;
    }


    /deep/ .file-type .el-upload-list__item {
        height: 100px;
    }

    /deep/ .file-limit .el-upload {
        display: none;
    }
</style>
