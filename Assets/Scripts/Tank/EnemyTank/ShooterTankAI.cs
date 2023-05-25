using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Tank))]
public class ShooterTankAI : MonoBehaviour {
    #region Variables
    private Transform rotationCenter;
    private Transform cannonTransform;
    private Transform turretTransform;
    private GameObject bullet;
    private Tank tank;

    [Header("Bullets Properties")]
    public float angle = 120;
    public int numOfBullets = 4;
    public float damage = 35f;

    [Header("Firing time")]
    public float initialDelay = 0;
    public float rate = 3;
    public int shootingMode = 3;


    private Bullet bulletScript;
    private AudioSource audioSource;
    private AudioClip shootClip;
    #endregion

    #region Properties
    
    #endregion
    
    #region Builtin Methods
    void Awake() {
        bullet = Resources.Load("Prefab/bullet") as GameObject;
        bulletScript = bullet.GetComponent<Bullet>();
        tank = GetComponent<Tank>();
        audioSource = tank.AudioSourceGet;
        shootClip = tank.ShootClip;

        rotationCenter = transform.GetChild(0).GetChild(0).GetChild(1); // Rotation Center
        cannonTransform = rotationCenter.GetChild(0).GetComponent<Transform>();
        turretTransform = rotationCenter.GetChild(1).GetComponent<Transform>();

        initialDelay = 0; // Test
    }

    IEnumerator Start() {
        yield return new WaitForSeconds(initialDelay);
        bulletScript.Damage = damage;
        switch (shootingMode) {
            case 0:
                StartCoroutine(shootOneCycle());
                break;
            case 1:
                StartCoroutine(shootAngleCycle());
                break;
            case 2:
                StartCoroutine(shootAlternate1Cycle());
                break;
        }
        
    }
    #endregion

    #region Custom Methods
    void setVectorAtMiddleOfCannon() {
        Vector3 auxTurretPos = turretTransform.position;
        auxTurretPos.y = cannonTransform.position.y;
        bulletScript.Cannon = auxTurretPos;
        bulletScript.Crosshair = cannonTransform.position;
        bulletScript.Damage = damage;
    } 

    void shootOne() {
        setVectorAtMiddleOfCannon();
        audioSource.pitch = 1;
        audioSource.PlayOneShot(shootClip);
        GameObject bulletCp = Instantiate(bullet, cannonTransform.position, bullet.transform.rotation);
        bulletCp.name = "bulletE";
    }

    void shootAngle() {
        setVectorAtMiddleOfCannon();
        audioSource.pitch = 1;
        audioSource.PlayOneShot(shootClip);
        float deltaAngle = angle/numOfBullets;
        int num = -numOfBullets/2;
        for (; num < numOfBullets/2; num++){
            GameObject bulletCp = Instantiate(bullet, cannonTransform.position, bullet.transform.rotation);
            bulletCp.transform.rotation = Quaternion.Euler(0, deltaAngle*(num), 0) * bulletCp.transform.rotation;
            bulletCp.name = "bulletE";
        }
        if ( numOfBullets % 2 != 0 ) {
            GameObject bulletCp = Instantiate(bullet, cannonTransform.position, bullet.transform.rotation);
            bulletCp.transform.rotation = Quaternion.Euler(0, deltaAngle*(num), 0) * bulletCp.transform.rotation;
            bulletCp.name = "bulletE";
        }
        
    }
    #endregion

    #region Cycle methods
    public IEnumerator shootOneCycle(){
        while (true) {
            yield return new WaitForSeconds(rate);
            shootOne();
        }
    }

    public IEnumerator shootAngleCycle(){
        while (true) {
            yield return new WaitForSeconds(rate);
            shootAngle();
        }
    }

    public IEnumerator shootAngleAtATimeCycle(float angle, int numOfBullets) {
        while (true) {
            yield return new WaitForSeconds(rate);
        }
    }

    public IEnumerator shootAlternate1Cycle() {
        while (true) {
            yield return new WaitForSeconds(rate);
            shootOne();
            yield return new WaitForSeconds(rate);
            shootAngle();
        }
    }
    #endregion
}