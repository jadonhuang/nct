import request from './request';



export const getCheckCode = () => { return request.get('/util/GetCheckCodeImg'); };


