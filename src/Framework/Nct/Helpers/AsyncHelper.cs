using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Helpers
{
    public static class AsyncHelper
    {
      
        public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Action postAction, Action<Exception> finalAction)
        {
            Exception exception = null;

            try
            {
                await actualReturnValue;
                postAction();
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                finalAction(exception);
            }
        }


        public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue, Action postAction, Action<Exception> finalAction)
        {

            Exception exception = null;

            try
            {
                var result = await actualReturnValue;
                postAction();
                return result;
            }
            catch (Exception ex)
            {
                exception = ex;
                throw;
            }
            finally
            {
                Console.WriteLine("finally 1");
                finalAction(exception);
            }
        }


    }
}
