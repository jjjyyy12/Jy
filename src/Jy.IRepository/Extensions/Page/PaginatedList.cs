
using System.Collections.Generic;

namespace Jy.IRepositories
{
    public class PaginatedList<T> : List<T> where T : class
    {
        public PaginationBase PaginationBase { get; }

        public PaginatedList(PaginationBase paginationBase, IEnumerable<T> data)
        {
            PaginationBase = paginationBase;
            AddRange(data);
        }

        public bool HasPrevious => PaginationBase.PageIndex > 0;
        public bool HasNext => PaginationBase.PageIndex < PaginationBase.PageCount - 1;
    }
}
