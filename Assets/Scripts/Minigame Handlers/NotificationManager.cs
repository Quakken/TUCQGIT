/*
 * Name: Quinn Farrell
 * Date: 1/9/2024
 * Desc: Manager class used to handle notification pop-ups & phone interaction
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    [System.Serializable]
    public struct Notification
    {
        [Tooltip("The name of the sender")]
        public string name;
        [Tooltip("The contents of the message")]
        public string contents;
        [Tooltip("The icon of the sender")]
        public Sprite icon;
        [Space]
        [Tooltip("The ID of the notification. Used to link to other notifications.")]
        public string ID;
        [Space]
        [Tooltip("Butter's reply to the notification")]
        public string reply;
        [Tooltip("The ID of the notification to send after the player replies")]
        public string afterReply;
        [Tooltip("The ID of the notification to send after the player fails to reply")]
        public string afterIgnore;
    }

    [Header("Notification Database")]
    [SerializeField] Notification[] notifications;

    // The current notification
    private string currentNoti;

    [Header("Notification Configuration")]
    [Tooltip("How long a notification stays on screen")]
    [SerializeField] float notiDuration = 7;
    [Tooltip("How long of a grace period the player gets between notifications")]
    [SerializeField] float notiSpacing = 3;

    [Header("Phone Configuration")]
    [Tooltip("The color that Butter's messages should be on the phone")]
    [SerializeField] Color butterTextColor;
    [Tooltip("The color that others' messages should be on the phone")]
    [SerializeField] Color otherTextColor;


    [Header("Assets")]
    [SerializeField] Animator phoneAnim;
    [SerializeField] GameObject phoneLayout;
    [SerializeField] GameObject textObj;
    [Space]
    [SerializeField] Animator notiAnim;
    [SerializeField] TextMeshProUGUI notiName;
    [SerializeField] TextMeshProUGUI notiContents;
    [SerializeField] Image notiIcon;

    private void Start()
    {
        currentNoti = notifications[0].ID;
        StartCoroutine(ShowNotification(currentNoti));
    }

    // Called when a notification is clicked
    public void OnNotiClicked()
    {
        // Hide the notification and stop the timer
        HideNotification();
        CancelInvoke();
        StopAllCoroutines();

        ShowPhone();
    }

    // Called when the player clicks the send button on the phone
    public void OnPhoneClicked()
    {
        // Create a text object in the phone
        CreateMessage("Butter", GetNotification(currentNoti).reply, butterTextColor);

        // Hide the phone
        phoneAnim.SetBool("Showing", false);

        // Set the current notification to the next replied notification
        currentNoti = GetNotification(currentNoti).afterReply;

        // Show the next notification after a delay
        CancelInvoke();
        StopAllCoroutines();
        StartCoroutine(ShowNotification(currentNoti, notiSpacing));
    }

    // Shows a notification on screen
    public IEnumerator ShowNotification(string id, float delay = 0)
    {
        // If the given ID is not valid, don't do anything
        if (!IsValidID(id))
            yield return null;

        yield return new WaitForSeconds(delay);

        currentNoti = id;
        Notification noti = GetNotification(id);

        // Show the notification and set all required fields
        notiAnim.SetBool("Showing", true);
        notiName.text = noti.name;
        notiContents.text = noti.contents;
        notiIcon.sprite = noti.icon;

        // Create a text object in the phone
        CreateMessage(GetNotification(currentNoti).name, GetNotification(currentNoti).contents, otherTextColor);

        // Hide the notification after a while
        CancelInvoke();
        StopAllCoroutines();
        Invoke("NotiIgnored", notiDuration);
    }

    // Called when a notification is ignored by the player
    private void NotiIgnored()
    {
        // Set the current notification to the next ignored notification
        currentNoti = GetNotification(currentNoti).afterIgnore;

        // Hide the notification
        HideNotification();

        // Show the next notification after a delay
        CancelInvoke();
        StopAllCoroutines();
        StartCoroutine(ShowNotification(currentNoti, notiSpacing));
    }

    // Hides the notification
    public void HideNotification()
    {
        notiAnim.SetBool("Showing", false);
    }

    // Shows the phone
    public void ShowPhone()
    {
        phoneAnim.SetBool("Showing", true);
    }

    // Creats a message bubble on the phone
    private void CreateMessage(string name, string contents, Color col)
    {
        GameObject message = Instantiate(textObj, phoneLayout.transform);
        message.GetComponent<Image>().color = col;
        foreach (TextMeshProUGUI text in message.GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (text.gameObject.name == "Name")
            {
                text.text = name;
            }
            if (text.gameObject.name == "Message Contents")
            {
                text.text = contents;
            }
        }
    }

    // Returns true if a notification exists with a given ID
    public bool IsValidID(string id)
    {
        foreach (Notification notification in notifications)
        {
            if (notification.ID == id)
            {
                return true;
            }
        }
        return false;
    }

    // Finds the notification with the matching ID. Make sure that the ID is valid first by using the IsValidID function.
    public Notification GetNotification(string id)
    {
        foreach(Notification notification in notifications)
        {
            if (notification.ID == id)
            {
                return notification;
            }
        }
        // If no notification with the given ID exists, then you didn't listen to the instructions at the top of the function. Will return the final notification in the array.
        Debug.LogError("ERROR: You done goofed");
        return notifications[notifications.Length - 1];
    }
}