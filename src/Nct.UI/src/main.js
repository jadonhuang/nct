import Vue from 'vue'
import App from './App'
import router from './router'
import ElementUI from 'element-ui'
import axios from 'axios';
import store from './store/';
import 'element-ui/lib/theme-chalk/index.css';
import './assets/css/base.css';
import './assets/css/font/css/font-awesome.min.css';
import util from './utils/util';
import { getStore } from '@/utils/store'
import consts from '@/utils/consts'
import Dialog from '@/components/Dialog/dialog'
import MonacoHelper from '@/utils/monacoHelper'
import msgEvent from '@/utils/msgEvent'
import directive from '@/components/Dialog/directive'
import { appUrl } from '@/api';
import PermissionCheck from '@/utils/permissionCheck'
import request from './api/request';
import UserSelect from './components/UserSelect';


window.$apiPost = request.post;
window.$apiGet = request.get;

window.Vue = Vue;
msgEvent.init();
MonacoHelper.configEditor();

Vue.prototype.$event = window.$msgEvent;
Vue.prototype.$consts = consts;
Vue.prototype.$appUrl = appUrl;
Vue.prototype.$publicPath = process.env.BASE_URL;

Vue.prototype.$showUserSelect = UserSelect.show;


window.appName = window.appName || '后台管理系统';

const requireComponent = require.context('./components', true, /.vue$/)

requireComponent.keys().forEach((item) => {

    const defaultObj = requireComponent(item).default;
    var name = defaultObj.name;
    if (!name) {
        var index = item.lastIndexOf('/') + 1;
        name = item.substring(index);
        index = name.indexOf('.');
        name = name.substring(0, index);
    }

    Vue.component(name, defaultObj)

})



Vue.config.productionTip = false
Vue.use(ElementUI, {
    size: 'medium' 
})
Vue.use(directive)
Vue.prototype.$okTip = function (msg) {
    Vue.prototype.$message({
        message: msg,
        type: 'success'
    })

}

Vue.prototype.$errTip = function (msg) {
    Vue.prototype.$message({
        message: msg,
        type: 'error'
    })

}

for (var key in util) {
    Vue.prototype[`$${key}`] = util[key];

}

Vue.prototype.$canDo = function (permissions) {
    return PermissionCheck.canDo(permissions);
}
Vue.prototype.$hasRole = function (roles) {
    return PermissionCheck.hasRole(roles);
}


  
Vue.prototype.$showDialog =  function (options) {
   
    Dialog.show(options, this);

}

Vue.prototype.$showDialogWithValue = async function (options) {

    return await Dialog.showWithValue(options, this);

}

Vue.prototype.$closeDialog =  function (value) {

   this.dispatch('Dialog', 'dialog.close',value);

}

Vue.prototype.$eventHub = Vue.prototype.$eventHub || new Vue();


Vue.prototype.$setTabCloseCallback = async function (callback, path) {
    path = path || this.$route.fullPath
    setTimeout(function () {
        store.commit('setTabCloseCallback', { path, callback });
    },1000)
  

},



Vue.prototype.$setDialogCloseCallback =  function (callback,componentObj) {

    var parent = componentObj.$parent;
    var name = parent.$options.componentName;

    while (parent && (!name || name !== 'Dialog')) {
        parent = parent.$parent;
        if (parent) {
            name = parent.$options.componentName;
        }
    }
    if (parent) {

        parent.setCloseCallback(callback)
    }


}



Vue.prototype.$confirmTip = async function (tip) {
    try {
        await this.$confirm(tip, '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        return true;
    } catch (e) {

        return false;
    }

},





Vue.prototype.$showLoading = function (text) {
    const loading = this.$loading({
        text: text || '处理中...',
        spinner: 'el-icon-loading',
        background: 'rgba(255, 255, 255, 0.5)',
        customClass: 'loading'
    });
    window.$$loading = loading
    return loading;

}

Vue.prototype.$hideLoading = function (text) {
   
    var loading = window.$$loading;
    if (loading) {
        loading.close();
    }


}


Vue.prototype.$openPage = function (path,title) {

    store.commit('addVisitedPage', { path, title });
    router.replace(path)

}

Vue.prototype.$closePage = function (path) {

    path = path || this.$route.fullPath;
    store.commit('removeVisitedPage', path);
    const pages = store.state.visitedPages;
    path = pages.length > 0 ? pages[pages.length-1].path : consts.indexPath;
    router.replace(path)

}

Vue.prototype.$refreshPage = function (path) {

    store.commit('removePageCache', path);
    path = '/redirect?path=' + util.urlEncode(path);
    router.replace(path)
}


axios.defaults.headers.common['X-Requested-With'] = `XMLHttpRequest`;

axios.interceptors.response.use(function (response) {
    var status = response.status;
    var data;
    if (status == 200) {
        data = response.data;
        if (data && data.StatusCode) {
            status = data.StatusCode;

        }

    }
    if (status == 401) { window.location = "/login" }
    return response.data;

}, function (error) {
    var msg = error.message;
    if (error.response.status == 403)
        msg = "无权限操作"

    var result = { IsSuccess: false, Msg: msg, StatusCode: 500 }
    return result;
});



const createVue = function () {
    new Vue({
        router,
        store,
        render: h => h(App),
    }).$mount('#app')

}

const initApp =  function () {

    const token = getStore('token');
    if (token) {
        store.commit('setToken', token);
        store.dispatch('getUserInfo', {
            callback() {
                createVue();

            }
        })

    } else {

        createVue();

    }
}

initApp();




