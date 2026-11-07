import util from '@/utils/util.js'
import LogicFlow from "@logicflow/core";
import { Menu } from '@logicflow/extension';
import "@logicflow/core/lib/style/index.css"
import '@logicflow/extension/lib/style/index.css';

import CaseNode from './FlNodes/CaseNode'
import StartNode from './FlNodes/StartNode'
import EndNode from './FlNodes/EndNode'
import TaskNode from './FlNodes/TaskNode'
import ScriptNode from './FlNodes/ScriptNode'

import ToolNode from './ToolNode'


export default {
    name: 'FlowDesign',
    componentName: 'FlowDesign',
    provide() {
        return {
            designer: this
        };
    },
    mixins: [],
    components: {
        ToolNode
    },
    data() {

        return {
            lf: null,
            curTabId: 'first',
            selected: false,
            curNode: null,
            saveType: 1,
            model: { Code: '', Name: '', Cate: null, Descpt: '', ClientRule: '', ServerRule: '' },
            rules: {
                Name: [
                    { required: true, message: '请输入流程名称', trigger: 'blur' },
                ],

                Code: [
                    { required: true, message: '请输入流程编码', trigger: 'blur' },
                    {
                        validator: function (rule, value, callback) {
                            var valid = util.isValidId(value)
                            if (!valid) {
                                callback(new Error('流程编码格式不正确'));

                            } else {
                                callback();
                            }

                        }
                    }

                ]


            },

            errInfo: {},
            saving: false,
            designDataBak: {},
            formDefs: [],
            users: [],
            loading: false,
            errMsg: [],
            cdtEditDialogVisible: false,
            cdt: null,
            cdtIndex: 0,
            cdtErrMsg: '',
            auditLeaderDialogVisible: false,
            checkedAuditLeaderIds: [],
            auditLeaders:[{ Id: 0, Type: 3, Name: "上1级领导" },
            { Id: 1, Type: 3, Name: "上2级领导" }, { Id: 2, Type: 3, Name: "上3级领导" }, { Id: 3, Type: 3, Name: "上4级领导" }
                , { Id: 4, Type: 3, Name: "上5级领导" }, { Id: 5, Type: 3, Name: "上6级领导" }

            ]




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

        async addAuditUser() {
            let members = this.curNode.props.Audit.Members ;
            let selectedIds = members.filter(m => m.Type == 1).map(m => m.Id);
            let options = { selectedIds };
            let users = await this.$showUserSelect(options);
            var auditUsers = members.filter(m => m.Type != 1);
            if (users) {

                for (let user of users) {

                    auditUsers.push({ Id: user.Id, Type: 1, Name: user.RealName || user.UserName })
                }

                this.curNode.props.Audit.Members = auditUsers;
            }

        },

        async showAuditLeaderDialog() {

            let members = this.curNode.props.Audit.Members;
            let selectedIds = members.filter(m => m.Type == 3).map(m => m.Id);
            this.checkedAuditLeaderIds = selectedIds;
            this.auditLeaderDialogVisible = true;
        },



        async showAuditRoleDialog() {

            let members = this.curNode.props.Audit.Members;
            let selectedIds = members.filter(m => m.Type == 2).map(m => m.Id);

            this.$refs.roleSelect.show(selectedIds);
        },

        setAuditLeader() {

            let members = this.curNode.props.Audit.Members ;
            var auditUsers = members.filter(m => m.Type != 3);

            for (var id of this.checkedAuditLeaderIds) {

                var item = this.auditLeaders.find(m => m.Id == id);
                auditUsers.push(item);

            }
            this.curNode.props.Audit.Members = auditUsers;
            this.auditLeaderDialogVisible = false;
        },

        setAuditRole(roles) {

            let members = this.curNode.props.Audit.Members ;
            var auditUsers = members.filter(m => m.Type != 2);
            for (var role of roles) {

                auditUsers.push({ Id: role.Id, Type: 2, Name: role.Name })

            }
            this.curNode.props.Audit.Members = auditUsers;

        },

        delAudit(index) {

            this.curNode.props.Audit.Members.splice(index, 1);
            this.saveCurNodeProps();

        },

        delCdt(index) {

            this.curNode.props.Cdts.splice(index, 1);
            this.saveCurNodeProps();
            var nodeModel = this.lf.getNodeModelById(this.curNode.id);
            nodeModel.clearInvalidCdtEdge();

        },
        editCdt(index) {

            let cdt = this.curNode.props.Cdts[index];
            this.cdt = this.$copyObj(cdt);
            this.cdtEditDialogVisible = true;
            this.cdtIndex = index;

        },
        addCdt() {

            var num = this.curNode.props.Cdts.length;
            var text = '分支' + num;
            this.curNode.props.Cdts.push({ Text: text, IsDefault: false, CdtType: 1, SqlCdt: '', CodeCdt: '', EdgeId: '' });
            this.saveCurNodeProps();

        },
        saveCdt() {

            this.cdtErrMsg = '';
            if (!this.cdt.SqlCdt && !this.cdt.CodeCdt) {
                this.cdtErrMsg = '请输入条件';
                return;
            }
            this.curNode.props.Cdts[this.cdtIndex] = this.cdt;
            this.cdtEditDialogVisible = false;
        },

        saveCurNodeProps() {

            let curNode = this.curNode;
            if (curNode) {
                let props = this.lf.getProperties(curNode.id);
                curNode.props.warnings = props.warnings;
                if (curNode.type == 'case') {

                    for (let i = 0; i < curNode.props.Cdts.length; i++) {
                        let cdtOld = curNode.props.Cdts[i];
                        let  cdtNew = props.Cdts[i];
                        if ( cdtNew)
                            cdtOld.EdgeId =  cdtNew.EdgeId;

                    }

                }

                this.lf.setProperties(curNode.id, curNode.props);
            }

        },
        setCurNode(data) {
            const props = data.properties;
            this.curNode = { id: data.id, type: data.type, text: data.text?.value, props: props };

        },

        async getFormDefs() {

            var res = await $formDefApi.getAllList();
            this.formDefs = res.Data || [];
        },

        loadData() {
            const id = this.$route.query.id;
            if (id) {
                this.saveType = 2;
                this.curTabId = 'second';
                this.getDesignData(id);
            } else {

                var data = {
                    "nodes": [{
                        "type": "start",
                        "x": 150,
                        "y": 200,
                        "text": '开始',

                    }, {

                        "type": "end",
                        "x": 700,
                        "y": 200,
                        "text": '结束'
                    },
                    ],
                }
                this.renderFlow(data);

            }
            this.getFormDefs();


        },

        saveBakDesignData() {

            this.designDataBak = this.getNewDesignData();

        },


        getDesignData: function (id) {
            this.loading = true;
            $flowDefApi.get(id).then(res => {
                this.loading = false;
                if (res.IsSuccess) {
                    let data = res.Data;
                    this.model = data;
                    let graphData = JSON.parse(data.Data);
                    this.saveBakDesignData();
                    this.renderFlow(graphData);

                } else {
                    this.$errTip(res.Msg);
                }

            });

        },

        isDesignDataChanged() {

            var bakData = '';
            if (this.designDataBak) {
                bakData = JSON.stringify(this.designDataBak);

            }
            var currentData = JSON.stringify(this.getNewDesignData());
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

        getNewDesignData() {

            var data = this.$copyObj(this.model);
            if (this.lf) {
                let graphData = this.$copyObj(this.lf.getGraphData());
                data.Data = JSON.stringify(graphData);

            }

            return data;
        },

        validateDesignData() {


            let data = this.lf.getGraphData();
            let nodes = data.nodes;

            let valid = true;
            if (nodes.find(m => m.type == 'task') == null) {

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

            if (!valid) {

                this.curTabId = 'second';
            }

            return valid;

        },

        saveDesignData: function (close) {
         
            if (this.saving) {
                return;
            }
            this.saveCurNodeProps();
            this.saving = true;

            this.$refs.infoform.validate((valid) => {

                if (!valid) {
                    this.saving = false;
                    this.curTabId = 'first'
                    return;
                }

                valid = this.validateDesignData();
                if (!valid) {
                    this.saving = false;
                    return;
                }

                var dto = this.getNewDesignData();
                const loading = this.$showLoading('保存中...');
                if (this.saveType == 1) {

                    $flowDefApi.create(dto).then(res => {
                        loading.close();
                        this.saving = false;
                        if (res.IsSuccess) {
                            this.$okTip("保存成功");
                            this.$closePage();
                            if (!close) {
                                let path = '/flowdef/design?id=' + res.Data.Id;
                                let name = `编辑流程 - ${this.model.Name}`
                                this.$openPage(path, name);
                                this.$eventHub.$emit('flowdef-refresh');
                            }



                        } else {

                            this.$errTip(res.Msg);
                        }

                    })


                } else {
                    $flowDefApi.update(dto).then(res => {
                        loading.close();
                        this.saving = false;
                        if (res.IsSuccess) {
                            this.$okTip("保存成功");
                            this.$eventHub.$emit('flowdef-refresh');
                            if (close) {
                                this.$closePage();
                            } else {

                                this.saveBakDesignData();
                            }

                        } else {

                            this.$errTip(res.Msg);
                        }

                    })

                }



            });

         


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
                nodeTextEdit: false,
                edgeTextEdit: false,
                nodeTextDraggable: false,
                edgeTextDraggable: false,
                adjustEdgeMiddle: true,
                style: this.$consts.flow.style,
                //edgeType: 'bezier',

            });

          

            this.lf = lf;
            let self = this;
            lf.extension.menu.setMenuConfig({
                nodeMenu: [
                    {
                        text: "删除",
                        callback(node) {
                            let model = lf.getNodeModelById(node.id);
                            if (model.type == 'start' || model.type == 'end') {
                                self.$errTip('开始和结束节点不能删除');
                                return;
                            }
                            lf.deleteNode(node.id);
                            self.saveCurNodeProps();
                            self.curNode = null;
                        },
                    },
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
            lf.batchRegister([StartNode, EndNode, TaskNode, CaseNode, ScriptNode]);


            lf.on("node:dnd-add", ({ data }) => {

                this.saveCurNodeProps();
                this.setCurNode(data);
                lf.selectElementById(data.id);


            });
            lf.on("edge:add,edge:delete", ({ data }) => {

                let srcId = data.sourceNodeId;
                let model = lf.getNodeModelById(srcId);
                if (model.type == 'case') {
                    model.setCdtEdge();
                }

            });

            lf.on("node:click", (data) => {
                this.saveCurNodeProps();
                this.setCurNode(data.data);
            });

            lf.on("blank:click", (e) => {
                this.saveCurNodeProps();
                this.curNode = null;
            });



            lf.render(graphData);

        },



    },

    mounted: function () {

        this.loadData();

    }

};






