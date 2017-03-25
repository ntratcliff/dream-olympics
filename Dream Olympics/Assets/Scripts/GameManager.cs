using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Minigame scene names, to be loaded additively
    /// </summary>
    public string[] Minigames;

    /// <summary>
    /// Index of the current minigame
    /// </summary>
    public int CurrentMinigame = -1;

    /// <summary>
    /// The minigame object in the scene
    /// </summary>
    [HideInInspector]
    public GameObject MinigameObject;

    private bool minigameLoading;
    private bool minigameLoaded;

    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Unloads the current minigame
    /// </summary>
    private IEnumerator unloadCurrentMinigame()
    {
        Debug.Log("Unloading current scene...");

        // TODO: get our player controllers back before unloading
        // dereference any objects referenced in the scene
        MinigameObject = null; 

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(Minigames[CurrentMinigame]);

        while (!unloadOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        minigameLoaded = false;
    }

    /// <summary>
    /// Loads a minigame
    /// </summary>
    /// <param name="name"></param>
    private IEnumerator loadMinigame(int index)
    {
        if (minigameLoaded)
            yield return unloadCurrentMinigame();

        Debug.Log("Loading scene: " + Minigames[index]);

        CurrentMinigame = index;
        minigameLoading = true;

        // start additive scene load
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Scenes/Minigames/"+Minigames[index], LoadSceneMode.Additive);

        while(!loadOperation.isDone)
        {
            //TODO: show loading bar and update progress
            yield return new WaitForEndOfFrame();
        }

        minigameLoaded = true;

        Debug.Log("Scene loaded! Sending Init message...");

        // get Minigame object
        MinigameObject = GameObject.Find("Minigame");
        if (!MinigameObject)
            Debug.LogError("Could not find Minigame object in scene!");

        // scene has finished loading, initialize
        sendInitMessage();
    }

    /// <summary>
    /// Sends a message to the minigame behaviours
    /// </summary>
    /// <param name="callback"></param>
    private void sendMinigameMessage(ExecuteEvents.EventFunction<IMinigameBehaviour> callback)
    {
        // execute on minigame and all children
        executeRecursive<IMinigameBehaviour>(MinigameObject, callback);
    }

    /// <summary>
    /// Recursively executes a Unity event
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    /// <param name="callback"></param>
    private void executeRecursive<T>(GameObject root, ExecuteEvents.EventFunction<T> callback) where T : IEventSystemHandler
    {
        ExecuteEvents.Execute(root, null, callback);

        for (int i = 0; i < root.transform.childCount; i++)
        {
            executeRecursive(root.transform.GetChild(i).gameObject, callback);
        }
    }

    /// <summary>
    /// Send init message to all minigame behaviours in the scene
    /// </summary>
    private void sendInitMessage()
    {
        sendMinigameMessage((x, y) => x.Init(this));
    }

    public void LoadTestMinigame()
    {
        StartCoroutine(loadMinigame(0));
    }
}
