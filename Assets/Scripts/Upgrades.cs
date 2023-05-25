using UnityEngine;

public class Upgrades : MonoBehaviour {
    #region Variables
    private Transform mainTank;
    public float speed = 500;
    #endregion

    #region Properties
    #endregion
    
    #region Properties
    void Awake() {
        mainTank = GameObject.FindWithTag("Player").transform;
    }
    #endregion
    
    #region Builtin Methods
    void Update() {
        transform.position += Vector3.Normalize(mainTank.position - transform.position)*Time.deltaTime*speed;
    }
    #endregion

    #region Custom Methods
    #endregion
}

