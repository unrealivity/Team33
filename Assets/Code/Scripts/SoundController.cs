using UnityEngine;
using System;
using System.Collections.Generic;


public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    [Serializable]
    public class SoundData {
        public Sounds type;
        public AudioClip clip;

        [Range(0f, 1f)] public float volume = 1f;
        public bool preload = true;
    }
    
    [SerializeField] private List<SoundData> sounds;
    [SerializeField] private AudioSource boilingSource;
    [SerializeField] private AudioSource cuttingSource;
    [SerializeField] private AudioSource musicSource;

    private Dictionary<Sounds, SoundData> _soundDictionary;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _soundDictionary = new Dictionary<Sounds, SoundData>();
        foreach (SoundData sound in sounds) {
            if (sound.clip == null) {
                continue;
            }

            if (!_soundDictionary.ContainsKey(sound.type)) {
                _soundDictionary.Add(sound.type, sound);
            }

            if (sound.preload) {
                sound.clip.LoadAudioData();
            }
        }
    }

    public void PlaySound(Sounds type, AudioSource source) {
        if (!_soundDictionary.TryGetValue(type, out SoundData sound)) {
            Debug.LogWarning("Sound not found" + type);
            return;
        }
        source.PlayOneShot(sound.clip, sound.volume);
    }

    
    
    public void PlayCooking() {
        PlaySound(Sounds.CookingDone, boilingSource);
    }
    public void PlayCutting() {
        PlaySound(Sounds.Cutting, cuttingSource);
    }


    public void StartCookingSound() {
        if (!_soundDictionary.TryGetValue(Sounds.Cooking, out SoundData sound)) {
            Debug.LogWarning("Sound not found: Cooking");
            return;
        }

        boilingSource.clip = sound.clip;
        boilingSource.volume = sound.volume;
        boilingSource.loop = true;
        boilingSource.Play();
    }

    public void StopCookingSound() {
        boilingSource.Stop();
        boilingSource.loop = false;
        boilingSource.clip = null;
    }

    public void StartBackgroundMusic() {
        if (!_soundDictionary.TryGetValue(Sounds.Cooking, out SoundData sound)) {
            Debug.LogWarning("Sound not found: BackgroundMusic");
            return;
        }
        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume;
        musicSource.loop = true;
        musicSource.Play();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
