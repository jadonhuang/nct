const devUrl = 'http://localhost:3838';

let _appUrl = window.baseUrl === undefined ? devUrl : window.baseUrl;

export const appUrl = _appUrl;


