import { HtmlNode, HtmlNodeModel, h } from "@logicflow/core";
import vueComponent from './BaseNode.vue';

class BaseNode extends HtmlNode {
    constructor(props) {
        super(props)
        this.isMounted = false
        this.vueComponent = vueComponent;
        this.root = document.createElement('div');

    }

    shouldUpdate() {

        const data = {
            ...this.props.model.properties,
            isSelected: this.props.model.isSelected,
            isHovered: this.props.model.isHovered,
            text: this.props.model.text,
        }
        if (this.preProperties && this.preProperties === JSON.stringify(data)) return
        this.preProperties = JSON.stringify(data)
        return true
    }


    setHtml(rootEl) {
        rootEl.appendChild(this.root)
        if (this.vm) {
            this.vm.$mount(this.root)
        } else {
            this.vm = new Vue({
                render: (h) => h(this.vueComponent, {
                    props: {
                        model: this.props.model,
                        disabled: this.props.graphModel.editConfigModel.isSilentMode,
                        isSelected: this.props.model.isSelected,
                        isHovered: this.props.model.isHovered,
                        properties: this.props.model.getProperties(),
                        text: this.props.model.text.value,

                    }
                })
            })
            this.vm.$mount(this.root)
        }
    }

    //getAnchorShape(anchorData) {
    //    const { x, y, type } = anchorData;

    //    return h(
    //        'circle',
    //        {
    //            cx: x,
    //            cy: y,
    //            r: 6,
    //            fill:'red',
    //            stroke:'#000',
    //            strokeWidth:1
    //        }
    //    );


    //    return h('rect', {
    //        x: x - 3,
    //        y: y - 3,
    //        width: 6,
    //        height: 6,
    //        fill: 'red',
    //        stroke: '#000',
    //        strokeWidth: 1,
    //        className: `custom-anchor`
    //    })
    //}

}

class BaseNodeModel extends HtmlNodeModel {


    getDefaultHeight() {

        return 40;
    }


    getParent() {

        const { graphModel, id } = this
        for (let node of graphModel.nodes) {
            if (node.id == id) continue;

            let nodeModel = graphModel.getNodeModelById(node.id);

            if (nodeModel.getChildren && nodeModel.getChildren().includes(id))
                return node.id;

        }

        return null;
    }


    initNodeData(data) {

        if (data.properties && !data.properties.warnings) {
            data.properties.warnings = [];
            data.properties.status = { status: 0, text: null }
        }

        this.width = 160;
        this.height = this.getDefaultHeight();
        data.resizable = data.resizable || false;
        super.initNodeData(data);

    }

    createId() {
        const timestamp = Date.now();
        let random = Math.floor(Math.random() * 100).toString();
        if (random.length == 1) random = '0' + random;
        const id = timestamp.toString() + random;
        return id;
    }



    getTextStyle() {
        const style = super.getTextStyle();
        style.fontSize = 0;
        return style;
    }


    getOutlineStyle() {
        const style = super.getOutlineStyle();
        style.stroke = 'none';
        style.hover.stroke = 'none';
        return style;
    }

    getIncomeAnchor() {
        const { id, x, y, width } = this
        const anchors = []
        anchors.push({
            x: x - width / 2,
            y,
            id: `${id}_incomming`,
            edgeAddable: false,
            type: 'incomming'
        })

        return anchors
    }

    getOutAnchor() {
        const { id, x, y, width } = this
        const anchors = []
        anchors.push({
            x: x + width / 2,
            y,
            id: `${id}_outgoing`,
            type: 'outgoing'
        })
        return anchors
    }

    getDefaultAnchor() {
        return [...this.getIncomeAnchor(), ...this.getOutAnchor()];

    }

    isValid() {
        return this.properties.warnings.length == 0;
    }


    validate() {
        this.clearWarnings();
        this.validateText();
        this.validateOutgoingNode();
        this.validateIncomingNode();
        return this.isValid();

    }

    clearWarnings() {
        this.properties.warnings = [];
    }


    addWarning(warning) {
        this.properties.warnings.push(warning);
    }


    validateText() {

        const { text } = this;
        if (!text || !text.value) {
            this.addWarning('请设置标签');

        }


    }


    validateOutgoingNode() {

        let nodes = this.graphModel.getNodeOutgoingNode(this.id);
        if (nodes.length == 0) {
            this.addWarning('请设置连出节点');
        }
        else if (nodes.length > 1) {
            this.addWarning('只能设置一个连出节点');
        }

        let index = nodes.findIndex(m => m.id == this.id);
        if (index != -1) {
            this.addWarning('连出节点不能是自己');
        }


    }

    validateIncomingNode() {

        let nodes = this.graphModel.getNodeIncomingNode(this.id);
        if (nodes.length == 0) {
            this.addWarning('请设置连入节点');
        }


    }

    remove() {

        this.graphModel.deleteNode(this.id);


    }



}

export {
    BaseNode,
    BaseNodeModel
}