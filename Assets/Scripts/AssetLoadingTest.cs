using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoadingTest : MonoBehaviour
{

    public List<Sprite> sprites;

    // Start is called before the first frame update
    void Start()
    {
        ImportSpritesFromFolder("CardImages/Set 1/Demacia");
        ImportSpritesFromFolder("CardImages/Set 1/PnZ");
    }

    // Update is called once per frame
    void ImportSpritesFromFolder(string path)
    {
        Object[] importedObjects = Resources.LoadAll(path, typeof(Sprite));
        foreach (Object thing in importedObjects)
        {
            sprites.Add((Sprite)thing);
        }
    }
}
