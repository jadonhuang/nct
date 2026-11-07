<template>

    <el-dialog :title="title" class="nct-dialog" :visible.sync="isShow" append-to-body width="60%" destroy-on-close :close-on-click-modal="false" :before-close="handleBeforeClose">

        <el-container class="nct-hastree-container" v-if="pageType==1">

            <el-aside width="200px">

                <el-card class="tree-card" shadow="never" :body-style="{ padding: '0px' }">
                    <div slot="header">
                        <span>部门</span>
                    </div>
                    <el-tree :data="orgs" :props="defaultProps" @node-click="handleNodeClick" class="dic-tree" ref="orgTree"></el-tree>
                </el-card>
            </el-aside>

            <el-main class="nct-work-area">


                <div class="nct-cmd-area">


                    <div class="nct-toolbar" style="padding-bottom:0">

                        <div class="nct-search">

                            <el-col :span="16">
                                <el-input auto-complete="off" clearable v-model="query.Name" placeholder="搜索关键字">
                                    <el-button slot="append" icon="el-icon-search" @click="doSearch"></el-button>
                                </el-input>
                            </el-col>

                        </div>
                        <div class="right-tool">
                            <el-button type="primary" @click="viewSelectedUsers" icon="el-icon-circle-check" size="small">已选用户({{selectedUsers.length}})</el-button>

                        </div>
                    </div>
                </div>


                <div class="nct-list-table" v-loading="loading">

                    <el-col :span="6" v-for="(user,index) in users" :key="index">
                        <div class="user-item" @click="setSelected(user)" :class="{'selected':isSelected(user)}">
                            <div>

                                <img src="@/assets/img/female.png" width="40" v-if="user.Sex==2" />
                                <img src="@/assets/img/male.png" width="40" v-else />

                            </div>
                            <div class="user-txt">

                                <div>{{user.UserName}}</div>

                                <div :title="user.RealName">{{user.RealName}}</div>

                            </div>

                        </div>

                    </el-col>

                    <div style="padding:30px;text-align:center;flex:1" v-if="users.length==0">
                        无数据
                    </div>

                </div>

            </el-main>


        </el-container>

        <el-container class="nct-hasnotree-container" v-else>


            <el-main class="nct-work-area">


                <div class="nct-cmd-area">


                    <div class="nct-toolbar" style="padding-bottom:0">

                        <div class="nct-search">

                            <el-col :span="16">
                                <el-input auto-complete="off" clearable v-model="query.Name2" placeholder="搜索关键字">
                                    <el-button slot="append" icon="el-icon-search" @click="doSearchSelected"></el-button>
                                </el-input>
                            </el-col>

                        </div>
                        <div class="right-tool">
                            已选({{selectedUsers.length}}) <el-button type="primary" @click="pageType=1" icon="el-icon-back" size="small">返回</el-button>

                        </div>
                    </div>
                </div>


                <div class="nct-list-table" v-loading="loading">

                    <el-col :span="6" v-for="(user,index) in selectedUsers2" :key="index">
                        <div class="user-item">
                            <div>

                                <img src="@/assets/img/female.png" width="40" v-if="user.Sex==2" />
                                <img src="@/assets/img/male.png" width="40" v-else />

                            </div>
                            <div class="user-txt">

                                <div>{{user.UserName}}</div>

                                <div :title="user.RealName">{{user.RealName}}</div>

                            </div>

                            <div style="display:flex;vertical-align:central;">
                                <el-link icon="el-icon-delete" @click="removeUser(user)"></el-link>
                            </div>

                        </div>

                    </el-col>

                    <div style="padding:30px;text-align:center;flex:1" v-if="selectedUsers2.length==0">
                        无数据
                    </div>

                </div>

            </el-main>


        </el-container>

        <span slot="footer">

            <el-button type="primary" @click.native="close(true)" size="small" icon="el-icon-circle-check">确认</el-button>
            <el-button @click.native="close(false)" size="small" icon="el-icon-circle-close">取消</el-button>

        </span>

    </el-dialog>


</template>
<script>
    
    export default {
        data() {
            return {
                orgs: [],
                loading: false,
                defaultProps: {
                    children: 'Children',
                    label: 'Name'
                },
                users: [],
                isShow: false,
                title: '',
                selectedIds: [],
                callback: null,
                isConfirmed: false,
                selectedUsers: [],
                selectedUsers2: [],
                pageIndex: 1,
                query: { Id: 0, Name: '', Name2:'' },
                total: 0,
                pageType: 1,
                multiSelect:true,


            }
        },
        computed: {
           

        },
        methods: {

            isSelected(user) {

                let index = this.selectedUsers.findIndex(m => m.Id == user.Id);
                return index != -1;
               
            },
            removeUser(user) {
                let index = this.selectedUsers.findIndex(m => m.Id == user.Id);
                this.selectedUsers.splice(index, 1);
                this.doSearchSelected();
            },
            show() {

                this.isShow = true;
                this.getOrgs();
                this.getPageList();
                this.getUsersByIds();

            },
            setSelected(user) {

                if (!this.multiSelect) {
                    this.selectedUsers = [];
                }
                let index = this.selectedUsers.findIndex(m => m.Id == user.Id);
                if (index != -1) {
                    this.selectedUsers.splice(index,1);
                } else {
                    this.selectedUsers.push(user);

                }

            },

            async viewSelectedUsers() {

                this.pageType = 2;
                this.loading = true;
                this.selectedUsers2 = this.selectedUsers.filter(m => m.Id > 0);
                this.query.Name2 = '';
                this.loading = false;

            },
            doSearchSelected() {

                let name = this.query.Name2;
                if (name) {

                    this.selectedUsers2 = this.selectedUsers.filter(m => m.UserName.indexOf(name) != -1 || m.RealName.indexOf(name) != -1);
                } else {

                    this.selectedUsers2 = this.selectedUsers.filter(m => m.Id > 0);
                }

            },
            handleBeforeClose(done) {
                this.close(false);
                done()
            },

            close(isOk) {

                this.isShow = false;
                if (isOk) {
                    this.callback(this.selectedUsers)

                } else {

                    this.callback(null)
                }

            },

            handleNodeClick(data) {
                this.query.Id = data.Id;
                this.doSearch();
            },

            async getOrgs() {

                let result = await $orgApi.getTreeForList({});
                let orgs = result.Data;
                orgs.splice(0, 0, { Name: '全部', Id: 0 });
                this.orgs = orgs;
            },

            async getUsersByIds() {

                if (this.selectedIds.length == 0) return;

                let data = await $userApi.getListByIds(this.selectedIds);
                this.selectedUsers = data.Data;
            },

            async getPageList() {
                this.loading = true;
                let dto = {
                    PageIndex: this.pageIndex, PageSize: 16, Query: { Id: this.query.Id, UserName: this.query.Name }

                }
                let result = await $userApi.getList(dto);
                this.users = result.Data.Rows;
                this.total = result.Data.Total;
                this.loading = false;
            },

            async doSearch() {
                this.pageIndex = 1;
                await this.getPageList();
            },



        },
        async mounted() {

        },



    }


</script>


<style scoped>

    .nct-list-table {
        border: 1px solid #ebeef5;
        box-sizing: border-box;
        padding: 10px;
        display: flex;
        gap: 0px;
        min-height: 300px;
        align-items: flex-start;
        align-content: flex-start;
        flex-wrap: wrap;
        flex-direction: row;
        padding:5px;
    }
   
    .user-item {
        border: solid 1px #ebeef5;
        box-sizing: border-box;
        border-radius: 3px;
        cursor: pointer;
        display: flex;
        margin: 5px;
        padding:6px;
    }
    .user-txt {
        margin-left: 10px;
        line-height:20px;
        overflow: hidden;
        white-space: nowrap;
        text-overflow: ellipsis;
        width: 50px;
        flex:1;
    }

    .selected {
        border: 2px solid #ff6a00;
    }

    .tree-card {
        width: 100%;
        height: 100%;
        box-sizing: border-box;
    }

    .user-item {
    }
</style>

