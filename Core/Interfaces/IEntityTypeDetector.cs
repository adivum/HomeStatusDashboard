using Core.Models;

namespace Core.Interfaces
{
    public interface IEntityTypeDetector
    {
        EntityType Detect(HaState? entity);
    }
}
