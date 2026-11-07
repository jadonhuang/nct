<template>

    <section class="nct-work-area" style="height:100%;overflow:hidden">
        <div ref="container" style="height:100%;width:100%">

        </div>

    </section>
</template>

<style scoped src="../FlowDef/FlNodes/Node.css"></style>

<script>

    import LogicFlow from "@logicflow/core";
    import "@logicflow/core/lib/style/index.css";

    import CaseNode from '@/pages/FlowDef/FlNodes/CaseNode'
    import StartNode from '@/pages/FlowDef/FlNodes/StartNode'
    import EndNode from '@/pages/FlowDef/FlNodes/EndNode'
    import TaskNode from '@/pages/FlowDef/FlNodes/TaskNode'
    import ScriptNode from '@/pages/FlowDef/FlNodes/ScriptNode'

    export default {
        props: {

            config: Object

        },

        data() {
            return {

            }
        },
        computed: {


        },

        methods: {

            renderFlow() {

                const lf = new LogicFlow({
                    isSilentMode: true,
                    container: this.$refs.container,
                    grid: false,

                    style: this.$consts.flow.style
                });

                lf.batchRegister([StartNode, EndNode, TaskNode, CaseNode, ScriptNode]);
                lf.render(this.config);

            },





        },
        created() {
           
        },
    
        mounted() {
           
            this.renderFlow();


        },
        components: {  }


    }


</script>

<style scoped>

</style>