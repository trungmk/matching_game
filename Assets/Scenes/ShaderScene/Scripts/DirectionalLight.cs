using UnityEngine;

public class DirectionalLight : MonoBehaviour
{
    public Color LightColor;
    public Color ShadowColor;

    [Range(0f, 360f)]
    public float Angle;
   

}