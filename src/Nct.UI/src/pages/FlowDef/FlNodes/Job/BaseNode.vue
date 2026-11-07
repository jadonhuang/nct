<template>

    <BaseNode v-bind="$props">
        <NewNodeWidget :isActive="isSelected||isHovered" v-if="!disabled&&model.type!=='case'" :nodeModel="model"></NewNodeWidget>
        <div class="node-status" v-if="showStatus">
            <slot name="status">
                <span v-if="properties.status.text" class="percent">{{properties.status.text}}</span>
            </slot>
            <i :class="[getStatusIcon]"></i>
        </div>

        <div class="more-wrap-outer" :class="{'show':showMore}">
            <el-popover placement="right"
                        width="60"
                        ref="pop"
                        v-model="popVisible"
                        trigger="click">

                <div class="cmd-item">
                    <el-link icon="el-icon-document-copy" :underline="false" @click="handleCopyNode">复制</el-link>
                </div>
                <div class="cmd-item">
                    <el-link icon="el-icon-delete-solid" :underline="false" @click="handleDelNode">删除</el-link>
                </div>

                <div slot="reference" class="more-wrap" @click.stop="">
                    <el-link icon="el-icon-more" :underline="false"></el-link>
                </div>
            </el-popover>
        </div>
        <slot></slot>
    </BaseNode>

</template>
<script>

    import BaseNode from '../BaseNode.vue';
    import NewNodeWidget from './NewNodeWidget.vue';
    export default {
        components: {
            BaseNode, NewNodeWidget
        },
        props: {
            model: Object,
            isSelected: Boolean,
            isHovered: Boolean,
            disabled: Boolean,
            properties: Object,
            text: String,
        },

        data() {
            return {
                popVisible: false
            }
        },
        computed: {
            getStatusIcon() {
                let status = this.properties.status.status;
                if (status == 1)
                    return ["el-icon-loading", "success"]
                if (status == 2)
                    return ["el-icon-success", "success"]

                return ["el-icon-error", "error"]
            },
            showStatus() {
                return this.properties.status && this.properties.status.status != 0;
            },
            showMore() {
                return !this.disabled && !this.showStatus && (this.isSelected || this.isHovered)
                    && this.model.type !== 'start' && this.model.type !== 'groupstart';
            }
        },
        methods: {

         
            handleCopyNode() {
                this.popVisible = false;
                const { graphModel, id } = this.model;
                let nodeData = graphModel.cloneNode(id);
                let nodeModel = graphModel.getNodeModelById(nodeData.id)

                if (nodeModel.getChildren) {
                    let children = nodeModel.getChildren()
                    nodeModel.clearChildren();
                    for (let child of children) {
                        let childNode = graphModel.cloneNode(child);
                        nodeModel.addChild(childNode.id);
                    }

                } else {

                    var parent = nodeModel.properties.Parent;
                    if (parent) {
                        var parentModel = graphModel.getNodeModelById(parent);
                        parentModel.addChild(nodeData.id);
                    }


                }

                graphModel.selectNodeById(nodeData.id);




            },
            handleDelNode() {
                this.popVisible = false;
                let parent = this.properties.Parent;
                if (parent) {
                    let parentModel = this.model.graphModel.getNodeModelById(parent);
                    parentModel.removeChild(this.model.id);

                }
                this.model.remove();
                this.model.graphModel.eventCenter.emit("node:delete", this.model);


            },





        }


    }


</script>

<style scoped>
    .percent {
        font-size: 12px;
        margin-right: 3px;
    }

    .node-status {
        position: absolute;
        right: 10px;
        height: 40px;
        top: 0;
        line-height: 40px;
        font-size: 16px;
    }

    .more-wrap {
        position: absolute;
        right: 0;
        height: 15px;
        line-height:15px;
        top: -16px;
        border-radius: 3px;
        padding-left: 5px;
        padding-right: 5px;
        background-color:#fff;
    }

        .more-wrap .el-link {
            font-size:14px;
            line-height:15px;

        }
    .more-wrap-outer {
        display: none;
    }

    .more-wrap-outer.show {
        display: block !important
    }

    .cmd-item {
        padding: 3px;
    }
</style>

