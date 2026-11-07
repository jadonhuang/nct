
import store from '@/store/';

export default {

    isAdmin(userName) {

        return userName&&userName.toLowerCase() == 'admin';

    },
    isLogin() {

        var token = store.state.token;
        return !!token;

    },
    hasRole(roles) {

        var user = store.state.user;

        if (roles && user) {

            if (this.isAdmin(user.UserName)) return true;

            var userRoles = user.Roles;
            if (userRoles.length == 0)
                return false;

            if (!Array.isArray(roles)) {
                roles = roles.split(',');
            }

            var has = userRoles.some(m => roles.some(n => n.toLowerCase() == m.toLowerCase()));

            return has;

        }


        return false;

    },
    
    canDo(permissions) {

        var user = store.state.user;
        if (permissions && user) {

            if (this.isAdmin(user.UserName)) return true;

            var permissionList = user.Permissions;

            var permssionArray = permissions.toLowerCase().split(',');

            var index = permissionList.findIndex(function (d) { return d.Code && permssionArray.indexOf(d.Code.toLowerCase()) != -1 });

            return index != -1;

        }


        return false;

    },



}