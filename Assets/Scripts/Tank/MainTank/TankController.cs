using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TankInput))]
public class TankController : MonoBehaviour {
    #region Variables
    [Header("Movement Properties")]
    public float forwardSpeed = 10f;
    public float rotationSpeed = 80f;
    public float lerpSpeed = 5;
    public float damage = 15f;
    private float health = 100, shield = 0;

    [Header("Data from Objects")]
    public Transform turretTransform;
    public Transform crosshairTransform;
    public Transform cannonTransform;
    public GameObject bullet;
    public Slider healthSlider;
    public Slider shieldSlider;
    public Image rage;
    public GameObject deathMenuUI;
    public TMP_Text tankLvl;
    // public Gradient healthGradient;
    // public Image fill;
    

    [SerializeField]
    private float shootDelay = 0.4f;

    private Vector3 finalTurretVector;
    private Rigidbody rb;
    private TankInput input;
    private AudioSource audioSource;
    private AudioClip shootClip,  collisionClip1, collisionClip2;
    private bool canFire;
    public static bool isDead = false; // There's only one maintank
    #endregion

    #region Properties
    #endregion

    #region Builtin Methods
    void Awake() {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<TankInput>();
        audioSource = GetComponent<AudioSource>();
        canFire = true;
        shootClip = Resources.Load("SFX/CasualGameSounds/DM-CGS-49") as AudioClip;
        collisionClip1 = Resources.Load("SFX/Grenade Sound FX/Grenade2Short") as AudioClip;
        collisionClip2 = Resources.Load("SFX/Free Pack/Explosion 8") as AudioClip;
        audioSource = GameObject.FindWithTag("Audio").GetComponent<AudioSource>();
        healthSlider.maxValue = health;
        healthSlider.value = health;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        // Transform[] tankChildrens = this.GetComponentsInChildren<Transform>();
        // foreach(Transform tr in tankChildrens){}
    }

    void Update() {
        // Input.GetButtonDown("Fire1")) Returns true only for the frame at wich we pressed the button (does not allow holding)
        // Input.GetButton("Fire1")}") returns true if the button is pressed (allows holding)
        if (Input.GetButton("Fire1") && canFire && !PauseMenu.gamePaused) { // updates keep running while Time.timeScale = 0
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.Cannon = cannonTransform.position;
            bulletScript.Crosshair = input.CrosshairPosition;
            bulletScript.Damage = damage;
            StartCoroutine(shoot());
        }
        healthSlider.value = Mathf.Lerp(healthSlider.value, health, lerpSpeed*Time.deltaTime);
        shieldSlider.value = Mathf.Lerp(shieldSlider.value, shield, lerpSpeed*Time.deltaTime);
    }

    void FixedUpdate() { // For dealing with RigidBody update each 0.02 sec
        if (rb && input) { 
            handleMovement();
            handleTurret(); 
            handleCrosshair();
        }
    }

    void OnTriggerEnter(Collider other) {
        switch (other.name) {
            case "bulletE":
                if (shield > 0) {
                    shield -= other.GetComponent<Bullet>().Damage;
                    audioSource.pitch = 2;
                    audioSource.PlayOneShot(collisionClip1);
                }else if ( health > 0) {
                    health -= other.GetComponent<Bullet>().Damage;
                    if (health <= 0) {
                        audioSource.pitch = 3;
                        audioSource.PlayOneShot(collisionClip2);
                        StartCoroutine(gameOver());
                    } else {
                        audioSource.pitch = 2;
                        audioSource.PlayOneShot(collisionClip1);
                    }
                }
                //Debug.Log(other.GetComponent<Bullet>().Damage);
                Destroy(other.gameObject);
                break;
            case "Shield":
                shield += 35;
                shield = Mathf.Clamp(shield, 0, 100);
                Destroy(other.gameObject);
                break;
            case "DmgBoost":
                damage += 10;
                damage = Mathf.Clamp(damage, 0, 100);
                tankLvl.text = $"Tank Power: {damage}";
                Destroy(other.gameObject);
                break;
            case "IncRate":
                shootDelay = 0.2f;
                StartCoroutine(rageTimer(8f));
                Destroy(other.gameObject);
                break;
        }
    }
    #endregion

    #region Custom Methods
    protected virtual void handleMovement() {
        Vector3 wantedPosition = transform.position + (transform.forward * input.ForwardInput * forwardSpeed * Time.deltaTime);
        if(Mathf.Abs(wantedPosition.x) < 335 && Mathf.Abs(wantedPosition.z) < 180)
            rb.MovePosition(wantedPosition);

        Quaternion wantedRotation = transform.rotation * Quaternion.Euler(Vector3.up * (input.RotationInput * rotationSpeed * Time.deltaTime));
        rb.MoveRotation(wantedRotation);
    }

    protected virtual void handleCrosshair() {
        crosshairTransform.position = input.CrosshairPosition;
    }

    protected virtual void handleTurret() {
        Vector3 wantedTurretVector = input.CrosshairPosition - turretTransform.position;
        wantedTurretVector.y = 0f;

        finalTurretVector = Vector3.Lerp(finalTurretVector, wantedTurretVector, Time.deltaTime * lerpSpeed);
        turretTransform.rotation = Quaternion.LookRotation(finalTurretVector)*Quaternion.Euler(0, -90, 0);
    }

    IEnumerator shoot(){
        canFire = false;
        audioSource.pitch = 1;
        audioSource.PlayOneShot(shootClip);
        GameObject bulletCp = Instantiate(bullet, cannonTransform.position, bullet.transform.rotation);
        bulletCp.name = "bulletP";
        yield return new WaitForSeconds(shootDelay);
        canFire = true;
    }

    IEnumerator gameOver() {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
        isDead = true;
        deathMenuUI.SetActive(true);
        Cursor.visible = true;
        PauseMenu.gamePaused = true;
        Time.timeScale = 0f;
    }

    IEnumerator rageTimer(float duration) {
        rage.enabled = true;
        float timeElapsed = 0;
        while (timeElapsed < duration) {
            float t = timeElapsed/duration;
            rage.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        rage.enabled = false;
        shootDelay = 0.4f;
    }
    #endregion
}
