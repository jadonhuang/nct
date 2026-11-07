using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.FormDefs.Dtos
{
    public class DeleteDataDto
    {

        [Required]
        public string FormCode { get; set; }


        [Required]
        public string Ids { get; set; }


    }


    public class SetItemValueDto: DeleteDataDto
    {

        [Required]
        public string ItemName { get; set; }


        [Required]
        public  string Value { get; set; }


}
}
