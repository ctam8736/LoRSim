using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaGemDisplay : MonoBehaviour
{
    public List<Sprite> manaGemSprites;
    public List<Sprite> spellManaSprites;

    public void SetManaGemSprite(int manaGems)
    {
        GetComponent<Image>().sprite = manaGemSprites[manaGems];
    }

    public void SetSpellManaSprite(int spellMana)
    {
        GetComponent<Image>().sprite = spellManaSprites[spellMana];
    }
}
