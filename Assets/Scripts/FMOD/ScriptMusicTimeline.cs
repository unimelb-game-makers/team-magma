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
using UnityEngine;


namespace Timeline 
{
    public class MusicTimeline : MonoBehaviour
    {
        public static MusicTimeline instance;
        private BeatSpawner beatSpawner;

        [Header("Parameters")]
        [Tooltip("The current song / tempo")]
        [SerializeField] private int _intensity = 0;
        [SerializeField] static float _beatWindowAround = 0.1f;
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

    #if UNITY_EDITOR
        void Reset()
        {
            EventName = FMODUnity.EventReference.Find("event:/music/music");
        }
    #endif

        void Awake()
        {
            instance = this;
            timelineInfo = new TimelineInfo();
        }
        void Start()
        {
            beatSpawner = BeatSpawner.Instance;
            // Explicitly create the delegate object and assign it to a member so it doesn't get freed
            // by the garbage collected while it's being used
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);

            musicInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);

            // Pin the class that will store the data modified during the callback
            timelineHandle = GCHandle.Alloc(timelineInfo);
            // Pass the object through the userdata of the instance
            musicInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));

            musicInstance.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            musicInstance.start();

            SetIntensity(0);
        }

        void Update() {
            // Wait for some time before spawning beats each time the tempo changes
            if (currentTempo != timelineInfo.CurrentMusicTempo) {
                currentChangeTempoTime = changeTempoDuration;
                currentTempo = timelineInfo.CurrentMusicTempo;
            }

            currentChangeTempoTime -= Time.deltaTime;
            if (currentChangeTempoTime <= 0) {
                if (toSpawnBeat) {
                    toSpawnBeat = false;
                    beatSpawner.SetTempo(currentTempo);
                    beatSpawner.SpawnBeat();
                }
            } else {
                toSpawnBeat = false;
            }

            //musicInstance.setParameterByName("Intensity", _intensity);
            //musicInstance.setParameterByName("Stinger", 0);

            if (timelineInfo.CurrentMusicBar == 6 && timelineInfo.CurrentMusicBeat == 4) {
                // Change to Low intensity after Intro is finished first loop
                //SetIntensity(1);
            }
            
            beatWindowAfter = Math.Max(beatWindowAfter - Time.deltaTime, 0);
            if (beatTrigger && beatWindowAfter == 0) {
                // Remove the beat trigger window after;
                beatTrigger = false;
            }
            // TODO: Figure out predictive "before window" via current tempo and last beat. 
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
            //musicInstance.setParameterByName("Stinger", 1);
        }

        public int GetIntensity() {
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
                        timelineInfo.CurrentMusicTempo = parameter.tempo; // Addded tempo info - Ryan
                        timelineInfo.CurrentMusicBeat = parameter.beat; // Added beats info - Ryan
                        timelineInfo.CurrentMusicBar = parameter.bar;
                        SetOnBeat();

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
    }
}