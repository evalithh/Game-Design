using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

using FMOD.Studio;
using System;
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;


    [Header("FMOD Parameters")]
    [Tooltip("The intensity of the randomness of the sound, 0% is no randomness, 100% is full randomness, this can be used to add variety to sounds, such as stylization, pitch, volume etc.")]
    [SerializeField][Range(0, 100)] private int GlobalRandomnessIntensityValue = 50;

    [Tooltip("A list of custom parameters that can be used to change the intensities for sounds, add your own custom parameter(s) here that you would like to be able to change")]
    public List<string> CustomParameter = new List<string>();
    //TODO: Dictionary for CustomParameter

    [Header("Ambient")]
    [SerializeField] EventReference SFX_Ambient;

    [Header("Developer Attributes")]
    [SerializeField] bool developerMode = false;
    [SerializeField] EventReference SFX_TestAudio;

    /// Local Attributes
    // FMOD Parameters
    private string RandomnessIntensity = "RandomnessIntensity";
    private EventInstance i_Ambient;


    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }

        i_Ambient = PlayLoop(SFX_Ambient, transform.position, gameObject);

    }


    private void Update() {
        if (developerMode) if (Input.GetKeyDown(KeyCode.P)) PlaySFX(SFX_TestAudio, transform.position);
    }

    public void PlaySFX( // Plays a oneshot SFX with position-dependent audio, where parameters cannot be changed after initiation, ideal for short-duration SFX
        EventReference sfx,
        Vector3 playPosition,
        int randomnessIntensityValue = 1
    ) {
        if (!sfx.IsNull) {
            EventInstance sfxInstance = CreateInstance(sfx, playPosition);
            sfxInstance = CreateInstance(sfx, playPosition);
            if (randomnessIntensityValue != 0) sfxInstance.setParameterByName(RandomnessIntensity, randomnessIntensityValue);
            if (randomnessIntensityValue < 0) sfxInstance.setParameterByName(RandomnessIntensity, GlobalRandomnessIntensityValue);

            sfxInstance.start();
            sfxInstance.release();
        }
        else Debug.Log("Sound not found: " + sfx);

    }

    public EventInstance PlayLoop( // Plays a looping sound with position-dependent audio, where parameters can be changed after initiation, ideal for long-duration sounds
        EventReference sound,
        Vector3 playPosition,
        GameObject gameObject,
        int randomnessIntensityValue = -1
    ) {
        if (!sound.IsNull) {
            EventInstance loopInstance = CreateInstance(sound, playPosition);
            RuntimeManager.AttachInstanceToGameObject(loopInstance, gameObject);
            if (randomnessIntensityValue != 0) loopInstance.setParameterByName(RandomnessIntensity, randomnessIntensityValue);
            if (randomnessIntensityValue < 0) loopInstance.setParameterByName(RandomnessIntensity, GlobalRandomnessIntensityValue);
            loopInstance.start();
            return loopInstance;
        }
        else Debug.Log("Sound not found: " + sound);
        return default(EventInstance);
    }

    public EventInstance CreateInstance(EventReference audio, Vector3 eventPosition) {
        EventInstance audioInstance = RuntimeManager.CreateInstance(audio);
        audioInstance.set3DAttributes(RuntimeUtils.To3DAttributes(eventPosition));
        return audioInstance;

    }

    public void ContinueLoop(params EventInstance[] audioInstances) {
        foreach (var instance in audioInstances) {
            instance.setPaused(false);
        }
    }
    public void PauseLoop(params EventInstance[] audioInstances) {
        foreach (var instance in audioInstances) {
            instance.setPaused(true);
        }
    }

    public void StopLoop(params EventInstance[] audioInstances) {
        foreach (var instance in audioInstances) {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void ClearInstance(params EventInstance[] audioInstances) {
        foreach (EventInstance instance in audioInstances) {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    public bool IsAudioPlaying(params EventInstance[] audioInstances) {
        foreach (EventInstance instance in audioInstances) {
            instance.getPaused(out bool paused);
            if (paused) return false;
        }
        return true;
    }


    public void SetParameter(EventInstance audioInstance, string parameterName, float value) => audioInstance.setParameterByName(parameterName, value);

}
