import Vue from 'vue'
import Router from 'vue-router'
import Login from '@/pages/Login'
import NotFound from '@/pages/404'
import Forbidden from '@/pages/403'
import Home from '@/pages/Home'
import Main from '@/pages/Main'
import Redirect from '@/pages/Redirect'

import PermissionCheck from '@/utils/permissionCheck'
import consts from '@/utils/consts'
import store from '../store';
Vue.use(Router)

const basePath = window.basePath || consts.basePath;
const indexPath = consts.indexPath;
const requireComponent = require.context('../pages', true, /\w+.vue$/)
var dynamicRoutes = [];

requireComponent.keys().forEach((item) => {

    const defaultObj = requireComponent(item).default;
    var path = defaultObj.path;
    var permissions = defaultObj.permissions;
    var routName = defaultObj.name;
    var meta = { permissions };
    var parts = item.split('/');
    if (parts.length == 3) {

        var route;
        var routePath;
        var dir = parts[1].toLowerCase();
        var name = parts[2].substr(0, parts[2].indexOf('.')).toLowerCase();
        if (!routName) {
            routName = dir + name;
        }
        if (name == 'list' && !path) {
            routePath = `/${dir}`
        } else {
            routePath = path || `/${dir}/${name}`
        }
        route = { path: routePath, component: defaultObj, name: routName, meta }
        dynamicRoutes.push(route)


    }

})

var router = new Router({
    mode: 'history',
    base: basePath,
    routes: [

        {
            path: '/login',
            component: Login,
            name: 'login',
            meta: {
                requireAuth: false,
            },

        },
        {
            path: '/404',
            name:'notfound',
            component: NotFound,
            meta: {
                requireAuth: false,
            },
        },
        {
            path: '/403',
            name: 'forbidden',
            component: Forbidden,
            meta: {
                requireAuth: false,
            },
        },
        {
            path: '/redirect',
            component: Redirect,
            name:'redirect',
            meta: {
                requireAuth: false,
            },
        },
        {
            path: '/',
            name: 'home',
            component: Home,
            redirect: indexPath,
            children: [
                { path: indexPath, component: Main, name: 'mainpage' },
                ...dynamicRoutes

            ]
        },
        {
            path: '*',
            redirect: { path: '/404' }
        }
    ]
});


router.beforeEach((to, from, next) => {

    if (to.path.toLowerCase() == '/login') {
        store.commit('logout');
    }
    if (!to.matched.every((ele) => { return ele.meta && ele.meta.requireAuth === false })) {
        if (store.state.token) {
            if (to.meta && to.meta.permissions && !PermissionCheck.canDo(to.meta.permissions)) {
                next({
                    path: '/403',
                })

            } else {
                next();
            }

        } else {
            next({
                path: '/login',
            })
        }
    } else {
        next();
    }

})



router.afterEach((to, from, next) => {

    if (to.query.noCache != 'Y') {

      //  store.commit('addPageCache', to.name);
    }

})

export default router

