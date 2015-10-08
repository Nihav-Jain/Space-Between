using UnityEngine;
using System.Collections;
using Fiea.Rpp.Storytelling;
using UnityEngine.UI;
public class MainScript : MonoBehaviour
{

    // Use this for initialization
    private GameObject debrisGenerator;
    private GameObject objectiveIndicator;      //Shows you where to go, other than the drifter you need to save
    private GameObject initialScene;    //Starts off the initial segments
    private GameObject soundManager;
    private GameObject subtitleHandler;
    private GameObject dialogueManager;
    private GameObject storyConditionManager;
    private GameObject postProcessManager;
    private GameObject oxygenBars; 
    private GameObject screenFader; //Fades screen in and out to black
    private GameObject drifter; //Who do we have to rescue?
    private GameObject endStatistics;   //Displays your stats at the end
    public static Vector3 drifterOffset = new Vector3(0, 0, -5.0f);
    private GameObject character;
    private GameObject story;
    private StoryTree tree;
    private StoryStatus curGameStatus;
    private bool paused;
    private DialogueHandler dialogueHandler;
    private float timer = 0;

    void Start()
    {
        this.paused = false;

        dialogueHandler = new DialogueHandler();
        
        debrisGenerator = new GameObject("DebrisGenerator");
        debrisGenerator.transform.parent = this.transform;
        debrisGenerator.AddComponent<DebrisGeneratorScript>();

        soundManager = new GameObject("SoundManager"); //Instantiate object that carries and plays all sounds
        soundManager.transform.parent = this.transform;
        soundManager.AddComponent<SoundManager>();

        subtitleHandler = new GameObject("SubtitleHandler");
        subtitleHandler.transform.parent = this.transform;
        subtitleHandler.AddComponent<DialogueHandler>();

        dialogueManager = new GameObject("DialogueManager"); //Instantiate object that shows dialogue on screen
        dialogueManager.transform.parent = this.transform;
        dialogueManager.AddComponent<DialogueManagerScript>();

        postProcessManager = new GameObject("PostProcessManager");
        postProcessManager.transform.parent = this.transform;
        postProcessManager.AddComponent<PostProcessManagerScript>();

        storyConditionManager = new GameObject("StoryConditionManager");
        storyConditionManager.transform.parent = this.transform;
        storyConditionManager.AddComponent<StoryConditionManager>();

        oxygenBars = GameObject.Find("OxygenBars");
        oxygenBars.transform.parent = this.transform;
        oxygenBars.AddComponent<OxygenBarScript>();

        screenFader = new GameObject("ScreenFader");
        screenFader.transform.parent = this.transform;
        screenFader.AddComponent<ScreenFadeScript>();

        endStatistics = GameObject.Find("EndStatistics");
        endStatistics.transform.SetParent(this.transform);
        endStatistics.AddComponent<EndStatisticsScript>();

        initialScene = new GameObject("InitialScene");
        initialScene.transform.parent = this.transform;
        initialScene.AddComponent<InitialScene>();

        objectiveIndicator = new GameObject("ObjectiveIndicator");
        objectiveIndicator.AddComponent<ObjectiveIndicatorScript>();

        character = new GameObject("CustomCharacterController");
        // character.transform.parent = this.transform;
        character.AddComponent<CustomCharacterController>();

        drifter = GameObject.Find("Drifter");
        drifter.transform.parent = this.transform;
        drifter.AddComponent<DrifterScript>();
        drifter.transform.position = Camera.main.transform.position + drifterOffset;

        tree = new StoryTree(this.gameObject);
        tree.growTree();
        curGameStatus = StoryStatus.STATUS_None;

        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().addCondition(StoryConditionValues.PlayerDead);
        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().addCondition(StoryConditionValues.DrifterDead);

        dialogueHandler.hideSubtitle();

        Pause();

        //WWW gameAnalytics = this.getAnalytics();
        //WWW updateGameAnalytics = this.updateAnalytics(GAME_ANALYTICS_DID_NOT_SAVE);
    }

    public SoundManager getSoundManager()
    {
        return soundManager.GetComponent<SoundManager>();
    }
    public void Pause()    //Pause sounds and the flow of time, and make instruction display visible if it isn't already
    {
        if (Time.timeScale == 0.0)
        {
            this.paused = false;
            foreach (AudioSource source in this.gameObject.GetComponents<AudioSource>())
            {
                source.UnPause();
            }
            soundManager.GetComponent<SoundManager>().UnPause();
            Time.timeScale = 1.0f;
            Scenes currentScene = GameObject.Find("InitialScene").GetComponent<InitialScene>().getCurrentScene();
            if (currentScene == Scenes.MoveInstructionDisplay || currentScene == Scenes.RotateInstructionDisplay) return;    //Only if we're not already displaying them
            foreach (CanvasRenderer canvasRender in GameObject.Find("InstructionDisplay").GetComponentsInChildren<CanvasRenderer>())
            {
                canvasRender.SetAlpha(0.0f);        //The instruction for attaching will be visible
            }
        }
        else
        {
            this.paused = true;
            initialScene.GetComponent<InitialScene>().Pause();
            soundManager.GetComponent<SoundManager>().Pause();
            Time.timeScale = 0.0f;
            foreach (AudioSource source in this.gameObject.GetComponents<AudioSource>())
            {
                source.Pause();
            }
            GameObject.Find("OxygenBars").GetComponent<OxygenBarScript>().Pause();
            foreach (CanvasRenderer canvasRender in GameObject.Find("InstructionDisplay").GetComponentsInChildren<CanvasRenderer>())
            {
                if (canvasRender.GetAlpha() <= 0.0f) canvasRender.SetAlpha(1.0f);        //The instruction for attaching will be visible
            }
        }
    }

    public bool isPaused()
    {
        return this.paused;
    }

    public void hideStartScreen()
    {
        if (Input.GetKeyDown(KeyCode.Return) && GameObject.Find("StartScreen").GetComponent<Canvas>().enabled)
        {
            GameObject.Find("StartScreen").GetComponent<Canvas>().enabled = false;
            Pause();
            Pause();
        }
    }

    public void toggleSubtitles()
    {

        if (Input.GetKeyDown(KeyCode.Z) && GameObject.Find("Subtitles").GetComponentInChildren<Text>().enabled)
        {
            GameObject.Find("subToggle").GetComponent<Text>().text = "Off";
            dialogueHandler.hideSubtitle();
        }
        else if (Input.GetKeyDown(KeyCode.Z) && GameObject.Find("Subtitles").GetComponentInChildren<Text>().enabled == false)
        {
            GameObject.Find("subToggle").GetComponent<Text>().text = "On";
            dialogueHandler.showSubtitle();
        }
    }

    // Update is called once per frame
    void Update()
    {
        hideStartScreen();
        toggleSubtitles();

        bool playerDead = GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.PlayerDead);
        bool drifterDead = GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.DrifterDead);
        bool isAttached = GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.isAttached);
        if (curGameStatus == StoryStatus.STATUS_Success)
            Debug.Log("WIN");
        else if (curGameStatus == StoryStatus.STATUS_Failure)
            Debug.Log("LOSE");
        else if(!(playerDead || drifterDead || isAttached))
        {
            if(tree != null) curGameStatus = tree.Update();  // move this to the top once the win and fail situations have been handled and calls to this function have been terminated
        }

        if (Input.GetKeyDown(KeyCode.Return) && !GameObject.Find("DialogueManager").GetComponent<DialogueManagerScript>().isOptionsActive()) Pause();
    }
}
