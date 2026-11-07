using Nct.Ioc;
using Nct.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Domain.Entities
{
    public class EntityAuditingHelper
    {

        public static void SetCreationAuditProperties(object entity)
        {



            if (entity is IHasCreateTime hasCreateTime)
            {
                hasCreateTime.CreateTime = DateTime.Now;
            }


            if (entity is IHasCreateUser hasCreateUser)
            {

                hasCreateUser.CreateUser = ISession.Session.UserName;
            }


            if (entity is IHasCreatorId hasCreatorId)
            {

                var session = ISession.Session;

                if (session.IsAuthenticated)
                {
                    hasCreatorId.CreatorId = session.UserId.Value;
                }



            }

        }


        public static void SetUpdateAuditProperties(object entity)
        {


            if (entity is IHasUpdateTime hasUpdateTime)
            {
                hasUpdateTime.UpdateTime = DateTime.Now;
            }

            if (entity is IHasUpdateUser hasUpdateUser)
            {
                hasUpdateUser.UpdateUser = ISession.Session.UserName;
              
            }



        }





    }
}
