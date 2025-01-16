using Platforms;

namespace Tempo
{
    public interface ISyncable : IGameService
    {
        void Affect(TapeType tapeType, float duration, float effectValue);
    }
}