using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.ApiJson.JsonDataPrivacys.Dtos
{

    [Map(typeof(JsonDataPrivacy))]
    public class JsonDataPrivacyDto : IEntityDto
    {

        public int Id { get; set; }


        public string TableName { get; set; }


        public bool SelectAllowAnonymous { get; set; }


        public string SelectAllowRoles { get; set; }


        public string SelectNotAllowedFields { get; set; }


        public bool UpdateAllowAnonymous { get; set; }


        public string UpdateAllowRoles { get; set; }


        public string UpdateNotAllowFields { get; set; }


        public bool DeleteAllowAnonymous { get; set; }


        public string DeleteAllowRoles { get; set; }


        public bool InsertAllowAnonymous { get; set; }


        public string InsertAllowRoles { get; set; }


        public string InsertNotAllowedFields { get; set; }


        public bool SelectEnabled { get; set; }


        public bool DeleteEnabled { get; set; }


        public bool UpdateEnabled { get; set; }


        public bool InsertEnabled { get; set; }


    }
}
