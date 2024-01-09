using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OwnershipTransfer : MonoBehaviourPun
{
    // Start is called before the first frame update
    public void RequestOwner()
    {
        photonView.RequestOwnership();
    }
}
