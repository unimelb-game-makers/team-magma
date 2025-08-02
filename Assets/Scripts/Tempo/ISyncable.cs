using Platforms;

namespace Tempo
{
    public interface ISyncable : IGameService
    {
        void Affect(TempoMode mode, float duration, float effectValue);
    }
}