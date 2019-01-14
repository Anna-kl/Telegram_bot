using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp;
using TLSharp.Core;
using TLSharp.Core.Network;
using TLSharp.Core.Requests;
using TLSharp.Core.Utils;

namespace telegram_test
{
    public partial class Form1 : Form
    {
        TelegramClient client;
        string hash;
        public Form1()
        {
            InitializeComponent();

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            //int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            //TimeSpan hours = new TimeSpan(-1, 0, 0);
            //var time = DateTime.Now.Add(hours);
           // Message mes = new Message();
           // var telephone = GetChatForNumber("+79951209052");
         
           //var dictionaty_chat = mes.GetLastIdMessage(telephone, "+79951209052");
          
           //     mes.GetReplyDictionary(dictionaty_chat);
                    
            //mes.GetLastIdMessage("DataLight stAff");
       
            var store = new FileSessionStore();
            client = new TelegramClient(632369, "7cb0f9a3d2e5108a4b9a961b86fd564a", store, "session3");

            await client.ConnectAsync();
            hash = await client.SendCodeRequestAsync("+79951209052");

        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        private Dictionary<int, string> GetChatForNumber(string number)
        {
            Dictionary<int, string> telethone = new Dictionary<int, string>();
            string connString = "Host=localhost;Username=postgres;Password=2537300;Database=postgres";
            string select = "SELECT  name_chat FROM public.bot_number where telethon=\'" + number + "\';";
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;

                    //cmd.Parameters.AddWithValue("p", "Hello world");
                    NpgsqlCommand command = new NpgsqlCommand(select, conn);
                    NpgsqlDataReader dr = command.ExecuteReader();
                    int key = 0;
                    try
                    {
                        while (dr.Read())
                        {

                            telethone.Add(key, dr[0].ToString());
                            key++;
                        }
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
                return telethone;
            }
        }

            private async void button2_Click_1(object sender, EventArgs e)
            {
                string code = textBox1.Text;

                var user = await client.MakeAuthAsync("+79951209052", hash, code);
                Message mes = new Message();
            var telephone = GetChatForNumber("+79951209052");
            var dictionaty_chat = mes.GetLastIdMessage(telephone, "+79951209052");
            foreach (var chat in dictionaty_chat)
            {
                var data = await GetChannel("DataLight stAff");
                mes.GetLastMessage(data, client, chat.Key, chat.Value);
                //var reply=mes.GetReplyDictionary()
            }

                //find channel by title
                //            var chat = dialogs.Chats
                //              .Where(c => c.GetType() == typeof(TLChannel))
                //              .Cast<TLChannel>()
                //              .FirstOrDefault(c => c.Title == "Крипто-скам-чат");
                //            var chat_send = dialogs.Chats
                //              .Where(c => c.GetType() == typeof(TLChannel))
                //              .Cast<TLChannel>()
                //              .FirstOrDefault(c => c.Title == "DataLight stAff");
                //            var tlAbsMessages =
                //                        await client.GetHistoryAsync(
                //                            new TLInputPeerChannel { ChannelId = chat.Id, AccessHash=chat.AccessHash.Value }, 11000,
                //                            10000000, 50);
                //            TLChannelMessages res = await client.SendRequestAsync<TLChannelMessages>
                //(new TLRequestGetHistory()
                //{
                //    Peer = new TLInputPeerChannel { ChannelId = chat.Id, AccessHash = chat.AccessHash.Value },
                //    Limit = 400,
                //    AddOffset = 100,
                //    OffsetId = 0
                //});
                //            var msgs = res.Messages;

                //            foreach (var message in msgs)
                //            {
                //                string text = message.ToString();

                //                await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = chat_send.Id, AccessHash = chat_send.AccessHash.Value }, text);
                //            }
                //      var dialogs = (TeleSharp.TL.Messages.TLDialogs)await client.GetUserDialogsAsync();

                //      var main_u = new TLUser();

                //      var found = await client.SearchUserAsync("chestnykh", 1);
                //      var u = found.Users
                //.Where(x => x.GetType() == typeof(TLUser))
                //.Cast<TLUser>().FirstOrDefault(x => x.FirstName == "Danila");
                //      var ur = dialogs.Users.Where(x => x.GetType() == typeof(TLUser))
                //.Cast<TLUser>().FirstOrDefault(x => x.FirstName == "Аня");
                //            var dialogs = (TeleSharp.TL.Messages.TLDialogs)await client.GetUserDialogsAsync();
                //            foreach (var element in dialogs.Chats)
                //            {
                //                await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = element., AccessHash = chat.AccessHash.Value }, "OUR_MESSAGE");
                //                if (element is TLChat)
                //                {
                //                    TLChat chat = element as TLChat;

                //                    //find channel by title
                //                    TeleSharp.TL.Messages.TLChatFull channelInfo = await client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>
                //(new TeleSharp.TL.Messages.TLRequestGetFullChat() { ChatId = chat.Id });

                //                    if (element is TLChannel)
                //                    {
                //                        var offset = 0;
                //                        TLChannel channel = element as TLChannel;
                //                        var chan = await client.SendRequestAsync<TeleSharp.TL.Messages.TLChatFull>(new TLRequestGetFullChannel()
                //                        {
                //                            Channel = new TLInputChannel()
                //                            {
                //                                ChannelId = channel.Id,
                //                                AccessHash = (long)channel.AccessHash
                //                            }
                //                        });
                //                        TLInputPeerChannel inputPeer = new TLInputPeerChannel()
                //                        {
                //                            ChannelId = channel.Id,
                //                            AccessHash = (long)channel.AccessHash
                //                        };
                //                        await client.SendMessageAsync(inputPeer, "text message");

                //                    }
                //                }
                //                //send message
                //                //await client.SendMessageAsync(new TLInputPeerChannel() { ChannelId = chat.Id, AccessHash = chat.AccessHash.Value }, "OUR_MESSAGE");
                //            }

                //            if (users != null)
                //            {
                //                await client.SendMessageAsync(new TLInputPeerUser() { UserId = users.Id }, "Hi there!");
                //            }

                //            await client.SendMessageAsync(new TLInputPeerUser() { UserId = users.Id }, "OUR_MESSAGE");
            }
            private async Task<TLChannel> GetChannel(string name_channel)
            {
                var dialogs = (TLDialogs)await client.GetUserDialogsAsync();

                //find channel by title
                var chat = dialogs.Chats
                  .Where(c => c.GetType() == typeof(TLChannel))
                  .Cast<TLChannel>()
                  .FirstOrDefault(c => c.Title == name_channel);
                return chat;
            }


        }


    } 