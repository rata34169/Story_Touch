using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    GameObject playerHand; //Player_Hand based on
    PickUpThrow flipMessage;
    //int initLevel = 0;
    /* Access method(get reading): flipMessage.reading
     * Variables LevelLoder need
     *      bool reading
     *      int  newPage
     */
    public Animator transitionAnima;
    float transitionTime = 1f;
    int oldPage;

    void Start()
    {
        playerHand = GameObject.Find("Player_Hand");
        flipMessage = playerHand.GetComponent<PickUpThrow>();
        oldPage = 0;
    }

    void Update()
    {
        if (oldPage != flipMessage.newPage)
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //Play Animation
        transitionAnima.SetTrigger("Change_scene");

        //Wait for animation complete
        yield return new WaitForSeconds(transitionTime);

        //Load new scene
        SceneManager.LoadScene(levelIndex);
    }
}