import { BaseNode, BaseNodeModel } from './BaseNode';

import vueComponent from './CaseNode.vue';


const ITEM_HEIGHT = 30

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
                PropType: 'JobCase',
                Cases: [
                    { IsElse: true, Conditions: [] }
                ]

            }
        }
        super.initNodeData(data);
        this.width = 240;


    }



    setAttributes() {
        super.setAttributes();
        const { properties} = this;
        const cases = properties.Cases;
        const defaultHeight = super.getDefaultHeight();
        let caseNum = cases.length;
        let height = defaultHeight;

        if (caseNum == 0) {
            height += ITEM_HEIGHT;

        } else {
            for (var item of cases) {
                height += (item.Conditions.length + 1) * ITEM_HEIGHT;

            }
        }

        this.height = height;

    }

    getDefaultAnchor() {
        const { id, x, y, width, height } = this
        const anchors = []
        anchors.push({
            x: x - width / 2,
            y,
            id: `${id}_incomming`,
            edgeAddable: false,
            type: 'incomming'
        })

        const { properties } = this;
        if (properties.Cases.length <= 1)
            return anchors;

        let index = 0
        const cases = properties.Cases || [];
        let top = y - height / 2 + super.getDefaultHeight() + ITEM_HEIGHT / 2;
        for (const item of cases) {

            anchors.push({
                x: x + width / 2,
                y: top,
                id: `${index}`,
                type: 'outgoing'
            })

            top += item.Conditions.length * ITEM_HEIGHT + ITEM_HEIGHT;
            index++;

        }

        return anchors
    }

    layoutCaseEdges() {
        const { properties, graphModel } = this;
        let anchors = this.getOutAnchor();
        for (const curCase of properties.Cases) {
            for (const edgId of curCase.EdgeIds||[]) {
                let edge = graphModel.getEdgeModelById(edgId);
                if (edge) {

                    let anchor = anchors.find(m => m.id == edge.sourceAnchorId);
                    if (!anchor) continue;
                    let point = edge.startPoint;
                    point.y = anchor.y;
                    edge.updateStartPoint(point); //看源码有此方法，文档没有看的

                }

            }

        }

    }


    getOutAnchor() {
        let anchors = this.getDefaultAnchor().filter(m => m.type == 'outgoing');
        return anchors;
    }

    setCaseEdge() {
        const { properties, id, graphModel } = this;
        let index = 0;
        let edges = graphModel.getNodeOutgoingEdge(id);
        let anchors = this.getOutAnchor();
        let offset = 10;
        for (const curCase of properties.Cases) {
            let edgeIds = [];
            let anchor = anchors.find(m => m.id == index);
            if (!anchor) continue;
            let top = anchor.y;
            for (const edge of edges) {
                let edgeY = edge.startPoint.y;
                if (Math.abs(edgeY - top) < offset) {
                    edgeIds.push(edge.id);
                    break;
                }
            }
            curCase.EdgeIds = edgeIds;
            index++;

        }

    }



    validate() {
        super.clearWarnings();
        super.validateText();
        super.validateIncomingNode();
        const { properties } = this;
        const cases = properties.Cases;
        if (cases.length < 2) {

            super.addWarning('至少设置一个条件分支');

        }

        let index = 1;
        for (const item of cases) {

            if (item.IsElse) continue;

            if (!item.EdgeIds || item.EdgeIds.length == 0) {
                const name = `Case${index}`
                super.addWarning(`${name}请设置连出节点`);
            }

            if (item.IsElse) continue;

            if (!item.Conditions.length) {
                super.addWarning(`Case${index}请设置条件分支`);
            }


            for (const condition of item.Conditions) {
                if (!condition.Left) {
                    super.addWarning(`Case${index}请设置条件左边变量`);
                }
                if (!condition.Right) {
                    super.addWarning(`Case${index}请设置条件右边变量`);
                }
                if (!condition.ComparisonOperator) {
                    super.addWarning(`Case${index}请设置条件运算符`);
                }
            }

            index++;
        }

        return super.isValid();


    }

}

export default {
    type: 'case',
    model: CaseNodeModel,
    view: CaseNode
}