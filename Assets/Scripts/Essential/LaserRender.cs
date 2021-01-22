using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LaserRender
{
    private static bool isRendered = false;
    private static bool isOverride = false;
    public static Vector2 pos1, pos2;


    public static void SetStatus(bool status)
    {
        isRendered = status;
    }

    public static bool IsRendered()
    {
        return isRendered;
    }

    public static void SetPositions(Vector2 pos1, Vector2 pos2)
    {
        LaserRender.pos1 = pos1;
        LaserRender.pos2 = pos2;
    }

    public static void Reset()
    {
        LaserRender.pos1 = new Vector2(0f, 0f);
        LaserRender.pos2 = new Vector2(0f, 0f);
    }

    public static void Override(bool status)
    {
        LaserRender.isOverride = status;
    }

    public static bool IsOverride()
    {
        return LaserRender.isOverride;
    }

    public static void Restart()
    {
        Reset();
        isRendered = false;
    }
}
