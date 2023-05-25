using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PositionPlane : MonoBehaviour {
    #region Variables
    [Header("Distance from near")]
    public float distance = 0.01f;

    private Camera cam;
    #endregion

    #region Properties
    #endregion
    
    #region Builtin Methods
    void Start() {
        cam = Camera.main;
        float pos = cam.nearClipPlane + distance;
        transform.position = cam.transform.position + cam.transform.forward*pos;
        transform.LookAt(cam.transform);
        transform.Rotate(90f, 0f, 0f);
        float h = (Mathf.Tan(cam.fieldOfView*Mathf.Deg2Rad*0.5f)*pos*2f)/10; // (h/2)/pos = tan(fov/2) -> h = tan(fov/2) * pos * 2
        transform.localScale = new Vector3(h*cam.aspect, 1f, h);
    }

    void Update() {
        
    }
    #endregion

    #region Custom Methods
    #endregion
}
