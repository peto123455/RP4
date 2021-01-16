using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lr;
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void LateUpdate()
    {
        if(LaserRender.IsRendered() && !LaserRender.IsOverride() && Player.player.wl.selected != Player.player.wl.weapons[0])
        {
            lr.positionCount = 2;
            lr.SetPosition(0, LaserRender.pos1);
            lr.SetPosition(1, LaserRender.pos2);
        }
        else lr.positionCount = 0;
    }
}
