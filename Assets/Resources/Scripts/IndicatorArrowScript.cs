using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum IndicatorTarget { Drifter, IndicatorSphere };
public class IndicatorArrowScript : MonoBehaviour {

    public static Vector2 indicatorArrowScale = new Vector2(0.05f, 0.1f); //How big is the arrow?
    public static Vector2 indicatorOffset = new Vector2(340.0f, -290.0f); //Where to put the arrow?
    public IndicatorTarget currentTarget;
    GameObject targetObject;
    void Start()
    {
        this.setIndicatorTarget(IndicatorTarget.Drifter);
        this.gameObject.layer = LayerMask.NameToLayer("UI");
    }

    public void setIndicatorTarget(IndicatorTarget target)
    {
        this.currentTarget = target;
        switch(target)
        {
            case IndicatorTarget.Drifter:
                targetObject = GameObject.Find("Drifter").transform.GetChild(0).gameObject;
                break;
            case IndicatorTarget.IndicatorSphere:
                targetObject = GameObject.Find("IndicatorSphere");
                break;
            default:
                break;
        }
    }

    public IndicatorTarget getIndicatorTarget()
    {
        return currentTarget;
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform rect = this.transform.GetChild(0).GetComponent<RectTransform>();
        if (targetObject.GetComponent<Renderer>().isVisible)
        {
            rect.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        Vector3 screenLocation = Camera.main.WorldToScreenPoint(targetObject.transform.position);
        if (screenLocation.z < 0) screenLocation *= -1; //things are flipped when behind player


        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2;

        //Find angle from center of screen to position
        screenLocation -= screenCenter;

        float angle = Mathf.Atan2(screenLocation.y, screenLocation.x);
        angle -= 90.0f * Mathf.Deg2Rad;
      
        rect.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        float dist = Vector3.Distance(targetObject.transform.position, Camera.main.transform.position);
        GameObject.Find("DistanceNumbers").GetComponent<Text>().text = dist.ToString();
    }
}
