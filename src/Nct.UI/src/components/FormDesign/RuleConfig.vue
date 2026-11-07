<template>
    <el-container class="h100" v-if="designer.curTabId=='fourth'">
        <template v-if="designer.curRuleMenu=='clientrule'">

            <el-aside width="200px">

                <el-card class="tree-card" shadow="never" :body-style="{ padding: '10px' }">
                    <div slot="header">
                        <span>代码片段</span>
                    </div>

                    <el-tree :data="jsNodes" :props="defaultProps" @node-click="handleJsNodeClick"></el-tree>

                </el-card>

            </el-aside>
            <el-main class="main">
                <MonacoEditor key="editorId" v-if="designer.model.ClientRule!==undefined" ref="editor" class="nct-monaco-editor " v-model="designer.model.ClientRule" :options="options" language="javascript" theme="vs" />
            </el-main>
        </template>

        <template v-else>

            <el-aside width="200px">

                <el-card class="tree-card" shadow="never" :body-style="{ padding: '10px' }">
                    <div slot="header">
                        <span>代码片段</span>
                    </div>
                    <el-tree :data="csNodes" :props="defaultProps" @node-click="handleCsNodeClick"></el-tree>
                </el-card>
            </el-aside>
            <el-main class="main">

                <MonacoEditor key="editorId2" ref="editor2" v-if="designer.model.ServerRule!==undefined" class="nct-monaco-editor " v-model="designer.model.ServerRule" :options="options" language="csharp" theme="vs" />

            </el-main>
        </template>

    </el-container>


</template>

<script>

    import MonacoEditor from 'vue-monaco'
    export default {
        inject: ['designer'],
        props: ["designData"],
        components: {
            MonacoEditor
        },
        watch: {

        },

        data() {
            return {
                editorId: 'cedt',
                editorId2: 'sedt',
                options: { minimap: { enabled: false }, automaticLayout: true },
                rule: {},
                jsNodes: [],
                csNodes: [],
                defaultProps: {
                    children: 'children',
                    label: 'label'
                }
            }
        },
        computed: {

        },
        methods: {


            insertContent(editor, text) {

                if (editor) {
                    const position = editor.getPosition()
                    const insertText = text
                    editor.executeEdits('', [

                        {

                            range: new monaco.Range(position.lineNumber,

                                position.column,

                                position.lineNumber,

                                position.column),

                            text: insertText

                        }

                    ])


                    editor.setPosition({

                        lineNumber: position.lineNumber,
                        column: position.column + insertText.length

                    })

                    editor.focus()

                }

            },


            async handleJsNodeClick(node) {

                if (node.type == 1) return;
                var rule = await this.getRule(node.value);
                var editor = this.$refs.editor.getEditor();
                this.insertContent(editor, rule.Data);


            },
            async handleCsNodeClick(node) {

                if (node.type == 1) return;
                var rule = await this.getRule(node.value);
                var editor = this.$refs.editor2.getEditor();
                this.insertContent(editor, rule.Data);


            },
            async getRule(id) {
                var rule = this.rule[id];
                if (rule)
                    return rule;

                var res = await $ruleDefApi.get(id);
                this.rule[id] = res.Data;
                return res.Data

            },

            async buildData() {
                var res = await $dicValueApi.getNameAndValuesByCode('rulecate');

                var cates = res.Data.Values;
                var args = {
                    pageIndex: 1, pageSize: 100, Query: {}
                }

                var res2 = await $ruleDefApi.getList(args);
                var rules = res2.Data.Rows;

                var jsNodes = [];
                for (let cate of cates) {
                    let cateNode = { label: cate.Name, value: cate.Value, type: 1 };
                    let childNodes = [];
                    let childRules = rules.filter(m => m.Cate == cate.Value && m.Type == 1);
                    for (let child of childRules) {
                        childNodes.push({ label: child.Name, value: child.Id, type: 2 })

                    }
                    cateNode.children = childNodes;
                    jsNodes.push(cateNode);
                }
                this.jsNodes = jsNodes;

                var csNodes = [];
                for (let cate of cates) {
                    let cateNode = { label: cate.Name, value: cate.Value, type: 1 };
                    let childNodes = [];
                    let childRules = rules.filter(m => m.Cate == cate.Value && m.Type == 2);
                    for (let child of childRules) {
                        childNodes.push({ label: child.Name, value: child.Id, type: 2 })

                    }
                    cateNode.children = childNodes;
                    csNodes.push(cateNode);
                }
                this.csNodes = csNodes;


            }


        },
        async mounted() {
            await this.buildData();

        }



    }</script>

<style scoped>

    .tree-card {
        width: 100%;
        height: 100%;
        box-sizing: border-box;
    }

    .main {
        padding: 0 !important;
        padding-left:20px !important;
        overflow: hidden;
    }

    .h100 {
        height: 100%;
    }
   /deep/ .work-area {
        border: solid 1px red;
    }
</style>

