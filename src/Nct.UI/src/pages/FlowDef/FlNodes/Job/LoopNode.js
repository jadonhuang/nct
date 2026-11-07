import  GroupNode  from  "./GroupNode";


class LoopNode extends GroupNode.view {

    

}

class LoopNodeModel extends GroupNode.model {



    
}

export default {
    type: 'loop',
    model: LoopNodeModel,
    view: LoopNode
}