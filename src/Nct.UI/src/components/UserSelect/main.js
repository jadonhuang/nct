import userSelectVue from './Main.vue';
import Vue from 'vue';

const UserSelectConstructor = Vue.extend(userSelectVue);

let msg, instance

const defaultCallback = (data) => {
    if (msg) {
        let callback = msg.callback;
        if (typeof callback === 'function') {
            callback(data);
        }
        if (msg.resolve) {
            msg.resolve(data);

        }
    }
};

const showMsg = (options) => {
    instance = new UserSelectConstructor({
        el: document.createElement('div')
    });

    instance.callback = defaultCallback;
    instance.selectedIds = options.selectedIds || [];
    instance.title = options.title || '选择用户';
    if (typeof options.multiSelect === 'boolean') {
        instance.multiSelect = options.multiSelect;
    } else {
        instance.multiSelect = true;
    }
    
    instance.show();
  
};



const UserSelect = function (options,callback) {

    if (typeof Promise !== 'undefined') {

        return new Promise((resolve, reject) => {
            msg = { resolve, reject, callback }
            showMsg(options);
        });
    } else {

        msg = { callback }
        showMsg(options);
    }



};

UserSelect.show = function (options, callback) {

    options = options || {};
    return UserSelect(options, callback);

};


export default UserSelect;