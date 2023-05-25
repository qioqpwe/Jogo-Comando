using UnityEngine;

public class Tank : MonoBehaviour {
    #region Variables
    [Header("Tank Rotation Speed")]
    public float lerpSpeed = 5;

    private Transform turretTransform;
    private Vector3 finalTurretVector;
    private Transform playerTank;
    private EnemyTankSpawnManager enemySpawner;

    [SerializeField]
    private float life = 100;
    private int speed = 50;

    private bool lookAtTank = false;
    private bool dropShield = false;
    private GameObject shieldObj;
    private bool dropDamageBoost = false;
    private GameObject dmgBoostObj;
    private bool dropIncreaseRate = false;
    private GameObject incRateObj;

    private AudioSource audioSource;
    private AudioClip collisionClip1, collisionClip2, shootClip;
    #endregion

    #region Properties
    public int Life { set {life = value;} }
    public int Speed { get {return speed;} }
    public bool LookAtTank { set {lookAtTank = value;} }
    public Vector3 PlayerTankPosition { get {return playerTank.position;} }
    public AudioSource AudioSourceGet {get {return audioSource;}}
    public AudioClip ShootClip {get {return shootClip;}}

    public bool DropShield {set {dropShield = value;}}
    public bool DropDamageBoost {set {dropDamageBoost = value;}}
    public bool DropIncreaseRate {set {dropIncreaseRate = value;}}
    #endregion
    
    #region Builtin Methods
    void Awake() {
        playerTank = GameObject.FindWithTag("Player").transform;
        turretTransform = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Transform>();
        enemySpawner = GameObject.FindWithTag("EnemySpawner").GetComponent<EnemyTankSpawnManager>();
        shootClip = Resources.Load("SFX/CasualGameSounds/DM-CGS-48") as AudioClip;
        collisionClip1 = Resources.Load("SFX/Free Pack/Cannon impact 9") as AudioClip;
        collisionClip2 = Resources.Load("SFX/Free Pack/Explosion 1") as AudioClip;
        audioSource = GameObject.FindWithTag("Audio").GetComponent<AudioSource>();
        shieldObj = Resources.Load("Prefab/Shield") as GameObject;
        dmgBoostObj = Resources.Load("Prefab/DmgBoost") as GameObject;
        incRateObj = Resources.Load("Prefab/IncRate") as GameObject;
    }

    void OnTriggerEnter(Collider other) { // Tank collision with bullet method
        if (other.name == "bulletP"){
            if ( (life -= other.GetComponent<Bullet>().Damage) > 0) {
                audioSource.pitch = 2;
                audioSource.PlayOneShot(collisionClip1);
            }else {
                audioSource.pitch = 3;
                audioSource.PlayOneShot(collisionClip2);
                if (dropShield) {
                    GameObject shield = Instantiate(shieldObj, transform.position, shieldObj.transform.rotation);
                    shield.name = "Shield";
                }else if (dropDamageBoost) {
                    GameObject dmgBoost = Instantiate(dmgBoostObj, transform.position, dmgBoostObj.transform.rotation);
                    dmgBoost.name = "DmgBoost";
                }else if (dropIncreaseRate) {
                    GameObject incRate = Instantiate(incRateObj, transform.position, incRateObj.transform.rotation);
                    incRate.name = "IncRate";
                }
                enemySpawner.NumberOfEnemies--;
                Destroy(gameObject); // Destroy this object
            }
            Destroy(other.gameObject);
        }
    }

    void FixedUpdate() {
        if (lookAtTank) {
            Vector3 wantedTurretVector = playerTank.position - turretTransform.position;
            wantedTurretVector.y = 0f;
            finalTurretVector = Vector3.Lerp(finalTurretVector, wantedTurretVector, Time.deltaTime * lerpSpeed);
            turretTransform.rotation = Quaternion.LookRotation(finalTurretVector)*Quaternion.Euler(0, -90, 0);
        }
    }
    #endregion
}