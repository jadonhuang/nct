import util from '@/utils/util';
import dialog from '@/components/Dialog/Dialog.vue';
import Vue from 'vue';

const buildInstance = function (options, parent) {

    var innerComponent = options.innerComponent;
    var url = options.url;
    if (!url && !innerComponent) return null;


    var root = parent.$root;
    var router = root.$router;
    var store = root.$store;

    var componentName;

    if (!innerComponent) {
        var path = url;

        var markIndex = url.indexOf('?');
        if (markIndex > 0) {

            path = url.substr(0, markIndex);

        }
        path = path.toLowerCase();
        const routes = router.options.routes;

        function getComponent(route) {
            var routePath = route.path.toLowerCase();
            if (routePath == path || routePath + '/' == path) {
                componentName = route.name;
                return route.component;

            }
            return null;

        }

        for (var route of routes) {

            innerComponent = getComponent(route);
            if (innerComponent) {
                break;
            }
            if (route.children) {
                for (var subRoute of route.children) {
                    innerComponent = getComponent(subRoute);
                    if (innerComponent) {
                        break;
                    }

                }
            }
            if (innerComponent) break;
        }

        if (!innerComponent) {

            console.error(`无法打开${url}`);
            return null;
        }

        var _componentName = innerComponent.name || innerComponent.componentName;
        if (_componentName) componentName = _componentName;



    } else {

        componentName = innerComponent.name || innerComponent.$options.componentName;

    }

    if (!Vue.options.components[componentName]) {

        Vue.component(componentName, innerComponent);
        
    }


    const componentsObject = {
        el: document.createElement('div'),
        root: root,
        parent: parent,
        router: router,
        store: store,
        data() {
            return {
                isShow: false,
                componentName: componentName,
                query: util.getUrlQuery(url),
                queryBak: {},
                options: options,
                callback: null,
                retValue: null

            }
        },

    }

    const Constructor = Vue.extend(dialog);
    const instance = new Constructor(
        componentsObject
    );

    return instance;

};



export default {

    show(options, parent) {

        let instance = buildInstance(options, parent);
        if (!instance) return;
        instance.isShow = true;

    },

    showWithValue(options, parent) {

        let instance = buildInstance(options, parent);
        if (!instance) return;
        return new Promise((resolve, reject) => {
            instance.callback = function (data) {
                resolve(data)
            };
            instance.isShow = true;
        });



    }

}
