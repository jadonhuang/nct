import store from '@/store/';
import { appUrl } from '@/api';
export default {

    init() {
        window.$msgEvent = (function () {

            var _callbacks = {};
            var on = function (eventName, callback) {
                if (!_callbacks[eventName]) {
                    _callbacks[eventName] = [];
                }

                _callbacks[eventName].push(callback);
            };

            var off = function (eventName, callback) {
                var callbacks = _callbacks[eventName];
                if (!callbacks) {
                    return;
                }

                var index = -1;
                for (var i = 0; i < callbacks.length; i++) {
                    if (callbacks[i] === callback) {
                        index = i;
                        break;
                    }
                }

                if (index < 0) {
                    return;
                }

                _callbacks[eventName].splice(index, 1);
            };

            var trigger = function (eventName) {
                var callbacks = _callbacks[eventName];
                if (!callbacks || !callbacks.length) {
                    return;
                }

                var args = Array.prototype.slice.call(arguments, 1);
                for (var i = 0; i < callbacks.length; i++) {

                    callbacks[i].apply(this, args);
                }
            };

            return {
                on: on,
                off: off,
                trigger: trigger
            };
        })();

    },

    connect() {
       
        Object.defineProperty(window.WebSocket, 'OPEN', { value: 1, });

        var token = store.state.token;
        if (!token) return;

        var hubUrl = appUrl + "/signalr?token=" + encodeURIComponent(token.AccessToken);

        var connection = new signalR.HubConnectionBuilder().withUrl(hubUrl).build();

        connection.start().then(function () {
            console.log("signalr started");
            window.$msgEvent.trigger("connected");
        }).catch(function (err) {
            return console.error(err.toString());
        });


        connection.on("getNotification", function (data) {

            window.$msgEvent.trigger("notify", data);

        });




    }






}