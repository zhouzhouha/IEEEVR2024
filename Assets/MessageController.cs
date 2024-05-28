using System.Collections;
using UnityEngine;
using TMPro;

public class MessageController : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float displayDuration = 300f; // Duration to display the message in seconds
    private GameObject parentCanvas;

    void Start()
    {
        // Initially hide the message text
        parentCanvas = messageText.transform.parent.gameObject;
        // Log the name of the parent canvas for debugging purposes
        Debug.Log("Parent Canvas GameObject: " + parentCanvas.name);
    }

    public void ShowMessage(string message)
    {
        StartCoroutine(DisplayMessageRoutine(message));
    }

    private IEnumerator DisplayMessageRoutine(string message)
    {
        // Set the message and show the text
        messageText.text = message;
        parentCanvas.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(displayDuration);

        // Hide the message text
        parentCanvas.SetActive(false);
    }
}
