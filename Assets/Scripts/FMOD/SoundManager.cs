//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// the FMOD Studio API in your game code. This can be more effective
// than just using the FMOD for Unity components, and provides access
// to the full capabilities of FMOD Studio.
//
//--------------------------------------------------------------------
// see full: https://fmod.com/docs/2.03/unity/examples-basic.html

using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void Start() {
        SetGameObjectsSFXVolume();
    }

    // Set the volume of all game objects that use SFX sounds.
    public void SetGameObjectsSFXVolume() {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);  // Default to 1 (100%) if not set
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);  // Default to 1 (100%) if not set
        SetEnemiesSFXVolume(masterVolume, sfxVolume);
    }

    // Set the volume of enemies SFX.
    public void SetEnemiesSFXVolume(float masterVolume, float sfxVolume) {
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemyController in enemyControllers) {
            enemyController.SetAudioVolume(masterVolume, sfxVolume);
        }
    }

    // // FMOD Tracked Instance Events
    // public FMODUnity.EventReference PlayerStateEvent;
    // FMOD.Studio.EventInstance playerState;

    // public FMODUnity.EventReference PlayerSoundsEvent;
    // FMOD.Studio.EventInstance playerSounds;

    // // FMOD One-Shot Events
    // public FMODUnity.EventReference DamageEvent;
    // public FMODUnity.EventReference ShootEvent;
    
    // // private float _noteCooldown;
    // // private float _noteLast;
    // // private float _noteDefaultWindow;

    // int health;
    // FMOD.Studio.PARAMETER_ID healthParameterId;
    // Rigidbody cachedRigidBody;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     cachedRigidBody = FindObjectOfType<PlayerController>().GetComponent<Rigidbody>();
    //     health = 1;

    //     // Creates an instance of an FMOD event and manually start it
    //     Debug.Log("Starting Player State Event" + PlayerStateEvent);
    //     playerState = FMODUnity.RuntimeManager.CreateInstance(PlayerStateEvent);
    //     playerState.start();

    //     playerSounds = FMODUnity.RuntimeManager.CreateInstance(PlayerSoundsEvent);
    //     playerSounds.start();

    //     // Automatic position updating of instance events.
    //     FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerSounds, gameObject, GetComponent<Rigidbody>());

    //     // Cache a handle to the "health" parameter for usage in Update() 
    //     // Better than trying to set the parameter by name every update. 
    //     FMOD.Studio.EventDescription healthEventDescription;
    //     playerState.getDescription(out healthEventDescription);
    //     FMOD.Studio.PARAMETER_DESCRIPTION healthParameterDescription;
    //     healthEventDescription.getParameterDescriptionByName("health", out healthParameterDescription);
    //     healthParameterId = healthParameterDescription.id;

    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     // Manual setting of position update for each instance event
    //     //playerState.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject, cachedRigidBody));

    //     // Updates parameter of instance every frame
    //     playerState.setParameterByID(healthParameterId, (float)health);

    //     // This shows how to query the playback state of an event instance.
    //     // This can be useful when playing a one-shot to take action
    //     // when it finishes. Other playback states can be checked including
    //     // Sustaining and Fading Out.
    //     if (playerSounds.isValid())
    //     {
    //         FMOD.Studio.PLAYBACK_STATE playbackState;
    //         playerSounds.getPlaybackState(out playbackState);
    //         if (playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
    //         {
    //             playerSounds.release();
    //             playerSounds.clearHandle();
    //             SendMessage("PlayerIntroFinished");
    //         }
    //     }
    // }

    // void OnDestroy()
    // {
    //     StopAllPlayerEvents();
    //     playerState.release();
    // }

    // void SpawnIntoWorld()
    // {
    //     health = 1;
    //     playerState.start();
    // }

    // void TakeDamageSound(bool isInvulnerable) {

    //     // This section shows how to manually play a one-shot sound so we
    //     // can set a parameter before starting.
    //     FMOD.Studio.EventInstance damage = FMODUnity.RuntimeManager.CreateInstance(DamageEvent);
    //     damage.setParameterByID(healthParameterId, isInvulnerable ? 1.0f : 0.0f); // Could be used to apply an effect to make dampened "shielded" hit sound.
    //     damage.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    //     damage.start();
    //     damage.release();

    //     // ALTERNATIVELY Shows how to play a one-shot at the game object's current location directly.
    //     FMODUnity.RuntimeManager.PlayOneShot(DamageEvent, transform.position);

    //     if (health == 0)
    //     {
    //         // This shows how to stop a sound while allowing the AHDSR set by
    //         // the sound designer to control the fade out.
    //         playerState.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    //     }
    // }

    // // Retrieves a "bus" controlling multiple events, and can stop one-shots still playing that don't have a tracked instance.
    // void StopAllPlayerEvents()
    // {
    //     FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("bus:/player");
    //     playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    // }
}
