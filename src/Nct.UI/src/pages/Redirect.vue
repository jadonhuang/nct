
<script>
    export default {
        created() {
            const path = this.$urlDecode(this.$route.query.path);
            this.$router.replace({ path: path })
        },
        render(h) {

            return h() 
        }
    }
</script>
