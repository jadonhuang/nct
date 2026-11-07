using System;

namespace Nct.Domain.Entities
{
    public abstract class CreationAuditEntity: CreationAuditEntity<int>,IEntity
    {

    }

    public abstract class CreationAuditEntity<T> : IEntity<T>, IHasCreateTime, IHasCreateUser
    {
        public T Id { get; set; }

        public string CreateUser { get; set; }

        public DateTime CreateTime { get; set; }
       
    }

}
