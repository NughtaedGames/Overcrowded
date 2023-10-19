using UnityEngine;

public class MiniMapSystem : MonoBehaviour
{
    public GameObject parent;
    public GameObject[] mapcollider;

    void Start()
    {
        mapcollider = new GameObject[12];
        for (int i = 0; i < 12; i++)
        {
            mapcollider[i] = parent.transform.Find("Map_Player").transform.GetChild(i).gameObject;
        }
    }

    void OnTriggerEnter(Collider target)
    {
        Debug.LogError("collision");
        if (target.tag == "Map_Collider_Market")
        {
            Debug.LogError("market");
            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[0].active == false)
            {
                Debug.LogError("setmarketactive");
                mapcollider[0].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_belowMarket")
        {
            
            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[1].active == false)
            {
                mapcollider[1].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_roadLeft")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[2].active == false)
            {
                mapcollider[2].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_roadRight")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[3].active == false)
            {
                mapcollider[3].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_belowTavern")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[4].active == false)
            {
                mapcollider[4].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_Church")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[5].active == false)
            {
                mapcollider[5].SetActive(true);
            }
        }

        else if (target.tag == "Map_Collider_Tavern")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[6].active == false)
            {
                mapcollider[6].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_aboveChurch")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[7].active == false)
            {
                mapcollider[7].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_Castle")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[8].active == false)
            {
                mapcollider[8].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_backAlley")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[9].active == false)
            {
                mapcollider[9].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_deadEnd_left")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[10].active == false)
            {
                mapcollider[10].SetActive(true);
            }
        }
        else if (target.tag == "Map_Collider_deadEnd_right")
        {

            for (int i = 0; i < mapcollider.Length; i++)
            {
                mapcollider[i].SetActive(false);
            }
            if (mapcollider[11].active == false)
            {
                mapcollider[11].SetActive(true);
            }
        }
    }
}
