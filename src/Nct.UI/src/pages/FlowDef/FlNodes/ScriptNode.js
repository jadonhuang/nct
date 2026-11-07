
import { BaseNode, BaseNodeModel } from './BaseNode';

class ScriptNode extends BaseNode {


}

class ScriptNodeModel extends BaseNodeModel {


    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'Script',
                Code: '',
            }
        }
        super.initNodeData(data);

    }


    

    validate() {

        const { properties } = this;
        super.validate();
        if (!properties.Code) {
            super.addWarning('请设置脚本');

        }
        return super.isValid();
    }

}

export default {
    type: 'script',
    model: ScriptNodeModel,
    view: ScriptNode
}