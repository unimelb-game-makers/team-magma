using Platforms;

namespace Tempo
{
    public interface ISyncable : IGameService
    {
        void Affect(TapeType tapeType, float duration, float effectValue);
        void OnChangeTrack();
        void OnChangeTempo();
        void OnChangeBar();
        void OnChangeBeat();
        void EnableSync();
        void SyncLow();
        void SyncMid();
        void SyncHigh();
    }
}