<template>
    <div class="container" :class="{active:isActive||visible}">
        <el-popover placement="right"
                    width="100"
                    ref="pop"
                    :value="visible"
                    trigger="manual">
            <div v-for="node in nodes" :key="node.type" class="node-item" @click="newNode(node)">
                <div class="node-img">
                    <img :src="getIcon(node.type)" />
                </div>
                <div class="node-text">
                    {{node.name}}
                </div>

            </div>
            <span slot="reference" class="icon-wrap">
                <i class="el-icon-circle-plus add-btn" @click="handleClick"></i>
            </span>
        </el-popover>
    </div>

</template>

<script>
    import { on, off } from '@/utils/dom';
    export default {
        props: {
            nodeModel: Object,
            anchorIndex: Number,
            isActive: {
                type: Boolean,
                default: false
            }
        },

        data() {
            return {
                visible: false,
                allNodes: [
                    { type: 'http', name: 'HTTP请求' },
                    { type: 'case', name: '条件' },
                    { type: 'script', name: '脚本' },
                    { type: 'iteration', name: '迭代' },
                    { type: 'jsonData', name: 'Json解析' },
                    { type: 'aiModel', name: 'Ai模型' },
                ]
            }
        },
        computed: {
            nodes() {

                const parent = this.nodeModel.getParent();
                if (parent) {
                    return this.allNodes.filter(m => m.type != 'loop' && m.type != 'iteration')
                }
                return this.allNodes;
            }
        },
        watch: {
           
        },
        mounted() {

            on(document, 'click', this.hidePop);

        },
        methods: {

           
            hidePop() {
                this.visible = false;

            },

            handleClick(e) {
                this.visible = true;
                e.stopPropagation();
            },
            getIcon(type) {
                return `${this.$publicPath}img/flow/${type}.svg`;
            },
            newNode(node) {

                this.visible = false;
                const { graphModel, x, y, width, properties } = this.nodeModel;
                const nodeType = node.type;
                const defaultHeight = this.nodeModel.getDefaultHeight();

                let nodeX = x + width / 2 + 200;
                let nodeY = y;

                let newNode = {
                    type: nodeType,
                    x: nodeX,
                    y: nodeY,
                    text: node.name,
                };
                    
                let newNodeModel;             
                if (nodeType == 'iteration') {
                
                    let loopNodeModel = graphModel.addNode(newNode);
                    newNodeModel = loopNodeModel;
                    nodeY = nodeY - ((loopNodeModel.height / 2) - defaultHeight) + defaultHeight / 2 + 10;
                    nodeX = nodeX - (loopNodeModel.width / 2) + 20 + 10;
                    const loopStartNode = {
                        type: "groupstart",
                        x: nodeX,
                        y: nodeY,
                        text: ''

                    };

                    let loopStartNodeModel = graphModel.addNode(loopStartNode);
                    loopStartNodeModel.setProperties({ Parent: loopNodeModel.id})
                    loopNodeModel.addChild(loopStartNodeModel.id);
                    loopNodeModel.autoResize();

                } else {

                    newNodeModel = graphModel.addNode(newNode);
                    let parent = properties.Parent;
                    if (parent) {
                        let parentModel = graphModel.getNodeModelById(parent);
                        parentModel.addChild(newNodeModel.id);
                        parentModel.autoResize();
                        newNodeModel.setProperties({ Parent: parent });
                    }

                }

                let startPoint = { x: 0, y: 0 }
                let endPoint = { x: 0, y: 0 }

                if (this.nodeModel.type == 'case') {
                    let anchor = this.nodeModel.getOutAnchor().find(m => m.id == this.anchorIndex);
                    startPoint.y = anchor.y;
                    startPoint.x = anchor.x;
                } else {
                    startPoint.x = x +width / 2;
                    startPoint.y = y;
                }

                endPoint.x = newNodeModel.x - newNodeModel.width / 2;
                endPoint.y = newNodeModel.y;

               graphModel.addEdge({
                    type: "polyline",
                    sourceNodeId: this.nodeModel.id,
                    targetNodeId: newNodeModel.id,
                    startPoint:startPoint,
                    endPoint: endPoint
              });
             
                if (this.nodeModel.type == 'case') {

                    this.nodeModel.setCaseEdge();
                }

        

            }

        },
        destroyed() {
            off(document, 'click', this.hidePop);

        }

    }


</script>

<style scoped>

    .active span {
        display: block !important;
    }

    .container:hover .icon-wrap {
        display: block !important;
    }

    .container {
        display: flex;
        position: absolute;
        right: -26px;
        top: 0;
        height: 100%;
        width: 24px;
        align-items: center;
        z-index: 1001;
        justify-content: flex-end;

    }

        .container .icon-wrap {
            background-color: transparent;
            margin-right: 2px;
            display: flex;
            align-items: center;
            display: none;
        }

    .add-btn {
        color: #33dd89;
        font-size: 18px;
        cursor: pointer;
    }

    .node-item {
        cursor: pointer;
        display: flex;
        align-items: center;
        padding: 5px;
    }

        .node-item:hover {
            background-color: #eee;
        }

    .node-img {
        background: none !important;
    }

    .node-text {
        padding-left: 5px !important;
    }
</style>