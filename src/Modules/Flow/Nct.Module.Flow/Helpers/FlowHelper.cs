using Nct.Module.Flow.Engine;
using Nct.Module.Flow.Engine.Models;
using System.Text.RegularExpressions;

namespace Nct.Module.Flow.Helpers
{
    public static partial class FlowHelper
    {

        [GeneratedRegex(@"\{\{#(.*?)#\}\}")]
        private static partial Regex VarTagRegex();
        public static ResultOfT<string> ReplaceArgTagToValue(string content, StepExecutionContext context)
        {
            var result = new ResultOfT<string>();
            if (content.IsEmpty())
            {
                return result.Success(content);
            }

            string errMsg = null;
            var tags = GetVariableTags(content);
            foreach (var tag in tags)
            {
                var tagResult = GetVariableValueByTag(tag, context);
                if (!tagResult.IsSuccess)
                {
                    errMsg += tagResult.Msg;
                    continue;
                }
                var value = tagResult.Data as VariableValue;
                content = content.Replace(tag.Raw, value.Value?.ToString());

            }
            if (errMsg != null)
                result.Error(errMsg);

            return result.Success(content);

        }



        public static List<VariableTag> GetVariableTags(string content)
        {
            var matchs = VarTagRegex().Matches(content);
            var tags = new List<VariableTag>();
            foreach (var match in matchs.Cast<Match>())
            {
                var key = match.Groups[1].Value;
                var parts = key.Split('.');
                if (parts.Length != 2)
                {
                    continue;
                }

                tags.Add(new VariableTag
                {
                    Name = parts[1],
                    NodeId = parts[0],
                    Raw = match.Value
                });
            }

            return tags;

        }


        public static ResultOfT<VariableValue> GetVariableValueByTag(VariableTag tag, StepExecutionContext context)
        {
            var result = new ResultOfT<VariableValue>();
            var nodeId = tag.NodeId;
            var argName = tag.Name;
            string errMsg = null;
            FlowNode node;
            var nodes = context.FlowConfig.Nodes;
            if (tag.NodeId == "sys")
            {
                node = nodes.FirstOrDefault(m => m.Type == "start");
                argName = $"sys.{argName}";
            }
            else
            {
                node = nodes.First(m => m.Id == nodeId);
            }

            if (!context.Outputs.TryGetValue(node.Id, out var nodeOutputs))
            {
                errMsg += $"参数未找到：{argName}；";
            }
            else if (!nodeOutputs.TryGetValue(argName, out var value))
            {
                errMsg += $"参数未找到：{argName}；";
            }
            else
            {
                var type = node.Properties.GetOutputVariables().FirstOrDefault(m => m.Name == argName).Type;
                var variable = new VariableValue
                {
                    Name = argName,
                    Value = value,
                    Type = type
                };
                return result.Success(variable);
            }
            return result.Error(errMsg);

        }




    }
}
