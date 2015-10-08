using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Transparency : MonoBehaviour {

    public float alpha;

    Image image;
	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();

        Color c = image.color;
        c.a = alpha;
        image.color = c;

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
