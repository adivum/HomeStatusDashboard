using Core.Models;
using Core.Models.EntityViews;

namespace Core.Interfaces
{
    public interface IEntityViewFactory
    {
        IEntityView CreateView(HaState entity);
    }
}
