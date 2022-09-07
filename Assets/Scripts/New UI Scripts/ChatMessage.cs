using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class ChatMessage : MonoBehaviour
{
    [SerializeField]
    Sprite otherPlayer, localplayer;
    [SerializeField]
    Image senderCahr, messageBack;
    [SerializeField]
    TMP_Text messageText;


    public void SetMessage(NetworkIdentity player,string message)
    {
        if(player.isLocalPlayer)
        {
            senderCahr.transform.parent.parent.GetComponent<Image>().color = new Color(0.9450981f, 0.1215686f, 0.172549f, 1f);
            senderCahr.sprite = IngameUIControler.instance.chatPics[player];
            messageBack.sprite = localplayer;
            messageText.text = message.ToUpper();
        }

        else
        {
            senderCahr.transform.parent.parent.GetComponent<Image>().color =IngameUIControler.instance.playerMap[player].color;
            senderCahr.sprite = IngameUIControler.instance.chatPics[player];
            messageBack.sprite = otherPlayer;
            messageText.text = message.ToUpper();
            messageText.color = IngameUIControler.instance.playerMap[player].color;
        }

        //transform.parent = chatContainer;
    }
}
