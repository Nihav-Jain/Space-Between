using UnityEngine;
using System.Collections;


public class DebrisScript : MonoBehaviour
{
    // Use this for initialization
    private Vector3 direction; //Both comprise velocity of debris at any time
    private float speed;
    public static float debrisMass = 200.0f;
    public static float debrisDrag = 0.7f;
    private Rigidbody debrisBody;
    private float spinTurbulence; //How much the debris is spinning
    private System.DateTime respawnStartTime; //Timer for when to speed up the debris (in seconds)
    public float timeRemaining = 0; //Debug value for timeRemaining
    private float respawnTimeMax; //How long it takes for respawning (in seconds)
    private bool respawning; //Whenever the debris is respawning
    private bool fast; //Is the debris the fast kind?
    private Vector3 orbitVelocity; //The debris part's own orbit velocity
    void Start()
    {
        this.localizeChildPosition(gameObject.transform);
        this.fast = this.GetComponentInParent<DebrisGeneratorScript>().isFast(); //Is this debris superfast?
        this.spinTurbulence = Random.Range(DebrisValues.turbulenceMinMax.x, //Set how much debris is rotating
           DebrisValues.turbulenceMinMax.y);
        if (!this.fast) this.respawnTimeMax = DebrisValues.respawnTime;
        else this.respawnTimeMax = DebrisValues.fastRespawnTime;
        this.respawning = false;

        this.initPosition();
        this.initCollider();
        this.initVelocity();
        this.initRigidBody();

        float scaleSize = Random.Range(DebrisValues.sizeMinMax.x, DebrisValues.sizeMinMax.y); //Initialize size of debris
        this.gameObject.transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
    }

    void localizeChildPosition(Transform trans)            //Put all children's local position to zero (recursively)
    {
        if (trans.childCount <= 0) return;
        foreach (Transform childTrans in trans)
        {
            childTrans.localPosition = Vector3.zero;
            
            if (childTrans.gameObject.name != trans.name) this.localizeChildPosition(childTrans);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == GameObject.Find("CustomCharacterController").GetComponent<Collider>())
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().playJessCollision(Camera.main.transform.position);
        }
        else if(collision.collider == GameObject.Find("Drifter").GetComponent<Collider>())
        {
            GameObject.Find("SoundManager").GetComponent<SoundManager>().playNickCollision(Camera.main.transform.position);
        }           
    }

    void initPosition()
    {
        if(!this.fast) this.gameObject.transform.position = new Vector3(Random.Range(DebrisValues.debrisAreaXLimits.x, DebrisValues.debrisAreaXLimits.y),
            Random.Range(DebrisValues.debrisAreaYLimits.x, DebrisValues.debrisAreaYLimits.y), Random.Range(DebrisValues.debrisAreaZLimits.x, DebrisValues.debrisAreaZLimits.y));
        else setPositionAtBoundary(); //Fast debris starts at border
    }

    void setPositionAtBoundary() //Set position of object at start of boundary, instead of inbetween
    {
        this.gameObject.transform.position = new Vector3(DebrisValues.debrisAreaXLimits.y,
            Random.Range(DebrisValues.debrisAreaYLimits.x, DebrisValues.debrisAreaYLimits.y), Random.Range(DebrisValues.debrisAreaZLimits.x, DebrisValues.debrisAreaZLimits.y));
    }

    void initCollider()
    {
        if(!this.fast) this.gameObject.AddComponent<SphereCollider>();
    }

    void initVelocity()
    {
        this.direction = Random.insideUnitSphere;
        this.speed = Random.Range(DebrisValues.speedMinMax.x, DebrisValues.speedMinMax.y);
    }

    void initRigidBody()
    {
        this.debrisBody = this.gameObject.AddComponent<Rigidbody>();
        this.debrisBody.useGravity = false; //Ignore gravity in weightlessness
        this.debrisBody.angularVelocity = Random.insideUnitSphere * this.spinTurbulence; //Set it to continuously spinning
        this.debrisBody.angularDrag = 0;
        this.debrisBody.mass = debrisMass; //Set mass of debris to player's mass
        this.orbitVelocity = new Vector3(Random.Range(DebrisValues.orbitVelocity.x, DebrisValues.orbitVelocity.y), 0, 0);
        if (this.fast) this.orbitVelocity = DebrisValues.fastOrbitVelocity;
        this.debrisBody.velocity = (this.speed * this.direction) + orbitVelocity;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.x < DebrisValues.debrisAreaXLimits.x)
        {
            if(!this.respawning)
            {
                this.respawnStartTime = System.DateTime.Now; //Start timer
                this.respawning = true;
            }
            this.timeRemaining = (float)(System.DateTime.Now - this.respawnStartTime).TotalSeconds;
            if (this.timeRemaining > this.respawnTimeMax) //Test how long things pass in seconds
            {
                Vector3 velocityGain = this.orbitVelocity * DebrisValues.debrisSpeedGain; //Increase velocity each respawn
                //Amplify the speed if the amplification event is triggered
                if(!this.fast) this.debrisBody.velocity += velocityGain;

                this.respawnTimeMax *= DebrisValues.debrisRespawnTimeReductionRate; //Reduce timer to wait
                this.respawning = false;
                this.setPositionAtBoundary();
            }
        }   
    }
}
