<template>
    <div class="node-wrap" :class="statusClass">
        <div class="node-error-info" :class="{'has-err':hasWarnning}">
            <el-popover placement="right" trigger="hover">
                <span slot="reference">
                    <i class="el-icon-warning"></i>
                </span>
                <div v-for="(item ,index) in properties.warnings" :key="index" style="padding:3px">{{item}}</div>
            </el-popover>
        </div>

        <div class="node-header">
            <div class="node-img">
                <img :src="getIcon" />
            </div>
            <div class="node-text" v-if="text">
                {{text}}
            </div>
        </div>
        <slot></slot>
    </div>

</template>
<script>


    export default {
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
        computed: {
            hasWarnning() {

                return this.properties.warnings && this.properties.warnings.length > 0
            },
            getIcon() {

                return `${this.$publicPath}img/flow/${this.model.type}.svg`;

            },

            statusClass() {
                let obj = {};
                if (this.disabled) return obj;

                if (this.isHovered) {

                    obj['node-hovered'] = true;

                }
                if (this.isSelected) {
                    obj["node-selected"] = true;
                }

                return obj;


            }

        },

        methods: {


        },
        created() {

        },

        mounted() {



        },



    }


</script>

<style scoped src="./Node.css"></style>