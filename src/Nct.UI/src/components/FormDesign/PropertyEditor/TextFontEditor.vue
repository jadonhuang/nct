<template>
  <el-form-item label="文字样式">

    <div class="item-font-holder">

      <el-select v-model="model.TextFont.FontSize" size="small" placeholder="请选择" filterable style="width:173px; margin-bottom:5px">
        <el-option v-for="item in fontSize"
                   :key="item.value"
                   :label="item.name"
                   :value="item.value">
        </el-option>
      </el-select>

      <el-color-picker size="small" v-model="model.TextFont.Color" title="颜色"></el-color-picker>
      <el-button :class="{'fa':true,'fa-bold':true,'selected':model.TextFont.FontWeight=='bold' }" @click="setItemFontAttr('FontWeight','bold')"></el-button>

      <el-button :class="{'fa':true,'fa-italic':true,'selected':model.TextFont.FontStyle=='italic' }" @click="setItemFontAttr('FontStyle','italic')"></el-button>

      <el-button :class="{'fa':true,'fa-underline':true,'selected':model.TextFont.TextDecoration=='underline' }" @click="setItemFontAttr('TextDecoration','underline')"></el-button>

    </div>
  </el-form-item>
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

    setItemFontAttr(key, value) {
      var titleFont = this.model.TextFont;
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
