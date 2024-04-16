using System.Collections;
using System.Collections.Generic;
using TEngine;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in Utility.Assembly.GetTypes())
        {
            if (item.FullName.Contains("MyColorTint"))
            {
                Debug.Log(item.FullName);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
