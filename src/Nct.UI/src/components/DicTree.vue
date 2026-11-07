<template>
    <el-card class="tree-card" shadow="never" :body-style="{ padding: '0px' }">
        <div slot="header">
            <span>{{title}}</span>

            <el-button icon="el-icon-refresh" @click="getData"   style="float: right; padding: 1px 0;" type="text"></el-button>
            <el-button icon="el-icon-plus"  @click="showAddForm"  style="float: right; padding: 1px 0; margin-right: 20px;" type="text">添加</el-button>

        </div>

        <el-tree :data="data" :props="defaultProps" @node-click="handleNodeClick" class="dic-tree" ref="dicTree"
                 @node-contextmenu="rightClick"></el-tree>

        <div id="contextMenu" v-if="showContextMenu" class="tree-menu" :style="menuStyle">
            <ul>
                <li @click="showAddForm"><i class="el-icon-plus"></i>新增</li>
                <li @click="showEditForm"><i class="el-icon-edit"></i> 编辑</li>
            </ul>
        </div>

        <CreateForm ref="createForm" @refresh="getData"> </CreateForm>
        <EditForm ref="editForm" @refresh="getData"> </EditForm>

    </el-card>
</template>
<script>

  import CreateForm from '@/pages/Dictionary/Create.vue';
  import EditForm from '@/pages/Dictionary/Edit.vue';

  export default {
    components: { CreateForm, EditForm },
    props: {
      title: { type: String, default: '分类选择' },

      parentCode: { type: String }, canEdit: {
        type: Boolean,
      }
    },

    data() {
      return {
        data: [],
        showContextMenu: false,
        parentNode: null,
        curNode: null,
        menuStyle: {},
        defaultProps: {
          children: 'Children',
          label: 'Name'
        }
      }
    },

    computed: {


    },

    methods: {

      showAddForm() {

        this.$refs.createForm.show();
        var pid = this.curNode ? this.curNode.Id : this.parentNode.Id;
        this.$refs.createForm.setPid(pid);

      },
      showEditForm() {

        this.$refs.editForm.show(this.curNode.Id);
      },

      rightClick(e, data, node, comp) {
        if (!this.canEdit) return;

        this.curNode = node.data;
        this.menuStyle = { top: e.pageY + 'px', left: e.pageX + 'px' }
        this.showContextMenu = true
        const self = this
        document.onclick = function (ev) {
  
          if (ev.target.id !== 'contextMenu') {
            self.showContextMenu = false;
            self.curNode = null;
          }
        }
      },
      uncheckNode() {
        this.$refs.dicTree.setCurrentKey(null);
        this.$emit('change', null);

      },

      handleNodeClick(data) {
        this.$emit('change', data);
      },
      getData() {
          $dicApi.getTreeByParentCode(this.parentCode).then(res => {

          this.data = res.Data;


        });
      },
      getByCode() {
          $dicApi.getByCode(this.parentCode).then(res => {

          this.parentNode = res.Data;

        });
      },

    },

    mounted() {

      this.getData();
      if (this.canEdit)
        this.getByCode();
    }




  }
</script>

<style scoped>

  .tree-card {
    width: 100%;
    height: 100%;
    box-sizing: border-box;

  }

  .clearfix:before,
  .clearfix:after {
    display: table;
    content: "";
  }

  .clearfix:after {
    clear: both
  }


  .tree-menu {
    position: fixed;
    display: block;
    z-index: 20000;
    background-color: #fff;
    padding: 5px 0;
    border: 1px solid #ebeef5;
    border-radius: 4px;
    box-shadow: 0 2px 12px 0 rgba(0,0,0,.1);
  }


  ul {
    margin: 0;
    padding: 0;
  }

    ul li {
      list-style: none;
      margin: 0;
      padding: 0 15px;
      font-size: 12px;
      line-height: 30px;
      cursor: pointer;
    }
      ul li i{

        margin-right:5px;

      }
      ul li:hover {
        background-color: #ebeef5
      }
    .dic-tree {
       margin-top:5px;
    
    }
    /deep/ .dic-tree .el-tree-node__label {
        font-size: 12px;
    }

</style>
