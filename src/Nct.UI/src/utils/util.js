const removeEmptyChildren= function (items, childKey) {
    childKey = childKey || 'Children';
    for (var item of items) {
        if (item[childKey]) {
            if (item[childKey].length == 0) {

                delete item[childKey];

            } else {

                removeEmptyChildren(item[childKey])

            }

         }

    }
};



export default {


    formatInterval(interval) {
        if (interval < 1000)
            return interval + 'ms';

        return (interval / 1000) + 's';

    },

    emptyObject(obj) {

        if (typeof obj !== 'object')
            return;

        for (var key in obj) {
            var value = obj[key];

            if (Array.isArray(value)) {
                obj[key] = [];

            } else {

                obj[key] = null;
            }

        }


    },
    getValueByPath(object, prop) {
        prop = prop || '';
        const paths = prop.split('.');
        let current = object;
        let result = null;
        for (let i = 0, j = paths.length; i < j; i++) {
            const path = paths[i];
            if (!current) break;

            if (i === j - 1) {
                result = current[path];
                break;
            }
            current = current[path];
        }
        return result;
    },
    getPropByPath(obj, path, strict) {
        let tempObj = obj;
        path = path.replace(/\[(\w+)\]/g, '.$1');
        path = path.replace(/^\./, '');

        let keyArr = path.split('.');
        let i = 0;
        for (let len = keyArr.length; i < len - 1; ++i) {
            if (!tempObj && !strict) break;
            let key = keyArr[i];
            if (key in tempObj) {
                tempObj = tempObj[key];
            } else {
                if (strict) {
                    throw new Error('please transfer a valid prop path to form item!');
                }
                break;
            }
        }
        return {
            o: tempObj,
            k: keyArr[i],
            v: tempObj ? tempObj[keyArr[i]] : null
        };
    },

    newGuid() {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
            if ((i == 8) || (i == 12) || (i == 16) || (i == 20)) guid += "-";
        }
        return guid;
    },

    newId(len) {
        var id = "";
        len = len || 5;
        for (var i = 1; i <= len; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            id += n;
        }

        var num = Math.floor(Math.random() * 26);
        var letter = String.fromCharCode(97 + num);
        return letter +id;
    },

    formatDate(v, format) {
        if (!v) return "";
        var d = v;
        if (typeof v === 'string') {
            if (v.indexOf("/Date(") > -1)
                d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
            else
                d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));
        }
        var o = {
            "M+": d.getMonth() + 1,  //month
            "d+": d.getDate(),       //day
            "h+": d.getHours(),      //hour
            "m+": d.getMinutes(),    //minute
            "s+": d.getSeconds(),    //second
            "q+": Math.floor((d.getMonth() + 3) / 3),  //quarter
            "S": d.getMilliseconds() //millisecond
        };
        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
        }
        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    },

    isValidId(str) {

        return /^[a-zA-Z]{1}\w*$/.test(str);

    },

    copyObj(obj) {

        return JSON.parse(JSON.stringify(obj));
    },


    isNull(value) {
        return value === null || value === undefined;
    },

    isNotNull(value) {
        return !(value === null || value === undefined);
    },

    isEmptyStr(str) {

        return (str === undefined) || (!str && str !== 0 && str !== '0' && str !== false) || (!/[^\s]/.test(str));
    },
    urlEncode(str) {

        return encodeURIComponent(str);
    },
    urlDecode(str) {

        return decodeURIComponent(str);
    },
    buildAbsUrl(baseUrl, url) {

        if (url.startsWith('http:') || url.startsWith('https:'))
            return url;

        if (!baseUrl.endsWith('/')) {

            baseUrl = baseUrl + '/';
        }

        if (url.startsWith('/')) {

            url = url.substring(1);
        }

        return baseUrl + url;
    },
    getUrlQuery(url) {
        var markIndex = url.indexOf('?');
        if (markIndex == -1) return {};
        let str = url.substr(markIndex + 1)
        let obj = {}
        const arr = str.split('&')
        for (let i = 0; i < arr.length; i++) {
            let params = arr[i].split('=')
            obj[params[0]] = decodeURIComponent(params[1])
        }
        return obj
    },
    loadScript(url) {
        var script = document.createElement('script');
        script.type = "text/javascript";
        script.src = url;
        document.body.appendChild(script);  
    },

    async loadScriptAsync(jsFile) {

        return new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = jsFile;
            script.onload = () => {
                resolve();
            };
            script.onerror = () => {
                const message = `${jsFile} º”‘ÿ ß∞‹`
                const err = new Error(message);
                reject(err);
            };
            document.body.appendChild(script);
        });
    },


    removeEmptyChildren:removeEmptyChildren,
  




}
