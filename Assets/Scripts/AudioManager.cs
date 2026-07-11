using UnityEngine;

/// <summary>
/// Persistent audio hub. Place ONE configured AudioManager object in the first
/// scene (MainMenu); it survives scene loads via DontDestroyOnLoad, so later
/// scenes reuse the same instance. Assign the clips in the Inspector.
///
/// All calls are null-safe: if no AudioManager exists (or a clip isn't
/// assigned) nothing plays and nothing errors, so scenes still run.
///   AudioManager.instance?.PlayPickup();
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources (auto-created if left empty)")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip pickupClip;
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip buttonClip;
    public AudioClip countdownTickClip;

    void Awake()
    {
        // Enforce a single persistent instance.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureSources();
        ApplyVolumes();
    }

    void EnsureSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
    }

    /// <summary>Push saved volume settings onto the live sources.</summary>
    public void ApplyVolumes()
    {
        if (musicSource != null) musicSource.volume = SaveManager.MusicVolume;
        if (sfxSource != null) sfxSource.volume = SaveManager.SfxVolume;
    }

    // ---- SFX ----
    public void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip); // scaled by sfxSource.volume set in ApplyVolumes
    }

    public void PlayPickup() => PlaySfx(pickupClip);
    public void PlayWin() => PlaySfx(winClip);
    public void PlayLose() => PlaySfx(loseClip);
    public void PlayButton() => PlaySfx(buttonClip);
    public void PlayCountdownTick() => PlaySfx(countdownTickClip);

    // ---- Music ----
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return; // already playing it
        musicSource.clip = clip;
        musicSource.volume = SaveManager.MusicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }
}
