import GroupNode from "./GroupNode";


class IterationNode extends GroupNode.view {



}

class IterationNodeModel extends GroupNode.model {


    initNodeData(data) {


        if (!data.properties) {
            data.properties = {
                PropType: 'JobIteration',
                Input:'',
                Output: '',
                ErrHandleMode: 'stop',
            }
        }

        super.initNodeData(data);

    }

    validate() {
        super.validate();
        const children = this.getChildren();
        if (children.length <= 1) {
            super.addWarning('请至少添加一个非开始节点');
        }
        const  properties  = this.properties;
        if (!properties.Input) {
            super.addWarning('请设置输入参数');
        }
        return super.isValid();
    }




}

export default {
    type: 'iteration',
    model: IterationNodeModel,
    view: IterationNode
}