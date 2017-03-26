using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Checked if there is already a loaded scene in the editor
    /// </summary>
    public bool Debug;
    public string ManagerSceneName = "Main";

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
    private bool minigameRunning;
    private bool minigameOver;
    public bool GameOver
    {
        get { return minigameOver; }
    }

    private Scene managerScene;


    void Awake()
    {

    }

    void Start()
    {
        if (Debug)
        {
            // initialize the current scene
            initializeScene();
            sendStartMessage();
        }   
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
        UnityEngine.Debug.Log("Unloading current scene...");

        if (minigameRunning)
        {
            UnityEngine.Debug.LogWarning("Unloading minigame while the game is still running!");
            PauseMinigame();
        }

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

        UnityEngine.Debug.Log("Loading scene: " + Minigames[index]);

        CurrentMinigame = index;
        minigameLoading = true;

        // start additive scene load
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Scenes/Minigames/" + Minigames[index], LoadSceneMode.Additive);

        while (!loadOperation.isDone)
        {
            //TODO: show loading bar and update progress
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(Minigames[index]));

        UnityEngine.Debug.Log("Scene loaded! Initializing...");

        // scene has finished loading, initialize
        initializeScene();

        // TODO: move this to title card eventually
        sendStartMessage();
    }

    private void initializeScene()
    {
        minigameLoaded = true;

        // get Minigame object
        MinigameObject = GameObject.Find("Minigame");
        if (!MinigameObject)
            UnityEngine.Debug.LogError("Could not find Minigame object in scene!");

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

        for (int i = 0; root && i < root.transform.childCount; i++)
        {
            executeRecursive(root.transform.GetChild(i).gameObject, callback);
        }
    }

    /// <summary>
    /// Send init message to all minigame behaviours in the scene
    /// </summary>
    private void sendInitMessage()
    {
        minigameOver = false;
        sendMinigameMessage((x, y) => x.Init(this));
    }

    private void sendStartMessage()
    {
        sendMinigameMessage((x, y) => x.StartGame());
        minigameRunning = true;
    }

    public void PauseMinigame()
    {
        sendMinigameMessage((x, y) => x.PauseGame());
        minigameRunning = false;
    }

    public void ResumeMinigame()
    {
        sendMinigameMessage((x, y) => x.ResumeGame());
        minigameRunning = true;
    }

    /// <summary>
    /// Called when the game has been won
    /// </summary>
    public void EndMinigame()
    {
        minigameOver = true;
        sendMinigameMessage((x, y) => x.PauseGame());
        minigameRunning = false;
    }

    public void LoadTestMinigame()
    {
        StartCoroutine(loadMinigame(0));
    }

}
