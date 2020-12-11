using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctions //Trieda v ktorej mám funkcie na rôzne prevody jednotiek
{
    public static bool IntToBool(int integer)
    {
        return integer == 1;
    }

    public static int BoolToInt(bool boolean)
    {
        return boolean ? 1:0;
    }

    public static Vector3 AngleToVector(float angle) //Funkcia, ktorá prepočíta Uhol na Vektor
    {
        float angleRad = angle * (Mathf.PI / 180f); //Prevenie stupne na radiany
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)); 
    }

    public static float VectorToAngle(Vector3 direction)
    {
        direction.Normalize();
        float n = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public static float SimplifyAngle(float angle)
    {
        while(angle < 0 || angle > 360)
        {
            if(angle < 0) angle += 360;
            else angle -= 360;
        }

        return angle;
    }
}
