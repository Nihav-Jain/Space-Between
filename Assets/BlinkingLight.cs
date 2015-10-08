using UnityEngine;
using System.Collections;

public class BlinkingLight : MonoBehaviour {


    public float timer = 0;

    Light linkedLight;
	// Use this for initialization
	void Start () {

        linkedLight = GetComponent<Light>();

        linkedLight.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {

        blinkLight();

    }

    private void blinkLight()
    {
        if(timer < 6.0f)
        {
            timer += Time.deltaTime;
        }

        if(timer > 3.0f)
        {
            linkedLight.enabled = true;
        }
        if (timer > 6.0f)
        {
            linkedLight.enabled = false;
            timer = 0;
        }
    }
}

