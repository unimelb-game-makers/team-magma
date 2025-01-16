using UnityEngine;

namespace RhythmStateMachine
{
    public class RhythmStateMachine : MonoBehaviour
    {
        [SerializeField] private float _BPM = 140.0f;

        // FMOD Tracked Instance Events
        public FMODUnity.EventReference PlayerStateEvent;
        FMOD.Studio.EventInstance playerState;

        // FMOD Non-Tracked Events
        public FMODUnity.EventReference DamageEvent;
        public FMODUnity.EventReference HealEvent;
        
        private float _noteCooldown;
        private float _noteLast;
        private float _noteDefaultWindow; // Default time window size for trigger events.

        private void Start()
        {
            _noteLast = Time.time;
        }
        
        public float GetQuarterNote()
        {
            return 0;
        }

        public float GetNote()
        {
            return 0;
        }
    }
}