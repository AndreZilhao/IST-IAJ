using UnityEngine;
using UnityEngine.UI;

public class DynamicUIManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        var controlers = GameObject.FindObjectsOfType<CharacterDynamicMovementController>();


        var textObj = GameObject.Find("InstructionsText");
        if (textObj != null)
        {
            string text = "Instructions\n\n";


            foreach (var control in controlers)
            {
                text += control.name + "\n";
                text += control.stopKey + " - Stationary\n";
                text += control.seekKey + " - Seek\n";
                text += control.fleeKey + " - Flee\n";
                text += control.arriveKey + " - Arrive\n";
                text += control.wanderKey + " - Wander\n";
                text += "\n";
            }

            textObj.GetComponent<Text>().text = text;
        }
    }
}
