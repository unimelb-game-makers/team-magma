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
    }
}