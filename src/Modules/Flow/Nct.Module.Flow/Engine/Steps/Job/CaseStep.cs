using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Helpers;
using System.Collections;
using static Nct.Module.Flow.Consts;

namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class CaseStep : Step
    {

        private static bool CompareNumberType(string leftValue, string rightValue, string op, out string error)
        {
            error = null;
            var left = 0m;
            var right = 0m;
            if (op != Operators.Empty && op != Operators.NotEmpty)
            {
                var leftValueNull = leftValue.ToDecimalOrNull();
                if (leftValueNull == null)
                {
                    error = $"{leftValue} 不是一个数字";
                }
                else
                {
                    left = leftValueNull.Value;
                }

                var rightValueNull = rightValue.ToDecimalOrNull();
                if (rightValueNull == null)
                {
                    error = $"{rightValue} 不是一个数字";
                }
                else
                {
                    right = rightValueNull.Value;
                }

            }

            switch (op)
            {
                case Operators.Equal:
                    return left == right;
                case Operators.NotEqual:
                    return left != right;
                case Operators.Greater:
                    return left > right;
                case Operators.GreaterEqual:
                    return left >= right;
                case Operators.Less:
                    return left < right;
                case Operators.LessEqual:
                    return left <= right;
                case Operators.Empty:
                    return leftValue.IsEmpty();
                case Operators.NotEmpty:
                    return !leftValue.IsEmpty();
                default:
                    error = $"操作符 {op} 不支持比较数字类型";
                    return false;

            }



        }

        private static bool CompareStringType(string leftValue, string rightValue, string op, out string error)
        {
            error = null;
            switch (op)
            {
                case Operators.Equal:
                    return leftValue == rightValue;
                case Operators.NotEqual:
                    return leftValue != rightValue;
                case Operators.Empty:
                    return leftValue.IsEmpty();
                case Operators.NotEmpty:
                    return !leftValue.IsEmpty();
                case Operators.Starts:
                    return rightValue.IsEmpty() || leftValue.StartsWith(rightValue);
                case Operators.Ends:
                    return rightValue.IsEmpty() || leftValue.EndsWith(rightValue);
                case Operators.Contains:
                    return rightValue.IsEmpty() || leftValue.Contains(rightValue);
                case Operators.NotContains:
                    return rightValue.IsEmpty() || !leftValue.Contains(rightValue);
                default:
                    error = $"操作符 {op} 不支持比较字符串类型";
                    return false;

            }



        }

        private static bool CompareObjectType(object leftValue, string op, out string error)
        {
            error = null;
            switch (op)
            {
                case Operators.Empty:
                    return leftValue == null;
                case Operators.NotEmpty:
                    return leftValue != null;
                default:
                    error = $"操作符 {op} 不支持比较对象类型";
                    return false;

            }


        }

        private static bool CompareArrayType(object leftValue, string op, out string error)
        {
            error = null;
            var hasEle = false;

            if (leftValue is Array array)
            {
                hasEle = array.Length > 0;

            }
            else if (leftValue is IList list)
            {
                hasEle = list.Count > 0;
            }
            else
            {
                error = $"操作符 {op} 不支持比较非数组类型";
                return false;
            }

            return op switch
            {
                Operators.Empty => leftValue == null || !hasEle,
                Operators.NotEmpty => hasEle,
                _ => false,
            };
        }



        public override Task<IStepExecutionResult> Run(StepExecutionContext context)
        {
            var ctx = (FlowJobStepExecutionContext)context;
            var props = ctx.Node.Properties.To<JobCaseNodeProps>();

            string error = null;
            CaseDef trueCase = null;
            bool caseTrue = false;
            string selectCaseName = null;
            var result = Success();
            var caseIndex = 1;

            foreach (var caseDef in props.Cases)
            {
                if (caseDef.IsElse)
                {
                    trueCase ??= caseDef;
                    continue;
                }
                var caseName = $"Case{caseIndex++}";
                List<object> caseInputs = [];
                var logicalOperator = caseDef.LogicalOperator;
                var logics = new List<bool>();
                foreach (var condition in caseDef.Conditions)
                {
                    var left = condition.Left;
                    var right = condition.Right;
                    var op = condition.ComparisonOperator;
                    var leftTag = FlowHelper.GetVariableTags(left).First();
                    var leftResultValue = FlowHelper.GetVariableValueByTag(leftTag, ctx);
                    if (!leftResultValue.IsSuccess)
                    {
                        error += leftResultValue.Msg;
                        continue;
                    }

                    bool hasRight = op != Operators.Empty && op != Operators.NotEmpty;

                    var rightResultValue = FlowHelper.ReplaceArgTagToValue(right, ctx);
                    if (!rightResultValue.IsSuccess && hasRight)
                    {
                        error += rightResultValue.Msg;
                        continue;
                    }


                    var rightValue = rightResultValue.Data?.ToString();
                    var leftVariableValue = leftResultValue.Data;
                    var leftValue = leftVariableValue.Value;

                    caseInputs.Add(new { left = leftValue, @operator = op, right = hasRight ? rightValue : null });
                    result.Inputs.Add(caseName, caseInputs);

                    string compareErr = null;
                    switch (leftVariableValue.Type)
                    {
                        case VariableType.String:
                            logics.Add(CompareStringType(leftValue?.ToString(), rightValue, op, out compareErr));
                            break;

                        case VariableType.Number:
                            logics.Add(CompareNumberType(leftValue?.ToString(), rightValue, op, out compareErr));
                            break;
                        case VariableType.Object:
                            logics.Add(CompareObjectType(leftValue, op, out compareErr));
                            break;
                        case VariableType.ArrayString:
                        case VariableType.ArrayObject:
                        case VariableType.ArrayNumber:
                            logics.Add(CompareArrayType(leftValue, op, out compareErr));
                            break;

                    }

                    error += compareErr;


                }

                if (logicalOperator == LogicalOperator.And)
                {
                    if (logics.Count > 0 && logics.All(x => x))
                    {
                        caseTrue = true;
                        selectCaseName = caseName;
                        trueCase = caseDef;
                        break;
                    }

                }
                else if (logicalOperator == LogicalOperator.Or)
                {
                    if (logics.Any(x => x))
                    {
                        caseTrue = true;
                        selectCaseName = caseName;
                        trueCase = caseDef;
                        break;
                    }
                }
            }

            if (error.IsNotEmpty())
            {
                result.Success = false;
                result.Exception = error;
                return Task.FromResult(result);

            }

            var edges = ctx.FlowConfig.Edges.Where(m => trueCase.EdgeIds.Contains(m.Id)).ToList();
            var nextNodes = ctx.FlowConfig.Nodes.Where(m => edges.Any(n => n.TargetNodeId == m.Id)).ToArray();
            result.NextNodes = nextNodes;
            result.Outputs.Add("result", caseTrue);
            result.Outputs.Add("select_case", selectCaseName ?? "else");

            return Task.FromResult(result);



        }




    }
}
