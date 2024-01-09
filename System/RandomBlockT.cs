using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBlockT : MonoBehaviour
{
    void Start()
    {
        float ran = Random.Range(-1.04f, 1.04f);
        this.transform.position = new Vector2(ran, this.transform.position.y);
    }
}
