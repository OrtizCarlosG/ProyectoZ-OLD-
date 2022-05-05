using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;

public class ChatWindow : MonoBehaviour
{
    public Transform _chatLog;
    public TMP_InputField _chatText;
    public int MaxMessages = 25;
    public Transform _chatPrefab;
    public Transform _chatContainer;

    [SerializeField]
    List<Message> messageList = new List<Message>();
    Animator _anim;

    private void Start()
    {
        _anim = _chatContainer.transform.GetComponent<Animator>();
        _anim.Play("hideChat", -1, 0f);
        _chatText.transform.gameObject.SetActive(false);
    }
    public void appendText(string _message, string _image)
    {
        if (messageList.Count >= MaxMessages)
        {
            Destroy(messageList[0]._text.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message _chatRec = new Message();
        _chatRec._message = _message;

        GameObject newChat = Instantiate(_chatPrefab.gameObject, _chatLog.transform);
        ChatMessage _chat = newChat.GetComponent<ChatMessage>();
        _chat._message.text = _message;
        _chatRec._text = _chat._message;
        _chatRec._text.text = _chatRec._message;
        _chatRec._profile = _chat._profilePhoto;
        Vector2 pivot = new Vector2(0.5f, 0.5f);
        _chatRec._profile.sprite = Sprite.Create(getImage(_image), new Rect(0.0f, 0.0f, getImage(_image).width, getImage(_image).height), pivot, 100.0f);
        messageList.Add(_chatRec);
        _anim.Play("showChat", -1, 0f);
        StartCoroutine(hideCoroutine());
    }
    private void Update()
    {
        if (!_chatText.Equals("Escribe algo...") && !string.IsNullOrEmpty(_chatText.text) && Input.GetKeyDown(KeyCode.Return))
        {
            SendChatMessage(_chatText.text);
            Cursor.lockState = CursorLockMode.Locked;
            _chatText.DeactivateInputField();
            _chatText.transform.gameObject.SetActive(false);

        } else if (!_chatText.isFocused && Input.GetKeyDown(KeyCode.Return))
        {
            _chatText.transform.gameObject.SetActive(true);
            _anim.Play("showChat", -1, 0f);
            _chatText.ActivateInputField();
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void SendChatMessage(string text)
    {
        ClientSend.ChatMessage(_chatText.text);
        _chatText.text = string.Empty;
    }

    private IEnumerator hideCoroutine()
    {
        yield return new WaitForSeconds(5f);
        _anim.Play("hideChat", -1, 0f);
    }

    public Texture2D getImage(string image)
    {
        Texture2D newPhoto = new Texture2D(1, 1);
        newPhoto.LoadImage(Convert.FromBase64String(image));
        newPhoto.Apply();
        return newPhoto;
    }
    [System.Serializable]
    public class Message
    {
        public string _message;
        public TextMeshProUGUI _text;
        public Image _profile;
    }
}
