import { BaseNode, BaseNodeModel } from './BaseNode';

class GroupNode extends BaseNode {
    toFront() {
        //阻止置顶
    }

}

class GroupNodeModel extends BaseNodeModel {

    initNodeData(data) {

        if (!data.properties.Children) {
            data.properties.Children = [];
        }
        data.resizable = true;
        super.initNodeData(data);
        this.addMoveRules();
        this.width = 240;
        this.height = 100;
        this.zIndex = -10000;

        //read code found this method
        super.addNodeResizeRules((model, deltaX, deltaY, width, height) => {

            const { x, y } = model;
            const minSize = this.getMinSize();
            if ((x + width / 2 + deltaX < minSize.x && deltaX < 0) || (y + height / 2 + deltaY < minSize.y && deltaY<0))
            return false;

            return true;

        })

    }

 

    autoResize() {
      
        const minSize = this.getMinSize();
        const padding =30;
        const { minX, minY, maxX, maxY } = this.getBounds();

        let destX = minSize.x + padding
        let destY = minSize.y + padding

        let { width, height } = this;

        if (maxX < destX) { 

            this.width = destX - minX;
            let deltaX = this.width - width;
            this.move(deltaX/2, 0);
           

        }
        if (maxY < destY) {
            this.height = destY - minY;
            let deltaY = this.height - height;
            this.move(0, deltaY/2);
        }

    }
    clearChildren() {
        this.properties.Children = [];
    }
    
    addChild(id) {
        this.properties.Children.push(id);
    }

    removeChild(id) {
        const index = this.properties.Children.indexOf(id);
        if (index > -1) {
            this.properties.Children.splice(index, 1);
        }
    }

    getChildren() {
        return this.properties.Children;
    }

    getMinSize() {
        let children = this.getChildren();
        let { x, y } = this;
        let size = { x: x + 60, y: y + 30 };

        if (children.length > 0) {
            for (let child of children) {

                const model = this.graphModel.getNodeModelById(child);
                const {maxX, maxY } = model.getBounds();

                size.y = Math.max(size.y, maxY);
                size.x = Math.max(size.x, maxX);

            }
        }
        return size;
    }


    isAllowMoveTo({ minX, minY, maxX, maxY }) {
        return {
            x: minX >= this.x - this.width / 2  && maxX <= this.x + this.width / 2,
            y: minY >= this.y - this.height / 2 + super.getDefaultHeight() && maxY <= this.y + this.height / 2,
        }
    }


    addMoveRules() {

        const { graphModel, id } = this

        graphModel.addNodeMoveRules((model, deltaX, deltaY) => {
            var chidren = this.getChildren();
            if (model.id == id) {
                if (this.isDragging) {
                    graphModel.moveNodes(chidren, deltaX, deltaY, true);
                }
                return true;
            }

            if (chidren.indexOf(model.id) != -1) {
                const { minX, minY, maxX, maxY } = model.getBounds();
                return this.isAllowMoveTo({
                    minX: minX + deltaX,
                    minY: minY + deltaY,
                    maxX: maxX + deltaX,
                    maxY: maxY + deltaY,
                })

            }
            return true;
        })


    }



    getResizeOutlineStyle() {
        const style = super.getResizeOutlineStyle();
        style.stroke = "none";
        return style;
    }

    getResizeControlStyle() {
        return this.getResizeOutlineStyle();
    }


    remove() {
        const { graphModel } = this;
        const children = this.getChildren();
        for (let child of children) {
            graphModel.deleteNode(child);
        }

        super.remove();
    }


}

export default {
    type: 'group',
    model: GroupNodeModel,
    view: GroupNode
}