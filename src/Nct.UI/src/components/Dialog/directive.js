
import elDragDialog from './drag.js'



export default {
    install(Vue) {
        Vue.directive('draggable', elDragDialog)
    },
};