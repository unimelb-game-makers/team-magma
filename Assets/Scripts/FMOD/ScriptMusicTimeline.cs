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
using FMOD.Studio;
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

        [SerializeField] private float _volume = 1.0f;

        private bool _started = false;
        public static Action OnBeat;
        public static Action<TempoMode> OnTempoChanged;

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
            OnBeat += OnBeatInternal;
        }

        private void Start()
        {
            // Init Beat Handler
            _beatHandler = new BeatHandler(settings);
            
            // Find and Init BeatSpawner
            _beatSpawner = GameManager.Instance.BeatSpawner;
            _beatSpawner.Init(_beatHandler);
        }

        /// <summary>
        /// Used to stop the currently playing track.
        /// </summary>
        public void StopTrack()
        {
            musicInstance.stop(STOP_MODE.IMMEDIATE);
        }
        
        /// <summary>
        /// Used to start the track. It will automatically stop a currently playing track.
        /// </summary>
        public void StartTrack()
        {
            // Stop the track if it is already playing
            musicInstance.stop(STOP_MODE.IMMEDIATE);
            
            // Explicitly create the delegate object and assign it to a member so it doesn't get freed
            // by the garbage collected while it's being used
            beatCallback = BeatEventCallback;

            musicInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);

            // Pin the class that will store the data modified during the callback
            timelineHandle = GCHandle.Alloc(timelineInfo);
            // Pass the object through the userdata of the instance
            musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

            musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            musicInstance.setVolume(_volume);
            musicInstance.start();

            SetSpeed(TempoMode.Default);
        }

        /// <summary>
        /// This is the main entry point into interacting with the music.
        /// It will process and action and determine its result, notifying the system and the UI
        /// </summary>
        /// <returns></returns>
        public BeatResult ProcessAction()
        {
            BeatResult result = _beatHandler.GetBeatResult();

            _beatHandler.ProcessBeat(result);
            _beatSpawner.ProcessBeat(result);
            return result;
        }

        private void OnBeatInternal()
        {
            _beatHandler.OnBeat();
            _beatSpawner.OnBeat(_beatHandler.Beat);
        }

        private void Update()
        {
            _beatHandler.Update(Time.deltaTime);
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
        
        public void SetSpeed(TempoMode mode)
        {
            if (!musicInstance.isValid())
            {
                Debug.LogWarning("Music instance is not valid. Cannot set speed.");
                return;
            }

            float speedRatio = TempoSetting.GetRatio(mode);
            _speedRatio = speedRatio;
            // musicInstance.setParameterByName("MusicSpeed", speedRatio);
            musicInstance.setPitch(speedRatio);
            
            _beatHandler.OnTempoChanged(mode);
            _beatSpawner.OnTempoChanged(mode);
            OnTempoChanged?.Invoke(mode);
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
                        
                        OnBeat?.Invoke();

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