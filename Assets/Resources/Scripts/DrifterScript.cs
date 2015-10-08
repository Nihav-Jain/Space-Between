using UnityEngine;
using System.Collections;
using Fiea.Rpp.Storytelling;

public class DrifterScript : MonoBehaviour
{
    Rigidbody rbody;
    public static float rBodyDrag = 0.8f;
    GameObject tetherRope;                    //Rope that you can attach to the drifter with
    public static float ropeMass = 0.01f;
    public static float playerAttachMass = 40.0f;
    public static float attachForceStrength = 5.0f;         //For applying to drifter when attaching
    private const string astronautMeshLocation = "Meshes/Astronaut";
    private const string ropeEndName = "Link20"; //End of the rope for attaching
    GameObject ropeEnd;                     //End of the rope
    public bool attached;                  //Are you attached to the drifter's rope?
    public bool attachedToStation;         //Attached to space station?
    public static float attachRadius = 5.0f;        //How far away you must be to attach rope
    public static float stationAttachRadius = 20.0f; //How far away you must be to attach to station
    //private Vector3 stationAttachLocation;    //For keeping ropeEnd on station
    GameObject player;                      //person trying to rescue the drifter
    SphereCollider sphereCollider;

    private StoryBehavior root;
    private StoryStatus attachDialogueTreeStatus;
   
    // Use this for initialization
    void Start () {
        player = GameObject.Find("CustomCharacterController");
        this.initVelocity();
        this.initRigidBody();
        this.initCollider();
        this.initRope();
        this.attached = false;
        this.attachedToStation = false;
        this.attachDialogueTreeStatus = StoryStatus.STATUS_None;
        this.root = null;
    }

    private void initRope()
    {
        this.tetherRope = GameObject.Instantiate(Resources.Load("Prefabs/Chain", typeof(GameObject))) as GameObject;
        Collider[] stationColliders = GameObject.Find("Space_Station").GetComponents<Collider>();
        foreach (Collider collider in this.tetherRope.GetComponentsInChildren<Collider>())  //Ignore colliders for less volatile rope
        {
            Physics.IgnoreCollision(collider, player.GetComponent<Collider>());
            Physics.IgnoreCollision(collider, this.gameObject.GetComponent<Collider>());
            Physics.IgnoreCollision(collider, stationColliders[0]);
            Physics.IgnoreCollision(collider, stationColliders[1]);
            Physics.IgnoreCollision(collider, stationColliders[2]);
        }

        this.tetherRope.transform.parent = this.transform;
        this.tetherRope.transform.localPosition = Vector3.zero;
        CharacterJoint firstJoint = this.transform.gameObject.AddComponent<CharacterJoint>();
        firstJoint.autoConfigureConnectedAnchor = false;
        firstJoint.connectedBody = this.tetherRope.transform.GetChild(0).GetComponent<Rigidbody>();
        firstJoint.connectedAnchor = new Vector3(0.0f, 0.0f, 0.0f);
        firstJoint.anchor = new Vector3(0.0f, 1.0f, 0.0f);
        foreach (Rigidbody body in this.tetherRope.GetComponentsInChildren<Rigidbody>())
        {
            body.useGravity = false;
            body.mass = ropeMass;
        }
        
        ropeEnd = GameObject.Find(ropeEndName);
    }

    private void initCollider()
    {
       this.sphereCollider = this.gameObject.AddComponent<SphereCollider>();
    }

    private void initRigidBody()
    {
        this.rbody = this.gameObject.AddComponent<Rigidbody>();
        this.rbody.useGravity = false; //Ignore gravity in weightlessness
        this.rbody.angularDrag = 0;
        this.rbody.drag = rBodyDrag;
    }

    public void attach(bool isTug = false)
    {
        if (this.attached || this.attachedToStation) return;
        this.attached = true;

        if (!isTug)
        {
            this.transform.position += player.transform.position - ropeEnd.transform.position;
            GameObject.Find("IndicatorArrow").GetComponent<IndicatorArrowScript>().setIndicatorTarget(IndicatorTarget.IndicatorSphere);
            GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.isAttached, true);
            if (!GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().getConditionStatus(StoryConditionValues.DrifterDead))
            {
                root = new StorySequence();
                root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_ThankGod", this.gameObject, 0, false, "Thank god, I wasn't sure you would make it."));
                root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_WhatDoYouMean", this.gameObject, 0, false, "What do you mean? I mean.. I don't hate you that much.."));
                root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_HoldOffOnTheJokes", this.gameObject, 0, false, "this isn't over yet¦ maybe hold off on the jokes until we've landed."));
                root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_FairEnough", this.gameObject, 0, false, "Fair enough."));
                root.addChild(new StoryDialogue("Audio/Dialogue/Scene02_NowLetsGet", this.gameObject, 0, false, "Now, let's get out of here."));
                attachDialogueTreeStatus = StoryStatus.STATUS_Running;
                GameObject.Find("OxygenBars").GetComponent<OxygenBarScript>().stopDialogueTree();
            }
        }
        CharacterJoint playerJoint = player.AddComponent<CharacterJoint>();
        playerJoint.autoConfigureConnectedAnchor = false;
        playerJoint.connectedBody = ropeEnd.GetComponent<Rigidbody>();
        playerJoint.anchor = Vector3.zero;
        playerJoint.connectedAnchor = Vector3.zero;
        Rigidbody playerBody = player.GetComponent<Rigidbody>();
        playerBody.drag = 1.0f;
        playerBody.angularDrag = 1.0f;
        playerBody.mass = playerAttachMass;
        playerBody.maxDepenetrationVelocity = 1.0f;
        playerBody.useGravity = false;
    }

    public void attachToStation(Vector3 location)               //Like attach, but with space stations
    {
        Physics.IgnoreCollision(this.sphereCollider, player.GetComponent<Collider>());
        this.transform.position += location - ropeEnd.transform.position;
        this.rbody.velocity = Vector3.zero;
        this.rbody.angularVelocity = Vector3.zero;
        FixedJoint stationJoint = ropeEnd.AddComponent<FixedJoint>();
        stationJoint.autoConfigureConnectedAnchor = false;
        stationJoint.connectedBody = GameObject.Find("ConsoleLocation").GetComponent<Rigidbody>();
        stationJoint.anchor = Vector3.zero;
        stationJoint.connectedAnchor = Vector3.zero;
        this.attachedToStation = true;
        Physics.IgnoreCollision(this.sphereCollider, player.GetComponent<Collider>(), false);
    }

    public Vector3 getRopeEndPosition()
    {
        return ropeEnd.transform.position;
    }

    public void detachFromStation()
    {
        Destroy(ropeEnd.GetComponent<FixedJoint>());
        this.attachedToStation = false;
    }

    public bool isAttached()
    {
        return this.attached;
    }

    bool isCloseEnoughToAttach()
    {
        if (Vector3.Distance(ropeEnd.transform.position, player.transform.position) < attachRadius) return true;

        return false;
    }

    public void detach()
    {
        if (!this.attached) return;
        this.attached = false;
        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.isDetached, true);
        Destroy(player.GetComponent<CharacterJoint>());
    }

    private void initVelocity()
    {

    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            if(isCloseEnoughToAttach())
            {
                if (this.attached && GameObject.Find("InitialScene").GetComponent<InitialScene>().isInitialSceneDone()) this.detach();
                else if(!this.attached) this.attach();
                else GameObject.Find("InitialScene").GetComponent<InitialScene>().CheckForConsoleAttach(ropeEnd.transform.position,
                     GameObject.Find("ConsoleLocation").transform.position, stationAttachRadius);
            }
        }
        if (root != null && attachDialogueTreeStatus == StoryStatus.STATUS_Running)
            attachDialogueTreeStatus = root.Tick();
        if (attachDialogueTreeStatus == StoryStatus.STATUS_Success)
            stopAttachedDialogueTree();
    }

    public void stopAttachedDialogueTree()
    {
        if(root != null)
            (root as StorySequence).StopPropogation();
        root = null;
        attachDialogueTreeStatus = StoryStatus.STATUS_None;
    }
}
