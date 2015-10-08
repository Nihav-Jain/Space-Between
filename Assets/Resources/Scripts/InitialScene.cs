using UnityEngine;
using System.Collections;
using Fiea.Rpp.Storytelling;
using UnityEngine.UI;

public enum Scenes { SceneryStare, MoveInstructionDisplay, RotateInstructionDisplay, Idle, DebrisField, StationTurn, None, ConsoleReached, CircleAround, DisasterPoint, Blackout };
//enum ColliderScenes { None, ConsoleReached, CircleAround, DisasterPoint }
public class InitialScene : MonoBehaviour {

    private System.DateTime sceneChangeTimer; //How long before moving forward in scene?
    public float sceneChangeTimeMax;
    public float sceneChangeTimeElapsed;        //Used for pausing the system timer             
    public static float sceneryStareTime = 6.0f;    //Initial stare at scenery moment
    public static float moveInstructionDisplayTime = 5.0f; //How long to show instructions 
    public static float rotateInstructionDisplayTime = 5.0f;
    public static float fastDebrisTime = 3.0f;      //How long fast debris goes before station spins
    public static float stationSpinTime = 2.0f;     //How long station spins before we go unconscious
    public static float blackOutTime = 2.0f;
    private bool waitingForSceneChange = false;
    private static string spaceStationName = "Space_Station";
    public static float spaceStationTurnSpeed = 50.0f;
    public static float satelliteTurnSpeed = 0.2f;          //For an ambient satellite feel
    public float timeRemaining = 0.0f;              //For debugging purposes
    GameObject spaceStation;
    GameObject player;
    GameObject instructionDisplay;
    GameObject indicatorSphere;
    SphereCollider sceneChangeCollider;                 //For collider-induced scene changes
    public static float sceneChangeColliderRadius = 5.0f;
    Scenes currentScene;
    Scenes currentColliderScene;
    private bool initialSceneDone;                      //To check if we're post-initial scene

    private bool isMovePressed = true;
    private bool isRotatePressed = true;

    public static Vector3 initialColliderLocation = new Vector3(2.0f, 0, 2.0f);
    public static Vector3 overThereColliderLocation = new Vector3(0, 0, -5.0f);
    public static float drifterNudge = 4.0f;                  //Nudges the drifter's angular velocity
    public static float tugSpeed = 1.0f;                      //Tugging the player on attach
    public static Vector3 postActDrifterLocation = new Vector3(0.0f, 0.0f, 0.0f);         //Where to put the drifter after the act

    public bool isSceneCompleted;

    public static string moveInstruction = "Press WAD to move forward and sideways.\nPress QE to move up or down.";
    public static string rotateInstruction = "Use the Arrow keys to rotate. \nPress Space to stop moving.";
    public static string attachInstruction = "Press Shift to attach and detach.";
    private string fullInstruction = moveInstruction + "\n" + rotateInstruction + "\n" + attachInstruction;
    public static Vector2 instructionBoxSize = new Vector2(300.0f, 110.0f);
    public static Vector2 instructionTextSize = new Vector2(300.0f, 100.0f);

    public void startScene(Scenes newScene)
    {
        currentScene = newScene; //allScenes[index];
        isSceneCompleted = false;

        if (currentScene == Scenes.DebrisField)
            makeDebrisField();
        else if (currentScene == Scenes.StationTurn)
        {
            this.sceneChangeTimeMax = stationSpinTime;
            turnStation();
        }
    }

    public 

    void Start () {
        initBackgroundObjects();
        this.initialSceneDone = false; 
        currentScene = Scenes.SceneryStare;
        currentColliderScene = Scenes.None;
        sceneChangeTimeMax = sceneryStareTime;

        initSceneCollider();
        initUI();
        player = GameObject.Find("CustomCharacterController");
        player.GetComponent<CustomCharacterController>().setInputDisabled(true);

      
        
            
    }

    void initUI()
    {
        GameObject.Find("ScreenFader").GetComponent<ScreenFadeScript>().fadeIn();
        foreach (CanvasRenderer canvasRender in GameObject.Find("InstructionDisplay").GetComponentsInChildren<CanvasRenderer>())
        {
            canvasRender.SetAlpha(0.0f);        //The instruction for attaching will be visible
        }
        this.setInstruction(fullInstruction);
    }

    void initBackgroundObjects()
    {
        spaceStation = GameObject.Find(spaceStationName);
        this.transform.parent = spaceStation.transform;
        GameObject.Find("Satellite").GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f, satelliteTurnSpeed, 0.0f); //Spin the satellite a bit
    }

    public void setRotateInstruction()
    {
        this.setInstruction(rotateInstruction);
    }

    void setInstruction(string instruction)
    {
        Text instructText = GameObject.Find("InstructionDisplay").GetComponentInChildren<Text>();
        instructText.text = instruction;
    }

    void initSceneCollider()
    {
        sceneChangeCollider = this.gameObject.AddComponent<SphereCollider>();
        sceneChangeCollider.radius = sceneChangeColliderRadius;
        sceneChangeCollider.isTrigger = true;
    }

    public Scenes getCurrentScene()
    {
        return currentScene;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject != player || currentColliderScene == Scenes.None) return;
        if (currentColliderScene == Scenes.Idle)
        {
            currentScene = Scenes.Idle;
            currentColliderScene = Scenes.ConsoleReached;
            Debug.Log("console reached");
            //Update condition for story
            //player.GetComponent<CustomCharacterController>().setInputDisabled(true);
            GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.ConsoleReached, true);
            this.setInstruction(attachInstruction);
            setInstructionDisplayVisibility(1.0f);
        }
        else if (currentColliderScene == Scenes.CircleAround)
        {
            Debug.Log("reached disaster point");
            currentColliderScene = Scenes.DisasterPoint;
            GameObject.Find("IndicatorArrow").GetComponent<IndicatorArrowScript>().setIndicatorTarget(IndicatorTarget.Drifter);
            this.triggerAlarm();
            this.transform.localPosition = initialColliderLocation;
            indicatorSphere.GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.ReachedDisasterPoint, true);
        }
        //else makeDebrisField();
    }

    public void setInstructionDisplayVisibility(float value)
    {
        foreach (CanvasRenderer canvasRender in GameObject.Find("InstructionDisplay").GetComponentsInChildren<CanvasRenderer>())
        {
            canvasRender.SetAlpha(value);        //The instruction for attaching will be visible
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == player) setInstructionDisplayVisibility(0.0f);
    }

    void triggerAlarm()
    {
        GameObject.Find("SoundManager").GetComponent<SoundManager>().playDebrisAlarmSound(Camera.main.transform.position);
    }

    public bool isInitialSceneDone()
    {
        return initialSceneDone;
    }
	
    public void CheckForConsoleAttach(Vector3 ropeEndLocation, Vector3 consoleLocation, float attachRadius)            //Check if the drifter is attached to the console
    {
        if (currentColliderScene != Scenes.ConsoleReached) return;
        float distancetoAttach = Vector3.Distance(consoleLocation, ropeEndLocation);
        if (distancetoAttach < attachRadius)
        {
            GameObject.Find("Drifter").GetComponent<DrifterScript>().detach();
            GameObject.Find("Drifter").GetComponent<DrifterScript>().attachToStation(consoleLocation);
            currentColliderScene = Scenes.CircleAround;
            this.transform.localPosition = overThereColliderLocation;
            indicatorSphere.transform.position = this.transform.position;
            GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.ActionPressed, true);
            this.setInstruction(fullInstruction);
        }
        Debug.Log("waiting for X");
    }

	// Update is called once per frame
	void Update () {
        if (GameObject.Find("MainGame").GetComponent<MainScript>().isPaused()) return;

        this.checkInstructionInput();                                   //Check input for instruction based events

        if (isSceneCompleted)
            return;
        if (Input.GetKeyDown(KeyCode.X)) turnStation();
        if (!this.waitingForSceneChange && currentScene != Scenes.Idle) //Are we in a state where time counts?
        {
            this.sceneChangeTimer = System.DateTime.Now; //Start timer
            this.waitingForSceneChange = true;
        }
        this.timeRemaining = (float)(System.DateTime.Now - this.sceneChangeTimer).TotalSeconds - sceneChangeTimeElapsed;
        if (this.timeRemaining > sceneChangeTimeMax) //Test how long things pass in seconds
        {
            changeTimedState();
            this.waitingForSceneChange = false;
        }
    }

    void checkInstructionInput()
    {
        if (!isMovePressed && currentScene == Scenes.MoveInstructionDisplay)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                currentScene = Scenes.RotateInstructionDisplay;
                isRotatePressed = false;
                isMovePressed = true;
                GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.CompletedMovementTutorial, true);
                //setInstruction(rotateInstruction);
                //Inform dialogue change here
            }
        }
        else if (!isRotatePressed && currentScene == Scenes.RotateInstructionDisplay)
        {
            //if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) ||
            //    Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space))
            //{
                currentScene = Scenes.Idle;
                isRotatePressed = true;
                GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.CompletedRotationTutorial, true);
                this.setInstructionDisplayVisibility(0.0f);
                //Inform dialogue change here
            //}
        }
    }

    public void Pause()
    {
        sceneChangeTimeElapsed = (float)(System.DateTime.Now - this.sceneChangeTimer).TotalSeconds % sceneChangeTimeMax;
    }

    void changeTimedState()
    {
        isSceneCompleted = true;

        switch (currentScene)
        {
            case Scenes.SceneryStare:
                currentScene = Scenes.MoveInstructionDisplay;
                this.sceneChangeTimeMax = moveInstructionDisplayTime;
                isMovePressed = false;
                tugPlayer();
                break;
            case Scenes.MoveInstructionDisplay:
                this.sceneChangeTimeMax = rotateInstructionDisplayTime;
                setInstruction(rotateInstruction);
                break;
            case Scenes.RotateInstructionDisplay:
                currentScene = Scenes.Idle;
                break;
            case Scenes.StationTurn:
                currentScene = Scenes.Blackout;
                this.sceneChangeTimeMax = blackOutTime;
                blackOut();
                break;
            case Scenes.Blackout:
                if (spaceStation != null) exitScene();
                break;
            default:
                break;
        }
    }

    void tugPlayer()
    {
        //Tug player backwards a bit
        player.GetComponent<CustomCharacterController>().setInputDisabled(false);
        GameObject.Find("Drifter").GetComponent<DrifterScript>().attach(true);      //Immediate tug upon attaching rope
        displayMoveInstructions();
        currentColliderScene = Scenes.Idle;
    }

    void displayMoveInstructions()
    {
        instructionDisplay = GameObject.Find("InstructionDisplay");
        this.setInstruction(moveInstruction);
        startInitialCollider();
    }

    void startInitialCollider()
    {
        indicatorSphere = GameObject.Find("IndicatorSphere");                   //Start the initial collider location and set the glowing sphere to it
        indicatorSphere.GetComponent<MeshRenderer>().enabled = true;
        this.transform.localPosition = initialColliderLocation;
        indicatorSphere.transform.position = this.transform.position;
        GameObject.Find("IndicatorArrow").GetComponent<IndicatorArrowScript>().setIndicatorTarget(IndicatorTarget.IndicatorSphere);

    }

    void makeDebrisField()
    {
        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.ReachedDisasterPoint, true);
        currentColliderScene = Scenes.None;                             //All done with the collider, get rid of it
        indicatorSphere.GetComponent<MeshRenderer>().enabled = false;
        //currentScene = Scenes.DebrisField;
        this.sceneChangeTimeMax = fastDebrisTime;
        GameObject.Find("DebrisGenerator").GetComponent<DebrisGeneratorScript>().createDebris(true);
    }

    void turnStation()              //Turns space station to knock you unconscious
    {
        GameObject.Find("Drifter").GetComponent<DrifterScript>().detachFromStation();           //Can't have drifter flinging on station
        foreach(Collider collider in spaceStation.GetComponents<Collider>())
        {
            collider.enabled = false;               //Disable all colliders
        }
        spaceStation.GetComponent<Rigidbody>().angularDrag = 0;

        GameObject.Find("SoundManager").GetComponent<SoundManager>().playThud(Camera.main.transform.position);
        currentScene = Scenes.StationTurn;
        this.sceneChangeTimeMax = stationSpinTime;
        spaceStation.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, spaceStationTurnSpeed);
        GameObject.Find("ScreenFader").GetComponent<ScreenFadeScript>().blackOut();
        player.GetComponent<CustomCharacterController>().setInputDisabled(true);
        //player.GetComponent<CustomCharacterController>().setRotationSpeed(spaceStationTurnSpeed, spaceStationTurnSpeed);
    }

    void blackOut()         //Used specifically for no sound during blackout
    {
        isSceneCompleted = false;
        AudioListener.volume = 0;
        currentScene = Scenes.Blackout;
        GameObject.Find("SoundManager").GetComponent<SoundManager>().stopSounds();
    }

    void exitScene()
    {
        GameObject.Find("EscapePod").AddComponent<EscapePodScript>().initEscapePod();       //Activate escape pod script
        player.GetComponent<CustomCharacterController>().setInputDisabled(false);           //Regain movement
        player.GetComponent<CustomCharacterController>().setRotationSpeed(0, 0);

        GameObject.Find("Drifter").GetComponent<Rigidbody>().angularVelocity += new Vector3(0, 0, drifterNudge); //Nudge the drifter a tad
        GameObject.Find("Drifter").transform.position = postActDrifterLocation;
        GameObject.Find(spaceStationName).transform.DetachChildren();
        Destroy(GameObject.Find(spaceStationName));         //Destroy the station
        this.initialSceneDone = true;
        AudioListener.volume = 1;
        GameObject.Find("SoundManager").GetComponent<SoundManager>().playEarRinging(Camera.main.transform.position);
        GameObject.Find("DebrisGenerator").GetComponent<DebrisGeneratorScript>().createDebris(false);
        GameObject.Find("ScreenFader").GetComponent<ScreenFadeScript>().fadeIn();   //Fade in and start decreasing oxygen at a steady rate
        GameObject.Find("OxygenBars").GetComponent<OxygenBarScript>().setRegularReductionRate();
        this.isSceneCompleted = true;
    }
}
