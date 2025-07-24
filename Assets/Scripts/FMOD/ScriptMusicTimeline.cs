//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// timeline markers in your game code. 
//
// Timeline markers can be implicit - such as beats and bars. Or they 
// can be explicity placed by sound designers, in which case they have 
// a sound designer specified name attached to them.
//
// Timeline markers can be useful for syncing game events to sound
// events.
//
// The script starts a piece of music and then displays on the screen
// the current bar and the last marker encountered.
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
// For information on using FMOD example code in your own programs, visit
// https://www.fmod.com/legal
//
//--------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;


namespace Timeline 
{
    public class MusicTimeline : MonoBehaviour
    {
        public static MusicTimeline instance;

        // New and Updated
        [SerializeField] private BeatSettings settings;
        private BeatHandler _beatHandler;
        private BeatSpawner _beatSpawner;

        // Deprecated or Old
        [Header("Parameters")]
        [Tooltip("The current song / tempo")]
        [SerializeField] private int _intensity = 0;
        [SerializeField] static float _beatWindowAround = 0.1f;
        [SerializeField] private float _speedRatio = 1f;
        private float currentTempo;
        [Tooltip("How long to wait between tempo changes")]
        [SerializeField] private float changeTempoDuration = 0.5f;
        private float currentChangeTempoTime;
        private bool toSpawnBeat = false;
        
        [Space(5)]
        [Header("Events")]
        [SerializeField] private bool _timelineInfoDisplayToggle = true;

        class TimelineInfo
        {
            public float CurrentMusicTempo = 0.0f;
            public int CurrentMusicBar = 0;
            public int CurrentMusicBeat = 0; // Added beats info - Ryan
            public FMOD.StringWrapper LastMarker = new FMOD.StringWrapper();
        }

        TimelineInfo timelineInfo;
        GCHandle timelineHandle;

        public FMODUnity.EventReference EventName;

        FMOD.Studio.EVENT_CALLBACK beatCallback;
        FMOD.Studio.EventInstance musicInstance;

        static bool beatTrigger = false;
        static float beatWindowAfter;
        public bool onTempo = false;
        [SerializeField] private float _volume = 1.0f;

        private bool _started = false;
        private static Action onBeat;

#if UNITY_EDITOR
        /// <summary>
        /// Why is this needed?
        /// </summary>
        void Reset()
        {
            EventName = FMODUnity.EventReference.Find("event:/Music/Regulator");
        }
#endif

        private void Awake()
        {
            instance = this;
            timelineInfo = new TimelineInfo();
            onBeat += OnBeat;
        }

        private void StartTrack()
        {
            // Explicitly create the delegate object and assign it to a member so it doesn't get freed
            // by the garbage collected while it's being used
            beatCallback = BeatEventCallback;

            musicInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);

            // Pin the class that will store the data modified during the callback
            timelineHandle = GCHandle.Alloc(timelineInfo);
            // Pass the object through the userdata of the instance
            musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

            musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            musicInstance.start();

            // Init Beat Handler
            _beatHandler = new BeatHandler(settings);
            
            // Find and Init BeatSpawner
            _beatSpawner = GameManager.Instance.BeatSpawner;
            _beatSpawner.Init(_beatHandler);

            SetSpeed(TempoMode.Default);
        }

        private void OnBeat()
        {
            _beatHandler.OnBeat();
            _beatSpawner.OnBeat(_beatHandler.Beat);
        }

        private void Update()
        {
            
            // Debugging Beat Handler
            if (Input.GetMouseButtonDown(0))
            {
                if (!_started)
                {
                    StartTrack();
                    _started = true;
                }
                else
                {
                    Debug.Log(_beatHandler.GetBeatResult());
                }
            }

            if (_started)
            {
                // Update the beat handler!
                _beatHandler.Update(Time.deltaTime);
            }
        }
        

        void LateUpdate() {
            if (onTempo) onTempo = false;
            // Wait for some time before spawning beats each time the tempo changes
            if (currentTempo != timelineInfo.CurrentMusicTempo)
            {
                currentChangeTempoTime = changeTempoDuration;
                currentTempo = timelineInfo.CurrentMusicTempo;
            }

            currentChangeTempoTime -= Time.deltaTime;
            if (currentChangeTempoTime <= 0) {
                if (toSpawnBeat)
                {
                    toSpawnBeat = false;
                    onTempo = true;
                }
            } else {
                toSpawnBeat = false;
            }

            musicInstance.setVolume(_volume);
            
            beatWindowAfter = Math.Max(beatWindowAfter - Time.deltaTime, 0);
            if (beatTrigger && beatWindowAfter == 0) {
                beatTrigger = false;
            }
        }

        void OnDestroy()
        {
            musicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            musicInstance.release();
        }

        void OnGUI()
        {
            if (_timelineInfoDisplayToggle && timelineInfo != null) {
                GUILayout.Box(String.Format("Current Beat = {0}, Current Bar = {1}, Current Tempo = {2}, Last Marker = {3}", timelineInfo.CurrentMusicBeat, timelineInfo.CurrentMusicBar, timelineInfo.CurrentMusicTempo, (string)timelineInfo.LastMarker));
            }
        }

        // Would be better to have in a MusicManager, but for demonstration is here.
        public void SetIntensity(int intensity) {
            _intensity = intensity;
            musicInstance.setParameterByName("Intensity", intensity);
        }
        
        public void SetSpeed(TempoMode mode)
        {
            if (!musicInstance.isValid())
            {
                Debug.LogWarning("Music instance is not valid. Cannot set speed.");
                return;
            }

            float speedRatio = TempoSetting.GetRatio(mode);
            _speedRatio = speedRatio;
            musicInstance.setParameterByName("MusicSpeed", speedRatio);
        }

        public float GetSpeedRatio()
        {
            return _speedRatio;
        }

        public int GetIntensity()
        {
            return _intensity;
        }

        static void SetOnBeat() {
            beatTrigger = true;
            beatWindowAfter = _beatWindowAround;
        }

        public bool GetOnBeat() {
            return beatTrigger;
        }

        // BeatEventCallback: This method is called each time a new beat occurs
        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr timelineInfoPtr;
            FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogError("Timeline Callback error: " + result);
            }
            else if (timelineInfoPtr != IntPtr.Zero)
            {
                // Get the object to store beat and marker details
                GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
                TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

                switch (type)
                {
                    case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        // note: parameter.tempo will be the default 160 at all time.
                        timelineInfo.CurrentMusicTempo = parameter.tempo * MusicTimeline.instance._speedRatio;
                        timelineInfo.CurrentMusicBeat = parameter.beat; // Added beats info - Ryan
                        timelineInfo.CurrentMusicBar = parameter.bar;
                        SetOnBeat();
                        
                        onBeat?.Invoke();

                        // A beat has to be spawned
                        MusicTimeline.instance.toSpawnBeat = true;
                        break;
                    }
                    case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                        timelineInfo.LastMarker = parameter.name;
                        break;
                    }
                    case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        timelineHandle.Free();
                        break;
                    }
                }
            }
            return FMOD.RESULT.OK;
        }

        public void SetMusicVolume(float volume)
        {
            _volume = Mathf.Clamp(volume, 0.0f, 1.0f);
            musicInstance.setVolume(_volume);
        }

        public void PauseMusic()
        {
            if (musicInstance.isValid())
            {
                musicInstance.setPaused(true);
            }

        }
        public void ResumeMusic()
        {
            if (musicInstance.isValid())
            {
                musicInstance.setPaused(false);
            }
        }
    }
}