using Nct.Ioc;

namespace Nct.Session
{
    public interface ISession
    {

        int? UserId { get; }

        string UserName { get; }

        bool IsAuthenticated { get; }


        public static ISession Session
        {
            get
            {
                ISession _session = null;
                IocManager.Instance.Using<ISession>(session =>
                {
                    _session = session;
                });

                return _session;

            }
        }


    }
}
