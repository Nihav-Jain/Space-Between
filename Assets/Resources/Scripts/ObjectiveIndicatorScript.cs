using UnityEngine;
using System.Collections;

public class ObjectiveIndicatorScript : MonoBehaviour {

    GameObject indicatorSphere;
    public static float indicatorScale = 2.0f;
	// Use this for initialization
	void Start () {
        indicatorSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        indicatorSphere.GetComponent<MeshRenderer>().material = Resources.Load("Textures/GlowMaterial") as Material;
        indicatorSphere.name = "IndicatorSphere";
        indicatorSphere.transform.localScale = new Vector3(indicatorScale, indicatorScale, indicatorScale);
        indicatorSphere.GetComponent<SphereCollider>().isTrigger = true;
        indicatorSphere.GetComponent<MeshRenderer>().enabled = false;
	}

    public void setPosition(Vector3 location)
    {
        this.transform.position = location;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
