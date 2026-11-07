<template>
    <div class="item">

        <template v-if="state.Viewonly">
            <div v-html="modelValue"></div>
        </template>
        <UEditor v-else  :key="prop.Id" v-model="modelValue" :config="{zIndex:8888, toolbar:prop.Toolbar,height:prop.Height,readonly:state.Readonly}"></UEditor>
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
                modelValue: this.value
            }
        },
        watch: {
            value(val) {
                this.modelValue = val;
            },
            modelValue() {
                this.$emit('input', this.modelValue);
                this.$emit('change', this.modelValue, this.state);
            }

        }



    }


</script>
