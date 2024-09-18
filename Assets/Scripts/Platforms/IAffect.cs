namespace Platforms
{
    public interface IAffectService : IGameService
    {
        void Affect(TapeType tapeType, float duration, float effectValue);
    }
}