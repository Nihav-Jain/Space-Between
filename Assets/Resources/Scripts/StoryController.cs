using UnityEngine;
using System.IO;
using System.Collections;
using Fiea.Rpp.Storytelling;
//using SimpleJSON;

public class StoryController : MonoBehaviour {

    private StoryTree tree;
    private StoryStatus curGameStatus;
    // Use this for initialization
	void Start () {

        //JSONNode node = JSONNode.LoadFromFile("./Assets/Resources/Text/behaviortree.json");
        tree = new StoryTree(this.gameObject);
        tree.growTree();
        curGameStatus = StoryStatus.STATUS_None;
	}
	
	// Update is called once per frame
	void Update () {
        if(curGameStatus != StoryStatus.STATUS_Success || curGameStatus != StoryStatus.STATUS_Failure)
            curGameStatus = tree.Update();
        //Debug.Log(curGameStatus);
	}
}
