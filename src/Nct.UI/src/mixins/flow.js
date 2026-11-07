export default {
    methods: {
        getParentNodes(node) {
            var nodes = [];
            function getIncoming(node) {

                var edges = node.incoming.edges;
                for (var edge of edges) {

                    var srcNode = edge.sourceNode;
                    if (nodes.find(m => m.id == srcNode.id) == null)
                        nodes.splice(0, 0, srcNode);
                    getIncoming(srcNode);
                }

            }

            getIncoming(node);
            return nodes;


        },
        getParentNodeVariables(node) {

            var nodes = this.getParentNodes(node);
            var nodeVariables = this.getNodeVariables(nodes);
            return nodeVariables;
        },

        getNodeVariables(nodes) {

            var nodeVariables = [];
            for (let node of nodes) {
                var outputs = node.getOutputs && node.getOutputs() || [];
                if (outputs.length > 0) {
                    var nodeVariable = { type: node.type, id: node.id, text: node.text.value, items: outputs }
                    nodeVariables.push(nodeVariable)
                }
            }

            return nodeVariables;

        },

        tranVariableTextToObject(nodeVariables, variable) {
            if (nodeVariables.length == 0) return null;
            if (variable.indexOf('{') == 0) {
                variable = variable.substring(3, variable.length - 3);
            }

            var parts = variable.split('.');
            var nodeId = parts[0];
            var varName = parts[1];
            if (nodeId == "sys") {
                let startNode = nodeVariables.find(m => m.type == 'start');
                if (startNode) {
                    nodeId = startNode.id;
                }
            }

            var v = nodeVariables.find(m => m.id == nodeId);
            if (v) {
                var varItem = v.items.find(m => m.Name == varName || m.Name == variable);
                return { text: v.text, name: varItem.Name, type: varItem.Type }
            }
            return null;
        },

        tranVariableTextToHtml(nodeVariables, content) {
            if (!content) return content;
            const self = this;
            content = content.replace(/\{\{#(.*?)#\}\}/g, function (match, variable) {

                let html = "";
                var varItem = self.tranVariableTextToObject(nodeVariables, variable);
                if (varItem) {
                    html = `<span contenteditable="false" class="tag" data-var="{{#${variable}#}}">${varItem.text}/<span class="tag-title">(x) ${varItem.name}</span></span>`;
                } else {
                    html = `<span contenteditable="false" title="参数不存在" class="tag tag-err" data-var="{{#${variable}#}}">/<span class="tag-title">(x) ${variable}</span></span>`;

                }
                return html;

            });

            content = content.replaceAll("\n", "<br>");
            return content;


        },

        tranVariableTextToDisplayHtml(content) {
            if (!content) return content;
            content = content.replace(/\{\{#(.*?)#\}\}/g, function (match, variable) {

                var parts = variable.split('.');
                var varName = parts[1];
                if (parts[0] == "sys") {
                    varName = variable;
                }

                let html = `<span class="var-name">${varName}</span>`;
                return html;

            });

            content = content.replaceAll("\n", "");
            return content;


        }




    }
};