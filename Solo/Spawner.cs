using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Spawner : MonoBehaviourPunCallbacks
{
    
    public Block[] blocks;
    int blockNumber;
    Vector2 spawnPos, spawnScale;
    public GameObject nextBox;
    Block block;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = nextBox.GetComponent<RectTransform>().position;
        spawnPos += new Vector2(0, -0.07f);
        spawnScale = new Vector2(0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Block spawnBlock()
    {
        blockNumber = Random.Range(0, blocks.Length);
        //blockNumber = 7;
        block = Instantiate(blocks[blockNumber], spawnPos, Quaternion.identity);
        block.transform.localScale = spawnScale;
        return block;
    }


}
