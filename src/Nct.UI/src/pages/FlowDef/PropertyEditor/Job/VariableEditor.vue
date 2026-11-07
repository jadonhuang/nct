<template>

    <el-popover placement="bottom" popper-class="pop" trigger="manual" :value="showDropdown" :visible-arrow="false">
        <div class="dropdown" ref="dropdown">
            <div v-for="(item,index0) in variables" :key="index0+'p'">
                <div class="dropdown-header">{{item.text}}</div>
                <div v-for="(v,index) in item.items" :key="index" class="dropdown-item" @click="selectVariable(item,v)">
                    <div class="var-name">{{v.Name}}</div> <div class="var-type">{{v.Type}}</div>

                </div>

            </div>

            <div v-if="variables&&variables.length==0&&enabled" style="padding:10px;">无可用变量</div>

        </div>

        <div slot="reference" style="position:relative;">
            <div class="editor" ref="editor" v-html="displayValue" :contenteditable="enabled&&!readonly" @click="handleClick" :style="{'min-height':modelMinHeight+'px'}">
            </div>
            <div class="placeholder" v-if="!value&&!focused">{{modelTip}}</div>

        </div>

    </el-popover>


</template>

<script>

    import flow from '@/mixins/flow.js'

    let currentRange = null;


    function createVariableTag(grp, variable){

        var name = variable.Name;
        if (name.indexOf('.') == -1) {
            name = `${grp.id}.${name}`;
        }

        var variableStr = `{{#${name}#}}`

        return variableStr
    }


    function insertContentAtCursor(grp, variable) {
        if (!currentRange) return;

        const selection = window.getSelection();

        // 创建虚拟范围检查前一个字符
        const preRange = currentRange.cloneRange();
        preRange.collapse(true);

        // 确保不会超出文本边界
        if (preRange.endOffset > 0) {
            preRange.setStart(currentRange.endContainer, currentRange.endOffset - 1);

            // 获取前一个字符
            const preText = preRange.toString();
            // 如果是"/"则删除
            if (preText === '/') {
                // 删除"/"字符
                preRange.setStart(currentRange.endContainer, currentRange.endOffset - 1);
                preRange.setEnd(currentRange.endContainer, currentRange.endOffset);
                preRange.deleteContents();

                // 更新当前范围
                currentRange.setStart(currentRange.endContainer, currentRange.endOffset);
            }
        }

        var variableStr = createVariableTag(grp,variable)

        const vNode = document.createElement("span");

        vNode.className = "tag";
        vNode.setAttribute("contenteditable", "false");
        vNode.setAttribute("data-var", variableStr)
        vNode.innerHTML = `${grp.text}/<span class="tag-title">(x) ${variable.Name}<span>`
        currentRange.insertNode(vNode);

        // 移动光标到末尾
        const newRange = document.createRange();
        newRange.setStart(vNode, 0);
        newRange.collapse(true);

        selection.removeAllRanges();
        selection.addRange(newRange);




    }


    // 检查光标前字符是否为"/"
    function isCursorBeforeSlash() {
        if (!currentRange || currentRange.endOffset==0) return false;

        // 创建虚拟范围检查前一个字符
        const preRange = currentRange.cloneRange();
        preRange.collapse(true);

         preRange.setStart(currentRange.endContainer, currentRange.endOffset - 1);

        // 获取前一个字符
        const preText = preRange.toString();
        return preText === '/';
    }



    export default {
        mixins: [flow],
        name: 'VariableEditor',
        props: {
            allowTypes: null,
            vars: null,
            value: String,
            tip: String,
            minHeight: Number,
            node: Object,
            enabled: {
                type: Boolean,
                default: true
            },
            readonly: {
                type: Boolean,
                default: false
            },

        },
        data() {
            return {
                displayValue: '',
                modelTip: this.tip || "输入'/'快速插入变量",
                modelMinHeight: this.minHeight || 100,
                showDropdown: false,
                focused: false,
            }
        },

        watch: {
            value(val) {
                if (!this.focused || !this.enabled || this.readonly) {
                    this.setDisplayValue();

                }


            },


        },

        computed: {
            variables() {
                const node = this.node;
                let variables=[];
                if (node.properties.Parent) {
                    let parentNode = node.graphModel.getNodeModelById(node.properties.Parent);
                    if (parentNode) {
                        variables = [...this.getParentNodeVariables(parentNode),... this.getParentNodeVariables(node)];
                        variables = variables.filter(m => m.type != 'groupstart');

                    }
                    if (parentNode && parentNode.type == 'iteration' && parentNode.properties.Input) {
                        let input = parentNode.properties.Input;
                        let parentVars = this.getParentNodeVariables(parentNode);
                        let varObj = this.tranVariableTextToObject(parentVars, input);

                        if (varObj && varObj.type.startsWith('Array')) {
                            let itemType = varObj.type.substring(6, varObj.type.length - 1);
                            let nodeVariable = { type: parentNode.type, id: parentNode.id, text: '当前迭代', items: [{ Name: 'item', Type: itemType }, { Name: 'index', Type: "Number" }] }
                            variables.splice(0, 0, nodeVariable)

                        }
                    }
                } else {

                    variables = this.vars || this.getParentNodeVariables(node);

                }
                if (this.allowTypes) {
                    for (var variable of variables) {
                        variable.items = variable.items.filter(m => this.allowTypes.includes(m.Type));
                    }

                }
                variables = variables.filter(m => m.items.length > 0)
                return variables;
            }


        },
        mounted() {
           this.initEditor();
           this.setDisplayValue();
        },



        methods: {

            handleClick() {

                if (!this.enabled) return;

                if (this.enabled && this.readonly) {

                    this.showDropdown = true;
                }

                this.$emit('click', this);
            },

            setDisplayValue() {

                this.displayValue = this.tranVariableTextToHtml(this.variables, this.value || '');
            },
            variableHtmlToText() {
                var str = "";
                function loop(parent) {
                    for (var node of parent.childNodes) {
                        var nodeType = node.nodeName;
                        if (nodeType == "SPAN") {
                            var variable = node.getAttribute('data-var');
                            if (variable) {
                                str += variable;
                            } else {
                                str += node.innerText;
                            }
                          

                        } else if (nodeType == '#text') {

                            str += node.textContent;

                        }
                        else if (nodeType == 'BR') {
                            if (node.parentElement.classList.contains('editor') == false)
                            str += '\n';

                        } else {

                            if (nodeType == "DIV" && node.className == "placeholder") continue;

                            loop(node);
                        }

                    }

                }

                loop(this.$refs.editor);
                console.log('str',str);
                this.$emit('input', str);

            },


            selectVariable(grp, variable) {
                this.showDropdown = false;
                if (this.readonly) {
                   let value = createVariableTag(grp, variable);
                    this.displayValue = this.tranVariableTextToHtml(this.variables, value);


                } else {
                    insertContentAtCursor(grp, variable);

                }

                this.$nextTick(() => {
                    this.variableHtmlToText();

                });


            },


            initEditor() {
               var editor = this.$refs.editor;
                var self = this;

                editor.addEventListener('focus', () => {
                    this.focused = true;
                    const selection = window.getSelection();

                    if (selection.rangeCount > 0) {
                        currentRange = selection.getRangeAt(0);
                    }
                });

                editor.addEventListener('blur', () => {
                    this.focused = false;
                    this.showDropdown = false;
                });


                document.addEventListener('click', (e) => {
                    if (!editor.contains(e.target)) {
                        this.showDropdown = false;

                    }
                });

                editor.addEventListener('input', () => {

                    console.log('input')
                    // 保存当前选区
                    const selection = window.getSelection();
                    if (selection.rangeCount > 0) {
                        currentRange = selection.getRangeAt(0);
                    }
                    // 检测光标前是否为"/"
                    if (isCursorBeforeSlash()) {
                        this.showDropdown = true;



                    } else {
                        this.showDropdown = false;
                    }
                    this.variableHtmlToText();

                });


                editor.addEventListener('keydown', function (event) {

                    if (event.key === 'Escape') {
                        self.showDropdown = false;

                    }
                    else if (event.key === 'Backspace') {
                        const selection = window.getSelection();

                        // 检查是否有选中文本
                        if (!selection.toString()) {
                            const range = selection.getRangeAt(0);

                            // 如果光标在文本节点的开始位置
                            if (range.startOffset === 0 && range.startContainer.nodeType === Node.TEXT_NODE) {
                                // 获取前一个兄弟节点
                                const previousSibling = range.startContainer.previousSibling;

                                // 如果前一个兄弟节点存在且是不可编辑的元素节点
                                if (previousSibling &&
                                    previousSibling.nodeType === Node.ELEMENT_NODE &&
                                    previousSibling.contentEditable === 'false') {
                                    // 阻止默认行为
                                    event.preventDefault();

                                    // 删除整个元素
                                    previousSibling.remove();

                                    // 重新设置光标位置
                                    range.setStart(range.startContainer, 0);
                                    range.collapse(true);
                                    selection.removeAllRanges();
                                    selection.addRange(range);
                                }
                            }
                            // 如果光标在元素节点内且在开始位置
                            else if (range.startContainer.nodeType === Node.ELEMENT_NODE &&
                                range.startOffset > 0) {
                                // 获取前一个兄弟节点
                                const previousSibling = range.startContainer.childNodes[range.startOffset - 1];

                                // 如果前一个兄弟节点存在且是不可编辑的元素节点
                                if (previousSibling &&
                                    previousSibling.nodeType === Node.ELEMENT_NODE &&
                                    previousSibling.contentEditable === 'false') {
                                    // 阻止默认行为
                                    event.preventDefault();

                                    // 删除整个元素
                                    previousSibling.remove();
                                }
                            }



                        }

                        self.variableHtmlToText();
                    }
                });

                // 防止用户将光标移动到不可编辑元素内部
                editor.addEventListener('keyup', function (event) {
                    const selection = window.getSelection();
                    if (selection.rangeCount > 0) {
                        const range = selection.getRangeAt(0);
                        const startContainer = range.startContainer;

                        // 如果光标在不可编辑的元素内
                        if (startContainer.nodeType === Node.ELEMENT_NODE &&
                            startContainer.contentEditable === 'false') {
                            // 将光标移到元素后面
                            range.setStartAfter(startContainer);
                            range.collapse(true);
                            selection.removeAllRanges();
                            selection.addRange(range);
                        }
                        // 如果光标在不可编辑元素内的文本节点中
                        else if (startContainer.nodeType === Node.TEXT_NODE &&
                            startContainer.parentElement.contentEditable === 'false') {
                            // 将光标移到元素后面
                            range.setStartAfter(startContainer.parentElement);
                            range.collapse(true);
                            selection.removeAllRanges();
                            selection.addRange(range);
                        }
                    }
                });






            }


        }


    }
</script>

<style scoped>


    .placeholder {
        color: #DCDFE6;
        padding-left: 5px;
        position: absolute;
        left: 0;
        top: 0;
        pointer-events: none;
    }

    .editor {
        border: 1px solid #DCDFE6;
        padding: 5px;
        outline: none;
        line-height: 1.5;
        border-radius: 5px;
        white-space: pre-wrap;
        word-break: break-all;
        box-sizing: border-box;
        width: 100%;
        height: 100%;
    }

        .editor:hover {
            border-color: #C0C4CC;
        }


        .editor:focus {
            border-color: #409EFF;
        }

    /deep/ .editor .tag {
        display: inline-block;
        background-color: #fff;
        border: 1px solid #DCDFE6;
        border-radius: 3px;
        padding: 1px;
        padding-left:3px;
        padding-right:3px;
        user-select: none;
        margin: 0 2px 0px 2px;
    }

    /deep/ .editor .tag-err {
        border-color: #f26868 !important;
    }

    /deep/ .editor .tag-title {
        color: #409EFF;
    }

    .tag.selected {
        background-color: #007cba;
        color: white;
        border-color: #005a9e;
    }


    .dropdown {
        max-height: 300px;
        overflow: auto;
        display: block;
        width: 260px;
        overflow-x: hidden;
        scrollbar-color: #e5e5e5 #f7f7f9;
        scrollbar-width: thin;
    }

    .dropdown-item {
        padding: 6px 16px;
        cursor: pointer;
        display: flex;
        flex-direction: row;
    }

        .dropdown-item .var-name {
            flex: 1;
        }

        .dropdown-item .var-type {
            text-align: right;
            color: #C0C4CC;
        }

        .dropdown-item:hover {
            background-color: #f5f7fa;
        }



    .dropdown-header {
        padding: 6px 16px;
        background: #f5f7fa;
        font-size: 12px;
        color: #808080;
        font-weight: bold;
    }
</style>
