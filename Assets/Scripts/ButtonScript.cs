using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour {

	public void StartGame() {
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ReloadScene() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            Debug.Log("Clicked button");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
