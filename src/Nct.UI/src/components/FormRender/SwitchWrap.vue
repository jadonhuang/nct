
<template>
    <div>
        <el-switch :disabled="disabled" :value="value" :active-value="activeValue" @change="change"
                   :inactive-value="inactiveValue" :active-text="activeText" :inactive-text="inactiveText" :active-color="activeColor" :inactive-color="intactiveColor"> </el-switch>
    </div>
</template>

<script>

    export default {
        name: 'SwitchWrap',
        props: {
            activeText: String,
            inactiveText: String,
            activeColor: String,
            intactiveColor: String,
            disabled: Boolean,
            state: Object,
            value: [Boolean, Number]

        },
        computed: {
            activeValue() {

                if (typeof this.value === 'boolean')
                    return true;
                return 1;

            },
            inactiveValue() {
                if (typeof this.value === 'boolean')
                    return false;
                return 0;

            },
        },

        methods: {

            change(value) {
                this.$emit('change', value, this.state);

            }
        },

    
     
    };
</script>
