using Nct.Module.Core;
using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Helpers;
using RestSharp;
using StackExchange.Redis;
using System.Net;
using static Nct.Module.Flow.Consts;

namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class HttpStep : Step
    {


        private static void ResolveVariables(List<Variable> variables, StepExecutionContext context, Action<Variable, string, string> callback)
        {

            foreach (var item in variables)
            {
                if (item.Name.IsEmpty() || item.Value.IsEmpty()) continue;
                var result = FlowHelper.ReplaceArgTagToValue(item.Value, context);
                if (!result.IsSuccess)
                {
                    callback(item, null, result.Msg);
                    return;
                }

                var value = result.Data;
                if (value.IsEmpty()) continue;

                callback(item, value, null);
            }

        }
        public override async Task<IStepExecutionResult> Run(StepExecutionContext context)
        {

            var ctx = (FlowJobStepExecutionContext)context;
            var props = ctx.Node.Properties.To<JobHttpNodeProps>();

            var result = FlowHelper.ReplaceArgTagToValue(props.Url, context);
            if (!result.IsSuccess)
                return Fail(result.Msg);

            var url = result.Data;
            var stepResult = new StepExecutionResult();
            Dictionary<string, string> inHeaders = [];
            stepResult.Inputs["headers"] = inHeaders;

            Method method = props.Method.ToUpper() switch
            {
                "POST" => Method.Post,
                "GET" => Method.Get,
                "PUT" => Method.Put,
                "DELETE" => Method.Delete,
                _ => Method.Post
            };

            var request = new RestRequest("", method);
            string error = null;
            if (props.Headers != null)
            {
                ResolveVariables(props.Headers, context, (item, value, msg) =>
                {
                    if (msg.IsNotEmpty())
                    {
                        error += $"请求头参数【{item.Name}】：{msg}{Environment.NewLine}";
                    }
                    else
                    {
                        request.AddHeader(item.Name, value);
                        inHeaders[item.Name] = value;
                    }
                });

            }

            if (props.Params != null)
            {
                ResolveVariables(props.Params, context, (item, value, msg) =>
                {
                    if (msg.IsNotEmpty())
                    {
                        error += $"url参数【{item.Name}】：{msg}{Environment.NewLine}";
                    }
                    else
                    {
                        if (!url.Contains('?'))
                            url += "?";

                        url += $"{item.Name}={value.UrlEncode()}&";
                    }
                });
                url = url.TrimEnd('&');
            }


            if (props.Body != null)
            {

                if (props.Body.Type == HttpBodyType.Json)
                {
                    var bodyArg = props.Body.Data.FirstOrDefault(m => m.Name.IsEmpty());
                    if (bodyArg != null)
                    {
                        bodyArg.Name = "body";
                        ResolveVariables([bodyArg], context, (item, value, msg) =>
                        {
                            if (msg.IsNotEmpty())
                            {
                                error += $"body：{msg}{Environment.NewLine}";
                            }
                            else
                            {
                                stepResult.Inputs["body"] = value;
                                request.AddJsonBody(value);
                            }
                        });
                    }

                }

                else if (props.Body.Type == HttpBodyType.FormUrlEncoded || props.Body.Type == HttpBodyType.FormData)
                {
                    var bodyArgs = props.Body.Data.Where(m => !m.Name.IsEmpty()).ToList();
                    var args = "";
                    ResolveVariables(bodyArgs, context, (item, value, msg) =>
                    {
                        if (msg.IsNotEmpty())
                        {
                            error += $"body参数【{item.Name}】：{msg}{Environment.NewLine}";
                        }
                        else
                        {

                            request.AddParameter(item.Name, value);
                            args += $"{item.Name}={value.UrlEncode()}&";
                        }
                    });
                    args = args.TrimEnd('&');
                    stepResult.Inputs["body"] = args;

                }

                else if (props.Body.Type == HttpBodyType.Raw || props.Body.Type == HttpBodyType.None)
                {
                    request.RequestFormat = DataFormat.None;
                }

            }

            if (error.IsNotEmpty())
                return Fail(error);


            if (props.Timeout > 0)
                request.Timeout = TimeSpan.FromSeconds(props.Timeout.Value);


            var tryNum = 1;
            var interval = 0;
            if (props.RetryConfig.Enabled)
            {
                if (props.RetryConfig.MaxRetryTimes > 0)
                    tryNum = props.RetryConfig.MaxRetryTimes + 1;
                interval = props.RetryConfig.Interval;
            }


            string body = null;
            string file = null;
            int statusCode = 0;
            Dictionary<string, string> headers = [];
            var success = false;

            var errNum = 1;

            using var client = new RestClient(url);
            while (tryNum > 0)
            {
                try
                {
                    var response = await client.ExecuteAsync(request);
                    statusCode = (int)response.StatusCode;
                    body = response.Content;
                    headers = response.Headers.ToDictionary(m => m.Name, m => m.Value);

                    if (response.Cookies != null)
                    {
                        foreach (var cookie in response.Cookies.Cast<Cookie>())
                        {
                            headers[cookie.Name] = cookie.Value;
                        }
                    }

                    if (response.IsSuccessful)
                    {
                        var contentDisposition = response.Headers.FirstOrDefault(h => h.Name == "Content-Disposition")?.Value?.ToString();
                        if (!string.IsNullOrEmpty(contentDisposition) && contentDisposition.Contains("attachment"))
                        {
                            var path = $"upload/jobfile/{Guid.NewGuid():n}";
                            FileStoreHelper.SaveFile(path, new MemoryStream(response.RawBytes), true);
                            file = PathHelper.WebPath(path);

                        }

                    }
                    success = true;
                    break;

                }
                catch (Exception ex)
                {
                    statusCode = 500;
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
            stepResult.Inputs.Add("url", url);
            stepResult.Inputs.Add("method", method.ToString());

            stepResult.Outputs.Add("status_code", statusCode);
            stepResult.Outputs.Add("headers", headers);
            stepResult.Outputs.Add("body", body);
            stepResult.Outputs.Add("file", file);
            if (success)
            {
                stepResult.NextNodes = context.FlowConfig.GetNextNodes(context.Node);
            }

            return stepResult;
        }

    }
}
