using Tempo;
using UnityEngine;

namespace Platforms

{
    public abstract class PlatformComponent : MonoBehaviour, ISyncable
    {
        public abstract void Affect(TapeType tapeType, float duration, float effectValue);
        public void OnChangeTrack()
        {
            throw new System.NotImplementedException();
        }

        public void OnChangeTempo()
        {
            throw new System.NotImplementedException();
        }

        public void OnChangeBar()
        {
            throw new System.NotImplementedException();
        }

        public void OnChangeBeat()
        {
            throw new System.NotImplementedException();
        }

        public void EnableSync()
        {
            throw new System.NotImplementedException();
        }

        public void SyncLow()
        {
            throw new System.NotImplementedException();
        }

        public void SyncMid()
        {
            throw new System.NotImplementedException();
        }

        public void SyncHigh()
        {
            throw new System.NotImplementedException();
        }
    }
}