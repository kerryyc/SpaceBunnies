using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour {

	public void StartGame() {
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
