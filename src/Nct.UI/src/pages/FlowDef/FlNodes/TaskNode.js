import { BaseNode, BaseNodeModel } from './BaseNode';

class TaskNode extends BaseNode {
  
}

class TaskNodeModel extends BaseNodeModel {


    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'Task',
                //type 1 会签 2 或签 3依次审批
                //unit 1天2小时3分钟
                //ExpireHandle 1消息通知2自动通过3自动拒绝
                Audit: { Members: [], Type: 1, Percent: 100, TimeLimit: { Enabled: false, Timeout: 6, Unit: 2, ExpireHandle: 1, LoopNotify: false } },
                FormEdit: false,

            };
        }
        super.initNodeData(data);

    }


    validate() {
        const { properties } = this;
        super.validate();    
        if (!properties.Audit.Members.length) {
            super.addWarning('请设置审批人');
                   
        }
        return super.isValid();

    }

}

export default {
    type: 'task',
    model: TaskNodeModel,
    view: TaskNode
}