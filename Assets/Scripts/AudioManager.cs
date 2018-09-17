using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

[System.Serializable]
public struct AudioTask
{
    public string clipName;
    public Transform parent;
    public bool setLocationButDontParent;
    [Range(0, 255)]
    public int priority;
    public float blockInterruptPriority;
    public bool loop;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] int MaxSources = 20;

    class AudioInstance
    {
        public AudioTask task;
        public AudioSource source;

        public void setup(AudioTask task) {
            this.task = task;
            if(!source) {
                GameObject go = new GameObject(string.Format("Audio{0}", task.clipName));
                source = go.AddComponent<AudioSource>();
                source.loop = task.loop;
                source.priority = task.priority;
                source.clip = FindClip(task.clipName);
            }
            source.transform.position = task.parent.transform.position;
            if (!task.setLocationButDontParent) {
                source.transform.SetParent(task.parent);
            }
        }
    }

    RingBuffer<AudioInstance> instances;

    public Transform listener;

    [SerializeField] float MaxAddAudioDistance = 100f;

    //[SerializeField]
    //private OnOffImageToggle displayMuted;

    //Dictionary<string, AudioSource> audios = new Dictionary<string, AudioSource>();

    [SerializeField]
    Transform audioSourceFolder;

    [SerializeField] AudioSource fireAudio;
    [SerializeField] AudioSource reloadAudio;

    private const string PPrefKeyMuted = "PPrefsMuted";
    private bool getPlayerPrefsMuted() {
        if (PlayerPrefs.HasKey(PPrefKeyMuted)) { return PlayerPrefs.GetInt(PPrefKeyMuted) > 0; }
        return false;
    }

    private Watchable<bool> _muted;
    private Watchable<bool> muted {
        get {
            if (_muted == null) {
                _muted = new Watchable<bool>(getPlayerPrefsMuted());
            }
            return _muted;
        }
    }

    private void setMuted(bool shouldMute) {
        muted._value = shouldMute;
    }


    public void toggleMute() {
        setMuted(!muted._value);
    }

    public static string Dink = "Dink";
    public static string Zap = "Zap";
    public void Awake() {
        instances = new RingBuffer<AudioInstance>(MaxSources);
    }

    private void Start() {
        //muted.subscribe((bool b) => { displayMuted.toggle(b); });
        //setMuted(getPlayerPrefsMuted());
    }

    bool shouldPlay(AudioTask audioTask) {
        if(!listener) { return true; }
        return (listener.transform.position - audioTask.parent.position).magnitude < MaxAddAudioDistance;
    }

    public void play(AudioTask audioTask) {
        if (muted._value) {
            print("muted");
            return;
        }
        if(!shouldPlay(audioTask)) {
            print("shouldnt play");
            return;
        }
        print("will play");
        AudioInstance inst = nextInstance();
        inst.setup(audioTask);
        inst.source.Play();

    }

    //lazy loading
    private AudioInstance nextInstance() {
        AudioInstance inst = findFreeInstance();
        if(inst == null) {
            inst = new AudioInstance();
            instances.push(inst);
        }

        return inst;

        //if (audios.ContainsKey(resourcesAudioRelativePath)) { return audios[resourcesAudioRelativePath]; }

        //AudioClip clip = FindClip(resourcesAudioRelativePath);
        //Assert.IsTrue(clip, "null audio clip? " + resourcesAudioRelativePath);
        //GameObject go = new GameObject(resourcesAudioRelativePath);
        //AudioSource aud = go.AddComponent<AudioSource>();
        //aud.clip = clip;
        //go.transform.SetParent(audioSourceFolder);
        //audios.Add(go.name, aud);
        //return aud;
    }

    AudioInstance findFreeInstance() {
        for(int i=0; i < instances.size; ++i) {
            AudioInstance inst = instances[i];
            if(inst == null) {
                instances[i] = new AudioInstance();
                return instances[i];
            }

            if(inst.source == null || !inst.source.isPlaying) {
                return inst;
            }
        }
        return null;
    }

    public static AudioClip FindClip(string resourcesAudioRelativePath) {
        return Resources.Load<AudioClip>(string.Format("Audio/{0}", resourcesAudioRelativePath));
    }

}
