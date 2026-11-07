import { BaseNode, BaseNodeModel } from './BaseNode';
import vueComponent from './AiModelNode.vue';

class AiModelNode extends BaseNode {

    constructor(props) {
        super(props)
        this.vueComponent = vueComponent;

    }
}

class AiModelNodeModel extends BaseNodeModel {


    initNodeData(data) {

        if (!data.properties) {
            data.properties = {
                PropType: 'JobAiModel',
                ModelType: 'deepseek-chat',
                ApiKey: '',
                ApiUrl: '',
                Prompt: '',
                Temperature: 0.7,
                MaxTokens: 4096,        
                RetryConfig: {
                    MaxRetryTimes: 3,
                    Enabled: false,
                    Interval: 200,
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
                Name: 'text',
                Type: 'String'
            }
          
        ]

    }
    getModels() {
        return [
            {
                Name: 'DeepSeekV3.2',
                Value: 'deepseek-chat'
            },
            {
                Name: 'Doubao-Seed-2.0-Code',
                Value: 'doubao-seed-2-0-code-preview-260215'
            }
        ]
    }


    validate() {
        const { properties } = this;
        super.validate();
        if (!properties.ApiKey) {
            super.addWarning('请设置 API Key');
        }
        if (!properties.Prompt) {
            super.addWarning('请设置提示词');
        }
        if (!properties.MaxTokens || properties.MaxTokens <= 0) {
            super.addWarning('请设置有效的最大 Token 数');
        }
        if (properties.Temperature < 0 || properties.Temperature > 2) {
            super.addWarning('Temperature 应在 0-2 之间');
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
    type: 'aiModel',
    model: AiModelNodeModel,
    view: AiModelNode
}
