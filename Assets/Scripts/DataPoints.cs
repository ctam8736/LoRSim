using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoints : MonoBehaviour
{

    public float dataInterval = 0f;
    public GameObject dataPrefab;
    private float timer = .2f;

    private float startValue = -500f;
    // Start is called before the first frame update

    // Update is called once per frame

    /**
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > dataInterval)
        {
            timer = 0f;
            for (int i = 0; i < 50; i++)
            {
                float xdat = Random.Range(-500f, 500f);
                float zdat = Random.Range(-500f, 500f);
                Instantiate(dataPrefab, new Vector3(xdat, (xdat * zdat) / 500, zdat), Quaternion.identity);
            }
        }
    }
    **/

    public void createDataPoint(float value)
    {
        Instantiate(dataPrefab, new Vector3(startValue, value * 5f, 0f), Quaternion.identity);
        startValue += 1;
    }

    public void createDataPoint(float value1, float value2, float value3)
    {
        Instantiate(dataPrefab, new Vector3(value1, value2 * 5f, value3), Quaternion.identity);
    }
}
