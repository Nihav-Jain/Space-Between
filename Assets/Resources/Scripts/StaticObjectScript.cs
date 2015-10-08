using UnityEngine;
using System.Collections;

public class StaticObjectScript : MonoBehaviour {

    private Vector3 direction = Vector3.zero; //Very small drift of object, if at all
    private float speed = 0;
    // Use this for initialization
    void Start () {
        this.initRigidbody();
    }

    public void init() //This ensures that subclasses can call Start on the base class
    {
        this.Start();
    }

    void initRigidbody()
    {
        Rigidbody rBody = this.gameObject.AddComponent<Rigidbody>();
        rBody.useGravity = false; //Ignore gravity in weightlessness
        rBody.velocity = (this.speed * this.direction);
    }

    public void initMesh(string meshPath, string texturePath)
    {
        Mesh objMesh = Resources.Load<Mesh>(meshPath);
        this.gameObject.AddComponent<MeshFilter>().mesh = objMesh;
        this.gameObject.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
        this.gameObject.AddComponent<MeshCollider>().sharedMesh = this.gameObject.GetComponent<MeshFilter>().mesh;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
