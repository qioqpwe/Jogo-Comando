using UnityEngine;

// Attached 
public class Bullet : MonoBehaviour {
    #region Variables
    [Header("Movement Properties")]
    public float speed = 10f;

    private Vector3 shootingVector;
    private int bulletRange = 600;
    [SerializeField]
    private float damage;

    [SerializeField]
    private Vector3 crosshair, cannon;
    #endregion

    #region Properties
    public Vector3 Cannon { set { cannon = value; } }
    public Vector3 Crosshair { set { crosshair = value; } }
    public float Damage {set {damage = value;} get {return damage;}}
    #endregion
    
    #region Builtin Methods
    void Awake() {
        crosshair.y = cannon.y;
        shootingVector = crosshair - cannon;        
        //Debug.DrawLine(cannon, crosshair, Color.black, 10f, false);
        transform.rotation = Quaternion.LookRotation(shootingVector);
        
        Transform localBulletAxis = this.transform.GetChild(0).GetComponent<Transform>();
        localBulletAxis.Rotate(-Vector3.Cross(shootingVector, transform.up), 90, Space.World);
        if (damage == 0) damage = 35f;
        //Debug.Log(damage);
        //transform.Rotate(-Vector3.Cross(shootingVector, transform.up), 90, Space.World);
        //Debug.DrawLine(cannon, cannon - Vector3.Cross(shootingVector.normalized, transform.up), Color.blue, 10f, false);
    }

    void Update() {
        transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
        
        if (Mathf.Abs(transform.position.z - cannon.z) > bulletRange || Mathf.Abs(transform.position.x - cannon.x) > bulletRange) 
            Destroy(this.gameObject);
    }
    #endregion

    #region Custom Methods
    #endregion
}