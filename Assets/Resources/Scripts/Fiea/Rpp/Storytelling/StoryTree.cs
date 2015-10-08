using UnityEngine;
using System.Collections;
using System;

namespace Fiea.Rpp.Storytelling
{
    public class StoryTree
    {
        private StoryBehavior root = null;
        private GameObject gameObject;

        public StoryTree (GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public void growTree()
        {
            /*
            root = new StorySequence();
            
            root.addChild(new StoryScene(Scenes.SceneryStare, this.gameObject));
            root.addChild(new StoryDialogue("Audio/Dialogue/Scene01_ArentYouSick", this.gameObject, 3, false, "Aren't you sick of looking at that thing yet? It looks exactly the same as the first day we got here! Come on, let's get back to work."));
            root.addChild(new StoryDialogue("Audio/Dialogue/Scene01_Hello", this.gameObject, 7, false, "Helloooo! Did you forget what we came out here to do? I'm only literally hanging by a string. Fine, let me remind you. Take me over to that console."));

            StorySelector consoleReachedSelector = new StorySelector();
            StorySequence consoleReachedIfSeq = new StorySequence();
            StoryCondition consoleReachedconditon = new StoryCondition(StoryConditionValues.ConsoleReached);
            StorySequence consoleReachedSuccess = new StorySequence();
            StoryDialogue finallyDialogue = new StoryDialogue("Audio/Dialogue/Scene01_Finally", this.gameObject, 0.5f, false, "Finally! For a minute I thought you had reverted back to the first day of training and all was doomed. Now hook me up.");

            StorySequence consoleReachedElseSeq = new StorySequence();
            StoryDialogue humming = new StoryDialogue("Audio/Dialogue/Scene01_Humming", this.gameObject, 10, true);

            consoleReachedSuccess.addChild(finallyDialogue);
            consoleReachedIfSeq.addChild(consoleReachedconditon);
            consoleReachedIfSeq.addChild(consoleReachedSuccess);

            consoleReachedElseSeq.addChild(humming);

            consoleReachedSelector.addChild(consoleReachedIfSeq);
            consoleReachedSelector.addChild(consoleReachedElseSeq);

            root.addChild(consoleReachedSelector);

            StorySelector xPressedSelector = new StorySelector();
            StorySequence xPressedIfSeq = new StorySequence();
            StoryCondition xPressedCondition = new StoryCondition(StoryConditionValues.ActionPressed);
            StorySequence xPressedSuccess = new StorySequence();
            StoryDialogue doACircleDialogue = new StoryDialogue("Audio/Dialogue/Scene01_DoACircle", this.gameObject, 0, false, "Thanks. Do a quick circle around the station and make sure we’re in good shape");

            xPressedSuccess.addChild(doACircleDialogue);
            xPressedIfSeq.addChild(xPressedCondition);
            xPressedIfSeq.addChild(xPressedSuccess);
            xPressedSelector.addChild(xPressedIfSeq);

            consoleReachedSuccess.addChild(xPressedSelector);

            StorySelector reachedDisasterPointSelector = new StorySelector();
            StorySequence reachedDisasterPointIfSeq = new StorySequence();
            StoryCondition reachedDisasterPointCondition = new StoryCondition(StoryConditionValues.ReachedDisasterPoint);
            StorySequence reachedDisasterPointSuccess = new StorySequence();
            StoryScene disasterScene = new StoryScene(Scenes.DebrisField, this.gameObject);
            StoryDialogue whatsThatDialogue = new StoryDialogue("Audio/Dialogue/Scene02_WhatsThat", this.gameObject, 0, false, "What's that?");

            reachedDisasterPointSuccess.addChild(disasterScene);
            reachedDisasterPointSuccess.addChild(whatsThatDialogue);

            reachedDisasterPointIfSeq.addChild(reachedDisasterPointCondition);
            reachedDisasterPointIfSeq.addChild(reachedDisasterPointSuccess);
            reachedDisasterPointSelector.addChild(reachedDisasterPointIfSeq);

            xPressedSuccess.addChild(reachedDisasterPointSelector);

            StorySelector whatsThatSelector = new StorySelector();
            StorySequence whatsThatIfSeq = new StorySequence();
            StoryCondition whatsThatAnswered = new StoryCondition(StoryConditionValues.WhatsThat_Answered, true);
            StorySequence whatsThatSuccess = new StorySequence();

            StoryDialogueOptions whatsThatOptions = new StoryDialogueOptions(new string[2] { "I don't know", "I'm pretty sure thats bad" }, new string[2]{StoryConditionValues.WhatsThat_Option1, StoryConditionValues.WhatsThat_Option2}, this.gameObject);

            StorySelector whatsThatOption1Selector = new StorySelector();
            StorySequence whatsThatOption1IfSeq = new StorySequence(true);
            StoryCondition whatsThatOption1Condition = new StoryCondition(StoryConditionValues.WhatsThat_Option1);
            StoryDialogue superHelpful = new StoryDialogue("Audio/Dialogue/Scene02_SuperHelpful", this.gameObject, 0, false, "Super helpful");

            StorySelector whatsThatOption2Selector = new StorySelector();
            StorySequence whatsThatOption2IfSel = new StorySequence(true);
            StoryCondition whatsThatOption2Condition = new StoryCondition(StoryConditionValues.WhatsThat_Option2);
            StoryDialogue captainObvious = new StoryDialogue("Audio/Dialogue/Scene02_CaptainObvious", this.gameObject, 0, false, "Thanks, Captain Obvious.");

            StoryDialogue okSeriously = new StoryDialogue("Audio/Dialogue/Scene02_GetBackIntoStation", this.gameObject, 1f, false, "Seriously, I think we should get back into the station. Get over here");
            StoryScene stationTurn = new StoryScene(Scenes.StationTurn, this.gameObject);

            whatsThatOption2IfSel.addChild(whatsThatOption2Condition);
            whatsThatOption2IfSel.addChild(captainObvious);
            whatsThatOption2Selector.addChild(whatsThatOption2IfSel);

            whatsThatOption1IfSeq.addChild(whatsThatOption1Condition);
            whatsThatOption1IfSeq.addChild(superHelpful);
            whatsThatOption1Selector.addChild(whatsThatOption1IfSeq);

            whatsThatSuccess.addChild(whatsThatOption1Selector);
            whatsThatSuccess.addChild(whatsThatOption2Selector);
            whatsThatSuccess.addChild(okSeriously);
            whatsThatSuccess.addChild(stationTurn);

            whatsThatIfSeq.addChild(whatsThatAnswered);
            whatsThatIfSeq.addChild(whatsThatSuccess);

            whatsThatSelector.addChild(whatsThatIfSeq);
            whatsThatSelector.addChild(whatsThatOptions);

            reachedDisasterPointSuccess.addChild(whatsThatSelector);
            */
            TextAsset jsonAsset = (TextAsset)Resources.Load("Text/DialogueTrees/DialogueTree_ActI");
            JSONObject json = JSONObject.Create(jsonAsset.text);
            //Debug.Log(json);
            root = treeDFS(json);
            //StoryBehavior treeRoot = treeDFS(json);
        }

        private StoryBehavior treeDFS(JSONObject json)
        {
            StoryBehavior root = null;
            string curType = "";
            json.GetField(ref curType, "type");
            //Debug.Log(curType);
            switch (curType)
            {
                case "SEQUENCE":
                    bool isResponseConditionSeq = false;
                    bool isSelectorElse = false;
                    json.GetField(ref isResponseConditionSeq, "isResponseOptionSequence");
                    json.GetField(ref isSelectorElse, "isSelectorElse");
                    root = new StorySequence(isResponseConditionSeq, isSelectorElse);
                    break;
                case "SELECTOR":
                    root = new StorySelector();
                    break;
                case "SCENE":
                    string sceneValue = "";
                    json.GetField(ref sceneValue, "sceneName");
                    Scenes curSceneName = (Scenes)Enum.Parse(typeof(Scenes), sceneValue);
                    root = new StoryScene(curSceneName, this.gameObject);
                    break;
                case "DIALOGUE":
                    string audioFile = "";
                    string startDelayStr = "";
                    bool looping = false;
                    string subtitles = "";
                    string character = "";
                    json.GetField(ref audioFile, "audioFile");
                    json.GetField(ref startDelayStr, "startDelay");
                    json.GetField(ref looping, "looping");
                    json.GetField(ref subtitles, "subtitles");
                    json.GetField(ref character, "character");
                    root = new StoryDialogue(audioFile, this.gameObject, float.Parse(startDelayStr), looping, subtitles, character);
                    break;
                case "CONDITION":
                    string conditionName = "";
                    json.GetField(ref conditionName, "condition");
                    bool isDialogueCondition = false;
                    bool isAgitated = false;
                    json.GetField(ref isDialogueCondition, "isDialogueCondition");
                    json.GetField(ref isAgitated, "isAgitated");
                    string defaultCondition = "";
                    if (isDialogueCondition)
                        json.GetField(ref defaultCondition, "defaultCondition");
                    root = new StoryCondition(conditionName, isDialogueCondition, isAgitated, defaultCondition);
                    break;
                case "OPTIONS":
                    JSONObject options = json.GetField("options");
                    string[] resp = new string[options.list.Count];
                    int i = 0;
                    foreach(JSONObject opt in options.list)
                    {
                        resp[i++] = opt.str;
                    }
                    JSONObject successConds = json.GetField("successConditions");
                    string[] successConditions = new string[options.list.Count];
                    i = 0;
                    foreach (JSONObject condValue in successConds.list)
                    {
                        successConditions[i++] = condValue.str;
                    }

                    root = new StoryDialogueOptions(resp, successConditions, this.gameObject);
                    break;
            }
            //Debug.Log(root);
            JSONObject children = json.GetField("children");
            if(children != null)
            {
                foreach (JSONObject child in children.list)
                {
                    root.addChild(treeDFS(child));
                }
            }

            return root;
        }

        public StoryStatus Update()
        {
            if (root == null)
                return StoryStatus.STATUS_None;
            return root.Tick();
        }
    }
}
