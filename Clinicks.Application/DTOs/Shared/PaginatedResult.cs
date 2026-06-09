using System.Collections.Generic;

namespace Clinicks.Application.DTOs.Shared
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
