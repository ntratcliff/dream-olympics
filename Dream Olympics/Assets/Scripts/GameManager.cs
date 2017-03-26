using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public float ShowScoreboardDelay = 1f;

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

    private CanvasGroup scoreboard, minigameLoad, loadingMessage, readyMessage;

    public float LoadingMessageFadeTime = 0.6f;

    public int StartDelaySeconds = 3;

    public Color DefaultNameColor, ConfirmColor;

    void Awake()
    {
        scoreboard = GameObject.Find("Canvas/Scoreboard").GetComponent<CanvasGroup>();
        minigameLoad = GameObject.Find("Canvas/Scoreboard/MinigameLoad").GetComponent<CanvasGroup>();
        loadingMessage = GameObject.Find("Canvas/Scoreboard/MinigameLoad/Loading Message").GetComponent<CanvasGroup>();
        readyMessage = GameObject.Find("Canvas/Scoreboard/MinigameLoad/Ready Message").GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (Debug)
        {
            // initialize the current scene
            initializeScene();

            if(scoreboard.alpha != 0)
            {
                StartCoroutine(waitForPlayerConfirmation());
            }
            else
            {
                sendStartMessage();
            }
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
    private IEnumerator loadMinigame(int index, float delay)
    {
        yield return new WaitForSeconds(delay);

        minigameLoad.alpha = 1f; // TODO: fade in

        if (minigameLoaded)
            yield return unloadCurrentMinigame();

        UnityEngine.Debug.Log("Loading scene: " + Minigames[index]);

        CurrentMinigame = index;
        minigameLoading = true;

        // start additive scene load
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Scenes/Minigames/" + Minigames[index], LoadSceneMode.Additive);

        while (!loadOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(Minigames[index]));

        UnityEngine.Debug.Log("Scene loaded! Initializing...");

        // scene has finished loading, initialize
        initializeScene();

        yield return new WaitForEndOfFrame();

        // fade out loading message, fade in ready message
        float elapsedTime = 0;
        while(loadingMessage.alpha > 0f)
        {
            elapsedTime += Time.deltaTime;
            loadingMessage.alpha = Mathf.Lerp(1f, 0f, elapsedTime / LoadingMessageFadeTime);
            yield return new WaitForEndOfFrame();
        }

        elapsedTime = 0;
        while (readyMessage.alpha < 1f)
        {
            elapsedTime += Time.deltaTime;
            readyMessage.alpha = Mathf.Lerp(0f, 1f, elapsedTime / LoadingMessageFadeTime);
            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(waitForPlayerConfirmation());
    }

    /// <summary>
    /// Wait for all players to be holding A before starting next game
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForPlayerConfirmation()
    {
        readyMessage.alpha = 1f;
        loadingMessage.alpha = 0f;
        minigameLoad.alpha = 1f;

        PlayerInfo[] players = GetComponentsInChildren<PlayerInfo>();
        int playersConfirmed = 0;
        while(playersConfirmed < players.Length)  // wait for players to confirm
        {
            playersConfirmed = 0;
            // set color based on state
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetAxis("Action") != 0)
                {
                    players[i].ScoreboardName.color = ConfirmColor;
                    playersConfirmed++;
                }
                else
                    players[i].ScoreboardName.color = DefaultNameColor;
            }
            yield return new WaitForEndOfFrame();
        }

        // fade out scoreboard
        yield return hideScoreboardDelay(0.3f);

        resetLoadMessage();

        int secondsLeft = StartDelaySeconds;
        while(--secondsLeft > StartDelaySeconds)
        {
            // TODO: update timer text here
            yield return new WaitForSeconds(1);
        }

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
        sendMinigameMessage((x, y) => x.GameOver());
        minigameRunning = false;

        StartCoroutine(showScoreboardDelay(ShowScoreboardDelay));
        StartCoroutine(loadMinigame(0, ShowScoreboardDelay + scoreboard.GetComponent<FadeCanvasGroup>().FadeInTime)); // TODO: cycle through games here
    }

    public void LoadTestMinigame()
    {
        StartCoroutine(loadMinigame(0, 0));
    }

    private IEnumerator showScoreboardDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        scoreboard.GetComponent<FadeCanvasGroup>().FadeIn();
    }

    private IEnumerator hideScoreboardDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        scoreboard.GetComponent<FadeCanvasGroup>().FadeOut();
    }

    private void resetLoadMessage()
    {
        readyMessage.alpha = 0f;
        loadingMessage.alpha = 1f;
        minigameLoad.alpha = 0f;
    }
}
