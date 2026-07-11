using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives Music / SFX volume sliders. Reads current values from SaveManager on
/// open, writes changes back (persisted), and applies them live to the running
/// AudioManager. Hook the sliders in the Inspector — no OnValueChanged wiring
/// needed in the editor, it's done here in Start().
/// </summary>
public class OptionsMenu : MonoBehaviour
{
    public Slider musicSlider; // expects range 0..1
    public Slider sfxSlider;   // expects range 0..1

    void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.value = SaveManager.MusicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = SaveManager.SfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }
    }

    public void OnMusicChanged(float value)
    {
        SaveManager.MusicVolume = value;
        AudioManager.instance?.ApplyVolumes();
    }

    public void OnSfxChanged(float value)
    {
        SaveManager.SfxVolume = value;
        AudioManager.instance?.ApplyVolumes();
        AudioManager.instance?.PlayButton(); // quick preview at the new level
    }
}
