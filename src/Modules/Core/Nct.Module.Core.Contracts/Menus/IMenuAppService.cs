using Nct.Application;
using Nct.Module.Core.Menus.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nct.Module.Core.Menus
{

    public interface IMenuAppService: ICrudAppService<Menu,MenuDto, Menu, CreateMenuDto>
    {

        Task<List<Menu>> GetAllList();


        Task<List<MenuDto>> GetList(int pid);


        Task<IList<MenuDto>> GetTree(Menu menu);


        List<MenuDto> GetCurrentUserPermissions();


        IList<MenuDto> GetCurrentUserMenus();


    }
}
