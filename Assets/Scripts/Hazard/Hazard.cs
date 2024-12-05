// Author : Peiyu Wang @ Daphatus
// 05 12 2024 12 04

using Platforms;
using Tempo;
using UnityEngine;

namespace Hazard
{
    [RequireComponent(typeof(Collider))]
    public class Hazard : MonoBehaviour, ISyncable
    {
        public void OnTriggerEnter(Collider other)
        {
            
        }
        
        public void Affect(TapeType tapeType, float duration, float effectValue)
        {
            throw new System.NotImplementedException();
        }

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