using Nct.Authorization;
using Nct.Event;
using Nct.Module.Core.Menus.Dtos;
using Nct.Module.Core.Roles;
using System.Data;

namespace Nct.Module.Core.Menus
{


    public class MenuAppService : CrudAppService<Menu, MenuDto, Menu, CreateMenuDto>, IMenuAppService
    {

        private readonly IRoleAppService _roleAppService;

        public const string UserPermissionsCacheKey = "userpemissions_";

        public MenuAppService(IRepository<Menu> repository, IRoleAppService roleAppService) : base(repository)
        {

            _roleAppService = roleAppService;
        }


        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addmenu";
            DeletePermissionName = "delmenu";
            UpdatePermissionName = "editmenu";

        }

        public override async Task<MenuDto> Create(CreateMenuDto input)
        {

            var menu = await Repository.FirstOrDefaultAsync(m => m.Code == input.Code);
            if (menu != null)
                throw new BusinessException($"编码[{input.Code}]已存在");

            var result = await base.Create(input);


            var path = result.Id.ToString();
            if (result.ParentId > 0)
            {
                path = (await Get(input.ParentId)).Path;
                path = path.Trim(',') + "," + result.Id;
            }

            path = $",{path},";

            await Repository.GetAll().Where(m => m.Id == result.Id).UpdateAsync(m => new Menu { Path = path });

            Cache.Clear(UserPermissionsCacheKey);
          
            return result;
        }


        public override async Task<MenuDto> Update(CreateMenuDto input)
        {

            var menu = await Repository.FirstOrDefaultAsync(m => m.Code == input.Code && m.Id != input.Id);
            if (menu != null)
                throw new BusinessException($"编码[{input.Code}]已存在");

            var result = await base.Update(input);

            Cache.Clear(UserPermissionsCacheKey);

            return result;

        }


        public override async Task<int> Delete(int id)
        {

            var ret = await Repository.GetAll().Where(m => m.Path.Contains("," + id.ToString() + ",")).DeleteAsync();
            Cache.Clear(UserPermissionsCacheKey);
        
            return ret;

        }

        public async Task<List<Menu>> GetAllList()
        {
            var list = await Repository.GetAllListAsync();
            return list.OrderBy(m => m.Sort).ToList();

        }

        public async Task<List<MenuDto>> GetList(int pid)
        {

            var list = await Repository.GetAll().WhereIf(m => m.ParentId == pid).OrderBy(m => m.Sort).ToListAsync();

            return list.MapTo<MenuDto>();

        }

        public List<MenuDto> GetCurrentUserPermissions()
        {


            return Cache.Get(UserPermissionsCacheKey + Session.UserId, () =>
             {

                 var allMenus = (Repository.GetAllList().OrderBy(m => m.Sort).ToList()).MapTo<MenuDto>();

                 if (Session.IsAdmin()) return allMenus;

                 var menus = new List<MenuDto>();

                 var roles = _roleAppService.GetUserRoles(Session.UserId.Value).ConfigureAwait(false).GetAwaiter().GetResult();
                 var permissions = roles.SelectMany(m => m.Permissions.SplitString().Select(t => t.ToInt())).ToList();

                 foreach (var menu in allMenus)
                 {
                     if (permissions.Contains(menu.Id))
                         menus.Add(menu);

                 }

                 return menus;


             });


        }

        public IList<MenuDto> GetCurrentUserMenus()
        {

            var allMenus = GetCurrentUserPermissions();

            var items = allMenus.Where(m => m.IsMenu && m.IsShow);

            return items.ToTree<int, MenuDto>();


        }


        public async Task<IList<MenuDto>> GetTree(Menu menu)
        {

            var allMenus = await GetAllList();
            var items = allMenus;
            var hasCdt = false;
            if (menu.Code.IsNotEmpty())
            {
                items = items.Where(m => m.Code.IgnoreCaseContains(menu.Code)).ToList();
                hasCdt = true;
            }
            if (menu.Name.IsNotEmpty())
            {
                items = items.Where(m => m.Name.IgnoreCaseContains(menu.Name)).ToList();
                hasCdt = true;
            }
            if (hasCdt && items.Count>0)
            {
                var parents = allMenus.Where(m => items.Any(n => n.Id != m.Id && n.Path.Contains($",{m.Id},")));
                items.AddRange(parents);

            }

            var dtos = items.MapTo<MenuDto>();

            return dtos.ToTree<int, MenuDto>();



        }


     


    }
}
