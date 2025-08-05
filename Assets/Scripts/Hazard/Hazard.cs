// Author : Peiyu Wang @ Daphatus
// 05 12 2024 12 04

using Platforms;
using Tempo;
using UnityEngine;

namespace Hazard
{
    public abstract class Hazard : MonoBehaviour, ISyncable
    {
        [SerializeField] protected float _fastEffectValue = 5f;
        [SerializeField] protected float _slowEffectValue = 5f;
        public abstract void Affect(TempoMode mode);
    }
}