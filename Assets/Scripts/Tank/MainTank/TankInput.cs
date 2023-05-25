using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankInput : MonoBehaviour {
    #region Variables
    [Header("Camera Object")]
    public Camera viewCamera;
    
    private Vector3 crosshairPosition, crosshairNormal;
    private float forwardInput, rotationInput;
    #endregion

    #region Properties
    public Vector3 CrosshairPosition { get {return crosshairPosition;} }
    public float ForwardInput { get {return forwardInput;} }
    public float RotationInput { get {return rotationInput;} }
    #endregion

    #region Builtin Methods
    void Update() {
        if (viewCamera) {
            handleInputs();
        }
    }
    #endregion

    #region Custom Methods
    protected virtual void handleInputs() {
        Ray screenRay = viewCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(screenRay, out hit)) {
            crosshairPosition = hit.point;
        }
        forwardInput = Input.GetAxis("Vertical");
        rotationInput = Input.GetAxis("Horizontal");
    }
    #endregion
}
