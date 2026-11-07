<template>
    <div class="node-wrap" @mousedown="()=>{addNode()}">
        <div class="node-header">
            <div class="node-img">
                <img :src="icon" />
            </div>
            <div class="node-text">
                {{text}}
            </div>
        </div>
    </div>

</template>
<script>


    export default {
        props: {
            type: String,
            text: String,
            lf: null
        },

        data() {
            return {

            }
        },
        computed: {

            icon() {

                return `${this.$publicPath}img/flow/${this.type}.svg`;

            },

        },
        methods:{

            addNode() {

                let nodel = {
                    type: this.type,
                    text: this.text,
                   
                };
                 this.lf.dnd.startDrag(nodel);


               
            }
          


        }
     
    }


</script>
<style scoped src="./FlNodes/Node.css"></style>
<style scoped>

    .node-wrap {
        width: 111px !important;
        margin-top: 5px;
        margin-bottom: 5px;
        cursor: pointer;
        height: auto !important;
        border: none !important;
        padding: 0 !important;
        box-shadow:none;
        border-radius:0px;

    }

        .node-wrap:hover {
            background-color: #eee !important;
        }
  
    .node-text {
        padding-left:0px !important;
    }
    .node-header {
        height: 35px !important;
        background-color: transparent !important;
    }
   

</style>

