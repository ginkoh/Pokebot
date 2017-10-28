using System.Collections.Generic;

namespace PokemonBot.Infos
{
    public class EmojiList
    {
        private static readonly Dictionary<string, string> _emojis = new Dictionary<string, string>
        {
            { "info", "<:information_source:370284846624276494>" },
            { "checked", "<:white_check_mark:372423101960421386>" }
        };

        public static Dictionary<string, string> Emojis
        {
            get { return _emojis; }
        }
    }
}