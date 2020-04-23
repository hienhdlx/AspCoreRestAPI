using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreRestAPI.Dtos
{
    public class PagedResult<T>
    {
        public List<T> ItemList { get; set; }
        public int TotalRow { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
