using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Globalization;
using Fiea.Rpp.Storytelling;

public class OxygenBarScript : MonoBehaviour {

    GameObject playerOxygenBar; //Oxygen bar for player
    GameObject drifterOxygenBar; //Oxygen bar for drifter
    GameObject indicatorArrow; //Indicator bar for arrow to find off-screen drifter
    public  Vector2 drifterOxygenBarOffset = new Vector2(-900.0f, 0.0f); //Sets offset of drifterBar on screen
    private Vector2 defaultDrifterBarLocation;
    private Vector2 defaultDrifterTextLocation;
    private static Vector2 oxygenTextOffset = new Vector2(0, 15); //Offsets text from oxygen bar (for drifter only!)
    private static float warningThreshold = 0.15f; //How much oxygen is left when the warning turns on
    public float playerOxygenReductionRate;
    public float drifterOxygenReductionRate;
    private static float initialOxygenReductionRate = 0.0001f; //Oxygen depletion rate in initial scene
    private static float regularOxygenReductionRate = 0.0003f; //How quickly oxygen depletes without stress
    private static float stressedOxygenReductionRate = 0.0005f; //How quickly oxygen depletes with stress
    private bool drifterStressed; //Is the drifter stressed?
    private System.DateTime oxygenDecreaseTimer; //How long to wait until oxygen depletes
    private bool waitingForOxygenDecrease;
    public static float oxygenDecreaseTimeMax = 50; //in milliseconds
    private float oxygenDecreaseTimeElapsed = 0;        //For pause
    private NumberFormatInfo oxygenTimerFormat;         //Format of oxygen bar timer
    GameObject oxygenDepletionDisplay;                  //To fit alongside player bar

    private bool isPlayerDead = false;
    private bool isDrifterDead = false;

    private StoryBehavior root;
    private StoryStatus lowOxygenStatus;

    public static string playerName = "Jess's";
    public static string drifterName = "Nick's";

	// Use this for initialization
	void Start () {
        this.initPlayerOxygenBar();
        this.initDrifterOxygenBar();
        this.initIndicatorArrow();
        this.gameObject.layer = LayerMask.NameToLayer("UI");
        oxygenDepletionDisplay = GameObject.Find("OxygenDepletionDisplay");
        foreach (CanvasRenderer canvasRender in oxygenDepletionDisplay.GetComponentsInChildren<CanvasRenderer>())
        {
            canvasRender.SetAlpha(0.0f);        //The instruction for attaching will be visible
        }
        oxygenTimerFormat = new CultureInfo("en-US", false).NumberFormat;
        root = null;
        lowOxygenStatus = StoryStatus.STATUS_None;
    }

    public Vector2 getDrifterBarLocation()
    {
        return defaultDrifterBarLocation;
    }

    void initIndicatorArrow()
    {
        indicatorArrow = GameObject.Find("IndicatorArrow");
        indicatorArrow.AddComponent<IndicatorArrowScript>();
        indicatorArrow.transform.SetParent(this.transform);    
    }

    public void setDrifterStressed(bool stressed)
    {
        if (!GameObject.Find("InitialScene").GetComponent<InitialScene>().isInitialSceneDone()) return;
        this.drifterStressed = stressed;
        if (this.drifterStressed) drifterOxygenReductionRate = stressedOxygenReductionRate;
        else drifterOxygenReductionRate = regularOxygenReductionRate;
    }

    public bool isDrifterStressed()
    {
        return this.drifterStressed;
    }

    void initPlayerOxygenBar()
    {
        playerOxygenBar = GameObject.Find("PlayerOxygenBar");
        playerOxygenBar.GetComponentInChildren<Text>().text = playerName + " Oxygen: ";
        this.playerOxygenReductionRate = initialOxygenReductionRate;
    }

    void initDrifterOxygenBar()
    {
        drifterOxygenBar = GameObject.Find("DrifterOxygenBar");
        drifterOxygenBar.GetComponentInChildren<Text>().text = drifterName + " Oxygen: ";
        RectTransform rect = drifterOxygenBar.GetComponentInChildren<RectTransform>();
        defaultDrifterTextLocation = rect.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
        defaultDrifterBarLocation = rect.GetChild(1).GetComponent<RectTransform>().anchoredPosition;
        this.drifterOxygenReductionRate = initialOxygenReductionRate;
    }

    public void setRegularReductionRate()
    {
        this.playerOxygenReductionRate = regularOxygenReductionRate;
        this.drifterOxygenReductionRate = regularOxygenReductionRate;
    }

	void setDrifterBar()
    {
        GameObject drifterCenter = GameObject.Find("Link1");
        Vector2 screenLocation = Camera.main.WorldToScreenPoint(drifterCenter.transform.position);
        RectTransform rect = drifterOxygenBar.GetComponentInChildren<RectTransform>();

        if (!drifterCenter.GetComponent<Renderer>().isVisible)
        {
            rect.GetChild(0).GetComponent<RectTransform>().anchoredPosition = defaultDrifterTextLocation;
            rect.GetChild(1).GetComponent<RectTransform>().anchoredPosition = defaultDrifterBarLocation;
            return;
        }
          
        rect.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(oxygenTextOffset.x + screenLocation.x + drifterOxygenBarOffset.x, 
                oxygenTextOffset.y + screenLocation.y + drifterOxygenBarOffset.y, 0);
        rect.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector3(screenLocation.x + drifterOxygenBarOffset.x, screenLocation.y + drifterOxygenBarOffset.y, 0);
        
    }

    void killPlayer()
    {
        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.PlayerDead, true);
        GameObject.Find("EndStatistics").GetComponent<EndStatisticsScript>().endGame();
    }

    void killDrifter()
    {
        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.DrifterDead, true);
    }


    void setLowOxygenAlarm()
    {
        GameObject.Find("SoundManager").GetComponent<SoundManager>().playOxygenWarningSound(Camera.main.transform.position);
        playerOxygenBar.GetComponentInChildren<Text>().color = Color.red;
        foreach (CanvasRenderer canvasRender in oxygenDepletionDisplay.GetComponentsInChildren<CanvasRenderer>())
        {
            canvasRender.SetAlpha(1.0f);        //The instruction for attaching will be visible
        }
        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.lowRescueTime, true);
        if (!GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.DrifterDead))
        {
            root = new StorySequence();
            root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_WereRunningOut", this.gameObject, 0, false, "Now, let's get out of here."));
            root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_NotHelping", this.gameObject, 0, false, "You're not helping."));
            root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_FastAsICan", this.gameObject, 0, false, "I'm going as fast as I can."));
            lowOxygenStatus = StoryStatus.STATUS_Running;
            GameObject.Find("Drifter").GetComponent<DrifterScript>().stopAttachedDialogueTree();
        }
    }

    public void Pause()
    {
        oxygenDecreaseTimeElapsed = (float)(System.DateTime.Now - this.oxygenDecreaseTimer).TotalMilliseconds % oxygenDecreaseTimeMax;
    }

	// Update is called once per frame
	void Update () {
        if (GameObject.Find("MainGame").GetComponent<MainScript>().isPaused()) return;

        if (!isPlayerDead && playerOxygenBar.transform.GetComponentInChildren<Slider>().value <= 0) killPlayer();
        if (!isDrifterDead && drifterOxygenBar.transform.GetComponentInChildren<Slider>().value <= 0) killDrifter();

        this.updateOxygenDepletion();
        this.setDrifterBar();
        float playerValue = playerOxygenBar.transform.GetComponentInChildren<Slider>().value;
        if (playerValue > warningThreshold && playerValue - regularOxygenReductionRate < warningThreshold) this.setLowOxygenAlarm(); //Play the warning alarm only once!

        if (!this.waitingForOxygenDecrease)
        {
            this.oxygenDecreaseTimer = System.DateTime.Now; //Start timer
            this.waitingForOxygenDecrease = true;
        }
        float timeRemaining = (float)(System.DateTime.Now - this.oxygenDecreaseTimer).TotalMilliseconds - oxygenDecreaseTimeElapsed;
        if (timeRemaining > oxygenDecreaseTimeMax) //Test how long things pass in seconds
        {
            oxygenDecreaseTimeElapsed = 0;
            playerOxygenBar.transform.GetComponentInChildren<Slider>().value -= playerOxygenReductionRate;      //Reduce oxygen bars
            drifterOxygenBar.transform.GetComponentInChildren<Slider>().value -= drifterOxygenReductionRate;
            this.waitingForOxygenDecrease = false;
        }
        if (root != null && lowOxygenStatus == StoryStatus.STATUS_Running)
            lowOxygenStatus = root.Tick();
        if (lowOxygenStatus == StoryStatus.STATUS_Success)
            stopDialogueTree();


    }

    public void stopDialogueTree()
    {
        if(root != null)
            (root as StorySequence).StopPropogation();
        root = null;
        lowOxygenStatus = StoryStatus.STATUS_None;
    }


    void updateOxygenDepletion()
    {
        Text timeRemaining = GameObject.Find("TimeRemaining").GetComponent<Text>();
        float millisecondsToLive = playerOxygenBar.transform.GetComponentInChildren<Slider>().value / playerOxygenReductionRate * oxygenDecreaseTimeMax;      
        double secondsToLive = millisecondsToLive / 1000.0;     //Transform to minutes
        
        timeRemaining.text = secondsToLive.ToString("N", oxygenTimerFormat);
    }
}
