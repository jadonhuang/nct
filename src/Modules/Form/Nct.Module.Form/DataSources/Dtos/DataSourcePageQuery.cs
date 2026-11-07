namespace Nct.Module.Form.DataSources.Dtos
{
    public class DsDataPageQueryDto : DsDataQueryDto, IPageQuery
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string OrderBy { get; set; }


    }

    public class DsDataQueryDto
    {

        public string Query { get; set; }

        public string Code { get; set; }

        

    }


}
