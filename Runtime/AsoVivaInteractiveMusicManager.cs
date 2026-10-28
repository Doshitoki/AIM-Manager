using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MusicLayer
{
    public string layerName;
    public AudioClip audioClip;
    public bool alwaysOn;
    public float targetVolume = 1f;

    [HideInInspector] public AudioSource audioSource;
}

[System.Serializable]
public class MusicState
{
    public string stateName;
    public List<MusicLayer> layers = new List<MusicLayer>();
}

public class AsoVivaInteractiveMusicManager : MonoBehaviour
{
    public List<MusicState> states = new List<MusicState>();

    public string defaultState = "Walking";
    public float fadeTime = 1.5f;
    public bool playOnStart = true;

    private string currentState;
    private readonly Dictionary<AudioSource, Coroutine> fadeCoroutines = new();

    private void Awake()
    {
        CreateAudioSources();
    }

    private void Start()
    {
        currentState = defaultState;

        if (playOnStart)
        {
            PlayAllSynced();
        }

        ApplyVolumes(true);
    }

    private void CreateAudioSources()
    {
        foreach (var state in states)
        {
            foreach (var layer in state.layers)
            {
                if (layer.audioClip == null) continue;

                GameObject child = new GameObject($"{state.stateName}_{layer.layerName}");
                child.transform.SetParent(transform);

                AudioSource source = child.AddComponent<AudioSource>();
                source.clip = layer.audioClip;
                source.loop = true;
                source.playOnAwake = false;
                source.volume = 0f;

                layer.audioSource = source;
            }
        }
    }

    public void PlayAllSynced()
    {
        foreach (var state in states)
        {
            foreach (var layer in state.layers)
            {
                if (layer.audioSource == null) continue;

                layer.audioSource.volume = 0f;

                if (!layer.audioSource.isPlaying)
                {
                    layer.audioSource.Play();
                }
            }
        }
    }

    public void ChangeState(string stateName)
    {
        currentState = stateName;
        ApplyVolumes(false);
    }

    public void UnlockLayer(string layerName)
    {
        AsoVivaMusicProgress.Unlock(layerName);
        ApplyVolumes(false);
    }

    public void LockLayer(string layerName)
    {
        AsoVivaMusicProgress.Lock(layerName);
        ApplyVolumes(false);
    }

    public void ResetProgress()
    {
        AsoVivaMusicProgress.Reset();
        ApplyVolumes(false);
    }

    public void StopAll()
    {
        foreach (var state in states)
        {
            foreach (var layer in state.layers)
            {
                if (layer.audioSource == null) continue;

                layer.audioSource.Stop();
                layer.audioSource.volume = 0f;
            }
        }
    }

    private void ApplyVolumes(bool instant)
    {
        foreach (var state in states)
        {
            bool isCurrentState = state.stateName == currentState;

            foreach (var layer in state.layers)
            {
                if (layer.audioSource == null) continue;

                bool unlocked = layer.alwaysOn || AsoVivaMusicProgress.IsUnlocked(layer.layerName);

                float target = isCurrentState && unlocked
                    ? layer.targetVolume
                    : 0f;

                if (instant)
                {
                    layer.audioSource.volume = target;
                }
                else
                {
                    FadeTo(layer.audioSource, target);
                }
            }
        }
    }

    private void FadeTo(AudioSource source, float targetVolume)
    {
        if (fadeCoroutines.TryGetValue(source, out Coroutine running))
        {
            StopCoroutine(running);
        }

        fadeCoroutines[source] = StartCoroutine(FadeRoutine(source, targetVolume));
    }

    private IEnumerator FadeRoutine(AudioSource source, float targetVolume)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float t = time / fadeTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, t);
            yield return null;
        }

        source.volume = targetVolume;
    }
}