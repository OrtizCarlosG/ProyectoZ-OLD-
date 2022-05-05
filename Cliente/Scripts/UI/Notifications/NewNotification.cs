using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewNotification : MonoBehaviour
{
    public Transform[] _notifications;

    float lastNotification1, lastNotification2, lastNotification3, lastNotification4, lastNotification5;
    private void Update()
    {
    }
    public void showNotification(string message)
    {
        if (Time.time - lastNotification1 > 1 / 0.5f)
        {
            lastNotification1 = Time.time;
            var notification = Instantiate(_notifications[0], this.transform);
           NewNotificationItem _not = notification.gameObject.GetComponent<NewNotificationItem>();
            _not._notificationMessage.text = message;
            notification.gameObject.SetActive(true);
            Destroy(notification.gameObject, 4f);
        } else if (Time.time - lastNotification2 > 1 / 0.5f)
        {
            lastNotification2 = Time.time;
            var notification = Instantiate(_notifications[1], this.transform);
            NewNotificationItem _not = notification.gameObject.GetComponent<NewNotificationItem>();
            _not._notificationMessage.text = message;
            notification.gameObject.SetActive(true);
            Destroy(notification.gameObject, 4f);
        }
        else if (Time.time - lastNotification3 > 1 / 0.5f)
        {
            lastNotification3 = Time.time;
            var notification = Instantiate(_notifications[2], this.transform);
            NewNotificationItem _not = notification.gameObject.GetComponent<NewNotificationItem>();
            _not._notificationMessage.text = message;
            notification.gameObject.SetActive(true);
            Destroy(notification.gameObject, 4f);
        }
        else if (Time.time - lastNotification4 > 1 / 0.5f)
        {
            lastNotification4 = Time.time;
            var notification = Instantiate(_notifications[3], this.transform);
            NewNotificationItem _not = notification.gameObject.GetComponent<NewNotificationItem>();
            _not._notificationMessage.text = message;
            notification.gameObject.SetActive(true);
            Destroy(notification.gameObject, 4f);
        }
        else if (Time.time - lastNotification5 > 1 / 0.5f)
        {
            lastNotification5 = Time.time;
            var notification = Instantiate(_notifications[4], this.transform);
            NewNotificationItem _not = notification.gameObject.GetComponent<NewNotificationItem>();
            _not._notificationMessage.text = message;
            notification.gameObject.SetActive(true);
            Destroy(notification.gameObject, 4f);
        }
    }
}
