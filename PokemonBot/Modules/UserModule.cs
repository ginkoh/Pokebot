using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PokemonBot.Infos;

namespace PokemonBot.Modules
{
    [Group("pokebot")]
    public class UserModule : ModuleBase<SocketCommandContext>
    {
        #region Vars
        // private embed builder member to create embed messages
        private EmbedBuilder _eb;
        #endregion

        private enum Fields
        {
            Name,
            Id,
            Status,
            Creation,
            Pokecoins
        }

        #region Basic Infos
        [Command("on?"), Alias("on")] // Alias("on", "on2")
        [Summary("Returns bot's connection state.")]
        private async Task IsOnline()
        {
            if (Context.Client.ConnectionState == ConnectionState.Connected)
                await Context.Channel.SendMessageAsync("Bot ligado");
        }

        [Command("id")]
        [Summary("Returns the mentioned user discord id. If mentioned user is null, returns the message owner id.")]
        private async Task UserId(IGuildUser user = null)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(Context.User.Id.ToString());
            }
            else
            {
                await Context.Channel.SendMessageAsync(user.Id.ToString());
            }
        }

        [Command("avatarId")]
        [Summary("Returns the mentioned user avatar id. If mentioned user is null, returns the message owner avatar id.")]
        private async Task UserAvatarId(IGuildUser user = null)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(Context.User.AvatarId);
            }
            else
            {
                await Context.Channel.SendMessageAsync(user.AvatarId);
            }
        }

        [Command("avatarUrl")]
        [Summary("Returns the mentioned user avatar url. If mentioned user is null, returns the message owner avatar url.")]
        private async Task UserAvatarURL(IGuildUser user)
        {
            await Context.Channel.SendMessageAsync(user.GetAvatarUrl());
        }
        #endregion


        #region Advanced infos
        [Command("profileInfo"), Alias("profInfo", "pokeProfile")]
        [Summary("Returns the profile info about the mentioned user. If mentioned user is null, returns the message owner profile info.")]
        private async Task Info(IGuildUser user)
        {
            _eb = new EmbedBuilder()
            {
                ThumbnailUrl = user.GetAvatarUrl(),

                Title = $"{EmojiList.Emojis["info"]}  " +
                $"Info about {user.Username}\n",

                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Discord Name",
                        IsInline = false,
                        Value = user.Username
                    },                   
                    // Se o construtor for vazio pode omitir os parenteses
                    new EmbedFieldBuilder
                    {
                        Name = "Discord ID: ",
                        IsInline = false,
                        Value = user.Id
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Status",
                        IsInline = false,
                        Value = user.Status
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Account creation date",
                        IsInline = false,
                        Value = user.CreatedAt
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "PokeCoin$",
                        IsInline = false,
                        Value = 0
                    }
                },

                Footer = new EmbedFooterBuilder
                {
                    Text = "Date",
                    IconUrl = user.GetAvatarUrl()
                },

                Timestamp = DateTime.UtcNow
            };

            var userStatus = _eb.Fields[(int)Fields.Status].Value;

            if ((string)userStatus == UserStatus.AFK.ToString())
                _eb.Color = Color.DarkerGrey;
            // Do not have to specify the type string, it's a object, i do specified to remove the warnings.
            else if ((string)userStatus == UserStatus.DoNotDisturb.ToString())
                _eb.Color = Color.Red;
            else if ((string)userStatus == UserStatus.Idle.ToString())
                _eb.Color = Color.Orange;
            else if ((string)userStatus == UserStatus.Invisible.ToString())
                _eb.Color = Color.LighterGrey;
            else if ((string)userStatus == UserStatus.Offline.ToString())
                _eb.Color = Color.LightGrey;
            else if ((string)userStatus == UserStatus.Online.ToString())
                _eb.Color = Color.Green;


            await Context.Channel.SendMessageAsync("", false, _eb); // Empty message, false tts, embed message.
        }

        [Command("roles")]
        [Summary("Returns all server roles.")]
        private async Task Roles()
        {
            int count = 0;
            int rolesNumber = Context.Guild.Roles.Count;

            string[] roles = new string[rolesNumber];

            foreach (var rolesList in Context.Guild.Roles)
            {
                roles[count] = rolesList.ToString();
                count++;
            }

            Array.Sort(roles);

            count = 0;

            string roleMessage = null;

            foreach (var role in roles)
            {
                if (!(count == rolesNumber - 1))
                {
                    roleMessage += $"{role}, ";
                }
                else
                {
                    roleMessage += $"e {role}.";
                }
                count++;
            }

            _eb = new EmbedBuilder
            {
                Fields = new List<EmbedFieldBuilder>
                {
                    // Já que é uma lista(ou um array) de EmbedFieldBuilder, objetos novos de EmbedFieldBuilder tem de serem criados
                    // e atributos desses objetos serão setados.
                    new EmbedFieldBuilder
                    {
                        Name = "Cargos atuais",
                        IsInline = false,
                        Value = roleMessage
                    }
                },

                Color = Color.Red
            };

            await Context.Channel.SendMessageAsync("", false, _eb);
        }
        #endregion


        #region Action Commands
        [Command("toUser")]
        [Summary("Sends a message, embed or not, to a specify user.")]
        private async Task MessageToUser(IGuildUser user, bool isEmbed, params string[] message)
        {
            var dateBefore = DateTime.UtcNow;

            await Context.Message.DeleteAsync();

            if (isEmbed)
            {
                _eb = new EmbedBuilder
                {
                    ThumbnailUrl = Context.User.GetAvatarUrl(),

                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = $"Recado de {Context.User.Username}",
                            IsInline = false,
                            Value = string.Join(" ", message)
                        },

                        //new EmbedFieldBuilder
                        //{
                        //    Name = "Data de envio",
                        //    IsInline = false,
                        //    Value = dateBefore
                        //},

                        //new EmbedFieldBuilder
                        //{
                        //    Name = "Data de recebimento",
                        //    IsInline = false,
                        //    Value = DateTime.UtcNow
                        //}
                    },
                };
                await Context.Client.GetUser(user.Id).SendMessageAsync("", false, _eb);
            }
            else
            {
                await Context.Client.GetUser(user.Id).SendMessageAsync(string.Join(" ", message));
            }
        }

        [Command("calc"), Alias("calculate", "calc")]
        [Summary("Returns the result of a specify expression requisited by the user.")]
        private async Task Calculate(params string[] expression)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            var exp = dt.Compute(string.Join("", expression), "");

            await Context.Channel.SendMessageAsync(exp.ToString());
        }
        #endregion
    }
}