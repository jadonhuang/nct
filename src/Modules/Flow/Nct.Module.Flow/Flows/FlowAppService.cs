using Nct.LinqQuery;
using Nct.MessageQueue;
using Nct.Module.Core.Events;
using Nct.Module.Core.Users;
using Nct.Module.Flow.Engine;
using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.FlowDefs;
using Nct.Module.Flow.Flows.Dtos;
using Nct.Module.Form.FormDefs;
using Nct.Module.Form.FormDefs.Dtos;
using Nct.ObjectMapper;
using Nct.SignalR;
using AuditStatus = Nct.Module.Flow.Consts.AuditStatus;
using FlowStatus = Nct.Module.Flow.Consts.FlowStatus;
using NodeType = Nct.Module.Flow.Consts.NodeType;
using TaskStatus = Nct.Module.Flow.Consts.TaskStatus;
namespace Nct.Module.Flow.Flows
{


    public class FlowAppService : CrudAppService<Flow, FlowDto, FlowDto, CreateFlowDto>, IFlowAppService
    {
        private readonly IFormDefAppService _formDefApp;
        private readonly FlowDefAppService _flowDefApp;
        private readonly IRepository<Flows.FlowTask> _taskRep;
        private readonly IRepository<TaskItem> _taskItemRep;
        private readonly IUserAppService _userApp;
        private readonly IFlowEngine _flowEngine;

        public FlowAppService(IRepository<Flow> repository, IFormDefAppService formDefApp,
            FlowDefAppService flowDefApp,
            IRepository<Flows.FlowTask> taskRep,
             IRepository<TaskItem> taskItemRep,
             IUserAppService userApp,
             IFlowEngine flowEngine
            ) : base(repository)
        {
            _formDefApp = formDefApp;
            _flowDefApp = flowDefApp;
            _taskRep = taskRep;
            _taskItemRep = taskItemRep;
            _userApp = userApp;
            _flowEngine = flowEngine;

        }





        private async Task<string> SaveFormData(FlowDef flowDef, SaveDataDto saveDataDto)
        {

            var startNode = flowDef.GetConfig().Nodes.First(m => m.Type == NodeType.Start);
            var formCode = startNode.Properties.To<StartNodeProps>().FormCode;

            saveDataDto.FormCode = formCode;
            var ret = await _formDefApp.SaveData(saveDataDto);

            if (!ret.IsSuccess) throw new BusinessException(ret.Msg);

            return ret.Data.ToString();
        }


        public override async Task<FlowDto> Create(CreateFlowDto dto)
        {

            var flowDef = _flowDefApp.GetByCode(dto.FlowCode);
            if (flowDef == null) throw new BusinessException("流程不存在");

            var saveFormDto = new SaveDataDto { Data = dto.FormData };

            var formDataId = await SaveFormData(flowDef, saveFormDto);

            var currentUser = await _userApp.GetCurrentUser();

            var flow = new Flow
            {
                FlowCode = flowDef.Code,
                Title = flowDef.Name,
                Status = FlowStatus.Started,
                FormDataId = formDataId,
                CreatorName = currentUser.GetDisplayName(),
            };

            await Repository.InsertAsync(flow);
            var context = new ApprovalStepExecutionContext
            {
                Flow = flow,
                FlowDef = flowDef,
                Node = flowDef.GetConfig().Nodes.FirstOrDefault(m => m.Type == NodeType.Start)
            };
            await _flowEngine.Run(context);

            var retDto = flow.MapTo<FlowDto>();
            return retDto;


        }


        public async Task<bool> Audit(AditDto dto)
        {

            var taskItem = await _taskItemRep.GetAsync(dto.TaskId);

            if (taskItem.UserId != Session.UserId.Value)
                throw new BusinessException("无权限");

            if (taskItem.Status != AuditStatus.NotStart)
                throw new BusinessException("已审核");

            taskItem.Status = dto.Status;
            taskItem.Remark = dto.Remark;
            var flow = await Get(taskItem.FlowId);
            var flowDef = _flowDefApp.GetByCode(flow.FlowCode);
            if (flowDef == null) throw new BusinessException("流程不存在");

            if (dto.FormData.IsNotEmpty())
            {
                var curNodeProp = flowDef.GetConfig().Nodes.FirstOrDefault(m => m.Id == taskItem.NodeId).Properties.To<TaskNodeProps>();
                if (curNodeProp.FormEdit)
                {
                    var saveFormDto = new SaveDataDto { Data = dto.FormData, Id = dto.FormDataId };
                    await SaveFormData(flowDef, saveFormDto);
                }

            }
            var context = new ApprovalStepExecutionContext
            {
                Flow = flow,
                FlowDef = flowDef,
                TaskItem = taskItem,
                Node = flowDef.GetConfig().Nodes.FirstOrDefault(m => m.Id == taskItem.NodeId)
            };
            await _flowEngine.Run(context);

            return true;

        }



        [RemoteService(false)]
        public override Task<PageResult<FlowDto>> GetList(PageQuery<FlowDto> pageQuery)
        {
            return base.GetList(pageQuery);
        }


        public async Task<PageResult<FlowDto>> GetMyFlowList(PageQuery<FlowDto> pageQuery)
        {


            var queryDto = pageQuery.Query;
            var query = Repository.GetAll().Where(m => m.CreatorId == Session.UserId.Value).WhereIf(m => m.Title.Contains(queryDto.Title), queryDto.Title.IsNotEmpty());
            long count = await query.LongCountAsync();

            query = query.PageBy(pageQuery);
            query = query.OrderByDescending(m => m.Id);

            var rows = await query.ToListAsync();

            var pageResult = new PageResult<FlowDto>
            {
                Total = count,
                Rows = rows.MapTo<FlowDto>(),
                PageIndex = pageQuery.PageIndex,
                PageSize = pageQuery.PageSize


            };
            return pageResult;

        }



        public async Task<PageResult<TaskItemDto>> GetMyTaskList(PageQuery<TaskQueryDto> pageQuery)
        {


            var queryDto = pageQuery.Query;
            var type = queryDto.Type;
            var query = _taskItemRep.GetAll().Where(m => m.UserId == Session.UserId.Value && m.Status < AuditStatus.Null).WhereIf(m => m.NodeName.Contains(queryDto.Name), queryDto.Name.IsNotEmpty())
                .WhereIf(m => m.CreateTime >= queryDto.StartDate.Value, queryDto.StartDate.HasValue)
                 .WhereIf(m => m.CreateTime <= queryDto.StartDate.Value.ToDayEnd(), queryDto.StartDate.HasValue)
                ;
            if (type.HasValue)
            {

                if (type.Value == 1)
                    query = query.Where(m => m.Status == 0);

                else if (type.Value == 2)
                    query = query.Where(m => m.Status > 0);


            }

            long count = await query.LongCountAsync();

            query = query.PageBy(pageQuery);

            query = query.OrderByDescending(m => m.Id);

            var rows = await query.ToListAsync();

            var dtoRows = rows.MapTo<TaskItemDto>();

            if (rows.Count > 0)
            {
                var flowIds = rows.Select(m => m.FlowId).Distinct().ToList();
                var flows = await Repository.GetAll().Where(m => flowIds.Contains(m.Id)).ToListAsync();

                foreach (var row in dtoRows)
                {
                    var flow = flows.Find(m => m.Id == row.FlowId);
                    row.Applicant = flow.CreatorName;

                }
            }
            var pageResult = new PageResult<TaskItemDto>
            {
                Total = count,
                Rows = dtoRows,
                PageIndex = pageQuery.PageIndex,
                PageSize = pageQuery.PageSize


            };
            return pageResult;

        }


        public async Task<TaskItemDto> GetTaskItem(int id)
        {

            var taskItem = await _taskItemRep.GetAll().Where(m => m.Id == id).FirstOrDefaultAsync();
            return taskItem.MapTo<TaskItemDto>();


        }


        public async Task<List<TaskDto>> GetTasks(int flowId)
        {


            var tasks = await _taskRep.GetAll().Where(m => m.FlowId == flowId).OrderBy(m => m.Id).ToListAsync();
            var taskDtos = tasks.MapTo<TaskDto>();
            var taskItems = await _taskItemRep.GetAll().Where(m => m.FlowId == flowId).OrderBy(m => m.Id).ToListAsync();
            var taskItemDtos = taskItems.MapTo<TaskItemDto>();

            foreach (var task in taskDtos)
            {
                var items = taskItemDtos.Where(m => m.TaskId == task.Id).ToList();
                task.Items = items;

            }

            return taskDtos;


        }

        [RemoteService(false)]
        public async Task HandleTimeoutTask()
        {

            var tasks = (await _taskRep.GetAllListAsync(m => (m.Status == TaskStatus.NotStart || m.Status == TaskStatus.Started) && m.TimeoutNotice > 0)).OrderBy(m => m.Id);
            foreach (var task in tasks)
            {

                var flow = await Get(task.FlowId);
                var flowDef = _flowDefApp.GetByCode(flow.FlowCode);
                var nodes = flowDef.GetConfig().Nodes;

                var isFirstNotify = task.LastNotifyTime == null;
                var lastRunTime = task.LastNotifyTime ?? task.CreateTime;
                var passedMinutes = (DateTime.Now - lastRunTime).TotalMinutes.ToInt();
                if (passedMinutes < task.TimeoutNotice) continue;

                var node = nodes.FirstOrDefault(m => m.Id == task.NodeId);
                if (node.Type != NodeType.Task) continue;

                var props = node.Properties.To<TaskNodeProps>();
                var timeLimit = props.Audit.TimeLimit;
                if (timeLimit.ExpireHandle == Consts.ExpireHandle.Notify)
                {
                    if (!timeLimit.LoopNotify && !isFirstNotify)
                    {
                        continue;

                    }

                    task.LastNotifyTime = DateTime.Now;
                    await _taskRep.UpdateAsync(task);

                }

                var items = (await _taskItemRep.GetAllListAsync(m => m.Status == AuditStatus.NotStart && m.TaskId == task.Id)).OrderBy(m => m.Id);

                foreach (var item in items)
                {
                    Msg msg;

                    if (timeLimit.ExpireHandle == Consts.ExpireHandle.Notify)
                    {

                        msg = new Msg
                        {
                            Title = "审核超时提醒",
                            Content = $"您的任务【{item.NodeName}】已超时，请尽快处理",
                            Type = 1,
                            Props = new { url = $"/flow/view?taskId={item.Id}&id={item.FlowId}" }.ToJson(),
                            UserId = item.UserId,

                        };

                    }
                    else
                    {

                        item.Status = timeLimit.ExpireHandle == Consts.ExpireHandle.Refuse ? AuditStatus.NotPassed : AuditStatus.Passed;
                        var context = new ApprovalStepExecutionContext
                        {
                            Flow = flow,
                            FlowDef = flowDef,
                            TaskItem = item
                        };
                        context.Node = nodes.FirstOrDefault(m => m.Id == item.NodeId);
                        await _flowEngine.Run(context);

                        string statusText = item.Status == AuditStatus.NotPassed ? "拒绝" : "同意";
                        msg = new Msg
                        {
                            Title = "审核超时自动处理提醒",
                            Content = $"您的任务【{item.NodeName}】已超时，系统已自动[{statusText}]",
                            Type = 1,
                            UserId = item.UserId,

                        };

                    }


                    MessageSender.Send(msg);
                    RealTimeNotifier.Send(new UserNotification
                    {
                        Data = new { msg.Title, msg.Content },
                        UserId = item.UserId,

                    });


                }
            }


        }


    }
}
