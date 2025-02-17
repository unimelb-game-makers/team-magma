using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMODUnity.STOP_MODE;

namespace Managers.AudioManager
{
    /// <summary>
    /// A concrete FMOD-based implementation of IAudioEventPlayer.
    /// This class is responsible for:
    /// - Preloading events and their sample data
    /// - Playing multiple instances of events
    /// - Stopping, releasing, and modifying parameters of those instances
    /// - Managing the lifecycle of events (one-shots or tracked instances)
    /// </summary>
    public class AudioEventPlayer : MonoBehaviour, IAudioEventPlayer
    {
        // Maps event identifier strings (usually FMOD event paths) to their FMODAudioEventDescription.
        private Dictionary<string, FMODAudioEventDescription> eventDescriptions = new Dictionary<string, FMODAudioEventDescription>();

        // Tracks active event instances by a unique integer ID. This allows you to have multiple instances of the same event.
        private Dictionary<int, IAudioEventInstance> activeInstances = new Dictionary<int, IAudioEventInstance>();

        // A counter used to assign unique IDs to newly created event instances.
        private int nextInstanceId = 1;

        /// <summary>
        /// Preloads the event referenced by an IAudioEventReference, ensuring its sample data is ready.
        /// This avoids potential playback latency the first time an event is played.
        /// </summary>
        /// <param name="reference">The reference to the FMOD event.</param>
        public void PreloadEvent(IAudioEventReference reference)
        {
            if (reference is FMODAudioEventReference fmodRef)
            {
                string key = fmodRef.Identifier;
                if (!eventDescriptions.ContainsKey(key))
                {
                    // Get the FMOD EventDescription from the FMOD Studio system using the provided reference path.
                    RuntimeManager.StudioSystem.getEvent(fmodRef.FmodReference.Path, out EventDescription eventDesc);
                    if (eventDesc.isValid())
                    {
                        // Wrap the FMOD EventDescription in our FMODAudioEventDescription class for easy handling.
                        var fmodDesc = new FMODAudioEventDescription(eventDesc);
                        fmodDesc.LoadSampleData();
                        eventDescriptions[key] = fmodDesc;
                    }
                    else
                    {
                        Debug.LogWarning($"[FMODAudioEventPlayer] Could not load event: {fmodRef.Identifier}");
                    }
                }
            }
            else
            {
                Debug.LogWarning("[FMODAudioEventPlayer] Provided reference is not an FMOD reference.");
            }
        }

        /// <summary>
        /// Plays a new instance of the specified event. This returns an integer ID which can be used to control
        /// the event instance later (e.g., stopping it, setting parameters).
        /// </summary>
        /// <param name="reference">The event reference to play.</param>
        /// <param name="parameters">Optional parameters to set before playback starts.</param>
        /// <param name="releaseOnFinish">If true, the instance is automatically released after it finishes playing.</param>
        /// <returns>The unique instance ID of the created event instance, or -1 if the event could not be played.</returns>
        public int PlayEventInstance(IAudioEventReference reference, IEnumerable<IAudioParameter> parameters = null, bool releaseOnFinish = false)
        {
            if (reference is FMODAudioEventReference fmodRef)
            {
                // Ensure event is preloaded
                PreloadEvent(fmodRef);

                // Retrieve the event description
                if (!eventDescriptions.TryGetValue(fmodRef.Identifier, out FMODAudioEventDescription desc))
                {
                    Debug.LogWarning($"[FMODAudioEventPlayer] Event not found: {fmodRef.Identifier}");
                    return -1;
                }

                // Create a new event instance
                IAudioEventInstance instance = desc.CreateInstance();

                // Apply any initial parameters before starting the event
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        instance.SetParameter(param.Name, param.Value);
                    }
                }

                // Start playing the event
                instance.Start();

                // Assign a unique ID and store the instance so we can manage it later
                int id = nextInstanceId++;
                activeInstances[id] = instance;

                // If one-shot, automatically release when the event has stopped playing
                if (releaseOnFinish)
                {
                    StartCoroutine(OneShotReleaseCoroutine(id));
                }

                return id;
            }
            else
            {
                Debug.LogWarning("[FMODAudioEventPlayer] Provided reference is not an FMOD reference.");
                return -1;
            }
        }

        /// <summary>
        /// Stops a currently playing event instance by its instance ID.
        /// You can allow fadeout or stop immediately.
        /// </summary>
        /// <param name="instanceId">The instance ID returned by PlayEventInstance.</param>
        /// <param name="stopMode">How to stop the event: with or without fadeout.</param>
        public void StopEventInstance(int instanceId, AudioStopMode stopMode = AudioStopMode.AllowFadeOut)
        {
            if (activeInstances.TryGetValue(instanceId, out IAudioEventInstance instance))
            {
                instance.Stop(stopMode);
            }
        }

        /// <summary>
        /// Releases a currently tracked event instance, removing it from memory.
        /// This should only be called after the event instance has stopped.
        /// </summary>
        /// <param name="instanceId">The ID of the instance to release.</param>
        public void ReleaseEventInstance(int instanceId)
        {
            if (activeInstances.TryGetValue(instanceId, out IAudioEventInstance instance))
            {
                instance.Release();
                activeInstances.Remove(instanceId);
            }
        }

        /// <summary>
        /// Sets parameters on a currently playing event instance.
        /// </summary>
        /// <param name="instanceId">The ID of the event instance.</param>
        /// <param name="parameters">A collection of parameters to apply.</param>
        public void SetEventInstanceParameters(int instanceId, IEnumerable<IAudioParameter> parameters)
        {
            if (activeInstances.TryGetValue(instanceId, out IAudioEventInstance instance))
            {
                foreach (var param in parameters)
                {
                    instance.SetParameter(param.Name, param.Value);
                }
            }
            else
            {
                Debug.LogWarning($"[FMODAudioEventPlayer] No active instance found with ID {instanceId}.");
            }
        }

        /// <summary>
        /// Retrieves the current playback state of the event instance.
        /// </summary>
        /// <param name="instanceId">The ID of the event instance.</param>
        /// <returns>The playback state (Playing, Stopped, Sustaining, Starting).</returns>
        public AudioPlaybackState GetPlaybackState(int instanceId)
        {
            if (activeInstances.TryGetValue(instanceId, out IAudioEventInstance instance))
            {
                return instance.GetPlaybackState();
            }

            return AudioPlaybackState.Stopped;
        }

        /// <summary>
        /// A coroutine used for one-shot events. It waits until the event finishes playing, then releases it automatically.
        /// </summary>
        private System.Collections.IEnumerator OneShotReleaseCoroutine(int instanceId)
        {
            IAudioEventInstance instance = activeInstances[instanceId];
            while (instance.GetPlaybackState() != AudioPlaybackState.Stopped)
            {
                yield return null; // Wait until the next frame, then check again
            }
            ReleaseEventInstance(instanceId);
        }

        private void OnDestroy()
        {
            // If this component is destroyed (e.g., on scene change), make sure to clean up active instances
            foreach (var kvp in activeInstances)
            {
                kvp.Value.Stop(AudioStopMode.Immediate);
                kvp.Value.Release();
            }
            activeInstances.Clear();
        }
    }

    /// <summary>
    /// A concrete implementation of IAudioEventReference for FMOD.
    /// Encapsulates an FMOD EventReference and provides a string identifier for lookup.
    /// </summary>
    public class FMODAudioEventReference : IAudioEventReference
    {
        private EventReference reference;
        public FMODAudioEventReference(EventReference eventRef)
        {
            reference = eventRef;
        }

        public string Identifier => reference.Path; // Could also use GUID.ToString() if desired
        public EventReference FmodReference => reference;
    }

    /// <summary>
    /// A basic implementation of IAudioParameter.
    /// This is just a name-value pair used to set parameters on event instances.
    /// </summary>
    public class FMODAudioParameter : IAudioParameter
    {
        public string Name { get; private set; }
        public float Value { get; set; }

        public FMODAudioParameter(string name, float value)
        {
            Name = name;
            Value = value;
        }
    }

    /// <summary>
    /// A concrete implementation of IAudioEventDescription for FMOD.
    /// Encapsulates an FMOD EventDescription, allowing creation of instances and loading/unloading of sample data.
    /// </summary>
    public class FMODAudioEventDescription : IAudioEventDescription
    {
        private EventDescription eventDescription;

        public FMODAudioEventDescription(EventDescription description)
        {
            eventDescription = description;
        }

        public string Path
        {
            get
            {
                eventDescription.getPath(out string path);
                return path;
            }
        }

        public IAudioEventInstance CreateInstance()
        {
            eventDescription.createInstance(out EventInstance instance);
            return new FMODAudioEventInstance(instance);
        }

        public void LoadSampleData()
        {
            eventDescription.loadSampleData();
        }

        public void UnloadSampleData()
        {
            eventDescription.unloadSampleData();
        }

        public IReadOnlyList<string> GetParameterNames()
        {
            eventDescription.getParameterDescriptionCount(out int count);
            List<string> paramNames = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                eventDescription.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION pDesc);
                paramNames.Add(pDesc.name);
            }
            return paramNames;
        }

        public EventDescription GetRawDescription() => eventDescription;
    }

    /// <summary>
    /// A concrete implementation of IAudioEventInstance for FMOD.
    /// Encapsulates an FMOD EventInstance, providing methods to start/stop/play, set parameters, and check state.
    /// </summary>
    public class FMODAudioEventInstance : IAudioEventInstance
    {
        private EventInstance instance;
        private Dictionary<string, PARAMETER_ID> parameterMap;

        public FMODAudioEventInstance(EventInstance instance)
        {
            this.instance = instance;
            BuildParameterMap();
        }

        public void Start()
        {
            instance.start();
        }

        public void Stop(AudioStopMode mode = AudioStopMode.AllowFadeOut)
        {
            var fmodMode = mode == AudioStopMode.AllowFadeOut ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE;
            instance.stop(fmodMode);
        }

        public void Release()
        {
            instance.release();
        }

        public void SetParameter(string parameterName, float value)
        {
            // Look up the parameter ID and set its value
            if (parameterMap.TryGetValue(parameterName, out PARAMETER_ID paramID))
            {
                instance.setParameterByID(paramID, value);
            }
            else
            {
                Debug.LogWarning($"Parameter '{parameterName}' not found.");
            }
        }

        public AudioPlaybackState GetPlaybackState()
        {
            instance.getPlaybackState(out PLAYBACK_STATE state);
            return state switch
            {
                PLAYBACK_STATE.PLAYING => AudioPlaybackState.Playing,
                PLAYBACK_STATE.STOPPED => AudioPlaybackState.Stopped,
                PLAYBACK_STATE.SUSTAINING => AudioPlaybackState.Sustaining,
                PLAYBACK_STATE.STARTING => AudioPlaybackState.Starting,
                _ => AudioPlaybackState.Stopped
            };
        }

        /// <summary>
        /// Builds a dictionary from parameter names to their PARAMETER_IDs for quick lookups.
        /// This is done once at construction to improve performance when setting parameters repeatedly.
        /// </summary>
        private void BuildParameterMap()
        {
            instance.getDescription(out EventDescription desc);
            desc.getParameterDescriptionCount(out int paramCount);
            parameterMap = new Dictionary<string, PARAMETER_ID>(paramCount);
            for (int i = 0; i < paramCount; i++)
            {
                desc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION pDesc);
                parameterMap[pDesc.name] = pDesc.id;
            }
        }
    }
}