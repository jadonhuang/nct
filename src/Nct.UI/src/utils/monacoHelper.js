import { setLocaleData } from "monaco-editor-nls"
import zh_CN from "monaco-editor-nls/locale/zh-hans"
setLocaleData(zh_CN)

//import * as monaco from 'monaco-editor'

const monaco = require("monaco-editor/esm/vs/editor/editor.api"); 

export default {
    getSqlCompletionProvider(monaco) {
        return {
            provideCompletionItems: function (model, position) {
                var container = {};
                const completionItems = [

                    {
                        label: 'nowhere',
                        insertText: '/*#nowhere#*/',
                        kind: monaco.languages.CompletionItemKind.Issue,
                        detail: '不加条件',
                    },
                  
                    {
                        label: 'join',
                        insertText: 'join',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '关键字',
                    },
                    {
                        label: 'where',
                        insertText: 'where',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '条件',
                    },
                    {
                        label: 'order by',
                        insertText: 'order by',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '排序',
                    },
                    {
                        label: 'group by',
                        insertText: 'group by',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '分组',
                    },

                    {
                        label: 'having',
                        insertText: 'having',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '分组条件',
                    },
                    {
                        label: 'exists',
                        insertText: 'exists',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '是否存在',
                    },
                    {
                        label: 'left join',
                        insertText: 'left join',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '关键字',
                    },

                    {
                        label: 'select',
                        insertText: 'select',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '关键字',
                    },
                    {
                        label: 'from',
                        insertText: 'from',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '关键字',

                    },
                    {
                        label: 'day',
                        insertText: 'day',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '关键字',

                    },
                    {
                        label: 'month',
                        insertText: 'month',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '关键字',

                    },
                    {
                        label: 'year',
                        insertText: 'year',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '关键字',

                    },


                    {
                        label: 'declare',
                        insertText: 'declare',
                        kind: monaco.languages.CompletionItemKind.Keyword,
                        detail: '定义变量',

                    },
                    {
                        label: 'uid',
                        insertText: "@_uid",
                        kind: monaco.languages.CompletionItemKind.Value,
                        detail: '当前用户Id',

                    },
                    {
                        label: 'getCols',
                        insertText: "@_getCols",
                        kind: monaco.languages.CompletionItemKind.Value,
                        detail: '是否返列',

                    },

                    {
                        label: 'getdate()',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'getdate()',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '获取当前时间'
                    }
                    , {
                        label: 'CHARINDEX(str,str2) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'CHARINDEX(${1:str},${2:str2})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '字符串位置'
                    }
                    , {
                        label: 'YEAR(date) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'YEAR(${1:date})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '返回时间年份'
                    }
                    , {
                        label: 'MONTH(date) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'MONTH(${1:date})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '返回时间月份'
                    }
                    , {
                        label: 'DAY(date) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'DAY(${1:date})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '返回时间天'
                    }

                    , {
                        label: 'SUBSTRING(str,startindex int,length int) return str',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'SUBSTRING(${1:str},${2:startindex},${3:length})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '截取字符串'
                    }
                    , {
                        label: 'SUM(expression) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'SUM(${1:expression})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '返回数值列的总数'
                    }
                    , {
                        label: 'COUNT(expression) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'COUNT(${1:expression})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '函数返回表中的记录数'
                    }
                    , {
                        label: 'MAX(expression) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'MAX(${1:expression})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '返回最大值'
                    }
                    , {
                        label: 'MIN(expression) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'MAX(${1:expression})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '返回最小值'
                    }
                    , {
                        label: 'DATEDIFF(interval,startdate,enddate) return int',
                        kind: monaco.languages.CompletionItemKind.Function,
                        insertText: 'DATEDIFF(${1:interval},${2:startdate},${3:enddate})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '返回时间差'
                    }

                ];

                container.suggestions = completionItems
                return container;
            }
        };
    },

    getCSharpCompletionProvider(monaco) {
        return {
            provideCompletionItems: function (model, position) {

                var keywords = [
                    'extern',
                    'alias',
                    'using',
                    'bool',
                    'decimal',
                    'sbyte',
                    'byte',
                    'short',
                    'ushort',
                    'int',
                    'uint',
                    'long',
                    'ulong',
                    'char',
                    'float',
                    'double',
                    'object',
                    'dynamic',
                    'string',
                    'assembly',
                    'is',
                    'as',
                    'ref',
                    'out',
                    'this',
                    'base',
                    'new',
                    'typeof',
                    'void',
                    'checked',
                    'unchecked',
                    'default',
                    'delegate',
                    'var',
                    'const',
                    'if',
                    'else',
                    'switch',
                    'case',
                    'while',
                    'do',
                    'for',
                    'foreach',
                    'in',
                    'break',
                    'continue',
                    'goto',
                    'return',
                    'throw',
                    'try',
                    'catch',
                    'finally',
                    'lock',
                    'yield',
                    'from',
                    'let',
                    'where',
                    'join',
                    'on',
                    'equals',
                    'into',
                    'orderby',
                    'ascending',
                    'descending',
                    'select',
                    'group',
                    'by',
                    'namespace',
                    'partial',
                    'class',
                    'field',
                    'event',
                    'method',
                    'param',
                    'public',
                    'protected',
                    'internal',
                    'private',
                    'abstract',
                    'sealed',
                    'static',
                    'struct',
                    'readonly',
                    'volatile',
                    'virtual',
                    'override',
                    'params',
                    'get',
                    'set',
                    'add',
                    'remove',
                    'operator',
                    'true',
                    'false',
                    'implicit',
                    'explicit',
                    'interface',
                    'enum',
                    'null',
                    'async',
                    'await',
                    'fixed',
                    'sizeof',
                    'stackalloc',
                    'unsafe',
                    'nameof',
                    'when'
                ];


                const completionItems = [

                    {
                        label: 'BusinessException',
                        insertText: 'BusinessException',
                        kind: monaco.languages.CompletionItemKind.Class,

                    },

                    {
                        label: 'ToList',
                        insertText: 'ToList()',
                        kind: monaco.languages.CompletionItemKind.Method,
                        detail: '',
                    },
                    {
                        label: 'ToJson()',
                        insertText: 'ToJson()',
                        kind: monaco.languages.CompletionItemKind.Method,
                        detail: '',
                    },
                    {
                        label: 'FirstOrDefault',
                        insertText: 'FirstOrDefault()',
                        kind: monaco.languages.CompletionItemKind.Method,
                        detail: '',
                    },
                    {
                        label: 'DataTable',
                        insertText: 'DataTable',
                        kind: monaco.languages.CompletionItemKind.Class,
                        detail: '',
                    },
                    {
                        label: 'RuleContext',
                        insertText: 'RuleContext',
                        kind: monaco.languages.CompletionItemKind.Class,
                        detail: '规则上下文',
                    },
                    {
                        label: 'Db',
                        insertText: 'Db',
                        kind: monaco.languages.CompletionItemKind.Class,
                        detail: '数据访问',
                    },
                    {
                        label: 'ToObject<T>(string json)',
                        kind: monaco.languages.CompletionItemKind.Method,
                        insertText: 'ToObject<${1:T}>(${2:json})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '字符转对象'
                    },
                    {
                        label: 'Db.Query<T>(string sql, object param = null)',
                        kind: monaco.languages.CompletionItemKind.Method,
                        insertText: 'Query<${1:T}>(${2:json})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '字符转对象'
                    }

                    , {
                        label: 'Db.QueryTable(string sql,object params=null)',
                        kind: monaco.languages.CompletionItemKind.Method,
                        insertText: 'Db.QueryTable(${1:sql},${2:params})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '查询数据表格'
                    }

                    , {
                        label: 'Db.Execute(string sql,object params=null)',
                        kind: monaco.languages.CompletionItemKind.Method,
                        insertText: 'Db.Execute(${1:sql},${2:params})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '执行sql语句'
                    }

                    , {
                        label: 'From(string tableName)',
                        kind: monaco.languages.CompletionItemKind.Method,
                        insertText: 'From(${1:tableName})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '数据表'
                    },
                    {
                        label: 'Fields(string fields)',
                        kind: monaco.languages.CompletionItemKind.Method,
                        insertText: 'Fields(${1:fields})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: '字段'
                    },
                    {
                        label: 'Sql',
                        kind: monaco.languages.CompletionItemKind.Class,
                        insertText: 'Sql',
                        detail: 'sql查询'
                    },
                    {
                        label: 'Where(lambda)',
                        kind: monaco.languages.CompletionItemKind.Method,
                        insertText: 'Where(${1:lambda})',
                        insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
                        detail: 'linq条件',
                    },



                ];

                for (var kw of keywords) {

                    completionItems.push({
                        label: kw,
                        insertText: kw,
                        kind: monaco.languages.CompletionItemKind.Keyword

                    })

                }

                const container = {}
                container.suggestions = completionItems
                return container;
            }
        };
    },
    getJsCodeLint() {
        var codeLint = `

class _formdef {


  /**
     * 添加事件
     * @param formItemId formItemId可为null
     * @param event  值为:change,addRow,delRow
     * @param handler function(value,event)  第一个参数 value当前值state状态数据
     */
    addListener(formItemId: string, event: string, handler = function (value,state) { }) { }

  

    /**
     * 设置控件的值
     * @param formItemId
     * @param value
     * @index 指定表格行
     */
    setItemValue(formItemId: string, value, index?:number) { }

    /**
     * 设置控件的值
     * @param formItemId
     * @index 指定表格行
     */
    getItemValue(formItemId: string,index?:number) { }

 

 

    /**
     * 显示控件
     * @param formItemId
     * @index 指定表格行
     */
    showItem(formItemId: string,index?:number) { }

      /**
     * 隐藏控件
     * @param formItemId
     * @index 指定表格行
     */
   hideItemItem(formItemId: string,index?:number) { }

   
    /**
     * 禁用控件
     * @param formItemId
     * @index 指定表格行
     */
    disableItem(formItemId: string,index?:number) { }

      /**
     * 启用控件
     * @param formItemId
     * @index 指定表格行
     */
   enableItem(formItemId: string,index?:number) { }




    /**
     * 在表格控件中增加一行记录
     * @param formItemId
     * @param rowData
     * @index 指定表格行
     */
    addTableRow(formItemId: string, rowData?: object,index:number) { }

    /**
     * 从表格控件中删除一行或多行或所有行
     * @param formItemId
     * @index 指定表格行 不填则全部行
     */
    removeTableRow(formItemId: string, index?: number) { }

  
      /**
     * 设置控件数据源选项
     * @param formItemId
     * @opts 选项namevalue格式的数组
     * @index 指定表格行 不填则全部行
     */
    setItemOpts(formItemId: string,opts:array, index?: number) { }


    /**
     * 调用ajax请求
     * @url 请求地址
     * @params 请求参数
     * httpMethod get|post
     */
    ajax(url, params, httpMethod) { }
     
      /**
     * 给控件添加验证规则
     * @formItemId 控件id
     * @rule 规则，格式参考element
     */
    addValidateRule(formItemId,rule)
 

}
var FORM = new  _formdef();

`
        return codeLint;

    },

    configEditor() {
        monaco.languages.typescript.javascriptDefaults.addExtraLib(this.getJsCodeLint());
        monaco.languages.registerCompletionItemProvider('csharp', this.getCSharpCompletionProvider(monaco));
        monaco.languages.registerCompletionItemProvider('sql', this.getSqlCompletionProvider(monaco));

    },


}
