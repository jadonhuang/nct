using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Helpers;
using RestSharp;
using System.Text.Json.Nodes;

namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class AiModelStep : Step
    {
        public override async Task<IStepExecutionResult> Run(StepExecutionContext context)
        {
            var ctx = (FlowJobStepExecutionContext)context;
            var props = ctx.Node.Properties.To<JobAiModelNodeProps>();
            var stepResult = new StepExecutionResult();

            // 解析 API Key
            var apiKeyResult = FlowHelper.ReplaceArgTagToValue(props.ApiKey, context);
            if (!apiKeyResult.IsSuccess)
                return Fail(apiKeyResult.Msg);
            var apiKey = apiKeyResult.Data;

            // 解析提示词
            var promptResult = FlowHelper.ReplaceArgTagToValue(props.Prompt, context);
            if (!promptResult.IsSuccess)
                return Fail(promptResult.Msg);
            var prompt = promptResult.Data;

            // 解析 API 地址（如果有）
            string apiUrl = props.ApiUrl;
            if (!string.IsNullOrEmpty(apiUrl))
            {
                var apiUrlResult = FlowHelper.ReplaceArgTagToValue(apiUrl, context);
                if (!apiUrlResult.IsSuccess)
                    return Fail(apiUrlResult.Msg);
                apiUrl = apiUrlResult.Data;
            }

            // 根据模型类型确定 API 端点
            var (endpoint, requestBody) = BuildRequest(props.ModelType, prompt, props);

            // 如果没有指定 API 地址，使用默认地址
            if (string.IsNullOrEmpty(apiUrl))
            {
                apiUrl = GetDefaultApiUrl(props.ModelType);
            }

            stepResult.Inputs["prompt"] = prompt;
           
            var tryNum = 1;
            var interval = 0;
            if (props.RetryConfig != null && props.RetryConfig.Enabled)
            {
                if (props.RetryConfig.MaxRetryTimes > 0)
                    tryNum = props.RetryConfig.MaxRetryTimes + 1;
                interval = props.RetryConfig.Interval;
            }

            string responseContent = null;
            var success = false;
            string error = null;
            var errNum = 1;

            var request = new RestRequest(endpoint, Method.Post);
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(requestBody);

            using var client = new RestClient(apiUrl);
            while (tryNum > 0)
            {
                try
                {
                    var response = await client.ExecuteAsync(request);
                    if (response.IsSuccessful)
                    {
                        responseContent = response.Content;
                        success = true;
                        break;
                    }
                    else
                    {
                        error = $"API 请求失败: {response.StatusCode} - {response.Content}";
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                tryNum--;
                if (tryNum > 0)
                {
                    if (interval > 0)
                        await Task.Delay(interval);

                    var nodeStatusChangeArgs = new NodeStatusChangeArgs(ctx.Node.Id, NodeStatus.Running)
                    {
                        Data = $"第{errNum++}次重试"
                    };
                    ctx.NodeStatusChanged?.Invoke(nodeStatusChangeArgs);
                }
            }

            stepResult.Success = success;
            stepResult.Exception = error;

            if (success)
            {
                var aiResponse = ExtractResponse(responseContent, props.ModelType);
                stepResult.Outputs.Add("text", aiResponse);
                stepResult.NextNodes = context.FlowConfig.GetNextNodes(context.Node);
            }

            return stepResult;
        }

        private (string endpoint, object requestBody) BuildRequest(string modelType, string prompt, JobAiModelNodeProps props)
        {
            var requestBody = new
            {
                model = modelType,
                messages = new[]
              {
                    new { role = "user", content = prompt }
                },
                temperature = props.Temperature,
                max_tokens = props.MaxTokens,

            };
            return ("chat/completions", requestBody);
        }

      


        private string GetDefaultApiUrl(string modelType)
        {

            var lowerModelType = modelType.ToLower();
            if (lowerModelType.StartsWith("deepseek"))
                return "https://api.deepseek.com";
            if (lowerModelType.StartsWith("doubao"))
                return "https://ark.cn-beijing.volces.com/api/v3";

            return "";
          
        }

        private string ExtractResponse(string responseContent, string modelType)
        {
            try
            {
                var jsonNode = JsonNode.Parse(responseContent);
                return jsonNode?["choices"]?[0]?["message"]?["content"]?.GetValue<string>() ?? "";
            }
            catch
            {
                return responseContent;
            }
        }
 
    }
}
