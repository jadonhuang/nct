
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Nct.Module.Form.Helpers;

namespace Nct.Module.Form.Items
{


    public abstract class DataSourceItem : BaseItem
    {
        public string CascadeParent { get; set; }
        public string CascadeField { get; set; }
        public string DataSourceCode { get; set; }
        public int DataSourceType { get; set; }
        public string ValueField { get; set; }
        public string TextField { get; set; }

    }



    [JsonConverter(typeof(ItemJsonConverter))]
    public abstract class BaseItem
    {

        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Table { get; set; }


        public virtual string Field { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// 所占列数
        /// </summary>
        public virtual int Span { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public virtual bool Required { get; set; }

        /// <summary>
        /// 是否只读
        /// </summary>
        public virtual bool ReadOnly { get; set; }

        public virtual bool Visible { get; set; }

 
        /// <summary>
        /// 提示
        /// </summary>
        public virtual string Tip { get; set; }

        [JsonIgnore]
        public bool IsTable
        {

            get
            {
                return Type == "Table";
            }
        
        }

    }
}
