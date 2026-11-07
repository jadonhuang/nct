using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Form.Items
{
    /// <summary>
    /// 并没有DataSourceItem的相关属性只是为了翻译值时方便处理
    /// </summary>
    public class Switch : DataSourceItem
    {
             
        public string ActiveText { get; set; }

        public string InActiveText { get; set; }

    }
}
