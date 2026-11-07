using Microsoft.Extensions.Options;
using Nct.DataAccess.Core;
using Nct.Ioc;
using Nct.Json;
using Nct.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace Nct.DataAccess.Log
{
    public class LogDbCommandInterceptor : IDbCommandInterceptor, ISingletonDependency
    {

        private readonly ConcurrentDictionary<DbCommandInfo, Stopwatch> _dbCommandInfoDic;
        private readonly ILogger _logger;
        private readonly DbLogConfiguration _configuration;

        public LogDbCommandInterceptor(ILogger logger, IOptions<DbLogOptions> options)
        {

            _logger = logger;
            _configuration = options.Value.DbLog;
            _dbCommandInfoDic = new ConcurrentDictionary<DbCommandInfo, Stopwatch>();
        }


        public void Executing(DbCommandInfo dbCommandInfo)
        {
            _dbCommandInfoDic.TryAdd(dbCommandInfo, Stopwatch.StartNew());

        }

        public void Executed(DbCommandInfo dbCommandInfo)
        {

            if (!_dbCommandInfoDic.TryRemove(dbCommandInfo, out Stopwatch stopwatch))
            {
                return;
            }

            var duration = stopwatch.Elapsed.TotalMilliseconds;

            var hasErr = dbCommandInfo.Exception != null;
            var hasWarn = duration >= _configuration.WarnExecuteTime;
            var hasInfo = _configuration.Level == LogLevel.Information;

            if (!(hasErr || hasWarn || hasInfo)) return;

            var logStr = new StringBuilder();
            logStr.AppendLine();
            logStr.AppendLine(dbCommandInfo.Sql);
            logStr.AppendLine("参数：");

            if (dbCommandInfo.Param != null)
            {
                logStr.AppendLine(dbCommandInfo.Param.ToJson());

            }
            logStr.AppendLine("用时：" + duration + "ms");

            var separateLine = "----------------------------------------------------------------------------------------------------------------------------";
            if (hasErr)
            {
                logStr.AppendLine(dbCommandInfo.Exception.Message);
                logStr.AppendLine(separateLine);
                _logger.Error(logStr);
            }
            else
            {
                logStr.AppendLine(separateLine);

                if (hasWarn)
                {
                    _logger.Warn(logStr);

                }

                if (hasInfo)
                {
                    _logger.Info(logStr);
                }


            }




        }
    }
}
