import { BaseNode, BaseNodeModel } from './BaseNode';
import vueComponent from './HttpNode.vue';

class HttpNode extends BaseNode {

    constructor(props) {
        super(props)
        this.vueComponent = vueComponent;

    }
}

class HttpNodeModel extends BaseNodeModel {


    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'JobHttp',
                Headers: [{ Name: "", Value: "" }],
                Params: [{ Name: "", Value: "" }],
                Body: { Type: 'none', Data: [] },
                Timeout: null,
                Method: 'GET',
                Url: '',
                RetryConfig: {
                    MaxRetryTimes: 3,
                    Enabled: false,
                    Interval: 100,
                },

            };
        }
        super.initNodeData(data);
        this.height = this.getDefaultHeight() * 2;
        this.width = 260;

    }



    getOutputs() {
        return [
            {
                Name: 'body',
                Type: 'String'
            },
            {
                Name: 'status_code',
                Type: 'Number'
            },
            {
                Name: 'headers',
                Type: 'Object'
            },
            {
                Name: 'file',
                Type: 'String'
            },
        ]

    }


    validate() {
        const { properties } = this;
        super.validate();
        if (!properties.Url) {
            super.addWarning('请设置接口地址');
        }

  

        for (let item of properties.Headers) {

            if (item.Name || item.Value) {

                if (!item.Name) {
                    super.addWarning(`请设置请求头名称`);
                    break;
                }
                if (!item.Value) {
                    super.addWarning('请设置请求头值');
                    break;
                }
            }
        }

        for (let item of properties.Params) {
            if (item.Name || item.Value) {
                if (!item.Name) {
                    super.addWarning(`请设置参数名称`);
                    break;
                }
                if (!item.Value) {
                    super.addWarning('请设置参数值');
                    break;
                }
            }
        }

        if (properties.RetryConfig.Enabled) {
            if (!properties.RetryConfig.MaxRetryTimes && properties.RetryConfig.MaxRetryTimes!=0) {
                super.addWarning('请设置最大重试次数');
            
            }
            if (!properties.RetryConfig.Interval && properties.RetryConfig.Interval!=0) {
                super.addWarning('请设置重试间隔');
            }
        }



        return super.isValid();

    }

}

export default {
    type: 'http',
    model: HttpNodeModel,
    view: HttpNode
}