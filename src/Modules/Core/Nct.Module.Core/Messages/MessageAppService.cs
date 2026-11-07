using Nct.Module.Core.Messages.Dtos;


namespace Nct.Module.Core.Messages
{


    public class MessageAppService : CrudAppService<Message, MessageDto, QueryDto, CreateMessageDto>, IMessageAppService
    {

        public MessageAppService(IRepository<Message> repository) : base(repository)
        {

        }

        [RemoteService(false)]
        public override Task<PageResult<MessageDto>> GetList(PageQuery<QueryDto> pageQuery)
        {
            return base.GetList(pageQuery);
        }

        public async Task<int> GetMyCount(int status)
        {

            return await Repository.CountAsync(m => m.Status == status && m.UserId == Session.UserId.Value);

        }

        public override async Task<int> Delete(int id)
        {
            return await Repository.GetAll().Where(m => m.UserId == Session.UserId.Value && m.Id == id).DeleteAsync();
        }

        public override async Task<int> MultiDelete(int[] ids)
        {
            return await Repository.GetAll().Where(m => m.UserId == Session.UserId.Value && ids.Contains( m.Id)).DeleteAsync();
        }


        public async Task<PageResult<MessageDto>> GetMyList(PageQuery<QueryDto> pageQuery)
        {

            var queryDto = pageQuery.Query;
            var query = Repository.GetAll().Where(m => m.UserId == Session.UserId.Value).WhereIf(m => m.Title.Contains(queryDto.Keyword) || m.Content.Contains(queryDto.Keyword), queryDto.Keyword.IsNotEmpty());
            long count = await query.LongCountAsync();

            query = query.OrderBy(m => m.Status).OrderByDescending(m => m.Id);

            query = query.PageBy(pageQuery);

            var rows = await query.ToListAsync();

            var pageResult = new PageResult<MessageDto>
            {
                Total = count,
                Rows = rows.MapTo<MessageDto>(),
                PageIndex = pageQuery.PageIndex,
                PageSize = pageQuery.PageSize


            };
            return pageResult;

        }

        public async Task<int> SetReaded(int[] ids)
        {

            var uid = Session.UserId.Value;
            if (ids.Length == 1 && ids[0] == 0)
            {
                return await Repository.GetAll().Where(m => m.Status == 0 && m.UserId == uid).UpdateAsync(m => new Message { Status = 1 });
            }
            int rows = await Repository.GetAll().Where(m => m.Status == 0 && ids.Contains(m.Id) &&
            m.UserId == uid).UpdateAsync(m => new Message { Status = 1 });
            return rows;
        }


    }
}
