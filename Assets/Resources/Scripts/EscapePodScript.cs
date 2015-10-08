using UnityEngine;
using System.Collections;
using Fiea.Rpp.Storytelling;

public class EscapePodScript : MonoBehaviour {

    // Use this for initialization
    public static Vector3 escapePodColliderOffset = new Vector3(0.0f, 0.0f, 2.0f);
    CapsuleCollider escapeCollider;
    public static float escapeColliderRadius = 3.0f;
    public static float escapeCollliderHeight = 8.0f;
    public static float indicatorSphereRadius = 1.0f;       //For the indicator sphere on the escape pod, since it's a big structure
    GameObject player;
	void Start () {
      
	}

    public void initEscapePod()
    {
        player = GameObject.Find("CustomCharacterController");
        this.gameObject.transform.GetChild(0).localPosition = Vector3.zero;
        GameObject indicatorSphere = GameObject.Find("IndicatorSphere");
        indicatorSphere.transform.parent = this.transform;
        escapeCollider = this.gameObject.AddComponent<CapsuleCollider>();
        escapeCollider.radius = escapeColliderRadius;
        escapeCollider.height = escapeCollliderHeight;
        escapeCollider.direction = 0;   //X-axis
        escapeCollider.isTrigger = true;
        indicatorSphere.transform.localPosition = escapePodColliderOffset;
        indicatorSphere.GetComponent<MeshRenderer>().enabled = true;
        indicatorSphere.transform.localScale = new Vector3(indicatorSphereRadius, indicatorSphereRadius, indicatorSphereRadius);

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider != player.GetComponent<Collider>()) return;

        exitGame();
    }

    void exitGame()
    {
        GameObject.Find("StoryConditionManager").GetComponent<StoryConditionManager>().updateCondition(StoryConditionValues.EscapePodReached, true);
        GameObject.Find("EndStatistics").GetComponent<EndStatisticsScript>().endGame();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
