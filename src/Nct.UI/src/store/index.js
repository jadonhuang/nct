import Vue from 'vue';
import Vuex from 'vuex';
import axios from 'axios';
import { setStore, getStore, removeStore } from '../utils/store'
import consts from '@/utils/consts'


Vue.use(Vuex);

const visitedPageCacheKey = 'visitedpages';

function saveVisitedPagesToStore(pages) {

    setStore({ name: visitedPageCacheKey, data: pages, type: 'page' });

}


export default new Vuex.Store({
    state: {
        user: null,
        token: null,
        visitedPages: [],
        cachedPages:[],


    },
    //getters: {
    //    user: state => state.user,
    //    token: state => state.token,

    //},

    mutations: {


        addPageCache(state, path) {
            if (state.cachedPages.indexOf(path) == -1)
                state.cachedPages.push(path);

        },
        removePageCache(state, path) {
            var pages = state.visitedPages;
            let page = pages.find(m => m.path == path);
            if (page) {

                state.cachedPages = state.cachedPages.filter(m => m != page.name);
            }
        

        },
      
        setUser(state, user) {
            state.user = user;
        },

        setToken(state, token) {
            if (!token) return;
            state.token = token;
            setStore({ name: consts.token, data: token, type: 't' })
            axios.defaults.headers.common['Authorization'] = `${token.TokenType} ${token.AccessToken}`;
        },
        logout(state) {
            state.token = null;
            removeStore(consts.token);
        },

        resetVisitedPages(state, pages) {

            state.visitedPages = pages;
            state.cachedPages = pages.map(m=>m.name);
            saveVisitedPagesToStore(pages);

        },
    
        restoreVisitedPages(state) {
            var pages = getStore(visitedPageCacheKey) || [];
            state.visitedPages = pages;
        },

        setTabCloseCallback(state, page) {
            var pages = state.visitedPages;
            var existPage = pages.find(m => m.path == page.path);
            if (existPage) {
                existPage.callback =page.callback;

            }
        },

        addVisitedPage(state, page) {
            var pages = state.visitedPages;
            var existPage = pages.find(m => m.path == page.path);
            if (!existPage) {
                pages.push(page)
                state.visitedPages = pages;
            } else {

                Object.assign(existPage, page);
            }
            saveVisitedPagesToStore(pages);

        },

        removeVisitedPage(state, path) {

            var pages = state.visitedPages;
            let index = pages.findIndex(m => m.path == path);
            let removeName = pages[index].name;    
            pages.splice(index, 1);
            state.cachedPages = state.cachedPages.filter(m => m != removeName);
            saveVisitedPagesToStore(pages);

        }
     


    },


    actions: {
        getUserInfo(context, payload) {
            $userApi.getCurrentUser().then(res => {

                if (res.IsSuccess) {
                    context.commit('setUser', res.Data);                 
                }
                if (payload && payload.callback) payload.callback()
            });



        },

    }

});
