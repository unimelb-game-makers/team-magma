// Author : Peiyu Wang @ Daphatus
// 05 12 2024 12 04

using Platforms;
using Tempo;
using UnityEngine;

namespace Hazard
{
    public abstract class Hazard : MonoBehaviour, ISyncable
    {   
        public abstract void Affect(TapeType tapeType, float duration, float effectValue);
    }
}