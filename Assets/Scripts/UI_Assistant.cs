using UnityEngine;
using UnityEngine.UI;

public class UI_Assistant : MonoBehaviour
{
    private Text messageText;
    private GameObject message;    

    private void Awake()
    {        
        message = GameObject.Find("Message");
        messageText = transform.Find("Message").Find("Text").GetComponent<Text>();
    }

    private void Start()
    {
        message.SetActive(false);
    }

    public void Trigger()
    {
        if (message.activeSelf)
            message.SetActive(false);
        else
            message.SetActive(true);        
    }

    public void SetMessage(string msgText)
    {       
        messageText.text = msgText;
        message.SetActive(true);
    }

    public void DeactivateMessage()
    {
        messageText.text = "";
        message.SetActive(false);
    }
}
