using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClearText : MonoBehaviour
{
    private TMP_Text myText;

    private float currentTime = 0f;
    private float clearTime = 2f;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > clearTime)
        {
            ClearMyText();
        }
    }
    void ClearMyText()
    {
        myText.text = "";
    }
    
}
