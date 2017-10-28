using Discord.Commands;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;

namespace PokemonBot.Modules
{
    public class HelpModule : UserModule
    {
        [Command("help")]
        private async Task Help()
        {
            string fileName = "help.txt";
            string path = Path.Combine(Environment.CurrentDirectory, @"Infos/", fileName);
            using (var str = new StreamReader(path, Encoding.Default))
            {
                await Context.Channel.SendMessageAsync(str.ReadToEnd());
            }
        }
    }
}