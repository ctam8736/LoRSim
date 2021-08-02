using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackToken : MonoBehaviour
{
    //note current implmentation doesn't work if both players have token
    public List<Sprite> tokenImages;

    public void ShowNoToken(int player)
    {
        UpdatePosition(player);
        GetComponent<Image>().sprite = tokenImages[0];
    }
    public void ShowEmptyToken(int player)
    {
        UpdatePosition(player);
        GetComponent<Image>().sprite = tokenImages[1];
    }
    public void ShowAttackToken(int player)
    {
        UpdatePosition(player);
        GetComponent<Image>().sprite = tokenImages[2];
    }

    void UpdatePosition(int player)
    {
        if (player == 1)
        {
            transform.localPosition = new Vector3(286f, -116.8f, 0f);
        }
        else
        {
            transform.localPosition = new Vector3(286f, 73f, 0f);
        }
    }
}
