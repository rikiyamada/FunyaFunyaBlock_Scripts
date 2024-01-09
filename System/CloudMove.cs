using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMove : MonoBehaviour
{
    public GameObject Cloud1, Cloud2, Cloud3, Cloud4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Cloud1.transform.Translate(0.003f, 0, 0);
        Cloud2.transform.Translate(0.0058f, 0, 0);
        Cloud3.transform.Translate(0.0041f, 0, 0);
        Cloud4.transform.Translate(0.002f, 0, 0);
        resetPos();
    }

    private void resetPos()
    {
        if(Cloud1.transform.position.x > 4)
        {
            Cloud1.transform.position = new Vector2(-4, 4.2f);
        }

        if (Cloud2.transform.position.x > 4)
        {
            Cloud2.transform.position = new Vector2(-4, 3.7f);
        }

        if (Cloud3.transform.position.x > 4)
        {
            Cloud3.transform.position = new Vector2(-4, 3.3f);
        }

        if (Cloud4.transform.position.x > 4)
        {
            Cloud4.transform.position = new Vector2(-4, 2.7f);
        }
    }

    
}
