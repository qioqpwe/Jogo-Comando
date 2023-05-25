using UnityEngine;
using PathCreation;

[RequireComponent(typeof(Tank))]
[RequireComponent(typeof(Rigidbody))]
public class PathFollowerAI : MonoBehaviour {
    #region Variables
    private PathCreator pathCreator;
    public float speed = 50f;
    public bool lookAtTank = true;
    private float distanceTravelled;
    #endregion

    #region Properties
    public void startPathFollowerAI(PathCreator _pathCreator) {
        pathCreator = _pathCreator;
    }
    #endregion

    #region Builtin Methods

    void Update() {
        distanceTravelled += speed*Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled)*Quaternion.Euler(0, 0, 90);
        //this.transform.Rotate(Vector3.up, pathCreator.path.GetRotationAtDistance(distanceTravelled).eulerAngles.y);
    }
    #endregion

    #region Custom Methods
    #endregion
}