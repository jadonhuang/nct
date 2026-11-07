
//const MonacoWebpackPlugin = require('monaco-editor-webpack-plugin')

const MonacoWebpackPlugin = require('monaco-editor-esm-webpack-plugin')

const publicPath = process.env.NODE_ENV == "development" ? '/' : '/admin';

module.exports = {

    publicPath: publicPath,
    configureWebpack: {
        module: {
            rules: [{
                test: /\.js/,
                enforce: 'pre',
                include: /node_modules[\\\/]monaco-editor[\\\/]esm/,
                use: MonacoWebpackPlugin.loader
            }]
        },
        plugins: [
            new MonacoWebpackPlugin({
                languages: ['javascript', 'typescript','csharp','sql']
            })
        ]
    },
  
}
