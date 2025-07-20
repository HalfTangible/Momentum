using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        float ppu = 100f; // Adjust to your sprite's PPU
        float pixelWidth = width * ppu;
        float pixelHeight = height * ppu;
        Debug.Log($"Camera Size: {pixelWidth}x{pixelHeight} pixels, Aspect: {cam.aspect}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
