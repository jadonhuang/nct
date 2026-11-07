using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Models
{


    public interface ITreeNode<TType, TModel>
    {
        TType Id { get; set; }
        TType ParentId { get; set; }
        IList<TModel> Children { get; set; }

    }



}
