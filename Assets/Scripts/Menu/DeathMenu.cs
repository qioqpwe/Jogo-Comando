using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour {
    #region Variables
    public GameObject deathMenuUI;
    #endregion

    #region Custom Methods
    void restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Cursor.visible = false;
        deathMenuUI.SetActive(false);
        Time.timeScale = 1f;
        PauseMenu.gamePaused = false;
        TankController.isDead = false;
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