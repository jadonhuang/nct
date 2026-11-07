<template>
  <div>

    <el-input placeholder="输入关键字进行过滤" size="small" prefix-icon="el-icon-search"
              v-model="filterText">
    </el-input>
    <div class="tree-container">
        <el-tree :data="data" 
                 show-checkbox
                 node-key="Id"
                 ref="tree"
                 :filter-node-method="filterNode"
                 :props="defaultProps" @check="changed">
        </el-tree>
    </div>
  </div>
</template>
<script>



  export default {
    props: {
      value: { type: String }, isShow: { type: Boolean, default: false }
    },

    data() {
      return {

        defaultProps: {
          children: 'Children',
          label: 'Name'
        },
        data: [],
        filterText: '',
        show: this.isShow



      }
    },
    watch: {
      filterText(val) {
        this.$refs.tree.filter(val);

      },
      isShow(value) {
        this.filterText = '';
        if (value)
          this.getList();

      }


    },
    mounted() {

      this.getList();
    },
    methods: {
      filterNode(value, data) {
        if (!value) return true;
        return data.Name.indexOf(value) !== -1;
      },

      getLeafMenuIds(menus) {

        var leafMenuIds = [];

        function loop(menus) {

          for (var menu of menus) {
            if (menu.HasChildren)
              loop(menu.Children)
            else
              leafMenuIds.push(menu.Id.toString());
            
          }

        }
        loop(menus)
        return leafMenuIds;

      },

      setCheckedKeys: function (keys, data) {

        if (!keys) {
           keys = []
        }
        else {

          keys = keys.split(',');
          var leafIds = this.getLeafMenuIds(data);
          keys = keys.filter(m => leafIds.indexOf(m) != -1);

        }

        this.$refs.tree.setCheckedKeys(keys, true);
      },

      changed: function (data) {

        var ids = [];

        this.$refs.tree.getCheckedNodes(false, true).forEach(function (n) {

          ids.push(n.Id);

        });
        var idStr = ids.join(',');
        this.$emit('changed', idStr);

      },
      getList() {

          $menuApi.getTree({}).then((res) => {

          var data = res.Data;
          this.data = data;
          this.setCheckedKeys(this.value, data);

        });

      }

    }




  }
</script>

<style scoped>

    .tree-container {
        margin-top: 10px;
        max-height:300px;
        overflow:auto;
    }

</style>
