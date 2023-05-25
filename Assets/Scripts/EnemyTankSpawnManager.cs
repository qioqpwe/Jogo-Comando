using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using PathCreation;

static class Const {
    public static Vector3 TOP = new Vector3(287, -62, 135);
    public static Vector3 MID = new Vector3(287, -62, 0);
    public static Vector3 BOTTOM = new Vector3(287, -62, -135);
    public static Vector3 CENTER = new Vector3(0, -62, 0);
    public static Vector3 tCol = new Vector3(-100, 0, 0);
    public static Vector3 tRow = new Vector3(0, 0, -50);
}

public class Enemie {
    // Characteristics
    public int tIdx {get; set;} // tankIndex index from tank models list
    public int AImd {get; set;} // AImodel static, follower, PathFollower, Boss
    public Vector3 srtPos {get; set;} // startPòsition
    public int pIdx {get; set;} // pathIndex wich path in paths list
    public bool isSht {get; set;} // isShooter enemy tank will be able to shoot
    public float dmg {get; set;} // amount of damge caused per bullet of enemy tank
    public int shtMd {get; set;} // shootingMode one bullet, at angle, alternate, ...
    public int ang {get; set;} // if shootingMode as angle set wich angle to use
    public int bullNum {get; set;} // number of bullets to shoot in angle mode
    public float rate {get; set;} // rate to shoot
    public float intDly {get; set;} // initialDelay time in seconds before first shot
    public bool lkAtTank {get; set;} // lookAtTank will it look at player tank
    public int life {get; set;} // Amount of life
    public float initRot {get; set;} // initialRotation defines the initial rotation for the tank body

    // Dropable items
    public bool dpPShld {get; set;} // dropPlayerShield increase player shield
    public bool dpPDmgBst {get; set;} // dropPlayerDamageBoost increase player bullet damage
    public bool dpPIncRate {get; set;} // dropPlayerIncreaseRate increase player bullet rate
}


public class EnemyTankSpawnManager : MonoBehaviour {
    #region Variables
    private int[] qntEnemiesPerPhase;
    private Enemie[] arrEnm;
    private int jmpIdx = 0;

    [SerializeField]
    [Header("Models of Enemy Tanks Objects")]
    private GameObject[] enemyTanks;

    [SerializeField]
    [Header("Paths for PathFollowingAI`s")]
    private PathCreator[] paths;

    [Header("Canvas Objects")]
    public Image uiImage;
    public TMP_Text phaseText;
    public TMP_Text victoryText;

    private int phase = 1, numPhases = 10;
    private int numberOfEnemies = 0;
    private bool changingPhase = false;
    #endregion

    #region Properties
    public int NumberOfEnemies {
        set {numberOfEnemies = value;}
        get {return numberOfEnemies;}
    }
    #endregion
    
    #region Builtin Methods
    void Awake() {
        if (SceneManager.GetActiveScene().buildIndex == 1) {
            initializeEnemiesLvl1();
        } else {
            initializeEnemiesLvl1();
        }
    }


    void Start() { // Spawn tank method
        changingPhase = true;
        StartCoroutine(changePhase(0, 3, (alphaDone) => {
            if (alphaDone) {
                spawnEnemies();
                changingPhase = false;
            }
        }));
    }

    void FixedUpdate() {
        if (phase < numPhases) {
            if (numberOfEnemies == 0 && !changingPhase) {
                changingPhase = true;
                phase++;
                GameObject[] arr = GameObject.FindGameObjectsWithTag("Path");
                for(int i = 0; i < arr.Length; i++) Destroy(arr[i]);

                StartCoroutine(changePhase(3, 3, (alphaDone) => {
                    if (alphaDone) {
                        GameObject.FindWithTag("Player").transform.position = new Vector3(-283, -68, 0);
                        spawnEnemies();
                        changingPhase = false;
                    }
                }));
            }
        }else {
            if (numberOfEnemies == 0 && !changingPhase) StartCoroutine(changeLevel());
        }
    }
    #endregion

    #region Custom Methods
    void spawnEnemies() {
        for (int count = 0; count < qntEnemiesPerPhase[(phase-1)]; count++) {
            Enemie enObj = arrEnm[count+jmpIdx];
            switch(enObj.AImd) {
                case 0: // Static AI
                    GameObject AI0 = Instantiate(enemyTanks[enObj.tIdx], 
                                    enObj.srtPos,
                                    enemyTanks[enObj.tIdx].transform.rotation) as GameObject;
                    configEnemie(enObj, AI0);
                    break;
                case 1: // Follower AI
                    GameObject AI1 = Instantiate(enemyTanks[enObj.tIdx], 
                                    enObj.srtPos,
                                    enemyTanks[enObj.tIdx].transform.rotation) as GameObject;
                    configEnemie(enObj, AI1);
                    AI1.AddComponent<FollowerAI>().startFollowerAI();
                    break;
                case 2: // Path Follower AI
                    GameObject AI2 = Instantiate(enemyTanks[enObj.tIdx], 
                                    paths[enObj.pIdx].transform.position,
                                    enemyTanks[enObj.tIdx].transform.rotation) as GameObject;
                    configEnemie(enObj, AI2);
                    PathCreator path = Instantiate(paths[enObj.pIdx], enObj.srtPos, paths[enObj.pIdx].transform.rotation) as PathCreator;
                    AI2.AddComponent<PathFollowerAI>().startPathFollowerAI(path);
                    break;
            };
        } 
        jmpIdx += qntEnemiesPerPhase[(phase-1)];
    }

    void configEnemie(Enemie enObj, GameObject AI) {
        if (enObj.isSht){
            ShooterTankAI shootComp = AI.AddComponent<ShooterTankAI>();
            shootComp.angle = enObj.ang;
            shootComp.rate = enObj.rate;
            shootComp.shootingMode = enObj.shtMd;
            shootComp.numOfBullets = enObj.bullNum;
            shootComp.initialDelay = enObj.intDly;
            shootComp.damage = enObj.dmg;
        }
        //Debug.Log($"{enObj} {enObj.lkAtTank}");
        AI.GetComponent<Tank>().LookAtTank = enObj.lkAtTank;
        AI.GetComponent<Tank>().DropShield = enObj.dpPShld;
        AI.GetComponent<Tank>().DropDamageBoost = enObj.dpPDmgBst;
        AI.GetComponent<Tank>().DropIncreaseRate = enObj.dpPIncRate;
        if (enObj.life != 0) AI.GetComponent<Tank>().Life = enObj.life;
        NumberOfEnemies++;
    }

    IEnumerator changePhase(float inDuration, float outDuration, Action<bool> action) {
        phaseText.text = $"Wave {phase}/{numPhases}";

        float timeElapsed = 0;
        while (timeElapsed < inDuration) {
            float t = timeElapsed/inDuration;
            uiImage.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, t));
            phaseText.alpha = Mathf.Lerp(0, 1, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        action(true);

        timeElapsed = 0;
        while (timeElapsed < outDuration) {
            float t = timeElapsed/outDuration;
            uiImage.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, t));
            phaseText.alpha = Mathf.Lerp(1, 0, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator changeLevel(){
        victoryText.enabled = true;
        yield return new WaitForSeconds(5f);
        if (SceneManager.GetActiveScene().buildIndex < 2) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }else{
            Debug.Log("End Of Game!"); // Load Credits Scene
        }
    }

    // new Enemie {tIdx = , AImd = , strtPos = , pIdx = , isSht = , dmg = , shtMd = , ang = , bullNum = , rate = , intDly = , lkAtTank = , life = , dpPShld = , dpPDmgBst = , dpIncPRate =0 };
    // shtMd 0 (shootOne), 1 (shootAngle), 2 (alternate between 0 and 1), 3 (rotate turret and shoot)
    // AImd 0 (static), 1 (player follower), 2 (path follower)
    // false is default and 0 is default
    void initializeEnemiesLvl1() {
        qntEnemiesPerPhase = new int[] {1, 2, 3, 4, 6, 3, 9, 2, 3, 1};
        int sum = 0;
        foreach(int item in qntEnemiesPerPhase) {
            sum+= item;
        }
        arrEnm = new Enemie[sum];
        int i = 0;
        // phase1 difficulty 1/10
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 1, srtPos = Const.MID, isSht = true, shtMd = 2, ang = 120, bullNum = 6, rate = 3, intDly = 1, lkAtTank = true};
        // phase2 difficulty 3/10 
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.TOP, pIdx = 2, isSht = true, shtMd = 0, rate = 3, intDly = 2, lkAtTank = true, dpPShld = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.BOTTOM, pIdx = 1, isSht = true, shtMd = 0, rate = 3f, intDly = 0, lkAtTank = true};
        // phase3 difficulty 5/10
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.TOP, pIdx = 0, isSht = true, shtMd = 0, rate = 4, intDly = 2, lkAtTank = true, dpPShld = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID, isSht = true, shtMd = 0, rate = 3, intDly = 1, lkAtTank = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.BOTTOM+Const.tCol, pIdx = 4, isSht = true, shtMd = 0, rate = 3f, intDly = 0, lkAtTank = true};
        // phase4 difficulty 5/10
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID, isSht = true, shtMd = 1, rate = 6, intDly = 5, ang = 120, bullNum = 5, lkAtTank = true, dpPDmgBst = true, life = 200};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID+Const.tCol, isSht = true, shtMd = 0, rate = 5, intDly = 3, ang = 120, bullNum = 5};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.TOP, pIdx = 1, isSht = true, shtMd = 0, rate = 5, intDly = 2, lkAtTank = true, dpPShld = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.BOTTOM, pIdx = 1, isSht = true, shtMd = 0, rate = 3, intDly = 2, lkAtTank = true, life = 150};
        // phase5 difficulty 5/10 PlayerDmg = 25
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.TOP, isSht = true, shtMd = 1, ang = 120, bullNum = 5, rate = 5f, intDly = 2, lkAtTank = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID, isSht = true, shtMd = 0, rate = 3f, intDly = 1, dpPShld = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.BOTTOM, isSht = true, shtMd = 0, rate = 3.5f, intDly = 1.5f, lkAtTank = true, dpPDmgBst = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.TOP+Const.tCol, isSht = true, shtMd = 0, rate = 5f, intDly = 2, lkAtTank = true, dpPIncRate = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID+Const.tCol, isSht = true, shtMd = 1, ang = 30, bullNum = 3, rate = 6f, intDly = 3};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.BOTTOM+Const.tCol, isSht = true, shtMd = 0, rate = 5f, intDly = 1, lkAtTank = true};
        // phase6 difficulty 3/10 PlayerDmg = 35
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 1, srtPos = Const.TOP, isSht = true, shtMd = 0, rate = 3, intDly = 2, lkAtTank = true, life = 150};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID, isSht = true, shtMd = 2, rate = 5, intDly = 5, lkAtTank = true, life = 150};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 1, srtPos = Const.BOTTOM, isSht = true, shtMd = 0, rate = 1.5f, intDly = 4, lkAtTank = true, life = 150};
        // phase7 difficulty 7/10
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID, isSht = true, shtMd = 1, ang = 120, bullNum = 5, rate = 8f, intDly = 8, lkAtTank = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.TOP, isSht = true, shtMd = 0, rate = 6f, intDly = 6, dpPShld = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.BOTTOM, isSht = true, shtMd = 0, rate = 6.5f, intDly = 1.5f, lkAtTank = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.TOP+Const.tCol, lkAtTank = true, dpPIncRate = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID+Const.tCol, lkAtTank = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.BOTTOM+Const.tCol, isSht = true, shtMd = 0, rate = 5f, intDly = 1};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.TOP+2*Const.tCol, isSht = true, shtMd = 0, rate = 10, intDly = 4, lkAtTank = true, dpPIncRate = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.MID+2*Const.tCol, isSht = true, shtMd = 2, ang = 60, bullNum = 3, rate = 6f, intDly = 3};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 0, srtPos = Const.BOTTOM+2*Const.tCol, isSht = true, shtMd = 0, rate = 5f, intDly = 1, lkAtTank = true};        
        // phase8 difficulty 5/10
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.CENTER, pIdx = 3, isSht = true, shtMd = 0, rate = 3, intDly = 2, lkAtTank = true, dpPDmgBst = true, life = 300};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.MID+Const.tCol, pIdx = 4, isSht = true, shtMd = 0, rate = 1.5f, intDly = 0, lkAtTank = true, life = 300};
        // phase9 difficulty 3/10 PlayerDmg = 45
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.BOTTOM, pIdx = 1, isSht = true, shtMd = 0, rate = 3, intDly = 2, lkAtTank = true, life = 200};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 1, srtPos = Const.MID, isSht = true, shtMd = 2, ang = 120, bullNum = 5, rate = 3, intDly = 1, lkAtTank = true};
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 2, srtPos = Const.TOP, pIdx = 1, isSht = true, shtMd = 0, rate = 3, intDly = 2, lkAtTank = true, dpPShld = true};
        // phase10 difficulty 10/10 boss 
        arrEnm[i++] = new Enemie {tIdx=0, AImd = 1, srtPos = Const.MID, isSht = true, shtMd = 2, ang = 120, bullNum = 5, rate = 3, intDly = 6, lkAtTank = true, life = 1000};
    }

    void initializeEnemiesLvl2() {

    }
    #endregion
}