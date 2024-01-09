using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Spawner_Online : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] NextBlock_Sprites;
    private Block block;
    private string[] blocks_string = new string[7];
    private int nextBlockNum;

    // Start is called before the first frame update
    void Start()
    {
        blocks_string[0] = "BlockI";
        blocks_string[1] = "BlockO";
        blocks_string[2] = "BlockZ";
        blocks_string[3] = "BlockL";
        blocks_string[4] = "BlockS";
        blocks_string[5] = "BlockT";
        blocks_string[6] = "BlockJ";

        nextBlockNum = Random.Range(0, blocks_string.Length);
    }

    public void spawnBlock(Vector2 spawnPos)
    {
        PhotonNetwork.Instantiate(blocks_string[nextBlockNum], spawnPos, Quaternion.identity);
        nextBlockNum = Random.Range(0, blocks_string.Length);
        photonView.RPC(nameof(UpdateNextBox), RpcTarget.All, nextBlockNum);
    }

    [PunRPC]
    public void UpdateNextBox(int spriteNum)
    {
        for (int i = 0; i < NextBlock_Sprites.Length; i++)
        {
            NextBlock_Sprites[i].SetActive(false);
        }

        NextBlock_Sprites[spriteNum].SetActive(true);
    }
}
