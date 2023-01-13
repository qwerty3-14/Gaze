using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GazeOGL.Entities.Ships;
using GazeOGL.MyraUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib.Utils;

namespace GazeOGL
{
    public static class Networking
    {
		//static NetServer server;
		//static NetClient client;
		public static List<bool[]> incomingControls = new List<bool[]>();
		public static bool[] myControls = null;
		public static int framePacketSize = 4;
		static int timeOutTimer = 0;
		const int timeOutMax = 600;
		static bool awaitResponse = false;
		public static void StartServer()
        {
			
			
		}
		public static void StartClient()
        {
			
			
		}
		public static void RunServer()
		{
			EventBasedNetListener listener = new EventBasedNetListener();
			NetManager server = new NetManager(listener);
			server.Start(9050 /* port */);

			listener.ConnectionRequestEvent += request =>
			{
				if(server.ConnectedPeersCount < 1)
					request.AcceptIfKey("SomeConnectionKey");
				else
					request.Reject();
			};

			listener.PeerConnectedEvent += peer =>
			{
				Console.WriteLine("We got connection: {0}", peer.EndPoint); // Show peer ip
				NetDataWriter writer = new NetDataWriter();                 // Create writer class
				writer.Put("Hello client!");                                // Put some string
				peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
			};

			while (!Console.KeyAvailable)
			{
				server.PollEvents();
				Thread.Sleep(15);
			}
			server.Stop();
			if (!awaitResponse)
			{
				/*
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
				*/
			}
			// server recieve messages
		}
		public static void RunClient()
		{
			EventBasedNetListener listener = new EventBasedNetListener();
			NetManager client = new NetManager(listener);
			client.Start();
			client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
			listener.NetworkReceiveEvent += (fromPeer, dataReader, channel, deliveryMethod) =>
			{
				Console.WriteLine("We got: {0}", dataReader.GetString(100 ));
				dataReader.Recycle();
			};

			while (!Console.KeyAvailable)
			{
				client.PollEvents();
				Thread.Sleep(15);
			}

			client.Stop();
			/*
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
			*/
			//client recieves messages
		}
		public static int netFrameCounter = 0;
		public static bool Update()
        {
			if (!IsConnected())
            {
				return false;
            }
			/*
			if(server != null)
            {
				RunServer();
			}
			if(client != null)
            {
				RunClient();
			}
			*/
			if(awaitResponse)
			{
				return true;
			}
			
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
				//Console.WriteLine("Waiting");
				return true;

			}
			return false;
        }
		public static void Disconnect()
        {
		}
		public static bool IsConnected()
        {
			return false;
        }
		public static NetMode GetNetMode()
        {
			/*
			if(client != null)
            {
				return NetMode.client;
            }
			if(server != null)
            {
				return NetMode.server;
            }
			*/
			return NetMode.disconnected;
        }
		static void StartupSync()
        {
			Main.startAI[0] = false;
			Main.startAI[1] = false;
			Main.random = new Random(42);
			Arena.Start();
			netFrameCounter = 1;
			Controls.Reset();
			debugMessageCounter = 0;
			lastDebugframeCounter = 0;
			incomingControls.Clear();
		}
		public static void ServerStartQuickPlay()
        {
			//tell client to start
			StartQuickPlay();
		}
		public static void ServerStartFleets()
		{
			/*
			if (server.Connections.Count > 0)
			{
				NetOutgoingMessage sendMsg = server.CreateMessage();
				sendMsg.Write((byte)DataMessageType.StartFleets);
				sendMsg.Write(Main.startAI[0]);
				sendMsg.Write(Main.startAI[1]);
				sendMsg.WritePadBits();
				FleetsManager.Repair();
				for (int t = 0; t <2; t++)
                {
					for(int i =0; i <12; i++)
                    {
						sendMsg.Write((byte)FleetsManager.fleets[t].ships[i]);
                    }
                }
				server.SendMessage(sendMsg, server.Connections[0], NetDeliveryMethod.ReliableOrdered);
			}
			*/
			Main.StartFleets();
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
		ConfirmQuickPlay,
		StartFleets
    }
}
