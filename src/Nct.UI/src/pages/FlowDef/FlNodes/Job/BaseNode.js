import { BaseNode as RootNode, BaseNodeModel as RootNodeModel } from '../BaseNode';
import vueComponent from './BaseNode.vue';

class BaseNode extends RootNode {

    constructor(props) {
        super(props)
        this.vueComponent = vueComponent;

    }

}

class BaseNodeModel extends RootNodeModel {

    validateOutgoingNode() {
        let nodes = this.graphModel.getNodeOutgoingNode(this.id);
        let index = nodes.findIndex(m => m.id == this.id);
        if (index != -1) {
            super.addWarning('连出节点不能是自己');
        }

    }

    getConnectedTargetRules() {
        const rules = super.getConnectedTargetRules()
        const notAsTarget = {
            message: '组内和组外节点不能连接',
            validate: (source, target) => {
                if (source.getParent() != target.getParent())
                    return false;
                  return true;

            }
        }
        rules.push(notAsTarget)
        return rules
    }



}




export {
    BaseNode,
    BaseNodeModel
}

