
import { BaseNode, BaseNodeModel } from './BaseNode';

class EndNode extends BaseNode {
    

}

class EndNodeModel extends BaseNodeModel {


    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'End',
            }
        }
        super.initNodeData(data);

    }

    getDefaultAnchor() {
        return super.getIncomeAnchor();
    }

    validate() {
        super.clearWarnings();
        super.validateIncomingNode();
        return super.isValid();
      
    }

}

export default {
    type: 'end',
    model: EndNodeModel,
    view: EndNode
}