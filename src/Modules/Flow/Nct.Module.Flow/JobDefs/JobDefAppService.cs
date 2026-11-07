using Nct.Domain.Uow;
using Nct.Jobs;
using Nct.LinqQuery;
using Nct.Modularity;
using Nct.Module.Flow.Engine;
using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.JobDefs.Dtos;
using Nct.Module.Flow.JobLogs;
using Nct.Module.Flow.JobLogs.Dtos;
using Nct.Module.Flow.Jobs;
using Nct.Scripting;
using Nct.SignalR;
using Quartz;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using static Nct.Module.Flow.Consts;

namespace Nct.Module.Flow.JobDefs
{


    public class JobDefAppService : CrudAppService<JobDef, JobDefDto, JobDefQueryDto, CreateJobDefDto>, IJobDefAppService
    {

        private readonly Script _script;
        private readonly IFlowEngine _flowEngine;
        private readonly IJobLogAppService _jobFlowLogAppService;
        private readonly IJobWorker _jobWorker;
        public JobDefAppService(IRepository<JobDef> repository, Script script, IFlowEngine flowEngine,
            IJobLogAppService jobFlowLogAppService, IJobWorker jobWorker) : base(repository)
        {
            _script = script;
            _flowEngine = flowEngine;
            _jobFlowLogAppService = jobFlowLogAppService;
            _jobWorker = jobWorker;
        }


        public async Task<string> GetFlowConfig(int id)
        {
            var flow = await Get(id);
            return flow.FlowConfig;
        }



        private void TryCompileScriptClassCode(string code)
        {
            code = _script.BuildCodeToClass(code);
            var result = _script.Compile(code, null, true);
            if (!result.IsSuccess)
                throw new BusinessException(result.Msg);

        }

        private void CheckNodeScript(string config)
        {

            var configObj = config.ToObject<FlowConfig>();
            var scriptNodes = configObj.Nodes.Where(m => m.Type == NodeType.Script);

            foreach (var scriptNode in scriptNodes)
            {
                var props = scriptNode.Properties.To<JobScriptNodeProps>();
                TryCompileScriptClassCode(props.Code);

            }

        }

        private static void CheckCronExpression(CreateJobDefDto dto)
        {
            if (dto.IntervalType == 2 && !CronExpression.IsValidExpression(dto.IntervalCron))
            {
                throw new BusinessException("Cron表达式格式错误");
            }
        }

        private void CheckExeMethod(CreateJobDefDto dto)
        {
            if (dto.JobType == 2)
            {
                var result = GetJobMethodInfo(dto.ExeMethod);
                if (!result.IsSuccess)
                    throw new BusinessException(result.Msg);
            }
        }



        async Task RegisterJob(JobDefDto dto)
        {
            var jobInfo = new JobInfo
            {
                Name = dto.Name + "_" + dto.Id,
                JobType = typeof(CommonJob),
                Group = "Flow",
                Data = dto.Id,
                LogIgnore = true
            };
            if (dto.IntervalType == 1)
            {
                jobInfo.Interval = new JobInterval
                {
                    Type = JobIntervalType.Minutes,
                    Value = TimeSpan.FromMinutes(dto.IntervalMinute.Value)
                };

            }
            else
            {
                jobInfo.Interval = new JobInterval
                {
                    Type = JobIntervalType.Cron,
                    CronExpression = dto.IntervalCron
                };

            }
            await _jobWorker.Create(jobInfo);


        }

        private async Task RemoveJob(JobDefDto dto)
        {
            await _jobWorker.Remove(new JobInfo
            {
                Name = dto.Name + "_" + dto.Id,
                JobType = typeof(CommonJob),
                Group = "Flow"
            });
        }

        public override async Task<JobDefDto> Create(CreateJobDefDto dto)
        {
            CheckCronExpression(dto);
            CheckExeMethod(dto);
            var result = await base.Create(dto);
            if (result.IsEnabled)
            {
                await RegisterJob(result);
            }
            return result;
        }

        public override async Task<JobDefDto> Update(CreateJobDefDto dto)
        {
            CheckCronExpression(dto);
            CheckExeMethod(dto);
            var oldDto = await GetDto(dto.Id);
            var newDto = await base.Update(dto);

            if (oldDto.IsEnabled && !newDto.IsEnabled)
            {
                await RemoveJob(oldDto);
            }
            else if (!oldDto.IsEnabled && newDto.IsEnabled)
            {
                await RegisterJob(newDto);
            }
            else if (oldDto.IsEnabled && newDto.IsEnabled)
            {
                if (oldDto.IntervalType != newDto.IntervalType || oldDto.IntervalMinute != newDto.IntervalMinute || oldDto.IntervalCron != newDto.IntervalCron)
                {

                    await RemoveJob(oldDto);
                    await RegisterJob(newDto);
                }

            }
            return newDto;
        }

        public async Task RegisterAllJobs()
        {
            var jobs = await Repository.GetAll().Where(m => m.IsEnabled && m.JobType < 3).ToListAsync();
            foreach (var job in jobs)
            {
                var dto = MapToEntityDto(job);
                await RegisterJob(dto);
            }
        }


        public async Task<int> SaveFlowConfig(SaveFlowConfigDto dto)
        {
            CheckNodeScript(dto.FlowConfig);
            return await Repository.GetAll().Where(m => m.Id == dto.Id)
                 .UpdateAsync(m => new JobDef { FlowConfig = dto.FlowConfig });
        }


        protected override void SetPermissionNames()
        {
            base.SetPermissionNames();
        }


        protected override IQueryable<JobDef> CreateQuery(JobDefQueryDto dto)
        {
            return base.CreateQuery(dto).WhereIf(m => m.Name.Contains(dto.Name), dto.Name.IsNotEmpty())
                .WhereIf(m => dto.Types.Contains(m.JobType), dto.Types != null && dto.Types.Any());
        }


        public override Task<PageResult<JobDefDto>> GetList(PageQuery<JobDefQueryDto> pageQuery)
        {
            return base.GetList(pageQuery);
        }

        private RetResult GetJobMethodInfo(string method)
        {
            var methodParts = method.Split('.');
            if (methodParts.Length < 2)
            {
                return Error("方法格式错误");
            }
            var className = string.Join(".", methodParts, 0, methodParts.Length - 1);
            var allTypes = IModuleManager.Instance.GetTypesOfAllAssemblies();
            var type = allTypes.FirstOrDefault(m => m.FullName == className && m.IsClass && m.IsPublic);
            if (type == null)
            {
                return Error($"找不到类[{className}]");
            }

            var methodName = methodParts[methodParts.Length - 1];
            var methodInfo = type.GetMethod(methodName);
            if (methodInfo == null)
            {
                return Error($"找不到方法[{methodName}]");
            }
            if (methodInfo.GetParameters().Length != 0)
            {
                return Error("执行方法不能有参数");
            }
            return Success(new object[] { type, methodInfo });

        }


        private async Task<RetResult> ExecuteFlowJob(JobDef job, bool notify = true)
        {
            var stopwatch = Stopwatch.StartNew();
            var success = false;
            var configData = job.FlowConfig;
            string msg;
            if (configData.IsNotEmpty())
            {

                var congig = configData.ToObject<FlowConfig>();
                var context = new FlowJobStepExecutionContext
                {
                    FlowConfig = congig,
                    Node = congig.Nodes.FirstOrDefault(m => m.Type == NodeType.Start),
                    JobId = job.Id,
                    NodeStatusChanged = (e) =>
                    {
                        if (!notify) return;
                        RealTimeNotifier.Send(new UserNotification
                        {
                            Data = e,
                            UserId = Session.UserId,
                            Type = "FlowNodeStatusChanged"
                        }
                        );
                    }
                };

                success = await _flowEngine.Run(context);
                msg = _flowEngine.Results.ToJson();
            }
            else
            {
                var stepResult = new List<IStepExecutionResult> { new StepExecutionResult { Exception = "流程配置为空", Success = false, StepName = "初始" } };
                msg = stepResult.ToJson();
            }

            var dto = await _jobFlowLogAppService.Create(new CreateJobFlowLogDto
            {
                ElapsedTime = (int)stopwatch.ElapsedMilliseconds,
                IsSuccess = success,
                JobName = job.Id.ToString(),
                Message = msg,
                LogType = 1,
            });

            var result = new RetResult
            {
                Data = dto.Id,
                IsSuccess = success
            };
            return result;

        }

        private async Task<RetResult> ExecuteMethodJob(JobDef job)
        {
            var stopwatch = Stopwatch.StartNew();
            var success = true;
            string error = null;

            var result = GetJobMethodInfo(job.ExeMethod);
            if (!result.IsSuccess)
                return result;

            var data = (object[])result.Data;
            var type = (Type)data[0];
            var methodInfo = (MethodInfo)data[1];

            try
            {
                using (var objectScope = IocManager.Instance.ResolveAsDisposable(type))
                {
                    var exeResult = methodInfo.Invoke(objectScope.Object, null);
                    if (exeResult is Task task)
                        await task;
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                success = false;
            }

            await _jobFlowLogAppService.Create(new CreateJobFlowLogDto
            {
                ElapsedTime = (int)stopwatch.ElapsedMilliseconds,
                IsSuccess = success,
                JobName = job.Id.ToString(),
                Message = error ?? "执行成功",
                LogType = 2,

            });
            result.Data = null;
            result.IsSuccess = success;
            return result;

        }

        private async Task<RetResult> ExecuteSystemJob(JobDef jobDef)
        {
            var jobs = JobWorkerBuilder.GetJobs();
            var job = jobs.FirstOrDefault(m => m.JobType.FullName == jobDef.ExeMethod);
            using (var disposableObject = IocManager.Instance.ResolveAsDisposable<JobExecutor>())
            {
                disposableObject.Object.JobDetail = job;
                var success = await disposableObject.Object.Execute();

                return new RetResult
                {
                    IsSuccess = success,
                };

            }



        }

        public async Task SyncSystemJobToDb()
        {
            var jobs = JobWorkerBuilder.GetJobs();
            var names = new List<string>();
            foreach (var job in jobs)
            {
                var model = await Repository.FirstOrDefaultAsync(m => m.ExeMethod == job.JobType.FullName);
                JobDef jobDef;
                if (model == null)
                {
                    jobDef = new JobDef
                    {
                        Name = job.Name,
                        ExeMethod = job.JobType.FullName,
                        IsEnabled = true,
                        JobType = 3
                    };
                }
                else
                {
                    jobDef = model;
                }
                if (job.Interval.Type == JobIntervalType.Cron)
                {
                    jobDef.IntervalCron = job.Interval.CronExpression;
                    jobDef.IntervalType = 2;
                }
                else if (job.Interval.Type == JobIntervalType.Daily)
                {
                    var time = job.Interval.Value;
                    jobDef.IntervalCron = $"{time.Seconds} {time.Milliseconds} {time.Hours} * * ?";
                    jobDef.IntervalType = 2;
                }
                else
                {
                    jobDef.IntervalMinute = (int)job.Interval.Value.TotalMinutes;
                    jobDef.IntervalType = 1;
                }


                if (model == null)
                {
                    await Repository.InsertAsync(jobDef);
                }
                else
                {
                    await Repository.UpdateAsync(jobDef);
                }
                names.Add(jobDef.ExeMethod);

            }

            await Repository.DeleteAsync(m => !names.Contains(m.ExeMethod) && m.JobType == 3);


        }

        public async Task<RetResult> SwitchJobEnabled(int id)
        {

            var job = await Get(id);
            var enabled = !job.IsEnabled;
            await Repository.GetAll().Where(m => m.Id == id).UpdateAsync(m => new JobDef
            {
                IsEnabled = enabled
            });
            return Success();

        }





        [UnitOfWork(false)]
        public async Task<RetResult> Run(int id, bool notify = true)
        {
            RetResult result;
            var job = await Get(id);
            if (job.JobType == 1)
            {
                result = await ExecuteFlowJob(job, notify);
            }
            else if (job.JobType == 2)
            {
                result = await ExecuteMethodJob(job);
            }
            else
            {
                result = await ExecuteSystemJob(job);
            }
            await Repository.GetAll().Where(m => m.Id == id).UpdateAsync(m => new JobDef { LastTime = DateTime.Now, LastExeSuccess = result.IsSuccess });
            return result;

        }



    }
}
