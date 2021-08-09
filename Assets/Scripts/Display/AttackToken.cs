using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackToken : MonoBehaviour
{
    //note current implmentation doesn't work if both players have token
    public List<Sprite> tokenImages;

    public void ShowNoToken()
    {
        GetComponent<Image>().sprite = tokenImages[0];
    }
    public void ShowEmptyToken()
    {
        GetComponent<Image>().sprite = tokenImages[1];
    }
    public void ShowAttackToken()
    {
        GetComponent<Image>().sprite = tokenImages[2];
    }
}
