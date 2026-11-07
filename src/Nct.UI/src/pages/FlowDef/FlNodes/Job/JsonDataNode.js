
import { BaseNode, BaseNodeModel } from './BaseNode';

class JsonDataNode extends BaseNode {


}

class JsonDataNodeModel extends BaseNodeModel {


    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'JobJsonData',
                Input: '',
                DataItems: [{Name:"",Path:"",DataType:"String"}],
              
            }
        }
        super.initNodeData(data);

    }

    getOutputs() {
        const { properties } = this;
        const variables = properties.DataItems.filter(m=>!!m.Name&&!!m.DataType).map(m => {

            return {
                Name: m.Name,
                Type: m.DataType,
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
        if (!properties.Input) {
            super.addWarning('请设置输入变量');

        }
        if (properties.DataItems.length==0) {
            super.addWarning('请添加输出参数');

        }
     
        for (const v of properties.DataItems) {
          
            if (!v.Name) {
                super.addWarning('请设置变量名称');
                break;
            }
            if (!this.isValidVarName(v.Name)) {
                super.addWarning('变量名称格式错误');
                 break;
            }

            if (!v.Path) {
                super.addWarning('请设置JsonPath');
                break;
            }


            if (!v.DataType) {
                super.addWarning('请选择数据类型');
                break;
            }
        }


        return super.isValid();

    }

}

export default {
    type: 'jsonData',
    model: JsonDataNodeModel,
    view:JsonDataNode
}