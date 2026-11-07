using Nct.Module.Form.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Nct.Module.Form.FormDefs
{

    public class DbTable
    {
        public string Name { get; set; }
        public string RefField { get; set; }

    }

    public class QueryCdt
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Field { get; set; }

        public int Span { get; set; }


        public int RenderType { get; set; }




    }


    public class ListCol
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Field { get; set; }

        public int Width { get; set; }

        public string Align { get; set; }

        public string Sort { get; set; }

        public string Format { get; set; }

        public string Condition { get; set; }

        public string SummaryType { get; set; }
    }


    public class ExportCol
    {

        public string Field { get; set; }

        public bool Unique { get; set; }



    }


    public class DataSourceInfo
    {
        public bool Enabled { get; set; }
        public string Code { get; set; }

    }


    public class QueryInfo
    {
        public List<QueryCdt> Cdts { get; set; }

    }

    public class ListInfo
    {
        public List<ListCol> Cols { get; set; }

        public SummaryInfo Summary { get; set; }

    }

    public class SummaryInfo
    {

        public bool Enabled { get; set; }

        public string Title { get; set; }

    }


    public class ListConfigInfo
    {
        public DataSourceInfo DataSource { get; set; }

        public QueryInfo Query { get; set; }

        public ListInfo List { get; set; }

        public int DataAuth { get; set; }


    }

    public class ExportConfigInfo
    {
        public List<ExportCol> Cols { get; set; }

        /// <summary>
        /// 1增量，2修改，3增量和修改
        /// </summary>
        public int ImportMode { get; set; }


        public string ImportApi { get; set; }

        public string ExportApi { get; set; }



    }
    public class DesignInfo
    {
        public List<DbTable> DbTables { get; set; }

        public List<BaseItem> Items { get; set; }

        public ListConfigInfo ListConfig { get; set; }

        public ExportConfigInfo ExportConfig { get; set; }

        public int DataSaveType { get; set; } = 1;


        private List<BaseItem> _allItems = null;


        [JsonIgnore]
        public List<BaseItem> AllItems
        {
            get
            {
                if (_allItems == null)
                {
                    _allItems = [];
                    foreach (var item in Items)
                    {
                        if (item is Tab tab)
                        {
                            var tabItems = tab.Tabs.SelectMany(x => x.Items);
                            _allItems.AddRange(tabItems);

                        }
                        else
                        {
                            _allItems.Add(item);
                        }

                    }


                }

                return _allItems;

            }


        }


    }
}
