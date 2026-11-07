
using Nct.Logging;
using Nct.Module.Flow.Engine.Models;
using Nct.Module.Form.FormDefs;
using Nct.Scripting;
using static Nct.Module.Flow.Consts;

namespace Nct.Module.Flow.Engine.Steps
{
    public class CaseStep(IFormDefAppService formDefAppService, Script script) : Step
    {

        private readonly IFormDefAppService _formDefAppService = formDefAppService;
        private readonly Script _script = script;

        public override Task<IStepExecutionResult> Run(StepExecutionContext ctx)
        {
            var context= (ApprovalStepExecutionContext)ctx;
            var node = context.Node;
            var flowConfig = context.FlowConfig;
            var startNode = flowConfig.Nodes.First(m => m.Type == NodeType.Start);
            var startNodeProps = startNode.Properties.To<StartNodeProps>();
            var formCode = startNodeProps.FormCode;
            var formDesign = _formDefAppService.GetDesignInfoByCode(formCode, out _);
            var tableName = formDesign.DbTables[0].Name;
            var props = node.Properties.To<CaseNodeProps>();


            NodeCdt trueCdt = null;

            foreach (var cdt in props.Cdts)
            {
                var flag = false;
             
                if (cdt.SqlCdt.IsNotEmpty())
                {
                    var sql = $"select count(1) from {tableName} where (({cdt.SqlCdt})) and id=@FormDataId";
                    var num = Db.ExecuteScalar<int>(sql,new { context.Flow.FormDataId });
                    flag = num > 0;

                }

                if (flag && (cdt.CdtType == 1 || cdt.CodeCdt.IsEmpty()))
                {
                    trueCdt = cdt;
                    break;
                }

                if (!flag && cdt.CdtType == 2)
                    continue;


                if (cdt.CodeCdt.IsNotEmpty())
                {

                    flag = _script.Evaluate<bool>(cdt.CodeCdt, context);
                    if (flag)
                    {
                        trueCdt = cdt;
                        break;
                    }

                }
            }

            if (trueCdt == null) 
            {
                trueCdt = props.Cdts.Find(m => m.IsDefault);

            }
            var result = Success();
            if (trueCdt == null)
            {
                Logger.Debug($"{context.FlowDef.Name} 节点 {node.Text.Value} 无任何满足条件分支");

            }
            else
            {
                var edge = flowConfig.Edges.Find(m => m.Id == trueCdt.EdgeId);
                var nextNode = flowConfig.Nodes.Find(m => m.Id == edge.TargetNodeId);
                result.NextNodes = [nextNode];
            }
            return Task.FromResult(result);


        }


    }
}
