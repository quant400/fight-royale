using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CFC
{
    public class Chat
    {
        public string id;
        public string writerId;
        public string receiverId;
        private List<Message> _messages = new List<Message>();

        public Chat(string id, string writerId, string receiverId)
        {
            this.id = id;
            this.writerId = writerId;
            this.receiverId = receiverId;
        }

        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }
    }
    
    public class Message
    {
        public string content;
        public string writerId;
        public string receiverId;
    }
}
