using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;

public class DirectionSense {
    public static bool playerOriginIsSet = false;
    public static Vector2d playerOrigin;
    public static Vector2d currPlayerPos;
    public static readonly float scale = 1f;

    public static Vector3 realToUnity(Vector2d realPos) {
        return (realPos - playerOrigin).ToVector3xz() / scale;
    }
    public static Vector2d unityToReal(Vector3 unityPos) {
        return playerOrigin + (unityPos * scale).ToVector2d();
    }
}
