using UnityEngine;
using System.Collections;

public class DebrisGeneratorScript : MonoBehaviour
{
    private ArrayList debris; //Holds all debris
    private bool fast; //Trigger event where debris is suddenly much faster
    void Start()
    {
    }


    public void createDebris(bool isFast)
    {
        if (this.debris == null) this.debris = new ArrayList();
        this.debris.Clear();
        foreach (Transform child in this.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        this.fast = isFast;
        int debrisCount = 0;
        if (this.fast) debrisCount = DebrisValues.fastDebrisCountMax;
        else debrisCount = DebrisValues.debrisCountMax;
        for(int i = 0; i < debrisCount; i++)
        {
            string meshLocation = DebrisValues.debrisMeshes[Random.Range(0, DebrisValues.debrisMeshes.Length)];
            GameObject debrisObject = GameObject.Instantiate(Resources.Load(meshLocation, typeof(GameObject))) as GameObject;
            debrisObject.name = "Debris" + i.ToString();
            debrisObject.AddComponent<DebrisScript>();
            debrisObject.transform.parent = this.transform;
            this.debris.Add(debrisObject);
        }  
    }

    public bool isFast()
    {
        return this.fast;
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
