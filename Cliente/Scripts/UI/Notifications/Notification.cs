using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{

    public Text _text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		Image _image = this.gameObject.GetComponent<Image>();
		if (Screen.width - Get_Width(this.gameObject)/2 < transform.position.x)
		{
			transform.position = new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z);
		} else
        {
			transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
			
			var tempColor = _image.color;
			tempColor.a -= 0.01f;
			_image.color = tempColor;
			var tempTextColor = _text.color;
			tempTextColor.a -= 0.01f;
			_text.color = tempTextColor;
		}
		if (_image.color.a <= 0)
        {
			Destroy(gameObject);
        }
    }


    public void setNotification(string _message)
    {
        _text.text = _message;
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
