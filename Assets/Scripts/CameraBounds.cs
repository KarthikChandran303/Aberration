using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraBounds {

    
    public static Vector3 TOPRIGHT = Vector3.zero;
    public static Vector3 BOTTOMLEFT = Vector3.zero;

    public static void updateBounds(float z)
    {
        TOPRIGHT = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, z));
        BOTTOMLEFT = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z));
    }
    
    public static void updateBoundsManual(float z, float width, float height)
    {
        TOPRIGHT = Camera.main.ScreenToWorldPoint(new Vector3(width, height, z));
        BOTTOMLEFT = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, z));
    }
}
