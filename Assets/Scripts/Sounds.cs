using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    List<AudioClip> clip;

    [SerializeField]
    private AudioClip
        soundHandgun,
        soundKnife,
        soundHandgunReload,
        soundRifle,
        soundPickup,
        shotgunReload,
        shotgunShoot,
        shotgunChamber;

    /*void Awake()
    {
        soundHandgun = (AudioClip) Resources.Load("Assets/Sounds/handgun.mp3");
        soundKnife = (AudioClip) Resources.Load("Assets/Sounds/stabSound.mp3");
        soundHandgunReload = (AudioClip) Resources.Load("Assets/Sounds/handgunReload.mp3");
        soundRifle = (AudioClip) Resources.Load("Assets/Sounds/rifleSound.mp3");
        soundPickup = (AudioClip) Resources.Load("Assets/Sounds/pickup.mp3");
    }*/

    public void PlaySound(int id, AudioSource sound)
    {
        /*if (id == 0) sound.PlayOneShot(soundHandgun);
        else if (id == 1) sound.PlayOneShot(soundKnife);
        else if (id == 2) sound.PlayOneShot(soundHandgunReload);
        else if (id == 3) sound.PlayOneShot(soundRifle);
        else if (id == 4) sound.PlayOneShot(soundPickup);
        else if (id == 5) sound.PlayOneShot(shotgunReload);
        else if (id == 6) sound.PlayOneShot(shotgunShoot);
        else if (id == 7) sound.PlayOneShot(shotgunChamber);*/

        sound.PlayOneShot(clip[id]);
    }

    void Awake()
    {
        clip = new List<AudioClip>
        {
            soundHandgun,
            soundKnife,
            soundHandgunReload,
            soundRifle,
            soundPickup,
            shotgunReload,
            shotgunShoot,
            shotgunChamber
        };
    }
}
