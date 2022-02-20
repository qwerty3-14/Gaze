using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectGaze.Entities.Ships;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public static class Networking
    {
		static NetServer server;
		static NetClient client;
		public static List<bool[]> incomingControls = new List<bool[]>();
		public static bool[] myControls = null;
		public static int framePacketSize = 4;
		static int timeOutTimer = 0;
		const int timeOutMax = 600;
		static bool awaitResponse = false;
		public static NetPeerConfiguration GetConfiguration()
        {
			NetPeerConfiguration config = new NetPeerConfiguration("ProjectGaze");
			config.Port = 14242;
			// Enable DiscoveryRequest messages
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

			return config;
		}
		public static void StartServer()
        {
			NetPeerConfiguration config = new NetPeerConfiguration("ProjectGaze")
			{ Port = 12345 };
			server = new NetServer(config);
			server.Start();
			Console.WriteLine("Server has started!");
		}
		static string localIP = "127.0.0.1";
		static string IPv6 = "10.21.33.101";
		static string remoteIP = "173.191.55.217";
		public static void StartClient()
        {
			NetPeerConfiguration config = new NetPeerConfiguration("ProjectGaze");
			client = new NetClient(config);
			client.Start();
			client.Connect(host: remoteIP, port: 12345);
			Console.WriteLine("Client has started!");
		}
		public static void RunServer()
		{
			if (!awaitResponse)
			{
				if (netFrameCounter == 0 && server.Connections.Count > 0)
				{
					NetOutgoingMessage sendMsg = server.CreateMessage();
					sendMsg.Write((byte)DataMessageType.Controls);
					myControls = new bool[6];
					myControls[0] = Controls.controlThrust[0];
					myControls[1] = Controls.controlRight[0];
					myControls[2] = Controls.controlLeft[0];
					myControls[3] = Controls.controlDown[0];
					myControls[4] = Controls.controlShoot[0];
					myControls[5] = Controls.controlSpecial[0];
					for (int i = 0; i < 6; i++)
					{
						sendMsg.Write(myControls[i]);
					}
					sendMsg.WritePadBits();
					server.SendMessage(sendMsg, server.Connections[0], NetDeliveryMethod.ReliableOrdered);
				}
				if (server.Connections.Count > 0)
				{
					netFrameCounter++;
				}
			}
			NetIncomingMessage message;
			while ((message = server.ReadMessage()) != null)
			{
				switch (message.MessageType)
				{
					case NetIncomingMessageType.Data:
						// handle custom messages
						DataMessageType type = (DataMessageType)message.ReadByte();
						switch (type)
                        {
							case DataMessageType.Controls:
								if (incomingControls.Count > 0)
								{
									Console.WriteLine("Overflow at: " + (debugMessageCounter + 1));
								}
								incomingControls.Add( new bool[6]);
								for (int i =0; i < 6; i++)
                                {
									incomingControls[incomingControls.Count-1][i] = message.ReadBoolean();
								}
								message.SkipPadBits();
								break;
							case DataMessageType.Disconnect:
								Disconnect();
								break;
						}
						break;

					case NetIncomingMessageType.StatusChanged:
						// handle connection status messages
						Console.WriteLine(message.SenderConnection.Status);
						switch (message.SenderConnection.Status)
						{
							case NetConnectionStatus.Connected:
								StartupSync();
								break;
							case NetConnectionStatus.Disconnected:
								Disconnect();
								break;
						}
						break;

					/* .. */
					default:
						Console.WriteLine("unhandled message with type: "
							+ message.MessageType);
						break;
				}
			}
		}
		public static void RunClient()
		{
			if (netFrameCounter == 0)
			{
				NetOutgoingMessage sendMsg = client.CreateMessage();
				sendMsg.Write((byte)DataMessageType.Controls);

				myControls = new bool[6];
				myControls[0] = Controls.controlThrust[1];
				myControls[1] = Controls.controlRight[1];
				myControls[2] = Controls.controlLeft[1];
				myControls[3] = Controls.controlDown[1];
				myControls[4] = Controls.controlShoot[1];
				myControls[5] = Controls.controlSpecial[1];
				for (int i = 0; i < 6; i++)
				{
					sendMsg.Write(myControls[i]);
				}
				sendMsg.WritePadBits();
				client.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);
			}
			if(client.Connections.Count > 0)
            {
				netFrameCounter++;
			}

			NetIncomingMessage message;
			while ((message = client.ReadMessage()) != null)
			{
				switch (message.MessageType)
				{
					case NetIncomingMessageType.Data:
						// handle custom messages
						DataMessageType type = (DataMessageType)message.ReadByte();
						switch (type)
						{
							case DataMessageType.Controls:
								if(incomingControls.Count > 0)
                                {
									Console.WriteLine("Overflow at: " + (debugMessageCounter + 1));
                                }

								incomingControls.Add(new bool[6]);
								for (int i = 0; i < 6; i++)
								{
									incomingControls[incomingControls.Count - 1][i] = message.ReadBoolean();
								}
								message.SkipPadBits();
								break;
							case DataMessageType.Disconnect:
								Disconnect();
								break;
							case DataMessageType.StartQuickPlay:
								Main.startAI[0] = message.ReadBoolean();
								Main.startAI[1] = message.ReadBoolean();
								message.SkipPadBits();
								StartQuickPlay();
								break;
						}
						break;

					case NetIncomingMessageType.StatusChanged:
						// handle connection status messages
						Console.WriteLine(message.SenderConnection.Status);
						switch(message.SenderConnection.Status)
                        {
							case NetConnectionStatus.Connected:
								StartupSync();
								break;
							case NetConnectionStatus.Disconnected:
								Disconnect();
								break;
						}
						break;
					default:
						Console.WriteLine("unhandled message with type: "
							+ message.MessageType);
						break;
				}
			}
		}
		public static int netFrameCounter = 0;
		public static bool Update()
        {
			if (!IsConnected())
            {
				return false;
            }
			if(server != null)
            {
				RunServer();
			}
			if(client != null)
            {
				RunClient();
			}
			if(awaitResponse)
			{
				return true;
			}
			/*
			if(debugMessageCounter >= 30)
            {
				StartupSync();
			}
			*/
			if (netFrameCounter >= framePacketSize)
			{
				if (incomingControls.Count > 0)
				{
					debugMessageCounter++;
					//lastDebugframeCounter = debugframeCounter;
					netFrameCounter = 0;
					timeOutTimer = 0;
					//Console.WriteLine("Recieved Controls");
					Console.WriteLine("Mes: " + debugMessageCounter
						+ " | Sent: " + myControls[0] + ", " + myControls[1] + ", " + myControls[2] + ", " + myControls[3] + ", " + myControls[4] + ", " + myControls[5]
						+ " | In: " + incomingControls[0][0] + ", " + incomingControls[0][1] + ", " + incomingControls[0][2] + ", " + incomingControls[0][3] + ", " + incomingControls[0][4] + ", " + incomingControls[0][5]);
					return false;
				}
				lastDebugframeCounter++;
				timeOutTimer++;
				if(timeOutTimer > timeOutMax)
                {
					Disconnect();
                }
				Console.WriteLine("Waiting");
				return true;

			}
			return false;
        }
		public static void Disconnect()
        {
			if (server != null)
			{
				if (server.Connections.Count > 0)
				{
					NetOutgoingMessage sendMsg = server.CreateMessage();
					sendMsg.Write((byte)DataMessageType.Disconnect);
					server.SendMessage(sendMsg, server.Connections[0], NetDeliveryMethod.ReliableOrdered);
				}
				server.Shutdown("Server Closed");
				server = null;
			}
			if (client != null)
			{
				NetOutgoingMessage sendMsg = client.CreateMessage();
				sendMsg.Write((byte)DataMessageType.Disconnect);
				client.SendMessage(sendMsg, NetDeliveryMethod.ReliableOrdered);
				client.Disconnect("Player has left");
				client = null;
			}
		}
		public static bool IsConnected()
        {
			return client != null || server != null;
        }
		public static NetMode GetNetMode()
        {
			if(client != null)
            {
				return NetMode.client;
            }
			if(server != null)
            {
				return NetMode.server;
            }
			return NetMode.disconnected;
        }
		static void StartupSync()
        {
			Main.random = new Random(42);
			Main.Start();
			netFrameCounter = 1;
			Controls.Reset();
			debugMessageCounter = 0;
			lastDebugframeCounter = 0;
			incomingControls.Clear();
		}
		public static void ServerStartQuickPlay()
        {
			if (server.Connections.Count > 0)
			{
				NetOutgoingMessage sendMsg = server.CreateMessage();
				sendMsg.Write((byte)DataMessageType.StartQuickPlay);
				sendMsg.Write(Main.startAI[0]);
				sendMsg.Write(Main.startAI[1]);
				sendMsg.WritePadBits();
				server.SendMessage(sendMsg, server.Connections[0], NetDeliveryMethod.ReliableOrdered);
			}
			StartQuickPlay();
		}
		static void StartQuickPlay()
		{
			Main.StartQuickPlay();
			//StartupSync();
		}
		static int debugMessageCounter = 0;
		static int lastDebugframeCounter = 0;
		public static void DisplayDebug(SpriteBatch spriteBatch)
        {
			spriteBatch.DrawString(Main.font, "Message: " + debugMessageCounter, new Vector2(10, 20), Color.Green, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
			spriteBatch.DrawString(Main.font, "Lag: " + lastDebugframeCounter, new Vector2(10, 60), Color.Green, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}
	}
	public enum NetMode : byte
    {
		disconnected,
		client,
		server
    }
	public enum DataMessageType : byte
    {
		Controls,
		Disconnect,
		StartQuickPlay,
		ConfirmQuickPlay
    }
}
