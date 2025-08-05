namespace Tempo
{
    public interface ISyncable : IGameService
    {
        void Affect(TempoMode mode);
    }
}