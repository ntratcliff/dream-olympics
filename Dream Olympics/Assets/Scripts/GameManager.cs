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

    /// <summary>
    /// How many seconds before inactive players are removed from the game on the scoreboard
    /// </summary>
    public float PlayerConfirmTime = 3f;

    public float ShowScoreboardDelay = 1f;

    /// <summary>
    /// The minigame object in the scene
    /// </summary>
    [HideInInspector]
    public GameObject MinigameObject;

    [HideInInspector]
    public int ActivePlayers;

    private bool minigameLoading;
    private bool minigameLoaded;
    private bool minigameRunning;
    private bool minigameOver;

    private bool pauseShown;

    public bool GameOver
    {
        get { return minigameOver; }
    }

    private Scene managerScene;

    private CanvasGroup scoreboard,
        minigameLoad,
        loadingMessage,
        readyMessage;

    public float LoadingMessageFadeTime = 0.6f;

    public int StartDelaySeconds = 3;

    public Color DefaultNameColor, ConfirmColor;

    private CanvasGroup pauseMenu, mainMenu;

    private float lastPauseVal = 0;

    void Awake()
    {
        scoreboard = GameObject.Find("Canvas/Scoreboard").GetComponent<CanvasGroup>();
        minigameLoad = GameObject.Find("Canvas/Scoreboard/MinigameLoad").GetComponent<CanvasGroup>();
        loadingMessage = GameObject.Find("Canvas/Scoreboard/MinigameLoad/Loading Message").GetComponent<CanvasGroup>();
        readyMessage = GameObject.Find("Canvas/Scoreboard/MinigameLoad/Ready Message").GetComponent<CanvasGroup>();
        pauseMenu = GameObject.Find("Canvas/Pause Menu").GetComponent<CanvasGroup>();
        mainMenu = GameObject.Find("Canvas/Main Menu").GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (pauseMenu.alpha != 0)
            pauseMenu.GetComponent<FadeCanvasGroup>().SetAlpha(0);

        if (mainMenu.alpha == 1)
        {
            // focus start button
            Transform startButton = mainMenu.transform.FindChild("Start Button");
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }

        if (Debug)
        {
            // initialize the current scene
            initializeScene();

            if (scoreboard.alpha != 0)
            {
                StartCoroutine(waitForPlayerConfirmation());
            }
            else if (mainMenu.alpha == 0)
            {
                sendStartMessage();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (minigameLoaded)
        {
            if (lastPauseVal < Input.GetAxis("Pause")
                && !pauseShown) // pause was just pressed!
            {
                Pause();
            }
            else if (lastPauseVal < Input.GetAxis("Pause"))
            {
                Unpause();
            }
        }

        lastPauseVal = Input.GetAxis("Pause");
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

        while (unloadOperation != null && !unloadOperation.isDone)
        {
            yield return new WaitForEndOfFrame();
        }

        minigameLoaded = false;
    }

    /// <summary>
    /// Loads a minigame
    /// </summary>
    /// <param name="name"></param>
    private IEnumerator loadMinigame(int index, float delay, bool skipConfirm = false)
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
        while (loadingMessage.alpha > 0f)
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

        if (skipConfirm)
        {
            StartCoroutine(startMinigame());
        }
        else
        {
            StartCoroutine(waitForPlayerConfirmation());
        }
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

        Transform playersObject = this.transform.FindChild("Players");
        PlayerInfo[] players = new PlayerInfo[playersObject.childCount];
        for (int i = 0; i < playersObject.childCount; i++)
        {
            players[i] = playersObject.GetChild(i).GetComponent<PlayerInfo>();
            players[i].gameObject.SetActive(true);
        }

        int playersConfirmed = 0;
        float confirmTime = 0f;
        while (playersConfirmed < players.Length)  // wait for players to confirm
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

            if (playersConfirmed >= 2
                && playersConfirmed < players.Length
                && confirmTime < PlayerConfirmTime) // increment counter
            {
                confirmTime += Time.deltaTime;
            }
            else if (confirmTime >= PlayerConfirmTime) // remove inactive players and start game
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].GetAxis("Action") == 0)
                    {
                        players[i].gameObject.SetActive(false);
                    }
                    else if (players[i].GetAxis("Action") == 1)
                        players[i].gameObject.SetActive(true);

                }
                break;
            }
            else // reset timer
            {
                confirmTime = 0;
            }
        }
        ActivePlayers = playersConfirmed;

        yield return startMinigame();

    }

    private IEnumerator startMinigame()
    {
        Transform playersObject = this.transform.FindChild("Players");
        PlayerInfo[] players = new PlayerInfo[playersObject.childCount];
        for (int i = 0; i < playersObject.childCount; i++)
        {
            players[i] = playersObject.GetChild(i).GetComponent<PlayerInfo>();
        }

        sendInitMessage();

        // fade out scoreboard
        yield return hideScoreboardDelay(0.3f);

        // reset colors
        for (int i = 0; i < players.Length; i++)
        {
            players[i].ScoreboardName.color = DefaultNameColor;
        }

        resetLoadMessage();

        int secondsLeft = StartDelaySeconds;
        while (secondsLeft > 0f)
        {
            // TODO: update timer text here
            UnityEngine.Debug.Log(secondsLeft--);
            yield return new WaitForSeconds(1);
        }

        // TODO: move this to title card eventually`
        sendStartMessage();
    }

    private void initializeScene()
    {
        minigameLoaded = true;

        // get Minigame object
        MinigameObject = GameObject.Find("Minigame");
        if (!MinigameObject)
            UnityEngine.Debug.LogError("Could not find Minigame object in scene!");

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

    public void Pause()
    {
        // show pause menu
        pauseMenu.GetComponent<FadeCanvasGroup>().FadeIn();
        pauseShown = true;

        if (scoreboard.alpha == 0f)
        {
            PauseMinigame();
        }
    }

    public void PauseMinigame()
    {
        sendMinigameMessage((x, y) => x.PauseGame());
        minigameRunning = false;
    }

    public void Unpause()
    {
        hidePauseMenu();
        if (scoreboard.alpha == 0)
        {
            ResumeMinigame();
        }
    }

    private void hidePauseMenu()
    {
        // hide pause menu
        pauseMenu.GetComponent<FadeCanvasGroup>().FadeOut();
        pauseShown = false;
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
        StartCoroutine(loadMinigame(getNextMinigame(), ShowScoreboardDelay + scoreboard.GetComponent<FadeCanvasGroup>().FadeInTime));
    }

    public void LoadTestMinigame()
    {
        scoreboard.GetComponent<FadeCanvasGroup>().SetAlpha(1f);
        StartCoroutine(loadMinigame(0, 0));
    }

    public void PauseMenuQuit()
    {
        StartCoroutine(unloadCurrentMinigame());
        hidePauseMenu();

        mainMenu.GetComponent<FadeCanvasGroup>().FadeIn();

        Transform startButton = mainMenu.transform.FindChild("Start Button");
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);

        StartCoroutine(hideScoreboardDelay(mainMenu.GetComponent<FadeCanvasGroup>().FadeInTime));
    }

    public void MainMenuStart()
    {
        EventSystem.current.SetSelectedGameObject(null);

        PlayerInfo[] players = GetComponentsInChildren<PlayerInfo>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].Score = 0;
        }

        // TODO: show some intermediary player join level
        scoreboard.GetComponent<FadeCanvasGroup>().FadeIn();
        float delay = scoreboard.GetComponent<FadeCanvasGroup>().FadeInTime;
        StartCoroutine(hideMainMenuDelay(delay));

        // TODO: shuffle minigames
        shuffleMinigames();
        StartCoroutine(loadMinigame(0, delay));

    }

    private void shuffleMinigames()
    {
        for (int i = 0; i < Minigames.Length; i++)
        {
            // get random index
            int ind = Random.Range(0, i + 1);

            // swap
            string game = Minigames[ind];
            Minigames[ind] = Minigames[i];
            Minigames[i] = game;
        }
    }

    private IEnumerator hideMainMenuDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        mainMenu.GetComponent<FadeCanvasGroup>().FadeOut();
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

    public void QuitGame()
    {
        Application.Quit();
    }

    private int getNextMinigame()
    {
        if (CurrentMinigame + 1 == Minigames.Length)
            return 0;

        return CurrentMinigame + 1;
    }

    public void SkipCurrentMinigame()
    {
        StartCoroutine(loadMinigame(getNextMinigame(), 0, true));
    }
}
