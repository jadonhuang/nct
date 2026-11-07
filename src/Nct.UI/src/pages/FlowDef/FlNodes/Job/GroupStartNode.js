import StartNode from './StartNode';

class GroupStartNode extends StartNode.view {


}

class GroupStartNodeModel extends StartNode.model {

    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: "JobGroupStart"
            }
        }
        //this.draggable = false;
        //this.isHitable = false;
        super.initNodeData(data);
        this.width = 40;
       
    }

    validate() {
        return true;
    }

    getOutputs() {
        return [];
    }
  

}

export default {
    type: 'groupstart',
    model: GroupStartNodeModel,
    view: GroupStartNode
}