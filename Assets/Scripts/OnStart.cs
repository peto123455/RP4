using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OnStart : MonoBehaviour
{
    [SerializeField] AudioMixer volumeMaster;

    void Start()
    {
        volumeMaster.SetFloat("volume", PlayerPrefs.GetFloat("volume", 0));
        Screen.fullScreen = IntToBool(PlayerPrefs.GetInt("fullscreen", 1));
    }
    private bool IntToBool(int integer)
    {
        return integer == 1;
    }
}
