using System.ComponentModel;
using System.Globalization;
using users_webapi.Models.Request;

namespace users_webapi.TypeConverters;

public class PageInfoTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if ((!(value is string queryString)))
        {
            return base.ConvertFrom(context, culture, value);
        }

        var parameters = queryString.Split('&');
        var pageInfo = new PageInfo();

        foreach (var parameter in parameters)
        {
            var keyValue = parameter.Split('=');
            var key = keyValue[0];
            var keyValueList = keyValue[1].Split(',');

            switch (key.ToLower())
            {
                case "sortbyname":
                    pageInfo.SortByName = keyValueList[0];
                    break;
                case "orderby":
                    pageInfo.OrderBy = keyValueList[0];
                    break;
                case "pagenumber":
                    pageInfo.PageNumber = int.Parse(keyValueList[0]);
                    break;
                case "pagesize":
                    pageInfo.PageSize = int.Parse(keyValueList[0]);
                    break;
            }
        }
        
        return pageInfo;
    }
}
