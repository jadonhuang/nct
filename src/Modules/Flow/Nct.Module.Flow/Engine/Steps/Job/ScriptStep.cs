using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Helpers;
using Nct.Scripting;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class ScriptStep : Step
    {

        public Script Script { get; set; }


        private void AddOutputError(ref string error, string argName)
        {
            error += $"无法获取输出变量{argName}；";
        }

        public override async Task<IStepExecutionResult> Run(StepExecutionContext context)
        {
            var ctx = (FlowJobStepExecutionContext)context;
            var props = ctx.Node.Properties.To<JobScriptNodeProps>();

            string error = null;
            var args = new ExpandoObject();
            var argDic = (IDictionary<string, object>)args;
            foreach (var item in props.Variables)
            {
                if (item.Name.IsEmpty() || item.Value.IsEmpty()) continue;
                var tag = FlowHelper.GetVariableTags(item.Value).First();
                var varResult = FlowHelper.GetVariableValueByTag(tag, ctx);
                if (!varResult.IsSuccess)
                {
                    error += varResult.Msg;
                    continue;
                }
                argDic[item.Name] = varResult.Data.Value;

            }

            if (error != null)
                return Fail(error);

            var classCode = Script.BuildCodeToClass(props.Code);
            var instance = Script.GetBaseScriptInstance(classCode);

            var isAsync = Regex.Match(props.Code, @"\s+Task<object>\s+ExecuteAsync", RegexOptions.IgnoreCase).Success;

            var tryNum = 1;
            var interval = 0;
            object exeResult = null;
            if (props.RetryConfig.Enabled)
            {
                if (props.RetryConfig.MaxRetryTimes > 0)
                    tryNum = props.RetryConfig.MaxRetryTimes + 1;
                interval = props.RetryConfig.Interval;
            }

            while (tryNum > 0)
            {
                try
                {
                    if (isAsync)
                    {
                        exeResult = await instance.ExecuteAsync(args);
                    }
                    else
                    {
                        exeResult = instance.Execute(args);
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                }

                tryNum--;
                if (tryNum > 0 && interval > 0)
                    await Task.Delay(interval);

            }
            if (error != null)
                return Fail(error);

            var result = Success();
            result.Inputs = argDic.ToDictionary(x => x.Key, x => x.Value);


            foreach (var item in props.Outputs)
            {
                if (item.Name.IsEmpty() || item.Type.IsEmpty()) continue;
                if (exeResult is IDictionary<string, object> dic)
                {
                    if (dic.TryGetValue(item.Name, out var value))
                    {
                        result.Outputs[item.Name] = value;
                    }
                    else
                    {
                        AddOutputError(ref error, item.Name);
                    }

                }
                else if (exeResult != null)
                {
                    try
                    {

                        var value = exeResult.GetType().GetProperty(item.Name).GetValue(exeResult);
                        result.Outputs[item.Name] = value;
                    }
                    catch
                    {
                        AddOutputError(ref error, item.Name);

                    }
                }
                else
                {

                    AddOutputError(ref error, item.Name);
                }

            }

            if (error == null)
            {
                result.NextNodes = context.FlowConfig.GetNextNodes(context.Node);
                return result;
            }

            result.Success = false;
            result.Exception = error;
            return result;




        }
    }
}
