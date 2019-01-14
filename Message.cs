using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.TL;
using TeleSharp.TL.Channels;
using TeleSharp.TL.Messages;
using TLSharp;
using TLSharp.Core;
using System.IO;
using System.Windows.Forms;



namespace telegram_test
{

    class Message
    {
        string connString = "Host=localhost;Username=postgres;Password=2537300;Database=postgres";
        TLMessage message;
        public void WriteMessageChannel(string name_chat, int Id_message)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT from_id, id_message, name_chat, message, \"date\" FROM telegram.message Limit 10 ";
                    //cmd.Parameters.AddWithValue("p", "Hello world");
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        try
                        {
                            string result = reader.GetString(1);//Получаем значение из второго столбца! Первый это (0)!
                        }
                        catch { }

                    }
                    conn.Close();

                }

                // Retrieve all rows

            }

        }

        public static int DateTimeToUnixTimeStamp(DateTime time)
        {
            int unixTime = (int)(time - new DateTime(1970, 1, 1)).TotalSeconds;
            return unixTime;
        }
        int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public async void GetLastMessage(TLChannel channel, TelegramClient client, string name_chat, int offsetId)
        {
            int temp_offset = 0;
            int now_time=0;
            if (offsetId == 0)
            {
                TimeSpan hours = new TimeSpan(-1, 0, 0);
                DateTime time = DateTime.Now.Add(hours);
                now_time = DateTimeToUnixTimeStamp(time);
            }
            while (true)
            {
                TLChannelMessages res = await client.SendRequestAsync<TLChannelMessages>
    (new TLRequestGetHistory()
    {
        Peer = new TLInputPeerChannel { ChannelId = channel.Id, AccessHash = channel.AccessHash.Value },
        Limit = 100,
        AddOffset = temp_offset,
        OffsetId = 0
    });

                temp_offset += 100;

                var msgs = (TLChannelMessages)res;

                foreach (TLAbsMessage message in msgs.Messages)
                {
                    var tlMessage = message as TLMessage;
                    if (tlMessage == null)
                        continue;
                    if (offsetId == 0)
                    {

                        if (tlMessage.Date < now_time)
                        {
                            break;
                        }

                    }
                    else
                    {
                        if (tlMessage.Id < offsetId)
                            break;
                    }
                    int replyId = -1;
                    if (tlMessage.ReplyToMsgId != null)
                    {
                        replyId = Convert.ToInt16(tlMessage.ReplyToMsgId);
                    }
                    string insert = "INSERT INTO public.message(messageid, fromid, chatname, processing_dttm, message, replyid)VALUES(";
                    using (var conn = new NpgsqlConnection(connString))
                    {
                        conn.Open();

                        // Insert some data
                        using (var cmd = new NpgsqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = insert + tlMessage.Id.ToString() + "," + tlMessage.FromId + ",\'" + name_chat + "\'," + tlMessage.Date.ToString() + ",\'" + tlMessage.Message.Replace("'","\"") + "\'," + replyId.ToString() + ");";
                            //cmd.Parameters.AddWithValue("p", "Hello world");
                            try
                            {
                                NpgsqlDataReader reader = cmd.ExecuteReader();
                            }
                            catch(Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }
                            conn.Close();

                        }

                    }

                }
            }

        }


        public async Task<TLChannelMessages> GetMessageFromChannelAsync(TLChannel channel, TelegramClient client)
        {
            TLChannelMessages res = await client.SendRequestAsync<TLChannelMessages>
                (new TLRequestGetHistory()
                {
                    Peer = new TLInputPeerChannel { ChannelId = channel.Id, AccessHash = channel.AccessHash.Value },
                    Limit = 400,
                    AddOffset = 100,
                    OffsetId = 0

                });
            return res;
        }

        public Dictionary<string, int> GetLastIdMessage(Dictionary<int, string> chats, string number)
        {
            System.IO.File.WriteAllText("D:\\" + number.Replace("+", "") + ".txt", string.Empty);
            Dictionary<string, int> chat_lastid = new Dictionary<string, int>();
            foreach (var chat in chats)
            {
                string select = "SELECT messageid FROM public.message where chatname=\'" + chat.Value + "\' order by messageid desc limit 1;";
                using (var conn = new NpgsqlConnection(connString))
                {
                    conn.Open();

                    // Insert some data
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = select;
                        //cmd.Parameters.AddWithValue("p", "Hello world");
                        NpgsqlCommand command = new NpgsqlCommand(select, conn);
                        NpgsqlDataReader dr = command.ExecuteReader();
                        bool flag = false;
                        try
                        {
                            while (dr.Read())
                            {
                                flag = true;
                                using (StreamWriter writetext = new StreamWriter("D:\\" + number.Replace("+", "") + ".txt", true))
                                {
                                    writetext.WriteLine(chat.Value+"|"+dr[0].ToString());
                                }
                                chat_lastid.Add(chat.Value, Convert.ToInt16(dr[0]));

                            }
                            if (flag == false)
                            {
                                using (StreamWriter writetext = new StreamWriter("D:\\" + number.Replace("+", "") + ".txt", true))
                                {
                                    writetext.WriteLine(chat.Value + "|" + "0");
                                }
                                chat_lastid.Add(chat.Value, 0);
                            }

                        }
                        catch (Exception e)
                        {
                            return null;
                        }

                    }
                }
            }
            return chat_lastid;
        }
        public int MaxReply(Dictionary<int,int> reply)
        {
         
            int chatId = 0;
            List<int> message_reply = new List<int>();
            foreach(var temp in reply)
            {
                message_reply.Add(temp.Value);
            }
            var g = message_reply.GroupBy(i => i);
            int max = 1;
            foreach (var temp in g)
            {
                if (temp.Count<int>() > max)
                {
                    max = temp.Count<int>();
                    chatId = temp.Key;
                }
            }
            
            return chatId;
        }
      
        public string ReturnMessage(int Idmessage)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                  // Insert some data
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        string select = "SELECT message FROM public.message where messageid="+Idmessage.ToString()+";";
                        NpgsqlCommand command = new NpgsqlCommand(select, conn);
                        NpgsqlDataReader dr = command.ExecuteReader();

                        while (dr.Read())
                        {
                            try
                            {
                            conn.Close();
                            return dr[0].ToString();
                            }
                            catch
                            {
                            conn.Close();
                            return null;
                            }

                        }
                       

                    
                }


            }
            return null;
        }
        public Dictionary<string, string> GetReplyDictionary(Dictionary<string,int> chat_data)
        {
            Dictionary<string, string> repli_message = new Dictionary<string, string>();
           
            foreach (var chat in chat_data)
            {
                using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
              
                    // Insert some data
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                       string select= "SELECT messageid, message, replyid FROM public.message where replyid<>-1 and chatname =\'" +chat.Key+ "\' and messageid>"+0.ToString()+"; ";
                        NpgsqlCommand command = new NpgsqlCommand(select, conn);
                        NpgsqlDataReader dr = command.ExecuteReader();
                        Dictionary<int, int> messageId = new Dictionary<int, int>();
                        while (dr.Read())
                        {
                            try
                            {
                                messageId.Add(Convert.ToInt16(dr[0]),Convert.ToInt16(dr[2]));
                            }
                            catch {
                               break;
                            }

                        }
                        if (messageId.Count != 0)
                        {
                            int maxReply = MaxReply(messageId);
                            repli_message.Add(chat.Key, ReturnMessage(maxReply));
                        }
                        conn.Close();

                    }
                }
               

            }

   

            return repli_message;
        }
    }

}