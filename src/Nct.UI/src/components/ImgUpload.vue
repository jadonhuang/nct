<template>
  <el-upload class="avatar-uploader"
             accept="image/*"
             :action="uploadUrl"
             :show-file-list="false"
             :on-success="handleSuccess"
             :before-upload="beforeUpload">
    <img v-if="getPic" :src="getPic" class="avatar">
    <i v-else class="el-icon-plus avatar-uploader-icon"></i>
  </el-upload>
</template>
<script>



  export default {
    props: {
      value: { type: String }, limitSize: {
        type: Number,
        default: 1024
      }
    },

    data() {
      return {
        uploadUrl: this.$appUrl + '/util/upload'
      }
    },

    computed: {
      getPic: function () {
        var pic = this.value ? this.$appUrl + this.value : null;
        return pic;

      }

    },

    methods: {
      handleSuccess(res, file) {
 
        if (res.IsSuccess) {
          this.$emit('success', res.Data);
        } else {
          this.$emit('fail', res.Msg );
          this.$errTip(res.Msg);
        }
      },
      beforeUpload(file) {

        const isLt200K = file.size / 1024 <= this.limitSize;
        if (!isLt200K) {
          this.$errTip('图片大小不能超过 ' + this.limitSize + 'K!');
        }
        return isLt200K;
      }
     
    
    }

  
    


  }
</script>
