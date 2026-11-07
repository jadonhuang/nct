<template>
    <el-form-item label="输入变量">

        <div>
            <el-link icon="el-icon-circle-plus-outline" type="primary" :underline="false" @click="startAdd" style="font-size:14px;">添加</el-link>
            
        </div>

        <div>

            <div v-for="(item,index) in model.props.Variables" :key="index" class="arg-item custom">

              <img src="../../../../assets/img/x.svg" />
                <div class="arg-name">
                    {{item.Name}}
                </div>

                <div class="arg-type">{{item.Type}}</div>
                <div class="btn-area">
                    <el-link icon="el-icon-edit" @click="startEdit(index)"></el-link>
                    <el-link icon="el-icon-delete" @click="remove(index)" style="margin-left:8px;"></el-link>
                </div>


            </div>
            <el-divider></el-divider>
            <div v-for="(item,index) in systemArgs" :key="index+'sys'" class="arg-item">
                <img src="../../../../assets/img/x.svg" />
                <div class="arg-name">
                    {{item.Name}}
                </div>
                <div class="arg-type">{{item.Type}}</div>
               
            </div>


        </div>


        <el-dialog title="变量设置" :visible.sync="dialogVisible" append-to-body width="400px">
            <el-form :model="arg" label-width="80px" :rules="rules" ref="form" size="medium">

                <el-form-item label="名称：" prop="Name" required>

                    <el-input v-model="arg.Name" placeholder="请输入名称"></el-input>

                </el-form-item>

                <el-form-item label="类型：" prop="Type" required>

                    <el-select v-model="arg.Type" placeholder="请选择" class="w100">
                        <el-option label="String" value="String"> </el-option>
                        <el-option label="Number" value="Number"> </el-option>
                    </el-select>

                </el-form-item>

                <el-form-item label="值：" prop="Value" required>

                    <el-input v-model="arg.Value" placeholder="请输入值"></el-input>
                </el-form-item>


            </el-form>
            <span slot="footer">
                <el-button @click="dialogVisible = false" size="small">取 消</el-button>
                <el-button type="primary" @click="save" size="small">确 定</el-button>
            </span>
        </el-dialog>


    </el-form-item>
</template>

<script>
    export default {
        name: 'StartNodePropEditor',
        props: {
          
            model: Object,
            lf:null,

        },
        data() {
            return {
                dialogVisible: false,
                editIndex: -1,
                arg: { Name: '', Type: '', Value: '' },
                rules: {

                    Name: [
                        { required: true, message: '请输入名称', trigger: 'blur' },
                        {
                            trigger: 'blur',
                            validator: (rule, value, callback) => {
                                if (value && /^[a-zA-Z]{1}\w*$/.test(value) == false) {

                                    callback(new Error('只能是字母和数字，且以字母开头'));

                                } else {

                                    callback();
                                }
                            }
                        }

                    ],

                    Type: [
                        { required: true, message: '请选择类型', trigger: 'blur' }

                    ],
                    Value: [
                        { required: true, message: '请输入值', trigger: 'blur' },
                        {
                            trigger: 'blur',
                            validator: (rule, value, callback) => {
                                if (this.arg.Type=='Number'&&isNaN(Number.parseFloat(value))) {

                                    callback(new Error('必须是数字'));

                                } else {

                                    callback();
                                }



                            }
                        }

                    ]

                },
            }
        },
        computed: {

            nodeModel() {
                return this.lf.getNodeModelById(this.model.id);

            },

            systemArgs() {
                return this.nodeModel.getSystemVariables();
             
            }
        },



        methods: {
            propsChange() {
                this.$eventHub.$emit('node-prop-change');

            },
            startEdit(index) {
                this.editIndex = index;
                this.arg = Object.assign({}, this.model.props.Variables[index]);
                this.dialogVisible = true;
            },

            
            startAdd() {
                this.editIndex = -1;
                this.arg = { Name: '', Type: '', Value: '' }
                this.dialogVisible = true;


            },

            remove(index) {
                this.model.props.Variables.splice(index, 1);
                this.propsChange();
            },

            save() {

                this.$refs.form.validate(valid => {

                    if (!valid) return; 

                    if (this.editIndex == -1) {
                       
                        this.model.props.Variables.push(this.arg);

                    } else {

                        var arg = this.model.props.Variables[this.editIndex];
                        arg.Name = this.arg.Name;
                        arg.Type = this.arg.Type;
                      
                        if (arg.Type == 'Number') {
                            arg.Value = parseFloat(this.arg.Value);
                          
                        } else {
                            arg.Value = this.arg.Value;
                        }

                    }
                    this.propsChange();
                    this.dialogVisible = false;
                 


                })

            }


        }


    }</script>

<style scoped>
    .el-divider--horizontal {
     
        margin: 10px 0;
    }

    .arg-item {
        display: flex;
        flex-direction: row;
        border: solid 1px rgb(234, 236, 240);
        padding-right:5px;
        padding-left:5px;
        margin-bottom: 5px;
        border-radius: 4px;
        box-sizing:border-box;
        line-height:30px;
        align-items:center;
       
    }
        .arg-item img {
        
           width:15px;
           height:15px;
        }
        .arg-name {
            flex: 1;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            padding-left: 5px;
            line-height: 30px;
        }

    .arg-type {
        width: 60px;
        text-align:right;
    }

    .btn-area {
        width: 40px;
        text-align: right;
        display: none;
    }

    .arg-item:hover .btn-area {
        display: block;
    }

    .custom:hover .arg-type {
        display: none;
    }

</style>

