using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonBot.Infos
{
    public class MessageList
    {
        private static Dictionary<string, Tuple<string, string>> _messageList = new Dictionary<string, Tuple<string, string>>
        {
            
        };

        public static Dictionary<string, Tuple<string, string>> Messages
        {
            get { return _messageList; }
        }
    }
}
