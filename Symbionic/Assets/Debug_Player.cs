using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_Player : MonoBehaviour
{

    public Transform Bionic_side;
    public Transform Organic_side;

    public GameObject boCanvas;

    public GameObject B_Panel;
    public GameObject O_Panel;

    private int indexB;
    private int indexO;

    private int maxN;

    private List<GameObject> OrderB;
    private List<GameObject> OrderO;

    private bool canvasA = true;
    // Start is called before the first frame update
    void Start()
    {
        OrderB = new List<GameObject>();
        OrderO = new List<GameObject>();
        indexB = 0;
        indexO = 0;

        maxN = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            canvasA = !canvasA;
            boCanvas.SetActive(canvasA);
        }
    }

    public void Bionic(string x)
    {
        if (canvasA)
        {
            GameObject temp = (GameObject)Instantiate(B_Panel);
            temp.gameObject.name = "Bio" + indexB;
            Text tempText = temp.transform.GetChild(0).gameObject.GetComponent<Text>();
            tempText.text = x;
            temp.transform.SetParent(Bionic_side);
            RectTransform tempT = temp.GetComponent<RectTransform>();
            tempT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, tempT.rect.width);
            if (indexB >= maxN)
            {
                Destroy(OrderB[0]);
                OrderB.RemoveAt(0);
                for (int i = 0; i < OrderB.Count; i++)
                {
                    RectTransform rt = OrderB[i].gameObject.GetComponent<RectTransform>();

                    rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0 + (i * 117), rt.rect.height);

                }
                tempT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0 + ((maxN - 1) * 117), tempT.rect.height);
                OrderB.Add(temp);
            }
            else
            {
                tempT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0 + (indexB * 117), tempT.rect.height);
                OrderB.Add(temp);
            }
            indexB++;
        }
        //Debug.Log(temp.transform);
    }
    public void Organic(string x)
    {
        if (canvasA)
        {
            GameObject temp = (GameObject)Instantiate(O_Panel);
            temp.gameObject.name = "Org" + indexO;
            Text tempText = temp.transform.GetChild(0).gameObject.GetComponent<Text>();
            tempText.text = x;
            temp.transform.SetParent(Organic_side);
            RectTransform tempT = temp.GetComponent<RectTransform>();
            tempT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, tempT.rect.width);
            if (indexO >= maxN)
            {
                Destroy(OrderO[0]);
                OrderO.RemoveAt(0);
                for (int i = 0; i < OrderO.Count; i++)
                {
                    RectTransform rt = OrderO[i].gameObject.GetComponent<RectTransform>();

                    rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0 + (i * 117), rt.rect.height);

                }
                tempT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0 + ((maxN - 1) * 117), tempT.rect.height);
                OrderO.Add(temp);
            }
            else
            {
                tempT.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0 + (indexO * 117), tempT.rect.height);
                OrderO.Add(temp);
            }
            indexO++;
        }
    }
}
