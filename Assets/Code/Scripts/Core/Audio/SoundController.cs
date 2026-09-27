using UnityEngine;
using System;
using System.Collections.Generic;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    [Serializable]
    public class SoundData
    {
        public Sounds type;
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volume = 1f;
        public bool preload = true;
        [Header("Pitch Variation")]
        public float minPitch = 1f;
        public float maxPitch = 1f;
    }

    // ---------------------------------------------------------------------
    // Inspector references
    // ---------------------------------------------------------------------
    [SerializeField] private List<SoundData> sounds;
    [SerializeField] private AudioSource boilingSource;
    [SerializeField] private AudioSource cuttingSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource luzzSource;
    [SerializeField] private AudioSource bellSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource chestSource;
    

    private Dictionary<Sounds, SoundData> _soundDictionary;

    // ---------------------------------------------------------------------
    // Unity lifecycle
    // ---------------------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        BuildSoundDictionary();
    }

    private void Start()
    {
        StartLoop(Sounds.BackgroundMusic, musicSource);
    }

    private void OnEnable()
    {
        Cooking.CookingStartedEvent += StartCookingSound;
        Cooking.CookingStoppedEvent += StopCookingSound;
        Cooking.CookingDoneEvent += CookingFinished;
        Cutting.CuttingEvent += PlayCutting;
        LuzzBehaviorController.MunchFood += PlayMunchFood;
        ServingCounter.RingBell += PlayBellRing;
        OrderBacklog.Instance.OrderFulfilled += PlayOrderSuccess;
        OrderBacklog.Instance.OrderFailed += PlayOrderFailed;
        IngredientSpawner.IngredientSpawned += PlayIngredientSpawnSound;
    }
    


    private void OnDisable()
    {
        Cooking.CookingStartedEvent -= StartCookingSound;
        Cooking.CookingStoppedEvent -= StopCookingSound;
        Cooking.CookingDoneEvent -= CookingFinished;
        Cutting.CuttingEvent -= PlayCutting;
        LuzzBehaviorController.MunchFood -= PlayMunchFood;
        ServingCounter.RingBell -= PlayBellRing;
        OrderBacklog.Instance.OrderFulfilled -= PlayOrderSuccess;
        OrderBacklog.Instance.OrderFailed -= PlayOrderFailed;
        IngredientSpawner.IngredientSpawned -= PlayIngredientSpawnSound;
    }

    // ---------------------------------------------------------------------
    // Setup
    // ---------------------------------------------------------------------
    private void BuildSoundDictionary()
    {
        _soundDictionary = new Dictionary<Sounds, SoundData>();
        foreach (SoundData sound in sounds)
        {
            if (sound.clips == null || sound.clips.Length == 0) continue;
            _soundDictionary[sound.type] = sound;

            if (sound.preload)
                foreach (var clip in sound.clips)
                    clip.LoadAudioData();
        }
    }

    // ---------------------------------------------------------------------
    // Core playback
    // ---------------------------------------------------------------------
    public void PlaySound(Sounds type, AudioSource source)
    {
        if (!TryGetSound(type, out SoundData sound)) return;

        source.pitch = UnityEngine.Random.Range(sound.minPitch, sound.maxPitch);
        source.PlayOneShot(RandomClip(sound), sound.volume);
    }

    public void StartLoop(Sounds type, AudioSource source)
    {
        if (!TryGetSound(type, out SoundData sound)) return;

        source.clip = RandomClip(sound);
        source.volume = sound.volume;
        source.loop = true;
        source.Play();
    }

    public void StopLoop(AudioSource source)
    {
        source.Stop();
        source.loop = false;
        source.clip = null;
    }

    private bool TryGetSound(Sounds type, out SoundData sound)
    {
        if (_soundDictionary.TryGetValue(type, out sound)) return true;
        Debug.LogWarning($"Sound not found: {type}");
        return false;
    }

    private static AudioClip RandomClip(SoundData sound) =>
        sound.clips[UnityEngine.Random.Range(0, sound.clips.Length)];

    // ---------------------------------------------------------------------
    // Game-specific event handlers
    // ---------------------------------------------------------------------
    private void StartCookingSound() => StartLoop(Sounds.Cooking, boilingSource);
    private void StopCookingSound() => StopLoop(boilingSource);
    private void CookingFinished()
    {
        StopCookingSound();
        PlaySound(Sounds.CookingDone, boilingSource);
    }
    private void PlayMunchFood() => PlaySound(Sounds.MunchFood, luzzSource);
    private void PlayBellRing() => PlaySound(Sounds.RingBell,bellSource);
    private void PlayCutting() => PlaySound(Sounds.Cutting, cuttingSource);
    private void PlayOrderFailed(ItemType _obj) => PlaySound(Sounds.DeliverFail,uiSource);

    private void PlayOrderSuccess(ItemType _obj) => PlaySound(Sounds.DeliverSuccess,uiSource);
    private void PlayIngredientSpawnSound() => PlaySound(Sounds.IngredientSpawned, chestSource);
}