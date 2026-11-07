using Json.Path;
using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Helpers;
using System.Text.Json;
using System.Text.Json.Nodes;
using static Nct.Module.Flow.Consts;

namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class JsonDataStep : Step
    {

        private static Result GetJsonNodeValue(string path, string dataType, JsonNode jsonNode)
        {
            var result = new Result();
            if (dataType == VariableType.Number)
            {
                if (jsonNode.TryGetValue(out double dNumber))
                {
                    result.Data = dNumber;
                }
                else
                {
                    return result.Error($"{path} 不是数字;");
                }
            }
            else if (dataType == VariableType.String)
            {
                if (!(jsonNode.GetValueKind() == JsonValueKind.String || jsonNode.GetValueKind() == JsonValueKind.Null))
                {
                    return result.Error($"{path} 不是字符串;");

                }
                result.Data = jsonNode?.GetValue<string>();


            }
            else if (dataType == VariableType.Object)
            {
                if (jsonNode.GetValueKind() != JsonValueKind.Object)
                {
                    return result.Error($"{path} 不是对象;");

                }
                result.Data = jsonNode.AsObject();
            }
            else
            {
                result.Error("不支持的类型");
            }
            return result;
        }


        public override Task<IStepExecutionResult> Run(StepExecutionContext context)
        {
            var ctx = (FlowJobStepExecutionContext)context;
            var props = context.Node.Properties.To<JobJsonDataNodeProps>();

            var inputArgTag = FlowHelper.GetVariableTags(props.Input).First();
            var inputResultValue = FlowHelper.GetVariableValueByTag(inputArgTag, ctx);
            if (!inputResultValue.IsSuccess)
                return Task.FromResult(Fail(inputResultValue.Msg));


            var value = inputResultValue.Data.Value?.ToString();
            JsonNode jsonNode;
            try
            {
                jsonNode = JsonNode.Parse(value);

            }
            catch (Exception ex)
            {
                return Task.FromResult(Fail("输入变量解析出错："+ex.Message));
            }


            IStepExecutionResult stepResult = new StepExecutionResult
            {
                Success = false
            };
            string error = null;

            foreach (var data in props.DataItems)
            {
                var path = data.Path;
                var dataType = data.DataType;
                var jsonPath = JsonPath.Parse(path);
              
                var nodeList = jsonPath.Evaluate(jsonNode).Matches;
                if (nodeList.Count == 0)
                {
                    error += $"{path} 未找到;";
                    continue;
                }

                if (dataType.StartsWith("Array"))
                {
                    var dataTypeInner = dataType.Substring(6, dataType.Length-7);
                    var list = new List<object>();
                    foreach (var node in nodeList)
                    {
                        var result = GetJsonNodeValue(data.Path, dataTypeInner, node.Value);
                        if (!result.IsSuccess)
                        {
                            error += $"{path} 不是{dataType}";
                            break;
                        }
                        else
                        {
                            list.Add(result.Data);
                        }
                    }

                    stepResult.Outputs[data.Name] = list;

                }
                else
                {
                    var result = GetJsonNodeValue(data.Path, dataType, nodeList[0].Value);
                    if (!result.IsSuccess)
                    {
                        error += result.Msg;
                    }
                    else
                    {
                        stepResult.Outputs[data.Name] = result.Data;
                    }

                }

            }

            stepResult.Exception = error;
            if (error.IsEmpty())
            {
                stepResult.Success = true;
                stepResult.NextNodes = context.FlowConfig.GetNextNodes(context.Node);
            }
          
            return Task.FromResult(stepResult);




        }

    }
}
