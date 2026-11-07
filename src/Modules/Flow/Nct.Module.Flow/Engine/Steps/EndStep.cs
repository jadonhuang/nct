using Nct.LinqQuery;
using Nct.Module.Core.Events;
using Nct.Module.Flow.Flows;
using AuditStatus = Nct.Module.Flow.Consts.AuditStatus;
using TaskStatus = Nct.Module.Flow.Consts.TaskStatus;

namespace Nct.Module.Flow.Engine.Steps
{
    public class EndStep : Step
    {

        public IRepository<FlowTask> TaskRep { get; set; }

        public IRepository<Flows.Flow> FlowRep { get; set; }

        public IRepository<TaskItem> TaskItemRep { get; set; }



        public override async Task<IStepExecutionResult> Run(StepExecutionContext ctx)
        {
            var context = (ApprovalStepExecutionContext)ctx;
            var taskItem = context.TaskItem;
            var taskStatus = TaskStatus.Passed;
            var flowStatus = Consts.FlowStatus.Passed;
            var flow = context.Flow;
            string msgTitle;
            string msgContent;

            var refuseNum = await TaskItemRep.CountAsync(m => m.TaskId == taskItem.TaskId && m.Status == AuditStatus.NotPassed);

            if (refuseNum > 0)
            {
                taskStatus = TaskStatus.NotPassed;
                flowStatus = Consts.FlowStatus.NotPassed;
                msgTitle = "审核失败提醒";
                msgContent = $"您的申请【{flow.Title}】未通过审核";
            }
            else
            {
                msgTitle = "审核成功提醒";
                msgContent = $"您的申请【{flow.Title}】已通过审核";
            }

            await TaskRep.GetAll().Where(m => m.Id == taskItem.TaskId).UpdateAsync(m => new Flows.FlowTask { Status = taskStatus });
            await FlowRep.GetAll().Where(m => m.Id == flow.Id).UpdateAsync(m => new Flows.Flow { Status = flowStatus, EndTime = DateTime.Now, IsEnd = true });



            var msg = new Msg
            {
                Title = msgTitle,
                Content = msgContent,
                Type = 1,
                UserId = context.Flow.CreatorId,

            };

            Notify(msg);

            return Success();

        }
    }
}
