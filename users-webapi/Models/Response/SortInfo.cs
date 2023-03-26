using users_webapi.Models.Request;

namespace users_webapi.Models;

public class SortInfo
{
    public SortInfo(string orderBy, SortOrder orderDirection)
    {
        this.OrderedBy = orderBy;
        this.OrderDirection = orderDirection.ToString();
    }
    public string? OrderedBy { get;  }
    public string OrderDirection { get;  }
}
