import { BaseNode, BaseNodeModel } from './BaseNode';
import vueComponent from './CaseNode.vue';

class CaseNode extends BaseNode {

    constructor(props) {
        super(props)
        this.vueComponent = vueComponent;

    }
}



class CaseNodeModel extends BaseNodeModel {

    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'Case',
                Cdts: [
                    { Text: '默认分支', IsDefault: true, CdtType: 1, SqlCdt: '', CodeCdt: '', EdgeId: '' },
                    { Text: '分支1', IsDefault: false, CdtType: 1, SqlCdt: '', CodeCdt: '', EdgeId: '' },
                ]

            }
        }
        super.initNodeData(data);

    }

    setAttributes() {
        super.setAttributes();
        const { properties } = this;
        const cdts = properties.Cdts || [];
        let height = 40 + cdts.length * 40;
        this.height = height;
    }

    getDefaultAnchor() {
        const { x, y, width, height } = this
        const anchors = [super.getIncomeAnchor()]


        const { properties } = this;
        let index = 0

        const cdts = properties.Cdts || [];

        for (const cdt of cdts) {

            anchors.push({
                x: x + width / 2,
                y: y - height / 2 + 60 + index * 40 ,
                id: `${cdt.Id}`,
                type: 'outgoing'
            })

            index++;

        }


        return anchors
    }

    setCdtEdge() {

        const { y, height, properties, id } = this;
        let index = 0;
        let edges = this.graphModel.getNodeOutgoingEdge(id);
        for (const cdt of properties.Cdts) {

            let startY = y - height / 2 + 40 + index * 40 ;
            for (const edge of edges) {

                let edgeY = edge.startPoint.y;
                if (edgeY > startY && edgeY < startY + 40) {
                    cdt.EdgeId = edge.id;
                    break;
                }
            }
            index++;

        }
    }

    clearInvalidCdtEdge() {

        const { properties, graphModel } = this;
        let edges = graphModel.getNodeOutgoingEdge(id);
        for (const edge of edges) {

            const index = properties.Cdts.findIndex(m => m.EdgeId == edge.id);
            if (index == -1) {
                graphModel.deleteEdgeById(edge.id);
            }

        }

    }




    validate() {

        super.clearWarnings();
        const { properties, id, x } = this;
        super.validateText();
        super.validateIncomingNode();
       
        const cdts = properties.Cdts;
        if (cdts.length < 2) {

            super.addWarning('至少设置两个条件分支');

        }

        let index = 1;

        let edges = this.graphModel.getNodeOutgoingEdge(id);

        let hasOutNode = true;

        for (var cdt of cdts) {

            if (!cdt.Text) {
                super.addWarning(`第${index}个分支请设置标签`);
            }

            if (!cdt.EdgeId || edges.findIndex(m => m.id == cdt.EdgeId) == -1) {

                super.addWarning(`第${index}个分支缺少连出节点`);
                hasOutNode = false;
            }

            if (!cdt.IsDefault && !cdt.SqlCdt && !cdt.SqlCdt) {

                super.addWarning(`第${index}个分支未设置条件`);
            }
            index++;
        }


        if (hasOutNode) {
            if (edges.length > cdts.length)
                super.addWarning(`一个分支只能设置一个连出节点`);

        }


        edges = this.graphModel.getNodeIncomingEdge(id);

        index = edges.findIndex(m => m.endPoint.x > x);
        if (index != -1) {
            super.addWarning(`分支不能设置连入节点`);

        }

        return super.isValid();
      

    }

}

export default {
    type: 'case',
    model: CaseNodeModel,
    view: CaseNode
}