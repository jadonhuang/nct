import { BaseNode, BaseNodeModel } from './BaseNode';

class StartNode extends BaseNode {

}

class StartNodeModel extends BaseNodeModel {

    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                "PropType": "Start",
                "FormCode": ""
            }
        }
        super.initNodeData(data);

    }

    getConnectedTargetRules() {

        const rules = super.getConnectedTargetRules()
        const notAsTarget = {
            message: '起始节点不能作为连线的终点',
            validate: () => false
        }
        rules.push(notAsTarget)
        return rules
    }

    getDefaultAnchor() {
        return super.getOutAnchor();
    }

    validate() {
        super.clearWarnings();
        const { properties } = this;
        super.validateOutgoingNode();

        if (!properties.FormCode) {
            super.addWarning('请设置表单');
        }
        return super.isValid();
    }


}

export default {
    type: 'start',
    model: StartNodeModel,
    view: StartNode
}