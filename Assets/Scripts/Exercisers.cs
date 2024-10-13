using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Exercisers : MonoBehaviour
{
    private string direction;
    private float time;
    private bool running = false;

    private GameObject newCard;
    public GameObject cardArea;
    public GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            if (time < 0.25)
            {
                if (direction == "up")
                {
                    newCard.transform.position += 2400 * Vector3.up * Time.deltaTime;
                }
                if (direction == "down")
                {
                    newCard.transform.position -= 1600 * Vector3.up * Time.deltaTime;
                }
                time += Time.deltaTime;
            }
            else
            {
                running = false;
                Destroy(newCard);
            }
        }
    }

    public void Init(string dir)
    {
        newCard = GameObject.Instantiate(cardPrefab, cardArea.transform);
        time = 0;
        direction = dir;
        running = true;
    }
}
