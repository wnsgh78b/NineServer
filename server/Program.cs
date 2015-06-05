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

        public static void Main(string[] args)
        {
			
           // Thread userInfoStartServer_thread = new Thread (new ThreadStart(UserInfoServer));
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
			//INSERT INTO table_name (column1,column2,column3,...)VALUES (value1,value2,value3,...);

			string sql = "INSERT INTO nine_room (user_id) values ('{0}');";
			DataSet ds = new DataSet();
			ds.Reset();
			NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);





			da.Fill (ds);

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
            TcpClient client;
            NetworkStream stream; 
            StreamReader reader;

            server = new TcpListener(IPAddress.Any, 4926);
            server.Start();

			Console.WriteLine ("ListenServer 시작");

            for (;;)
            {
                client = server.AcceptTcpClient();
                stream = client.GetStream();
                reader = new StreamReader(stream);
                messageList.Add(reader.ReadLine());
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

					if (rPacket [0] == "Login") 
					{
						if (Login(rPacket))
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
							}
						}
					} 
					else 
					{
						for (int i = 0; i < userInfoList.Count; i++) {  
							user = (User)userInfoList [i];
							if (user.userId == rPacket [1]) {
                           
								break;
//                            writer = new StreamWriter(stream);
//                            writer.WriteLine(Msg);
//                            writer.Flush();
							}
						}

						client = (TcpClient)user.client;
						stream = client.GetStream ();

						if (rPacket [0] == "AddText") {
							AddText (rPacket, client);
						} else if (rPacket [0] == "AddFile") {
							AddFile (rPacket, stream, client); 
						} else if (rPacket [0] == "AddProfile") {
							AddProfile (rPacket, client);
						} else if (rPacket [0] == "AddProfilePicture") {
							AddProfilePicture (rPacket, stream, client);
						} else if (rPacket [0] == "AddRoom") {
							AddRoom (rPacket, client);
						} else if (rPacket [0] == "AccessRoom") {
							AccessRoom (rPacket, client);
						} else if (rPacket [0] == "RoomListUpdate") {
							RoomListUpdate (rPacket, client);
						}
					}

                    messageList.RemoveAt(m);
                }
            }
        }


        static bool Login(string[] pac)
        {
            //로그인됨 로그인하는이유 소켓 때문에

            //성공하면 true 실패하면 false

            return true;
        }

        static void AddText(string[] pac,TcpClient client)
        {

        }

        static void AddFile (string[] pac,NetworkStream stream,TcpClient client)
        {
            // 여기서 파일이름 유저 방번호등 db로 전
            int i = 0;
            byte[] b = new byte[1024]; // 1024 이거 파일크기 받아와서 그 크기로 해줘야함
            FileStream fs = new FileStream(pac[2], FileMode.Create, FileAccess.Write);

            while ((i = stream.Read(b, 0, b.Length)) > 0)
            {
                fs.Write(b, 0, i);
            }

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
            //db에 방번호 추가요청
            //그 다음 방번호 알려달라고 요청
            //그 다음 방만든사람 아이디 를 접속 을 한번에

			string sql = "INSERT nine_room SET\n\t\t\tuser_id = '{0}'";
			NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
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

//Login_rlawnsgh78_21410000_End
//AddText_rlawnsgh78_방번호_안녕_End
//AddFile_rlawnsgh78_방번호_사진.jpg_End 보낸 다음 파일전송
//AddProfile_rlawnsgh78_방번호_프로필내용_End;
//AddProfilePicture_rlawnsgh78_방번호_End 보낸 다음 파일전송
//AddRoom_rlawnsgh78_End 방번호가져오기
//AccessRoom_rlawnsgh78_방번호_End
//Update_rlawnsgh78_방번호_End


// RoomListUpdate   RoomContentUpdate
//
//
// RoomListUpdate_방갯수_방이름_방이름_방이름_방이름_End 보낸 다음 클라 쪽에서 이방갯수로 반복문 돌리고 그만큼 서버에서 반복적으로 쏴준다.