using Nct.Module.Core.Menus.Dtos;
using System;

namespace Nct.Module.Core.Users.Dtos
{

    [Map(typeof(User))]
    public class UserDto : IEntityDto
    {

        public int Id { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string UserName { get; set; }

        public int UserType { get; set; }

        public bool IsActive { get; set; }

        public string Pic { get; set; }

        public string RealName { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }

        public int Sex { get; set; }

        public string Address { get; set; }

        public DateTime? Birthday { get; set; }

        public DateTime CreateTime { get; set; }

        public string GetDisplayName()
        {
            return RealName.IsEmpty() ? UserName : RealName;

        }


    }

    [Map(typeof(User))]
    public class CurrentUserDto
    {

        public int Id { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public string UserName { get; set; }

        public int UserType { get; set; }

        public bool IsActive { get; set; }

        public string Pic { get; set; }

        public string RealName { get; set; }

        public string Email { get; set; }

        public string Tel { get; set; }

        public int Sex { get; set; }

        public string Address { get; set; }

        public DateTime? Birthday { get; set; }

        public DateTime CreateTime { get; set; }

        public List<ShortMenuDto> Permissions { get; set; }

        public List<string> Roles { get; set; }


        public string GetDisplayName()
        {
            return RealName.IsEmpty() ? UserName : RealName;

        }


    }

}
