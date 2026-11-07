
import { BaseNode, BaseNodeModel } from './BaseNode';

class ScriptNode extends BaseNode {


}

class ScriptNodeModel extends BaseNodeModel {


    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'JobScript',
                Code: '',
                RetryConfig: {
                    MaxRetryTimes: 3,
                    Enabled: false,
                    Interval: 100,
                },
                Variables: [
                    { Name: "arg1", Value: "" }

                ],
                Outputs: [{ Name: "result", Type:""}],
            }
        }
        super.initNodeData(data);

    }

    getOutputs() {
        const { properties } = this;
        const variables = properties.Outputs.filter(m=>!!m.Name&&!!m.Type).map(m => {

            return {
                Name: m.Name,
                Type: m.Type,
            }

        });
        return variables;

    }

    isValidVarName(name) {

        if (/^[a-zA-Z]{1}\w*$/.test(name) == false)
            return false;

        return true;

    }




    validate() {

        const { properties } = this;
        super.validate();
        if (!properties.Code) {
            super.addWarning('请设置脚本');

        }
     
        for (const v of properties.Variables) {
          
            if (!v.Name) {
                super.addWarning('请设置输入变量名称');
                break;
            }
            if (!this.isValidVarName(v.Name)) {
                super.addWarning('输入变量名称格式错误');
                 break;
            }

            if (!v.Value) {
                super.addWarning('请设置输入变量值');
                break;
            }
        }

        for (const v of properties.Outputs) {

            if (!v.Name) {
                super.addWarning('请设置输出变量名称');
                break;
            }
            if (!this.isValidVarName(v.Name)) {
                super.addWarning('输出变量名称格式错误');
                break;
            }

            if (!v.Type) {
                super.addWarning('请选择输出变量类型');
                break;
            }
        }

        const retryConfig = properties.RetryConfig;
        if (retryConfig.Enabled) {
            if (!retryConfig.MaxRetryTimes)
                super.addWarning('请设置重试次数');
            if (!retryConfig.Interval && retryConfig.Interval != 0)
                super.addWarning('请设置重试间隔');

        }


        return super.isValid();

    }

}

export default {
    type: 'script',
    model: ScriptNodeModel,
    view: ScriptNode
}