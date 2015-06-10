using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data;
using Npgsql;


namespace Server
{
    class MainClass
    {
        public static ArrayList userInfoList = new ArrayList();
        public static ArrayList messageList = new ArrayList();
		static NpgsqlConnection conn = null;
		static TcpClient client;
      
        public static void Main(string[] args)
        {
			
            Thread seedServer_thread = new Thread(new ThreadStart(SeedInfoServer));
			Thread listenServer_thread = new Thread(new ThreadStart(ListenServer));

			try
			{
				conn = new NpgsqlConnection("Server=localhost;Port=8000;User Id=postgres;Password=nine;Database=Nine;");
				conn.Open();
				Console.WriteLine("DB 접속성공");
			}
			catch(Exception e) 
			{
				Console.WriteLine (e.Message);
			}

			/*string sql = "INSERT INTO nine_room (user_id) values ('{0}');";
			
			N
            pgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);*/

			//da.Fill (ds);

			/*//string sql = "INSERT INTO nine_user VALUES ('1','심선생' ,'{0,1,2,3,4,5,6,7}')";
			string sql = "select * from nine_user;";
			NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);

			byte[] inputBytes;


			DataSet ds = new DataSet();
			DataTable dt = new DataTable();

			ds.Reset();

			da.Fill(ds);
			dt = ds.Tables[0];
			//dt.Rows[0].ItemArray.GetValue(0); */


			//userInfoStartServer_thread.Start();
            seedServer_thread.Start();
			listenServer_thread.Start();

            Console.WriteLine ("서버 시작");
          
            for (;;)
            {
                if("종료"==Console.ReadLine())
                {
                   
                }
               
            }        
        }

   


        public static void ListenServer()
        {
            TcpListener server;
            NetworkStream stream; 
            StreamWriter writer;
            StreamReader reader;

            server = new TcpListener(IPAddress.Any, 8001);
            server.Start();

			Console.WriteLine ("ListenServer 시작");

           
            for (;;)
            {
                String msg;
                string[] rPacket = null;
                client = server.AcceptTcpClient();
                stream = client.GetStream();
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                    
                msg = reader.ReadLine();
                if (msg != null)
                {
                    rPacket = msg.Split('_');
                    if (rPacket[0] == "Login")
                    {
                        if (Login(msg.Split('_')))
                        {
                            Console.WriteLine("{0} 로그인 성공", rPacket[1]);

                            int check = 0;
                            for (int i = 0; i < userInfoList.Count; i++)
                            {
                                User tmp = (User)userInfoList[i];
                                if (rPacket[1] == tmp.userId)
                                {
                                    check = 1;
                                    break;
                                }
                            }

                            if (check == 0)
                            {
                                userInfoList.Add(new User(rPacket[1], client));
                                Console.WriteLine("client 와 접속 성공 및 저장 {0}", client.Client.RemoteEndPoint.ToString());
                                writer.WriteLine("login suc");
                                writer.Flush();

                                messageList.Add("AddRoom_1111_1111_End");
                            }
                        }
                    }
                    else
                    {
                        messageList.Add(msg);
                    }
                }
            }
        }


        public static void SeedInfoServer()
        {

            NetworkStream stream = null;
            StreamWriter writer = null;

			TcpClient client = null;

			Console.WriteLine ("SeedInfoServer 시작");

            for (;;)
            {
                for (int m = 0;m< messageList.Count; m++)
                {
                    User user = null;
                    string Msg = string.Empty;
					string[] rPacket = null;
                    Msg = (string)messageList[m];
                    rPacket = Msg.Split('_');


					for (int i = 0; i < userInfoList.Count; i++) 
					{  
						user = (User)userInfoList [i];
						if (user.userId == rPacket [1]) 
						{
							client = (TcpClient)user.client;
							stream = client.GetStream ();
							break;                          
						}
					}
							

					if (rPacket [0] == "AddText") 
					{
						AddText (rPacket, client);
					} 
					else if (rPacket [0] == "AddFile") 
					{
						AddFile (rPacket, stream, client); 
					} 
					else if (rPacket [0] == "AddProfile") 
					{
						AddProfile (rPacket, client);
					} 
					else if (rPacket [0] == "AddProfilePicture") 
					{
						AddProfilePicture (rPacket, stream, client);
					} 
					else if (rPacket [0] == "AddRoom") 
					{
						AddRoom (rPacket, client);
					} 
					else if (rPacket [0] == "AccessRoom") 
					{
						AccessRoom (rPacket, client);
					} 
					else if (rPacket [0] == "RoomListUpdate") 
					{
						RoomListUpdate (rPacket, client);
					}
						
                    messageList.RemoveAt(m);
                }
            }
        }


        static bool Login(string[] pac)
        {
            //로그인됨 로그인하는이유 소켓 때문에

            //성공하면 true 실패하면 false
          
            String sql = String.Format("select user_id from nine_user where user_id = '{0}';", pac[1]);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            ds.Reset();

            da.Fill (ds);

            dt = ds.Tables[0];

            if(dt.Rows.Count == 0)
            {
                sql = String.Format("INSERT INTO nine_user VALUES ('{0}','{1}')", pac[1],pac[2]);
                da = new NpgsqlDataAdapter(sql,conn);
                da.Fill(ds);
                Console.WriteLine(pac[2]+" reg suc");
            }
            return true;
        }

        static void AddText(string[] pac,TcpClient client)
        {

        }

        static void AddFile (string[] pac,NetworkStream stream,TcpClient client)
        {
            // 여기서 파일이름 유저 방번호등 db로 전
     
			int i = 0;
	
            Byte[] buf = new Byte[1]; // 1024 이거 파일크기 받아와서 그 크기로 해줘야함
            FileStream fs = new FileStream(pac[3], FileMode.Create);



           // while ((i = stream.Read(buf, 0, buf.Length)) > 0)
           // {
           //     fs.Write(buf, 0, buf.Length);
           // }

			fs.Close ();

            Console.WriteLine("suc");
            //여기서 fs를 db로 쏴야함 
        }

        static void AddProfile(string[] pac,TcpClient client)
        {

        }

        static void AddProfilePicture(string[] pac,NetworkStream stream,TcpClient client)
        {
            // 여기서 파일이름 유저 방번호등 db로 전
            int i = 0;
            byte[] b = new byte[1024];
            FileStream fs = new FileStream(pac[2], FileMode.Create, FileAccess.Write);

            while ((i = stream.Read(b, 0, b.Length)) > 0)
            {
                fs.Write(b, 0, i);
            }

            //여기서 fs를 db로 쏴야함
        }

        static void AddRoom(string[] pac,TcpClient client)
        {
            string sql = String.Format("select room_id from nine_room where room_id = '{0}';",pac[2]);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();

            ds.Reset();
            da.Fill (ds);
            dt = ds.Tables[0];

            if (dt.Rows.Count == 0)
            {
                sql = String.Format("INSERT INTO nine_room VALUES ('{0}','{{1}}');", pac[2], pac[1]);
                da = new NpgsqlDataAdapter(sql, conn);

                dt = new DataTable();
                ds = new DataSet();

                ds.Reset();

                da.Fill(ds);
            }
            else
            {
                sql = String.Format("select user_id from nine_room where room_id = '{0}';", pac[2]);
                da = new NpgsqlDataAdapter(sql, conn);
                dt = new DataTable();
                ds = new DataSet();

                ds.Reset();
                da.Fill(ds);
                dt = ds.Tables[0];

                int[] user_ids = (int [])dt.Rows[0].ItemArray.GetValue(0);
                String temp = "{";
                for (int i = 0; i < user_ids.Length; i++)
                {
                    temp += user_ids[i]+",";
                }
                temp += pac[1] + "}";
               
                sql = String.Format("UPDATE nine_room Set user_id = '{0}' where room_id = '{1}';", temp, pac[2]);
                da = new NpgsqlDataAdapter(sql, conn);
                da.Fill(ds);
               
            }
        }

        static void AccessRoom(string[] pac,TcpClient client)
        {

        }

        static void RoomContentUpdate(string id,TcpClient client)
        {

        }

        static void RoomListUpdate(string[] pac,TcpClient client)
        {
            NetworkStream stream;                                                   
            StreamWriter writer;
            int cnt = 3;
            string packet = "RoomListUpdate_";  //이거 만들어서 보내야함

            string[] roomName = new string[cnt];  roomName[0] = "1번방"; roomName[1] = "2번방"; roomName[2] = "3번방";
            int[] roomPeopleNum = new int[cnt]; roomPeopleNum[0] = 6; roomPeopleNum[1] = 2; roomPeopleNum[2] = 4; 

        
            stream = client.GetStream();

            writer = new StreamWriter(stream);

            packet += cnt+"_";

            for (int i = 0; i < cnt; i++)
            {
                packet+=roomName[i] +'_'+roomPeopleNum[i]+'_'; 
            }

            packet += "End";

            writer.WriteLine(packet);
            writer.Flush();
		

        }
    }
}

// Login  AddText  AddFile  AddProfile  AddProfilePicture AddRoom AccessRoom
// UserId
// 내용
// End

//Login_123125151(id)_junhokim(name)_End
//AddText_rlawnsgh78_방번호_안녕_End
//AddFile_rlawnsgh78_방번호_사진.jpg_End 보낸 다음 파일전송
//AddProfile_rlawnsgh78_방번호_프로필내용_End;
//AddProfilePicture_rlawnsgh78_방번호_End 보낸 다음 파일전송
//AddRoom_1221414(id)_RoomNumber_End 방번호가져오기
//AccessRoom_rlawnsgh78_방번호_End
//Update_rlawnsgh78_방번호_End


// RoomListUpdate   RoomContentUpdate
//
//
// RoomListUpdate_방갯수_방이름_방이름_방이름_방이름_End 보낸 다음 클라 쪽에서 이방갯수로 반복문 돌리고 그만큼 서버에서 반복적으로 쏴준다.