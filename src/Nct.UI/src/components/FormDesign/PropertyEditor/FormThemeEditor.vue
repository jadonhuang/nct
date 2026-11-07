<template>


    <el-form ref="form2" class="edit-form" size="small" label-width="80px" label-position="left">
        <el-form-item label="标题对齐">
            <el-radio-group v-model="model.TitlePosition" size="mini">
                <el-radio-button label="left">左对齐</el-radio-button>
                <el-radio-button label="right">右对齐</el-radio-button>
                <!--<el-radio-button label="top">顶部对齐</el-radio-button>-->

            </el-radio-group>
        </el-form-item>

        <el-form-item label="标题宽度">

            <el-input-number v-model="model.TitleWidth" :min="0" :precision="0" controls-position="right"></el-input-number>
        </el-form-item>


        <el-form-item label="标题样式">

            <div class="item-font-holder">

                <el-select v-model="model.TitleFont.FontSize" size="small" placeholder="请选择" filterable style="width:173px; margin-bottom:5px">
                    <el-option v-for="item in fontSize"
                               :key="item.value"
                               :label="item.name"
                               :value="item.value">
                    </el-option>
                </el-select>

                <el-color-picker title="颜色" size="small" v-model="model.TitleFont.Color"></el-color-picker>
                <el-button :class="{'fa':true,'fa-bold':true,'selected':model.FontWeight=='bold' }" @click="setFormFontAttr('FontWeight','bold')"></el-button>

                <el-button :class="{'fa':true,'fa-italic':true,'selected':model.FontStyle=='italic' }" @click="setFormFontAttr('FontStyle','italic')"></el-button>

                <el-button :class="{'fa':true,'fa-underline':true,'selected':model.TextDecoration=='underline' }" @click="setFormFontAttr('TextDecoration','underline')"></el-button>

            </div>
        </el-form-item>

        <el-form-item label="内边距">

            <el-input-number v-model="model.Padding" :min="0" :precision="0" controls-position="right"></el-input-number>
        </el-form-item>


    </el-form>


</template>

<script>
  export default {
    props: {

      desiner: Object,
      model: Object

    },
    computed: {
      fontSize() {

        var list = [];

        list.push({ value: 0, name: '默认' });

        for (var i = 9; i <= 30; i++) {

          list.push({ value: i, name: i + 'px' });
        }
        list.push({ value: 32, name: '32px' });
        list.push({ value: 34, name: '34px' });
        list.push({ value: 36, name: '36px' });
        list.push({ value: 40, name: '40px' });
        list.push({ value: 48, name: '48px' });
        list.push({ value: 56, name: '56px' });
        return list;
      },



    },


    methods: {

      setFormFontAttr(key, value) {
            var titleFont = this.model.TitleFont;
        if (!titleFont[key]) {
          titleFont[key] = value;

        } else {
          titleFont[key] = '';
        }

      },




    }


  }</script>

<style scoped>

  .item-font-holder {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
  }

  /deep/ .item-font-holder button {
    width: 30px;
    height: 30px;
  }


  /deep/ .el-radio-button__inner {

      padding:7px 8px;

  }

  /deep/ .item-font-holder .el-color-picker {
    margin-right: 10px;
  }


  /deep/ .item-font-holder .el-button--small, .el-button--small.is-round {
    padding: 9px 9px;
  }

  .selected {
    border: 1px solid #409EFF !important;
  }
</style>


