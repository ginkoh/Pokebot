using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using System;

using PokemonBot.Infos;

namespace PokemonBot.Modules
{
    [Group("pokeadmin")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {

        [Command("del")]
        [Summary("Deletes a maximum of 100 messages.")]
        private async Task Del(int number)
        {
            if (number <= 0)
            {
                await ReplyAsync("There are no messages to delete.");
                return;
            }
            else if (number > 100)
            {
                await ReplyAsync("Cannot delete more than one hundred messages.");
                return;
            }
            else
            {
                var getMessages = await Context.Channel.GetMessagesAsync(number).Flatten();
                await Context.Channel.DeleteMessagesAsync(getMessages);
            }
        }

        [Command("addRole")]
        [Summary("Creates a role with specify name and color (r, g, b).")]
        private async Task AddRole(string role, int c1, int c2, int c3)
        {
            await Context.Guild.CreateRoleAsync
                (role,
                GuildPermissions.All,
                new Color(c1, c2, c3),
                false, 
                RequestOptions.Default);

            await ReplyAsync($"{EmojiList.Emojis["checked"]} The Role {role} was successfully created.");
        }

        [Command("roleTo")]
        [Summary("Gives a role to a specify user.")]
        private async Task RoleTo(IGuildUser user, IRole role)
        {
            await user.AddRoleAsync(role);
            await ReplyAsync($"{EmojiList.Emojis["checked"]} The Role '{role}' was successfully added to user {user}.");
        }
    }
}