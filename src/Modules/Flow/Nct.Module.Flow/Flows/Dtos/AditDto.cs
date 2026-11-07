using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Flow.Flows.Dtos
{
    public class AditDto
    {

        public int TaskId { get; set; }

        [Range(1,2,ErrorMessage ="审核状态错误")]
        public int Status { get; set; }

        public string Remark { get; set; }

        public string FormData { get; set; }

        public string FormDataId { get; set; }

    }
}
