using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{


    public Transform _prefab;

	public Transform _heliPrefab;

	public Transform _kickNotification;

	public SafeZone _safezone;

	public ZoneThreat _zone;

	public NewNotification _notifications;


	public void showKickNotification(string reason = "")
    {
		Cursor.lockState = CursorLockMode.None;
		var notification = (Transform)Instantiate(
					   _kickNotification,
					  new Vector3(Screen.width/2, Screen.height / 2, 0f),
					  new Quaternion());
		KickNotification _not = notification.gameObject.GetComponent<KickNotification>();
		_not.setMessage(reason);
		notification.SetParent(this.transform);
	}
    public void showNotification(string _message)
    {
		_notifications.showNotification(_message);
        //var notification = (Transform)Instantiate(
        //                 _prefab,
        //                new Vector3(Screen.width, Screen.height/2, 0f),
        //                new Quaternion());
        //Notification _not = notification.gameObject.GetComponent<Notification>();
        //_not.setNotification(_message);
		//notification.SetParent(this.transform);
    }

	public void ShowHelicopter()
    {
		var heliNot = (Transform)Instantiate(
						_heliPrefab,
					   new Vector3(Screen.width/2, Screen.height - 35f, 0f),
					   new Quaternion());
		heliNot.SetParent(this.transform);
		StartCoroutine(_destroyNotification(heliNot.gameObject));
	}

	public IEnumerator _destroyNotification(GameObject _not)
    {
		yield return new WaitForSeconds(10f);
		Destroy(_not);

    }

	public static Rect Get_Rect(GameObject gameObject)
	{
		if (gameObject != null)
		{
			RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

			if (rectTransform != null)
			{
				return rectTransform.rect;
			}
		}
		else
		{
			Debug.Log("Game object is null.");
		}

		return new Rect();
	}


	public static float Get_Width(Component component)
	{
		if (component != null)
		{
			return Get_Width(component.gameObject);
		}

		return 0;
	}
	public static float Get_Width(GameObject gameObject)
	{
		if (gameObject != null)
		{
			var rect = Get_Rect(gameObject);
			if (rect != null)
			{
				return rect.width;
			}
		}

		return 0;
	}
}
