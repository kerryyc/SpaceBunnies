using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EndLevel1 : MonoBehaviour {
    public int levelNum;
    public string sceneName;
    public bool isDoorOpen;
    public bool isUseSceneName = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "PlayerHitbox" || collision.gameObject.tag == "Player")
        {
            Debug.Log("Collision with door");
            string levelName = "Level" + levelNum;
            // check if the door trigger has been triggered, this should only happen in levels 2 and onwards
            Debug.Log(isDoorOpen);
            if (isDoorOpen)
            {
                Debug.Log("Open Sesame");
                if (isUseSceneName)
                {
                    //loads a scene under scenename
                    SceneManager.LoadScene(sceneName);
                }
                else
                {
                    // loads a scene under the levelName;
                    SceneManager.LoadScene(levelName);

                }
                
            }

            

           
        }

    }
}
