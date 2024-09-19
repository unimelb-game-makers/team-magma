using UnityEngine;

namespace Platforms

{
    public abstract class PlatformComponent : MonoBehaviour, IAffectService
    {
        public abstract void Affect(TapeType tapeType, float duration, float effectValue);
    }
}