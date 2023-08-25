using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MoveMap : MonoBehaviour
{
    private const float scale = 0.5f;
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        Camera.main.useOcclusionCulling = false;
    }

    void Update()
    {
        float scroll = Input.mouseScrollDelta.y * scale;//gets scroll wheel input
        if (cam.orthographicSize - scroll > 1 && cam.orthographicSize - scroll < 10)
        {
            cam.orthographicSize -= scroll; //zooms camera in+out with boundaries
        }

        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        cam.transform.Translate(cam.orthographicSize * Time.deltaTime * move);//allows wasd movement with speed scaled to fps and zoom
    }
}
