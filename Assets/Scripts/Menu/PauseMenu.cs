using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    #region Variables
    public static bool gamePaused = false;
    public GameObject pauseMenuUI;
    #endregion

    #region Properties
    #endregion
    
    #region Builtin Methods
    void Update() {
        if (!TankController.isDead && Input.GetKeyDown(KeyCode.Escape)) {
            if (gamePaused){
                resume();
            }else{
                pause();
            }
        }
    }
    #endregion

    #region Custom Methods
    void resume() {
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    void pause() {
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    void loadSettings() {

    }

    void restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        resume();
    }

    void quit() {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion
}