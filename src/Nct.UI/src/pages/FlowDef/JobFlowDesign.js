

import LogicFlow from "@logicflow/core";
import { Menu } from '@logicflow/extension';
import "@logicflow/core/lib/style/index.css"
import '@logicflow/extension/lib/style/index.css';

import CaseNode from './FlNodes/Job/CaseNode'
import StartNode from './FlNodes/Job/StartNode'
import HttpNode from './FlNodes/Job/HttpNode'
import ScriptNode from './FlNodes/Job/ScriptNode'
import LoopNode from './FlNodes/Job/LoopNode'
import IterationNode from './FlNodes/Job/IterationNode'
import GroupNode from './FlNodes/Job/GroupNode'
import GroupStartNode from './FlNodes/Job/GroupStartNode'
import JsonDataNode from './FlNodes/Job/JsonDataNode'
import AiModelNode from './FlNodes/Job/AiModelNode'

import ToolNode from './ToolNode'

import StartNodePropEditor from './PropertyEditor/Job/StartNodePropEditor.vue'
import HttpNodePropEditor from './PropertyEditor/Job/HttpNodePropEditor.vue'
import CaseNodePropEditor from './PropertyEditor/Job/CaseNodePropEditor.vue'
import ScriptNodePropEditor from './PropertyEditor/Job/ScriptNodePropEditor.vue'
import IterationNodePropEditor from './PropertyEditor/Job/IterationNodePropEditor.vue'
import JsonDataNodePropEditor from './PropertyEditor/Job/JsonDataNodePropEditor.vue'
import AiModelNodePropEditor from './PropertyEditor/Job/AiModelNodePropEditor.vue'

import VariableEditor from './PropertyEditor/Job/VariableEditor.vue'


import LogDetail from '../JobLog/Detail.vue'

const tempProps = ["status", "warnings"]

export default {
    name: 'JobFlowDesign',
    componentName: 'JobFlowDesign',
    provide() {
        return {
            designer: this
        };
    },
    mixins: [],
    components: {
        ToolNode, StartNodePropEditor, HttpNodePropEditor, VariableEditor, CaseNodePropEditor, ScriptNodePropEditor, IterationNodePropEditor, JsonDataNodePropEditor, AiModelNodePropEditor, LogDetail
    },
    data() {

        return {
            lf: null,
            curNode: null,
            saving: false,
            designDataBak: null,
            loading: false,
            id: null,
            runLogVisible: false,
            logId: null,
            running: false


        }


    },


    watch: {
        "curNode.text": function (val) {

            if (this.curNode)
                this.lf.updateText(this.curNode.id, val);

        }


    },

    computed: {

    },

    methods: {

        getNodePorpEditorName(node) {

            var type = node.type;
            switch (type) {
                case 'start':
                    return 'StartNodePropEditor';
                case 'http':
                    return 'HttpNodePropEditor';
                case 'script':
                    return 'ScriptNodePropEditor';
                case 'case':
                    return 'CaseNodePropEditor';
                case 'iteration':
                    return 'IterationNodePropEditor';
                case 'jsonData':
                    return 'JsonDataNodePropEditor';
                case 'aiModel':
                    return 'AiModelNodePropEditor';
                default:
                    return '';
            }



        },


        saveCurNodeProps() {
            let curNode = this.curNode;
            if (curNode) {
                let props = this.lf.getProperties(curNode.id);  
                for (var prop of tempProps) {
                    curNode.props[prop]=props[prop];
                }
                if (props.Children) {
                    curNode.props.Children = props.Children;
                }
                if (curNode.type == 'case') {

                    for (let i = 0; i < curNode.props.Cases.length; i++) {
                        let caseOld = curNode.props.Cases[i];
                        let caseNew = props.Cases[i];
                        if (caseNew)
                            caseOld.EdgeIds = caseNew.EdgeIds;

                    }

                }
                this.lf.setProperties(curNode.id, curNode.props);
            }

        },
        setCurNode(data) {
            const props = data.properties;
            this.curNode = { id: data.id, type: data.type, text: data.text?.value, props: props };

        },
      

        resetNodeStatus() {
            let graphData = this.lf.getGraphData();
            for (var node of graphData.nodes) {

                this.lf.setProperties(node.id, { status: { status: 0, text: null } });
            }


        },


        loadData() {
            const id = this.$route.query.id;
            this.id = id;
            this.getDesignData(id);

        },

        getNewDesignData() {
            let graphData = this.$copyObj(this.lf.getGraphData());
            for (var node of graphData.nodes) {

                for (var prop of tempProps) {
                    delete node.properties[prop];
                }

            }
            return JSON.stringify(graphData);

        },

        saveBakDesignData() {

            this.designDataBak = this.getNewDesignData();

        },


        getDesignData: function (id) {
            this.loading = true;
            $jobDefApi.get(id).then(res => {
                this.loading = false;
                if (res.IsSuccess) {
                    let data = res.Data;
                    let flowConfig = data.FlowConfig;
                    if (!flowConfig) {
                        flowConfig = {
                            "nodes": [
                                {
                                    "type": "start",
                                    "x": 150,
                                    "y": 200,
                                    "text": '开始',

                                }
                            ],
                        }

                    } else {

                        flowConfig = JSON.parse(data.FlowConfig);
                    }

                    this.designDataBak = JSON.stringify(flowConfig);
                    this.renderFlow(flowConfig);

                } else {
                    this.$errTip(res.Msg);
                }

            });

        },

        isDesignDataChanged() {

            var bakData = this.designDataBak
            var currentData = this.getNewDesignData();
            var changed = currentData != bakData;
            return changed;

        },

        async confirmCloseCurPage() {

            var changed = this.isDesignDataChanged()
            if (changed) {
                const confirm = await this.$confirmTip('有未保存的设计数据, 是否关闭?');
                if (confirm)
                    this.$closePage();

            } else {

                this.$closePage();
            }


        },
        renderFlow(graphData) {
            LogicFlow.use(Menu);
            const lf = new LogicFlow({
                container: this.$refs.container,
                grid: {
                    size: 12,
                    visible: true,
                    type: "dot",
                    config: {
                        color: "rgba(0, 0, 0, 0.1)",
                        thickness: 0.05,
                    },
                },
                background: {
                    backgroundColor: 'rgb(245, 245, 249)'
                },

                //background: {
                //    backgroundImage: 'url("data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+PGRlZnM+PHBhdHRlcm4gaWQ9ImdyaWQiIHdpZHRoPSI0MCIgaGVpZ2h0PSI0MCIgcGF0dGVyblVuaXRzPSJ1c2VyU3BhY2VPblVzZSI+PHBhdGggZD0iTSAwIDEwIEwgNDAgMTAgTSAxMCAwIEwgMTAgNDAgTSAwIDIwIEwgNDAgMjAgTSAyMCAwIEwgMjAgNDAgTSAwIDMwIEwgNDAgMzAgTSAzMCAwIEwgMzAgNDAiIGZpbGw9Im5vbmUiIHN0cm9rZT0iI2QwZDBkMCIgb3BhY2l0eT0iMC4yIiBzdHJva2Utd2lkdGg9IjEiLz48cGF0aCBkPSJNIDQwIDAgTCAwIDAgMCA0MCIgZmlsbD0ibm9uZSIgc3Ryb2tlPSIjZDBkMGQwIiBzdHJva2Utd2lkdGg9IjEiLz48L3BhdHRlcm4+PC9kZWZzPjxyZWN0IHdpZHRoPSIxMDAlIiBoZWlnaHQ9IjEwMCUiIGZpbGw9InVybCgjZ3JpZCkiLz48L3N2Zz4=")',
                //    backgroundRepeat: 'repeat'
                //},

                allowResize: true,
                nodeTextEdit: false,
                edgeTextEdit: false,
                nodeTextDraggable: false,
                edgeTextDraggable: false,
                adjustEdgeMiddle: true,
                style: this.$consts.flow.style,

            });


            this.lf = lf;
            lf.extension.menu.setMenuConfig({
                nodeMenu: [

                ],
                edgeMenu: [
                    {
                        text: "删除",
                        callback(edge) {
                            lf.deleteEdge(edge.id);
                        },
                    },
                ],
                graphMenu: [],
            });
            lf.batchRegister([StartNode, HttpNode, CaseNode, ScriptNode, GroupNode, LoopNode, GroupStartNode, IterationNode, JsonDataNode, AiModelNode]);

            lf.on("edge:add,edge:delete", ({ data }) => {
                let srcId = data.sourceNodeId;
                let model = lf.getNodeModelById(srcId);
                if (model.type == 'case') {
                    model.setCaseEdge();
                }
              
            });

            lf.on("node:click", ({ data }) => {
                if (data.type != 'groupstart') {
                    this.setCurNode(data);
                } else {
                    this.curNode = null;                    
                }
            });

            lf.on("node:delete,blank:click", ({ data }) => {
                this.curNode = null;
            });


            lf.render(graphData);

        },



        validateDesignData() {
            let data = this.lf.getGraphData();
            let nodes = data.nodes;

            let valid = true;
            if (nodes.length < 2) {

                this.$errTip('至少要有一个任务节点');
                valid = false
            }
            else {

                for (const node of nodes) {
                    const nodeModel = this.lf.getNodeModelById(node.id);
                    if (!nodeModel.validate()) {
                        valid = false
                    }
                }
                if (!valid) {

                    this.$errTip('节点属性设置有误');
                }
            }

            return valid;

        },


        saveDesignData: async function (close, showMsg) {     
            this.saveCurNodeProps();
            let valid = this.validateDesignData();
            if (!valid) return;

            this.saving = true;
            var flowConfig = this.getNewDesignData();
            showMsg = showMsg !== false;

            let dto = { Id: this.id, FlowConfig: flowConfig }
            if (showMsg) {
                 this.$showLoading('保存中...');
            }
            var res = await $jobDefApi.saveFlowConfig(dto);

            if (showMsg) {
                this.$hideLoading();
            }
            this.saving = false;
            if (res.IsSuccess) {
                if (showMsg) {
                    this.$okTip("保存成功");
                }
                if (close) {
                    this.$closePage();
                } else {

                    this.saveBakDesignData();
                }
                return true;
            } else {

                this.$errTip(res.Msg);
                return false;
            }

        },

        closeRunLogDetail() {
            this.runLogVisible = false;
            this.resetNodeStatus();
        },

        async testRun() {
            this.running = true;
            this.runLogVisible = false;
            var success = await this.saveDesignData(false, false);
            if (!success) return;

            this.lf.updateEditConfig({
                isSilentMode: true
            });
            this.saving = true;
            let res = await $jobDefApi.run(this.id)

            if (res.IsSuccess) {
                this.$okTip("运行成功");

            } else {

                this.$errTip('运行失败请看运行详情');
            }
            this.lf.updateEditConfig({
                isSilentMode: false
            });
            this.logId = res.Data;
            this.runLogVisible = true;
            this.saving = false;
            this.running = false;

        },
        bindEvent() { 

            this.$eventHub.$on('node-prop-change', this.saveCurNodeProps);
            const self = this;
            this.$event.on("notify", function (event) {
                if (event.Type != 'FlowNodeStatusChanged') return;
                let data = event.Data;
                let nodeId = data.NodeId;
                let status = { status: data.Status, text: data.Data };
                let nodeModel = self.lf.getNodeModelById(nodeId);
                nodeModel.setProperties({ status: status });
                if (nodeModel.properties.Parent) return;
                self.lf.focusOn({
                    id: nodeId
                });


            });
        },




    },

    mounted: function () {

        this.bindEvent();
        this.loadData();




    }

};






