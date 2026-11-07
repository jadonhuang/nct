<template>

    <el-form-item label="验证规则">
        <el-autocomplete :value="model.Verify" placeholder="正则表达式"
                         :fetch-suggestions="querySearch"
                         @select="handleSelect" clearable></el-autocomplete>
    </el-form-item>


</template>

<script>

    export default {
        props: {
            desiner: Object,
            model: Object


        },
        data() {
            return {
                rules: [
                    { "regex": "/^1\\d{10}$/", "value": "必须为手机格式" },
                    { "regex": "/^((0\\d{2,3}-\\d{7,8})|(1[3584]\\d{9}))$/", "value": "必须是固定电话号码或者手机号码" },
                    { "regex": "/^(([1-9][0-9]*)|(([0]\\.\\d{1,2}|[1-9][0-9]*\\.\\d{1,2})))$/", "value": "必须是金额" },
                    { "regex": "/^\\w+$/", "value": "必须是数字字母或者下划线" },
                    { "regex": "/^[-+]?\\d+$/", "value": "必须是数字" },
                    { "regex": "/^\\d{15}(\\d{2}[A-Za-z0-9;])?$/", "value": "必须是身份证格式a" },
                    { "regex": "/^\\w{3,}@\\w+(\\.\\w+)+$/", "value": "必须是E-mail格式" },
                    { "regex": "/^[\\u0391-\\uFFE5]+$/", "value": "必须是中文" },
                    { "regex": "/^http:\\/\\/[a-zA-Z0-9]+\\.[a-zA-Z0-9]+[\\/=\\?%\\-&_~`@[\\]\\':+!]*([^<>\"\"])*$/", "value": "必须是网址格式" },



                ]


            }

        },
        methods: {
            handleSelect(item) {
                this.model.Verify = item.regex;
                if (!this.model.VerifyInfo)
                    this.model.VerifyInfo = item.value;
            },
            querySearch(queryString, cb) {
                var rules = this.rules;
                var results = queryString ? restaurants.filter(this.createFilter(queryString)) : rules;
                cb(results);
            },
            createFilter(queryString) {
                return (r) => {
                    return (r.value.indexOf(queryString) === 0);
                };
            },
        }


    }



</script>

