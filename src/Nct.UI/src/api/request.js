import axios from 'axios';
import { appUrl } from '@/api';


function correctUrl(url) {

    if (!url.startsWith('/')) url = '/' + url;

    if (url.toLowerCase().indexOf('/api/') ==-1)
        url = '/api' + url

    return url;

}

export default {
    get: function (url, params) {
        url = correctUrl(url);
        return axios.get(`${appUrl}${url}`, { params: params }); 

  },
  post: function (url,params) {
      url = correctUrl(url);
      return axios.post(`${appUrl}${url}`,params); 
  }


}
