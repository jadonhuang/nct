using Nct.Logging;
using Nct.MessageQueue;
using Nct.Module.Core.Events;
using Nct.Module.Flow.Engine.Models;
using Nct.SignalR;

namespace Nct.Module.Flow.Engine.Steps
{
    public abstract class Step : IStep
    {

        public ILogger Logger { get; set; }

        private bool isDisposed = false;

        public bool IsDisposed => isDisposed;


        public virtual void Dispose()
        {
            isDisposed = true;
        }


        public virtual IStepExecutionResult Success(params FlowNode[] nextNodes)
        {
            var result = new StepExecutionResult
            {
                Success = true,
                NextNodes = nextNodes
            };
            return result;
        }

        public virtual IStepExecutionResult Fail(string exception = null)
        {
            var result = new StepExecutionResult
            {
                Success = false,
                Exception = exception
            };
            return result;
        }

        public virtual Task<IStepExecutionResult> Run(StepExecutionContext context)
        {

            return null;
        }

        protected virtual void Notify(Msg msg)
        {

            MessageSender.Send(msg);
            RealTimeNotifier.Send(new UserNotification
            {
                Data = new { msg.Title, msg.Content, },
                Type = "msg",
                UserId = msg.UserId
            }
            );

        }




    }
}
