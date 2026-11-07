import { isNull } from './validator';

const keyPrefix = 'nct-';

export function setStore(params = {}) {

    let {
      name,
      data,
      type,
    } = params;
    name = keyPrefix + name
    let obj = {
      dataType: typeof (data),
      data: data,
      type: type,
      datetime: new Date().getTime()
    }

    if (type) window.sessionStorage.setItem(name, JSON.stringify(obj));
    else window.localStorage.setItem(name, JSON.stringify(obj));

}

export function getStore(name) {

    name = keyPrefix + name
    let obj = window.sessionStorage.getItem(name);
    if (isNull(obj)) obj = window.localStorage.getItem(name);
    if (isNull(obj)) return;
    try {
      obj = JSON.parse(obj);
    } catch(e) {
      return obj;
    }
  
  var data;
    if (obj.dataType == 'string') {
      data = obj.data;
    } else if (obj.dataType == 'number') {
      data = Number(obj.data);
    } else if (obj.dataType == 'boolean') {
      data = eval(obj.data);
    } else if (obj.dataType == 'object') {
      data = obj.data;
    }
  return data;
}


export function removeStore(name) {

  name = keyPrefix + name
  localStorage.removeItem(name);
  sessionStorage.removeItem(name);
 
 
}





