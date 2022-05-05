using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CreateCharacter : MonoBehaviour
{

    public InputField _charName;
    public void sendCharacterCreation()
    {
        ClientSend.CreateCharacter(_charName.text);
    }
}
