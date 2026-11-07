import { BaseNode, BaseNodeModel } from './BaseNode';

class StartNode extends BaseNode {

}

class StartNodeModel extends BaseNodeModel {

    initNodeData(data) {

        if (!data.properties) {       
            data.properties = {
                PropType: "JobStart",              
                Variables:[]
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

    getSystemVariables() {
        return [
            {
                Name: 'sys.user_id',
                Type: 'Number',
            },
            {
                Name: 'sys.user_name',
                Type: 'String',
            },
            {
                Name: 'sys.job_id',
                Type: 'Number',
            },
        ]
    }

    getOutputs() {
        const { properties } = this;
        const variables = properties.Variables.map(m => {

            return {
                Name: m.Name,
                Type: m.Type,
            }

        });
        return [...variables, ...this.getSystemVariables()]
     
    }
       
    validate() {
        super.clearWarnings();
        super.validateText();
        let nodes = this.graphModel.getNodeOutgoingNode(this.id);
        if (nodes.length == 0) {
            this.addWarning('请设置连出节点');
        }
        return super.isValid();
    }





}

export default {
    type: 'start',
    model: StartNodeModel,
    view: StartNode
}