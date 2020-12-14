using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds
{
    List<AudioClip> clip;

    //[SerializeField]
    private AudioClip
        soundHandgun,
        soundKnife,
        soundHandgunReload,
        soundRifle,
        soundPickup,
        shotgunReload,
        shotgunShoot,
        shotgunChamber,
        knifeDraw,
        gunDraw;

    public Sounds()
    {
        /*soundHandgun =          Resources.Load<AudioClip>("Sounds/handgun");
        soundKnife =            Resources.Load<AudioClip>("Sounds/stabSound");
        soundHandgunReload =    Resources.Load<AudioClip>("Sounds/handgunReload");
        soundRifle =            Resources.Load<AudioClip>("Sounds/rifleSound");
        soundPickup =           Resources.Load<AudioClip>("Sounds/pickup");
        shotgunReload =         Resources.Load<AudioClip>("Sounds/shotgun_oneAmmo");
        shotgunShoot =          Resources.Load<AudioClip>("Sounds/shotgun_shot");
        shotgunChamber =        Resources.Load<AudioClip>("Sounds/shotgun_chamber");
        knifeDraw =             Resources.Load<AudioClip>("Sounds/knife_draw");
        gunDraw =               Resources.Load<AudioClip>("Sounds/gun_draw");*/

        clip = new List<AudioClip>
        {
            Resources.Load<AudioClip>("Sounds/handgun"),            //0
            Resources.Load<AudioClip>("Sounds/stabSound"),          //1
            Resources.Load<AudioClip>("Sounds/handgunReload"),      //2
            Resources.Load<AudioClip>("Sounds/rifleSound"),         //3
            Resources.Load<AudioClip>("Sounds/pickup"),             //4
            Resources.Load<AudioClip>("Sounds/shotgun_oneAmmo"),    //5
            Resources.Load<AudioClip>("Sounds/shotgun_shot"),       //6
            Resources.Load<AudioClip>("Sounds/shotgun_chamber"),    //7
            Resources.Load<AudioClip>("Sounds/knife_draw"),         //8
            Resources.Load<AudioClip>("Sounds/gun_draw")            //9
        };
    }

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
}
