// Author : Peiyu Wang @ Daphatus
// 16 12 2024 12 16

using System.Collections.Generic;

namespace Managers.AudioManager
{
    public interface IAudioEventReference
    {
        string Identifier { get; } // A unique identifier, could be path or GUID string.
    }

    public interface IAudioParameter
    {
        string Name { get; }
        float Value { get; set; }
    }

    public interface IAudioEventDescription
    {
        string Path { get; }
        IAudioEventInstance CreateInstance();
        void LoadSampleData();
        void UnloadSampleData();
        IReadOnlyList<string> GetParameterNames();
    }

    public interface IAudioEventInstance
    {
        /// <summary>
        /// Starts playing the event instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the event instance.
        /// </summary>
        void Stop(AudioStopMode mode = AudioStopMode.AllowFadeOut);

        /// <summary>
        /// Releases the event instance from memory.
        /// </summary>
        void Release();

        /// <summary>
        /// Sets a parameter by name.
        /// </summary>
        void SetParameter(string parameterName, float value);

        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        AudioPlaybackState GetPlaybackState();
    }

    public interface IAudioEventPlayer
    {
        /// <summary>
        /// Preloads the event referenced by the given IAudioEventReference.
        /// </summary>
        void PreloadEvent(IAudioEventReference reference);

        /// <summary>
        /// Plays a new instance of the given event. Returns a unique instance ID.
        /// Optionally sets parameters before playback and chooses whether to release on finish.
        /// </summary>
        int PlayEventInstance(IAudioEventReference reference, IEnumerable<IAudioParameter> parameters = null, bool releaseOnFinish = false);

        /// <summary>
        /// Stops the specified event instance.
        /// </summary>
        void StopEventInstance(int instanceId, AudioStopMode stopMode = AudioStopMode.AllowFadeOut);

        /// <summary>
        /// Releases the specified event instance.
        /// </summary>
        void ReleaseEventInstance(int instanceId);

        /// <summary>
        /// Sets parameters on a currently playing event instance.
        /// </summary>
        void SetEventInstanceParameters(int instanceId, IEnumerable<IAudioParameter> parameters);

        /// <summary>
        /// Returns the playback state of a currently playing event instance.
        /// </summary>
        AudioPlaybackState GetPlaybackState(int instanceId);
    }

    /// <summary>
    /// Represents how an event instance is stopped.
    /// </summary>
    public enum AudioStopMode
    {
        AllowFadeOut,
        Immediate
    }

    /// <summary>
    /// Represents the current playback state of an event instance.
    /// </summary>
    public enum AudioPlaybackState
    {
        Stopped,
        Playing,
        Sustaining,
        Starting
    }
}