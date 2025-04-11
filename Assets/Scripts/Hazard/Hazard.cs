// Author : Peiyu Wang @ Daphatus
// 05 12 2024 12 04

using Platforms;
using Tempo;
using UnityEngine;

namespace Hazard
{
    public abstract class Hazard : MonoBehaviour, ISyncable
    {
        [Header("How long the effect will last when switching between tapes.")]
        [Tooltip("If not using default times/values, change to false and adjust. Otherwise ignore.")]
        [SerializeField] protected bool useDefaultEffectTimeValues = true;
        [SerializeField] protected float _fastEffectTime = 5f;
        [SerializeField] protected float _slowEffectTime = 5f;
        [SerializeField] protected float _fastEffectValue = 5f;
        [SerializeField] protected float _slowEffectValue = 5f;
        public abstract void Affect(TapeType tapeType, float duration, float effectValue);
    }
}