<template>
    <BaseNode v-bind="$props">
        <template v-if="properties.Cases.length>1">
            <div class="group" v-for="(grp,index) in properties.Cases" :key="index+'g'">
                <div class="group-title item-wrap">
                    <NewNodeWidget :isActive="isSelected||isHovered" v-if="!disabled" :nodeModel="model" :anchorIndex="index"></NewNodeWidget>
                    <div class="group-title-case">
                    <template v-if="!grp.IsElse">
                        Case{{index+1}}
                    </template></div>
                    <div class="group-title-name">{{index==0?"IF":(grp.IsElse)?"ELSE":"ELIF"}}</div>
                </div>

                <div class="group-cdt-wrap" v-if="!grp.IsElse">
                    <div v-for="(cdt,idx) in grp.Conditions" class="group-cdt item-wrap" :key="idx+'c'">
                        <template v-if="cdt.Left&&cdt.ComparisonOperator&&cdt.Right">
                            <div class="group-cdt-left" v-html="tranVariableTextToDisplayHtml(cdt.Left)">
                            </div>
                            <div class="group-cdt-op">{{displayOperator(cdt.ComparisonOperator)}}</div>
                            <div class="group-cdt-right" v-html="tranVariableTextToDisplayHtml(cdt.Right)"></div>
                        </template>
                        <span v-else>未设置条件</span>

                        <div class="logic-op" v-if="grp.Conditions.length>1&&idx!=grp.Conditions.length-1">{{grp.LogicalOperator}}</div>

                    </div>
                </div>

            </div>

        </template>

        <div class="item-wrap" v-else>
            <span class="node-tip">未设置条件</span>
        </div>
    </BaseNode>
</template>

<style scoped>

    /deep/ .item-wrap .var-name {
        color: #155aef !important;
    }

    .group-cdt-op {
        width: auto;
        text-align: center;
        padding: 0 2px 0 2px;
        position: relative;
    }

    .group-cdt-wrap, .group-title {
        padding-left: 10px;
        padding-right: 10px;
        position: relative;
    }

    .group-cdt-left, .group-cdt-right {
        height: 100%;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis
    }

    .group-cdt-right {
        text-align: right;
    }


    .item-wrap {
        height: 30px;
        line-height: 30px;
        box-sizing: border-box;
        display: flex;
        flex-direction: row;
        align-items: center;
    }

    .group-cdt {
        background-color: #f2f4f7;
        border-bottom: 2px solid #fff;
        padding-right: 5px;
        padding-left: 5px;
        border-radius: 5px;
        position: relative;
    }

    .logic-op {
        position: absolute;
        right: 2px;
        top: 50%;
        color: #155aef;
        z-index: 100;
    }

    .group-title-case {
        height: 100%;
        width: 50%;
        box-sizing: border-box;
    }

    .group-title-name {
        height: 100%;
        width: 50%;
        text-align: right;
        box-sizing: border-box;
        font-weight: bold;
    }
</style>
<script>
    import flow from '@/mixins/flow.js'
    import BaseNode from './BaseNode.vue';
    import NewNodeWidget from './NewNodeWidget.vue';
    export default {
        mixins: [flow],
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

            }
        },
        methods: {
            displayOperator(val) {
                const ops = this.$consts.logicOperators;
                let op = ops.find(m => m.name == val || m.value == val);
                if (op) return op.name;
                return val;



            }

        }


    }


</script>

