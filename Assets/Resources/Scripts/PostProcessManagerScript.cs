using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class PostProcessManagerScript : MonoBehaviour {

    private const float FisheyeStrengthX = 0.18f; //Strength of the fisheye effect
    private const float FisheyeStrengthY = 0.0f; //Strength of the fisheye effect
                                                  
    private Fisheye fisheye; //Post process components to keep track of

	void Start () {
        this.fisheye = Camera.main.gameObject.AddComponent<Fisheye>();
        this.AddFisheye();
        fisheye.fishEyeShader = Shader.Find("Hidden/FisheyeShader");
    }

    public void AddFisheye() //Adds Fisheye effect to camera
    {
        this.fisheye.strengthX = FisheyeStrengthX;
        this.fisheye.strengthY = FisheyeStrengthY;
    }

    public void RemoveFisheye() //Removes Fisheye effect from camera
    {
        this.fisheye.strengthX = 0;
        this.fisheye.strengthY = 0;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
