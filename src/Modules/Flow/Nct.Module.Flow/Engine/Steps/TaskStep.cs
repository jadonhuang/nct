using Nct.LinqQuery;
using Nct.Module.Core.Events;
using Nct.Module.Core.Organizations;
using Nct.Module.Core.Users;
using Nct.Module.Core.Users.Dtos;
using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Flows;
using AuditStatus = Nct.Module.Flow.Consts.AuditStatus;
using AuditType = Nct.Module.Flow.Consts.AuditType;
using NodeType = Nct.Module.Flow.Consts.NodeType;
using TaskStatus = Nct.Module.Flow.Consts.TaskStatus;

namespace Nct.Module.Flow.Engine.Steps
{
    public class TaskStep : Step
    {
 
        private readonly IUserAppService _userAppService;
        private readonly IOrganizationAppService _orgAppService;

        public IRepository<FlowTask> TaskRep { get; set; }
        public IRepository<TaskItem> TaskItemRep { get; set; }

        public TaskStep(IOrganizationAppService orgAppService, IUserAppService userAppService)
        {

            _orgAppService = orgAppService;
            _userAppService = userAppService;
        }

        private async Task<TaskItem> CreateTaskItem(string flowName, UserDto user, FlowNode node, int taskId, Flows.Flow flow, int status)
        {

            var taskItem = new TaskItem
            {
                NodeId = node.Id,
                Status = status,
                NodeName = $"{flowName}({node.Text.Value})",
                UserName = user.GetDisplayName(),
                UserId = user.Id,
                TaskId = taskId,
                FlowId = flow.Id
            };
            await TaskItemRep.InsertAsync(taskItem);

            return taskItem;


        }


        private void SendNofityMsg(UserDto user, TaskItem taskItem, Flows.Flow flow)
        {

            var msg = new Msg
            {
                Title = "审核任务提醒",
                Content = $"您有新的审核任务【{taskItem.NodeName}】，申请人【{flow.CreatorName}】，请尽快处理",
                Type = 1,
                UserId = user.Id,
                Props = new { url = $"/flow/view?taskId={taskItem.Id}&id={taskItem.FlowId}" }.ToJson(),

            };

            Notify(msg);

        }


        private async Task<List<UserDto>> GetAuditUsers(int creatorId, List<AuditMember> auditMembers)
        {

            var users = new List<UserDto>();
            var userAdded = false;

            foreach (AuditMember auditMember in auditMembers)
            {
                if (auditMember.Type == Consts.AuditMemberType.User)
                {
                    if (userAdded) continue;
                    userAdded = true;
                    var userIds = auditMembers.Where(m => m.Type == Consts.AuditMemberType.User).Select(m => m.Id).ToArray();
                    var userList = await _userAppService.GetListByIds(userIds);
                    users.AddRange(userList);
                }
                else if (auditMember.Type == Consts.AuditMemberType.Leader)
                {
                    var userList = await _orgAppService.GetManagers(creatorId, auditMember.Id);
                    users.AddRange(userList);
                }
                else if (auditMember.Type == Consts.AuditMemberType.Role)
                {
                    var userList = await _userAppService.GetListByRoleId(auditMember.Id);
                    users.AddRange(userList);
                }

            }

            return users;

        }


        public override async Task<IStepExecutionResult> Run(StepExecutionContext ctx)
        {
            var context = (ApprovalStepExecutionContext)ctx;
            IStepExecutionResult result = new StepExecutionResult();
            var flowConfig = context.FlowConfig;
            var node = context.Node;
            var props = node.Properties.To<TaskNodeProps>();
            var preNode = context.PreNode;
            var curTaskItem = context.TaskItem;
            var flow = context.Flow;
            var flowName = context.FlowDef.Name;
            var auditMembers = props.Audit.Members;

            if (preNode != null)
            {
               
                var auditUsers = await GetAuditUsers(flow.CreatorId, auditMembers);

                if (auditUsers.Count == 0) return result;

                var task = new FlowTask
                {
                    NodeName = node.Text.Value,
                    NodeId = node.Id,
                    FlowId = flow.Id,
                    Status = TaskStatus.NotStart,
                };

                if (props.Audit.TimeLimit.Enabled)
                {
                    var timeOut = props.Audit.TimeLimit.Timeout;
                    if (timeOut > 0)
                    {
                        var unit = props.Audit.TimeLimit.Unit;
                        if (unit == 1) timeOut = timeOut * 60 * 24;
                        else if (unit == 2) timeOut *= 60;
                    }
                    task.TimeoutNotice = timeOut;

                }

                await TaskRep.InsertAsync(task);

                bool first = true;

                foreach (var user in auditUsers)
                {

                    var needNotify = props.Audit.Type != AuditType.Order || first;
                    var status = needNotify ? AuditStatus.NotStart : AuditStatus.Null;
                    var taskItem = await CreateTaskItem(flowName, user, node, task.Id, flow, status);

                    if (needNotify)
                    {
                        SendNofityMsg(user, taskItem, flow);
                    }

                    first = false;

                }

                return result;
            }

            Msg msg = null;
            var taskId = curTaskItem.TaskId;
            await TaskRep.GetAll().Where(m => m.Id == taskId && m.Status == TaskStatus.NotStart).UpdateAsync(m => new FlowTask { Status = TaskStatus.Started });
            curTaskItem.AuditTime = DateTime.Now;
            await TaskItemRep.UpdateAsync(curTaskItem);

            string msgContent;

            if (curTaskItem.Status == AuditStatus.Passed)
            {

                var userNum = await TaskItemRep.CountAsync(m => m.TaskId == taskId);

                var isPassed = false;

                if (props.Audit.Type == AuditType.Any)
                {
                    isPassed = true;

                }
                else if (props.Audit.Type == AuditType.All)
                {
                    var passedNum = await TaskItemRep.CountAsync(m => m.TaskId == taskId && m.Status == AuditStatus.Passed);
                    if (passedNum * 100 / userNum >= props.Audit.Percent)
                    {
                        isPassed = true;
                    }

                }
                else if (props.Audit.Type == AuditType.Order)
                {
                    var nextTaskItem = await TaskItemRep.GetAll().Where(m => m.TaskId == taskId && m.Status == AuditStatus.Null).OrderBy(m => m.Id).FirstOrDefaultAsync();
                    if (nextTaskItem != null)
                    {
                        nextTaskItem.Status = AuditStatus.NotStart;
                        nextTaskItem.CreateTime = DateTime.Now;
                        await TaskItemRep.UpdateAsync(nextTaskItem);
                        var nextUser = await _userAppService.GetDto(nextTaskItem.UserId);
                        if (nextUser != null)
                        {
                            SendNofityMsg(nextUser, nextTaskItem, flow);
                        }

                    }
                    else
                    {
                        isPassed = true;
                    }

                }

                if (isPassed)
                {

                    await TaskRep.GetAll().Where(m => m.Id == taskId && m.Status == TaskStatus.Started).UpdateAsync(m => new FlowTask { Status = TaskStatus.Passed });
                    await TaskItemRep.GetAll().Where(m => m.TaskId == taskId && m.Status == AuditStatus.NotStart).UpdateAsync(m =>
                    new TaskItem { AuditTime = DateTime.Now, Status = TaskStatus.Passed, Remark = "系统自动审核通过" });

                    var nextNode = context.FlowConfig.GetNextNode(node);
                    result.NextNodes = [nextNode];

                }

                msgContent = $"审核节点【{curTaskItem.NodeName}】已通过审核，操作人【{curTaskItem.UserName}】";

            }
            else
            {

                msgContent = $"审核节点【{curTaskItem.NodeName}】未通过审核，操作人【{curTaskItem.UserName}】";
                var nextNode = context.FlowConfig.Nodes.Find(m => m.Type == NodeType.End);
                result.NextNodes = [nextNode];

            }

            msg = new Msg
            {
                Title = "审核已处理提醒",
                Content = msgContent,
                Type = 1,
                UserId = context.Flow.CreatorId,
            };

            Notify(msg);
            return result;
        }


    }
}
