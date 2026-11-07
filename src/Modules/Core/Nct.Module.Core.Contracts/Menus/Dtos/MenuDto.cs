using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;
using System.ComponentModel.DataAnnotations;
using Nct.Models;
using System.Linq;

namespace Nct.Module.Core.Menus.Dtos
{

    [Map(typeof(MenuDto))]
    public class ShortMenuDto
    {


        public int Id { get; set; }


        public int ParentId { get; set; }


        public string Code { get; set; }


        public string Name { get; set; }


        public string Url { get; set; }

        public bool IsMenu { get; set; }

    } 


    [Map(typeof(Menu))]
    public class MenuDto : IEntityDto,ITreeNode<int, MenuDto>
    {

        [Key]
        public int Id { get; set; }


        public int ParentId { get; set; }


        public string Code { get; set; }


        public string Name { get; set; }


        public string Url { get; set; }


        public int Sort { get; set; }


        public string Icon { get; set; }


        public bool IsShow { get; set; }


        public bool IsMenu { get; set; }


        public IList<MenuDto> Children { get; set; }

        public bool HasChildren
        {
            get
            {
                return Children != null && Children.Any();

            }
        }


    }
}
