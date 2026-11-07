<template>
    <div class="item">

        <FileUpload v-if="modelValue||!state.Viewonly" :limit="prop.Limit" v-model="modelValue" :type="prop.Type" :accept="prop.Accept" :disabled="state.Readonly||state.Viewonly" :data="state.Opts"></FileUpload>
        <div v-else>暂无文件</div>


    </div>
</template>
<script>


    export default {
        props: {
            value: String,
            prop: Object,
            state: Object,

        },
        data() {

            return {
                modelValue: this.value,
            }
        },
        watch: {
            value(val) {
                this.modelValue = val;
            },
            modelValue() {
                this.$emit('input', this.modelValue)
                this.$emit('change', this.modelValue, this.state);
            }

        }


    }


</script>


