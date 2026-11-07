<template>

    <el-container class="nct-container">
        <el-aside :style="getStyle">

            <div class="nct-logo-title">{{appName}}</div>

            <el-scrollbar>
                <div class="nct-menu">
                    <el-menu unique-opened router background-color="#001529" :default-active="currentPath" @select="handleMenuSelect" :collapse="isCollapse" text-color="rgba(255, 255, 255, 0.65)" active-text-color="rgb(64, 158, 255)">
                        <template v-for="(menu, index) in menus">
                            <el-submenu :key="index" :index="menu.Id.toString()" v-if="menu.HasChildren">
                                <template slot="title">
                                    <i :class="menu.Icon"></i>
                                    <span>{{menu.Name}}</span>
                                </template>

                                <template v-for="(m) in menu.Children">
                                    <el-menu-item :index="m.Url" :key="m.Id"><i :class="m.Icon"></i>{{m.Name}}</el-menu-item>
                                </template>

                            </el-submenu>

                            <el-menu-item :key="index" :index="menu.Url" v-else>
                                <i :class="menu.Icon"></i>
                                <span slot="title">{{menu.Name}}</span>
                            </el-menu-item>

                        </template>
                    </el-menu>




                </div>
            </el-scrollbar>


        </el-aside>
        <el-container>
            <el-header height="91px">
                <div class="nct-header">
                    <div class="nct-header-left">

                        <i :class="{'toggle-menu-btn':true,'el-icon-s-fold':!isCollapse,'el-icon-s-unfold':isCollapse}" @click="isCollapse=!isCollapse"></i>

                    </div>
                    <div class="nct-header-right">
                        <el-dropdown trigger="click" @command="handleUserDropdown">
                            <span class="el-dropdown-link"> {{displayName}}<i class="el-icon-arrow-down el-icon--right"></i></span>
                            <el-dropdown-menu slot="dropdown">
                                <el-dropdown-item command="profile" icon="el-icon-user">个人资料</el-dropdown-item>
                                <el-dropdown-item command="pwd" icon="el-icon-lock">修改密码</el-dropdown-item>
                                <el-dropdown-item command="logout" icon="el-icon-switch-button">退出登录</el-dropdown-item>
                            </el-dropdown-menu>
                        </el-dropdown>

                        <img :src="userPic" class="head-pic" />

                        <div class="msg" @click="openMsgPage">

                            <el-badge :hidden="msgNum==0" :max="99" :value="msgNum">

                                <i class="el-icon-message-solid"></i>
                            </el-badge>
                        </div>

                        <i class="el-icon-full-screen full-screen" title="全屏" @click="isFullScreen=!isFullScreen"></i>
                    </div>

                </div>
                <div class="nct-tab">
                    <div class="left">
                        <el-tabs :value="currentPath" editable @tab-remove="removeTab" @tab-click="tabClick">
                            <el-tab-pane :label="tab.title" :key="tab.path" :name="tab.path" v-for="tab in tabs"></el-tab-pane>

                        </el-tabs>

                    </div>

                    <div class="right">

                        <el-dropdown trigger="click" @command="handleTabDropdown">
                            <span class="close-tab"><i class="el-icon-arrow-down el-icon--right"></i></span>
                            <el-dropdown-menu slot="dropdown">
                                <el-dropdown-item :command="1">刷新当前页</el-dropdown-item>
                                <el-dropdown-item :command="2">关闭所有</el-dropdown-item>
                                <el-dropdown-item :command="3">关闭其他</el-dropdown-item>
                            </el-dropdown-menu>
                        </el-dropdown>

                    </div>

                </div>

            </el-header>
            <el-main class="nct-body">
                <transition name="fade" mode="out-in">
                    <keep-alive :include="cachedPages">
                        <router-view :key="$route.fullPath"></router-view>
                    </keep-alive>
                </transition>
            </el-main>
            <UserProfile ref="profileForm"> </UserProfile>
            <ModifyPwd ref="modifyPwdForm"> </ModifyPwd>
        </el-container>
    </el-container>




</template>

<script>
   
    import UserProfile from './User/Profile.vue';
    import ModifyPwd from './User/ModifyPwd.vue';
    import { mapState } from 'vuex';
    import defImg from '../assets/img/user.png';
    import consts from '@/utils/consts';
    import msgEvent from '@/utils/msgEvent.js';

    const indexPath = consts.indexPath;

    export default {
        data() {
            return {
                appName: window.appName,
                isCollapse: false,
                menus: [],
                menuData: [],
                isFullScreen: false,
                msgNum: 0

            }
        },
        watch: {
            $route(to, from) {
                if (to.path == indexPath) return;
                let page = { name: to.name, path: to.fullPath };
                this.$store.commit('addVisitedPage', page);

            },

            isFullScreen(value) {
                if (value)
                    this.requestFullScreen();
                else
                    this.exitFullScreen();
            }

        },
        computed: {
            ...mapState(['user', 'visitedPages', 'cachedPages']),
            currentPath() {

                return this.$route.fullPath;
            },
            tabs() {

                var tabs = [];

                for (var page of this.visitedPages) {

                    var menu = this.menuData.find(m => m.Url && m.Url.toLowerCase() == page.path.toLowerCase());
                    if (menu) {
                        var tab = { path: menu.Url, title: menu.Name, name: page.name }
                        tabs.push(tab);

                    } else {
                        if (page.title)
                            tabs.push(page);

                    }
                }
                tabs.splice(0, 0, { path: indexPath, title: '首页', name: 'main' });
                return tabs;


            },
            userPic: function () {
                var user = this.user || {}
                var pic = user.Pic ? this.$appUrl + user.Pic : defImg;
                return pic;

            },

            getStyle() {
                if (this.isCollapse)
                    return { "width": "64px" }

                return { "width": "220px" }

            },
            displayName() {

                var user = this.user || {};
                if (user) {
                    return user.RealName || user.UserName;
                }
                return '匿名';

            }


        },
        methods: {

            handleTabDropdown(cmd) {

                if (cmd == 1)
                    this.refreshTab();
                else if (cmd == 2)
                    this.closeTab(true);
                if (cmd == 3)
                    this.closeTab(false);

            },

            async getUnReadMsgNum() {
                var res = await $messageApi.getMyCount(0);
                this.msgNum = res.Data;


            },

            refreshTab() {
                this.$refreshPage(this.currentPath);

            },

            requestFullScreen: function () {
                var de = document.documentElement;
                if (de.requestFullscreen) {
                    de.requestFullscreen();
                } else if (de.mozRequestFullScreen) {
                    de.mozRequestFullScreen();
                } else if (de.webkitRequestFullScreen) {
                    de.webkitRequestFullScreen();
                }
            },
            exitFullScreen: function () {
                var de = document;
                if (de.exitFullscreen) {
                    de.exitFullscreen();
                } else if (de.mozCancelFullScreen) {
                    de.mozCancelFullScreen();
                } else if (de.webkitCancelFullScreen) {
                    de.webkitCancelFullScreen();
                }
            },
            handleUserDropdown(cmd) {
                if (cmd == 'profile') {
                    this.showProfileForm();
                }
                else if (cmd == 'pwd') {
                    this.showModifyPwdForm();
                }
                if (cmd == 'logout') {
                    this.logout();
                }

            },

            showProfileForm() {

                this.$refs.profileForm.show();

            },
            showModifyPwdForm() {

                this.$refs.modifyPwdForm.show();

            },

            async removeTab(path) {

                var page = this.visitedPages.find(m => m.path == path);
                if (!page) return false;
                if (page.callback) {
                    
                    var result = await page.callback();
                    if (!result) return false;

                }

                this.$closePage(path);
                return true;

            },

            tabClick(tab) {

                var path = tab._props.name;
                this.handleMenuSelect(path);

            },
            openMsgPage() {
                this.$openPage('/msg', "我的消息")

            },

            closeTab(all) {

                var pages = this.visitedPages;
                if (!all) {
                    pages = pages.filter(m => m.path != this.currentPath);
                } else {
                    pages = [];
                    this.handleMenuSelect(indexPath);

                }
                this.$store.commit('resetVisitedPages', pages);

            },

            handleMenuSelect(path) {
                this.$router.replace(path);

            },

            hideProfile(data) {
                this.showProfile = false;
                if (data) this.$store.commit('setUser', data);

            },

            logout: function () {
                var _this = this;
                this.$confirm('确认退出吗?', '提示', {

                }).then(() => {
                    _this.$store.commit('logout');
                    _this.$router.push('/login');
                });


            },

            getMenus: function () {

                $menuApi.getCurrentUserMenus().then((res) => {
                    var data = res.Data;
                    this.menus = data;

                    var menuData = [];

                    for (var item of data) {

                        menuData = [...menuData, item, ...item.Children || []]

                    }

                    this.menuData = menuData;



                });


            }



        },

        mounted() {
            window.document.title = this.appName;
            this.$store.commit('restoreVisitedPages');
            this.getMenus();

            let { name, fullPath } = this.$route;
            this.$store.commit('addVisitedPage', { name: name, path: fullPath });
            this.$eventHub.$on('msg-change', this.getUnReadMsgNum);

            msgEvent.connect();

            var self = this;

            this.$event.on("notify", function (data) {

                if (data.Type != 'msg') return;

                self.getUnReadMsgNum();
                data = data.Data;
                if (data && data.Title) {

                    self.$notify({
                        title: data.Title,
                        message: data.Content,
                        duration: 0
                    });

                }

            });

            this.getUnReadMsgNum();

        },

        components: { UserProfile, ModifyPwd }


    }

</script>
<style scoped>

    /deep/ .msg .el-badge__content {
        font-size: 10px;
        height: 16px;
        line-height: 16px;
        padding: 0 5px;
    }


    .msg {
        cursor: pointer;
        font-size: 20px;
        margin-right: 15px;
        line-height: normal;
        height: 100%;
        display: flex;
        align-items: center;
    }

    .full-screen {
        cursor: pointer;
        font-size: 20px;
        margin-right: 15px;
    }

    * {
        box-sizing: border-box;
    }

    .nct-menu {
        max-height: calc(100vh - 50px);
        padding-bottom: 2px;
    }

    .el-aside {
        overflow: hidden;
    }

    .el-header {
        padding: 0;
    }

    .nct-header-left {
        width: 50px;
        display: flex;
        align-items: center;
    }

    .nct-header {
        background: rgb(64, 158, 255);
        color: #fff;
        line-height: 50px;
        height: 50px;
        display: flex;
        padding-left: 15px;
        padding-right: 15px;
    }


    .nct-header-right {
        display: flex;
        align-items: center;
        flex-direction: row-reverse;
        flex: 1;
        height: 50px;
        line-height: 50px;
    }


    .nct-logo-title {
        height: 50px;
        font-size: 20px;
        padding-left: 20px;
        background-color: rgb(62, 132, 208);
        line-height: 50px;
        overflow: hidden;
        color: #fff;
        width: 100%;
    }



    .nct-body {
        padding: 0px;
        background-color: #f0f2f5;
    }



    .nct-container {
        height: calc(100vh);
    }



    .el-aside {
        background-color: #001529;
        height: 100%;
        transition: .3s width ease-in-out;
    }

    .nct-logo-title.collapse {
        max-width: 64px;
        min-width: 64px;
    }

    .head-pic {
        width: 40px;
        height: 40px;
        border-radius: 50%;
        margin-right: 10px;
    }

    .el-dropdown-link {
        cursor: pointer;
        color: #fff;
    }

    .el-menu {
        border: 0px;
    }

        .el-menu:not(.el-menu--collapse) {
            width: 220px;
        }

    .toggle-menu-btn {
        font-size: 22px;
        cursor: pointer;
    }

    .close-tab {
        cursor: pointer;
    }

    .fade-enter-active, .fade-leave-active {
        transition: opacity .25s ease;
    }

    .fade-enter, .fade-leave-active {
        opacity: 0;
    }



    .slide-left-enter, .slide-right-leave-active {
        opacity: 0;
        -webkit-transform: translate(30px, 0);
        transform: translate(30px, 0);
    }

    .slide-left-leave-active, .slide-right-enter {
        opacity: 0;
        -webkit-transform: translate(-30px, 0);
        transform: translate(-30px, 0);
    }
</style>

