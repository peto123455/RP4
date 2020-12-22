using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    [SerializeField] AudioMixer masterVolume;
    [SerializeField] Dropdown resolutionDropdown;
    [SerializeField] Slider volumeSlider;
    [SerializeField] Toggle fullscreenToggle;

    Resolution[] resolutions;

    int currentResolution = 0;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();    

        List<string> list = new List<string>();
        for(int i = 0; i < resolutions.Length; ++i)
        {
            list.Add(resolutions[i].height + "x" + resolutions[i].width + " " + resolutions[i].refreshRate + "Hz");

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height && resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolution = i;
            }
        }

        resolutionDropdown.AddOptions(list);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();

        RefreshSettings();
    }

    public void RemoveKey()
    {
        PlayerPrefs.DeleteKey("key");
        SceneManager.LoadScene(0);
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullscreenToggle.isOn);
    }

    public void ApplySettings()
    {
        /* Aplikuje nastavenia */
        masterVolume.SetFloat("volume", volumeSlider.value);
        SetResolution(resolutionDropdown.value);

        /* Uloží nastavenia */
        PlayerPrefs.SetInt("fullscreen", BoolToInt(fullscreenToggle.isOn));
        PlayerPrefs.SetFloat("volume", volumeSlider.value);

        RefreshSettings(); //Len tak pre istotu
    }

    public void RefreshSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 0);
        fullscreenToggle.isOn = IntToBool(PlayerPrefs.GetInt("fullscreen", 1));
    }

    public void Resolution()
    {

    }

    private bool IntToBool(int integer)
    {
        return integer == 1;
    }

    private int BoolToInt(bool boolean)
    {
        return boolean ? 1:0;
    }
}
