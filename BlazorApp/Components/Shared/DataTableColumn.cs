using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Shared
{
    public class DataTableColumn<TItem>
    {
        public string Title { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public RenderFragment<TItem>? Template { get; set; }

        public DataTableColumn(string title, string propertyName)
        {
            Title = title;
            PropertyName = propertyName;
        }

        public DataTableColumn(string title, string propertyName, RenderFragment<TItem> template)
        {
            Title = title;
            PropertyName = propertyName;
            Template = template;
        }
    }
}
