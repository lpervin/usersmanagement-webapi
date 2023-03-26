using System.Text.Json.Serialization;

namespace users_webapi.Models.Request;

public class PageInfo
{
    private int _pageNum;
    private int _pageSize;
    private string? _sortBy;

    private string? _orderBY;
   // private SortOrder? _sortOrder;
    public PageInfo()
    {
        _pageNum = 1;
        _pageSize = 5;
        _sortBy = "Id";
        _orderBY = "Desc";
    }
    public int PageNumber { get => _pageNum; set => _pageNum = value; }
    public int PageSize { get => _pageSize; set => _pageSize = value; }
    public string? SortByName { get => _sortBy;  }
     public string? OrderBy { get => _sortBy; set => _orderBY = value;
     }
    [JsonIgnore]
    internal SortOrder? Order {
        get
        {
            var parsedSortBy = SortOrder.Desc;
            if (!Enum.TryParse(_orderBY, out parsedSortBy))
                return SortOrder.Desc;

            return parsedSortBy;

        }
          }

}

public enum SortOrder
{
    Asc=0,
    Desc=1
}
