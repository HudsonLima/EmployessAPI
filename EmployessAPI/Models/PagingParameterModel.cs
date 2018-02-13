using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployessAPI.Models
{
    public class PagingParameterModel
    {
        const int maxPageSize = 50;

        public int page { get; set; }
        public int page_size { get; set; }
        
    }
}