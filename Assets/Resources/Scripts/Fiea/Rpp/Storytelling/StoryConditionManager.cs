using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fiea.Rpp.Storytelling;

namespace Fiea.Rpp.Storytelling
{
    public class StoryConditionManager : MonoBehaviour
    {
        Dictionary<string, bool> currentConditions;      //Holds all available conditions

        // Use this for initialization
        void Start()
        {
            currentConditions = new Dictionary<string, bool>();
            //populateConditions();
        }

        void populateConditions()           //Populate dictionary with values
        {
            addCondition(StoryConditionValues.ConsoleReached);
            addCondition(StoryConditionValues.ActionPressed);
            addCondition(StoryConditionValues.ReachedDisasterPoint);
        }

        public void updateCondition(string name, bool condition)
        {
            //(currentConditions[name] as StoryCondition).setCondition(condition);
            if (!currentConditions.ContainsKey(name))
                currentConditions.Add(name, condition);
            else
                currentConditions[name] = condition;
        }

        public bool getConditionStatus(string name)         //Gets status of condition by name
        {
            if (currentConditions == null)
                currentConditions = new Dictionary<string, bool>();
            if (!currentConditions.ContainsKey(name))
                currentConditions.Add(name, false);
            return currentConditions[name];
        }

        public void addCondition(string name)
        {
            if (currentConditions == null)
                currentConditions = new Dictionary<string, bool>();
            if(!currentConditions.ContainsKey(name))
                currentConditions.Add(name, false);
            Debug.Log("condition added " + name + " = " + currentConditions[name]);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}