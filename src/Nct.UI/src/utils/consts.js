

const fontCss = { "FontSize": 0, "FontWeight": "", "Color": "", "FontStyle": "", "TextDecoration": "" }

const arrayTypes = ['Array[String]', 'Array[Number]', 'Array[Object]'];

export default {
    token: 'token',
    indexPath: '/main',
    basePath: '/admin',
    diaWidth: 1200,
    diaHeight: 700,
    form: {

        propEditor: {
            "Table": "TableEditor",
            "Field": "FieldEditor",
            "Name": "NameEditor",
            "Text": "TextEditor",
            "Title": "TitleEditor",
            "TitleVisible": "TitleVisibleEditor",
          
            "Visible": "VisibleEditor",
            "Required": "RequiredEditor",
            "Readonly": "ReadonlyEditor",
            "Span": "SpanEditor",
            "MaxLen": "MaxLenEditor",
            "MinLen": "MinLenEditor",
            "Rows": "RowsEditor",
            "Tip": "TipEditor",

            "Max": "MaxEditor",
            "Min": "MinEditor",
            "Precision": "PrecisionEditor",
            "Suffix": "SuffixEditor",
            "Verify": "VerifyEditor",
            "VerifyInfo": "VerifyInfoEditor",

            "DataSourceType": "DataSourceTypeEditor",
            "DataSourceCode": "DataSourceCodeEditor",
            "ValueField": "ValueFieldEditor",
            "TextField": "TextFieldEditor",
            "ParentField": "ParentFieldEditor",
            "CascadeParent": "CascadeParentEditor",
            "CascadeField": "CascadeFieldEditor",
            "ActiveText": "ActiveTextEditor",
            "InActiveText": "InActiveTextEditor",

            "DialogTitle": "DialogTitleEditor",
            "DialogWidth": "DialogWidthEditor",
            "DialogHeight": "DialogHeightEditor",
            "QueryJson": "QueryJsonEditor",

            "RenderType": "RenderTypeEditor",
            "MultiSelect": "MultiSelectEditor",
            "CollapseTags": "CollapseTagsEditor",

            "Limit": "LimitEditor",
            "Accept": "AcceptEditor",

            "TextAlign": "TextAlignEditor",

            "DateFormat": "DateFormatEditor",


            "Layout": "LayoutEditor",
            "MaxRows": "MaxRowsEditor",

            "Height": "HeightEditor",
            "Toolbar": "ToolbarEditor",

            "FullFill": "FullFillEditor",
            "Tabs": "TabsEditor",     
            "Style": "StyleEditor",
            "TabPosition": "TabPositionEditor",

            "DefaultValue": "DefaultValueEditor",

            "BtnType": "BtnTypeEditor",
            "Plain": "PlainEditor",
            "Round": "RoundEditor",
            "Size": "SizeEditor",
            "Icon": "IconEditor",

            "TextFont": "TextFontEditor",

            "TitleFont": "TitleFontEditor",


           

        },


        commonItemProps: {
            "Id": "",
            "Name": "",
            "Span": 24,
            "Table": "",
            "Field": "",
            "Visible": true,
            "Required": true,
            "Readonly": false,
            "TitleFont": fontCss,
            "TitleVisible": true
            
        },

        itemDefs: {
            "常用":
                [
                    {
                        "Title": "文本框",
                        "Type": "Text",
                        "DefaultValue": "",
                        "MaxLen": 100,
                        "MinLen": 0,
                        "Tip": "",
                        "Verify": "",
                        "VerifyInfo": ""

                    },
                    {
                        "Title": " 文本区",
                        "Type": "TextArea",
                        "DefaultValue": "",
                        "Rows": 5,
                        "MaxLen": 200,
                        "MinLen": 0,
                        "Tip": "",

                    },
                    {
                        "Title": "数字框",
                        "Type": "Number",
                        "DefaultValue": null,
                        "Max": 9999999,
                        "Min": 0,
                        "Precision": 0,
                        "Tip": "",
                        "Suffix": ""

                    },

                    {

                        "Title": "单选框",
                        "Type": "Radio",
                        "DataSourceType": 1,
                        "DataSourceCode": "",
                        "ValueField": "",
                        "TextField": "",
                        "DefaultValue": "",
                        "CascadeParent": "",
                        "CascadeField": ""


                    },

                    {

                        "Title": "多选框",
                        "Type": "CheckBox",
                        "DataSourceType": 1,
                        "DataSourceCode": "",
                        "ValueField": "",
                        "TextField": "",
                        "DefaultValue": [],
                        "CascadeParent": "",
                        "CascadeField": ""

                    },

                    {
                        "Title": "下拉框",
                        "Type": "Select",
                        "DataSourceType": 1,
                        "DataSourceCode": "",
                        "ValueField": "",
                        "TextField": "",
                        "DefaultValue": "",
                        "CascadeParent": "",
                        "CascadeField": "",
                        "RenderType": 1,
                        "MultiSelect": false,
                        "CheckStrictly": false,
                        "CollapseTags": true,
                        "ParentField": "",
                        "Tip": "请选择",

                    },

                    {

                        "Title": "文件上传",
                        "Type": "Upload",
                        "Limit": 1,
                        "Accept": ""



                    },
                    {

                        "Title": "图片上传",
                        "Type": "Image",
                        "Limit": 1,

                    },


                    {

                        "Title": "文本",
                        "Text":"文本",
                        "Type": "Label",
                        "TextAlign": "left",
                        "Field": "undefined",
                        "Table": "undefined",
                        "Required": "undefined",
                        "Readonly": "undefined",
                        "TextFont": fontCss,
                        "TitleVisible": false

                    },

                    {

                        "Title": "时间日期",
                        "Type": "DatePicker",
                        "DateFormat": 1,
                        "Tip": "请选择",

                    },

                    {

                        "Title": "开关",
                        "Type": "Switch",
                        "ActiveText": "",
                        "InActiveText": "",
                        "DefaultValue": 0,
                    },


                    {

                        "Title": "分割线",
                        "Text":"分割线",
                        "Type": "Divider",
                        "TextAlign": "left",
                        "Field": "undefined",
                        "Table": "undefined",
                        "Required": "undefined",
                        "Readonly": "undefined",
                        "TextFont": fontCss,
                        "TitleVisible": false

                    },


                ],

            "高级": [
                
                {
                    "Title": "表格控件",
                    "Type": "Table",
                    "Layout": 1,
                    "MaxRows": 0,
                    "Field": "undefined",
                    "Items": [],


                },
                {

                    "Title": "数据选择",
                    "Type": "DataSelect",
                    "DataSourceType": 3,
                    "DataSourceCode": "",
                    "ValueField": "",
                    "TextField": "",
                    "CascadeParent": "",
                    "CascadeField": "",
                    "DialogWidth": 1200,
                    "DialogHeight": 600,
                    "DialogTitle": "",
                    "QueryJson": "",

                },
                {

                    "Title": "富文本",
                    "Type": "RichText",
                    "DefaultValue": "",
                    "Height": 300,
                    "Toolbar": "default",

                },
                {

                    "Title": "标签页",
                    "Type": "Tab",
                    "Field": "undefined",
                    "Table": "undefined",
                    "Readonly": "undefined",
                    "Required": "undefined",
                    "Style": "border-card",
                    "TabPosition": "top",
                    "FullFill": true,
                    "TitleVisible": false,
                    "Tabs": [
                        { "Title": "标签1", "Icon": "", "Items": []}, 
                        { "Title": "标签2", "Icon": "", "Items": []}


                    ]

                },
                {
                    "Title": "按钮",
                    "Text": "按钮",
                    "Type": "Button",
                    "Field": "undefined",
                    "Table": "undefined",
                    "Required": "undefined",
                    "TextAlign": "left",
                    "BtnType": "default",
                    "Plain": false,
                    "Round": 0,
                    "Icon": "",
                    "Size": "medium",
                    "TitleVisible": false,
                    "TextFont": fontCss,
                },
                {
                    "Title": "位置",
                    "Type": "Location", 
                },

            ],
        },

        defaultDesignData: {
            "DbTables": [],
            "Items": [],
            "ListConfig": {
                "DataSource": { "Enabled": false, "Code": '' },
                "Query": { "Cdts": [] },
                "DataAuth": 0,
                "List": {
                    "Type": 1, /*1普通表单，2展开，3树形*/
                    "SubForm": { FormCode: '', RefField: '__RefId' },
                    "Tree": { ParentField: 'ParentId', LazyLoad: false },
                    "ActionCol": { Width: 180 },
                    "Page": { "Enabled": true, "PageSize": 10 },
                    "Summary": { "Enabled": false, "Title": '合计' },
                    "TopBtns": [
                        { Name: '添加', Icon: 'el-icon-plus', IsShow: true, Target: "opendialog", ShowInMore: false, Url: "", Type: "primary", Sort: 1, Roles: [], IsSys: true, Id: 'add', Condition: "" },
                        { Name: '删除', Icon: 'el-icon-delete', IsShow: true, Target: "api", Method: "post", ArgTran: "body", ShowInMore: false, Url: "", Type: "danger", Sort: 2, Roles: [], IsSys: true, Id: 'batchdelete', Condition: "" },
                        { Name: '导入', Icon: 'el-icon-upload2', IsShow: false, Target: "opendialog", ShowInMore: false, Url: "", Type: "def", Sort: 3, Roles: [], IsSys: true, Id: 'import', Condition: "" },
                        { Name: '导出', Icon: 'el-icon-download', IsShow: false, Target: "api", Method: "post", ArgTran: "body", ShowInMore: false, Url: "", Type: "def", Sort: 4, Roles: [], IsSys: true, Id: 'export', Condition: "" }

                    ], "RowBtns": [
                        { Name: '查看', Icon: 'el-icon-view', IsShow: true, Target: "opendialog", ShowInMore: false, Url: "", Type: "primary", Sort: 1, Roles: [], IsSys: true, Id: 'view', Condition: "" },
                        { Name: '编辑', Icon: 'el-icon-edit', IsShow: true, Target: "opendialog", ShowInMore: false, Url: "", Type: "primary", Sort: 2, Roles: [], IsSys: true, Id: 'edit', Condition: "" },
                        { Name: '删除', Icon: 'el-icon-delete', IsShow: true, Target: "api", Method: "post", ArgTran: "body", ShowInMore: false, Url: "", Type: "primary", Sort: 3, Roles: [], IsSys: true, Id: 'delete', Condition: "" }

                    ], "Cols": []
                },

            },
            ExportConfig: { ImportApi: "", ExportApi: "", ImportMode: 1, Cols: [] },
            "Theme": {
                "TitlePosition": "left",
                "TitleWidth": 100,
                "TitleFont": fontCss,
                "Padding": 20

            },
            DataSaveType: 1


        },
        itemSwitchDefs: {
            "Text": ['Number', 'TextArea', 'DatePicker', 'RichText'],
            "RichText": ['TextArea', 'Text'],
            "TextArea": ['Text', 'RichText'],
            "Image": ['Upload'],
            "Upload": ['Image'],
            "Number": ['Text'],
            "Select": ['Radio', 'CheckBox', 'DataSelect'],
            "Radio": ['Select', 'CheckBox'],
            "CheckBox": ['Select', 'Radio'],
            "DataSelect": ['Select'],
            "DatePicker": ['Text'],

        },
        spanOpts: [
            {
                value: 24,
                label: '1'
            },
            {
                value: 12,
                label: '1/2'
            },
            {
                value: 8,
                label: '1/3'
            },

            {
                value: 6,
                label: '1/4'
            },
            {
                value: 4,
                label: '1/6'
            },
            {
                value: 16,
                label: '2/3'
            },
            {
                value: 18,
                label: '3/4'
            },
            {
                value: 20,
                label: '5/6'
            },
            //{
            //    value: 100,
            //    label: '自动'
            //},


        ],

        queryRenderTypes: [
            {
                value: 1, label: '默认'
            },
            {
                value: 2, label: '文本'
            },
            {
                value: 3, label: '数字'
            },
            {
                value: 4, label: '日期'
            },
            {
                value: 5, label: '下拉'
            },
            {
                value: 6, label: '下拉(多选)'
            },
            {
                value: 7, label: '单选'
            },
            {
                value: 8, label: '多选'
            },
            {
                value: 9, label: '选项卡'
            },
            {
                value: 10, label: '树形'
            },


        ],

        alignTypes: [
            { 'value': 'left', 'label': '靠左' },
            { 'value': 'center', 'label': '居中' },
            { 'value': 'right', 'label': '靠右' }
        ],
        sortTypes: [
            { 'value': '', 'label': '默认' },
            { 'value': 'asc', 'label': '升序' },
            { 'value': 'desc', 'label': '降序' },
            { 'value': 'no', 'label': '不排序' }
        ],
        summaryTypes: [
            { 'value': '', 'label': '默认' },
            { 'value': 'sum', 'label': '求和' },
            { 'value': 'avg', 'label': '平均值' },
            { 'value': 'max', 'label': '最大值' },
            { 'value': 'min', 'label': '最小值' }
        ],

        dfRuleFnNames: ['init', 'beforeSave', 'afterSave'],
        editorToolbar: {
            simple: [
                [
                    'bold', 'italic', 'underline', 'strikethrough', '|', 'forecolor', 'backcolor',
                    'customstyle', 'paragraph', 'fontfamily', 'fontsize',
                    'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify', '|',
                    'link', 'unlink',
                    'simpleupload', 'attachment'

                ]
            ],
            default: [
                ['fullscreen', 'source', '|', 'undo', 'redo', '|',
                    'bold', 'italic', 'underline', 'strikethrough', 'superscript', 'subscript', 'removeformat', 'formatmatch', 'autotypeset', 'blockquote', 'pasteplain', '|', 'forecolor', 'backcolor', 'insertorderedlist', 'insertunorderedlist', 'selectall', 'cleardoc', '|',
                    'rowspacingtop', 'rowspacingbottom', 'lineheight',/* '|',*/
                    'customstyle', 'paragraph', 'fontfamily', 'fontsize',
                    'indent', '|',
                    'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify', '|',
                    'link', 'unlink', 'anchor', '|', 'imagenone', 'imageleft', 'imageright', 'imagecenter', '|',
                    'simpleupload', 'insertimage', 'emotion', 'insertvideo', 'attachment', 'map', 'pagebreak', 'template', '|',
                    'horizontal', 'date', 'spechars', '|',
                    'inserttable', 'deletetable', 'insertparagraphbeforetable', 'insertrow', 'deleterow', 'insertcol', 'deletecol', 'mergecells', '|',
                    'searchreplace']
            ]



        },
     

    },

    predefineColors: [
        '#ff4500',
        '#ff8c00',
        '#ffd700',
        '#90ee90',
        '#00ced1',
        '#1e90ff',
        '#c71585',
        'rgba(255, 69, 0, 0.68)',
        'rgb(255, 120, 0)',
        'hsv(51, 100, 98)',
        'hsva(120, 40, 94, 0.5)',
        'hsl(181, 100%, 37%)',
        'hsla(209, 100%, 56%, 0.73)',
        '#c7158577'
    ],
    flow: {
        style: {
            polyline: {
                stroke: '#ddd',
                strokeWidth: 2,
            },

            edgeText: {
                color: '#000',
                fontSize: 12,
                background: {
                    fill: 'transparent',
                },
            },

            nodeText: {
                color: 'transparent',
                overflowMode: 'default',
                lineHeight: 1.2,
                fontSize: 12,
            },
            outline: {
                fill: "transparent",
                stroke: "none",
                strokeDasharray: "3,3",
                hover: {
                    stroke: "none",
                },
            },
        }
    },

    logicOperators: [
        { name: '=', value: "=", types: ['String', 'Number'] },
        { name: '<>', types: ['String', 'Number'] },
        { name: '<', types: ['Number'] },
        { name: '>', types: ['Number'] },
        { name: '<=', types: ['Number'] },
        { name: '>=', types: ['Number'] },
        { name: '包含', value: 'contain', types: ['String'] },
        { name: '不包含', value: 'notcontain', types: ['String'] },
        { name: '为空', value: 'empty', types: ['String', 'Number', ...arrayTypes,'Object'], },
        { name: '不为空', value: 'notempty', types: ['String', 'Number', ...arrayTypes, 'Object'] },
        { name: '开始是', value: 'starts', types: ['String'] },
        { name: '结束是', value: 'ends', types: ['String'] },

    ],

    variableTypes: [
        { name: 'String' },
        { name: 'Number' },
        { name: 'Array[String]' },
        { name: 'Array[Number]' },
        { name: 'Array[Object]' },
        { name: 'Object' },

    ],


}
