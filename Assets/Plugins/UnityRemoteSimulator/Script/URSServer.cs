
using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;
using System.Collections.Generic;

namespace URS
{
  public class URSServer : MonoBehaviour
  {
    public int listenPort = 2222;
    private static bool received = false;
    private static List<Action<string>> actions = new List<Action<string>>();

    private struct UdpState
    {
      public System.Net.IPEndPoint e;
      public System.Net.Sockets.UdpClient u;
    }

    public static void ReceiveCallback(System.IAsyncResult ar)
    {
      System.Net.Sockets.UdpClient u = (System.Net.Sockets.UdpClient)((UdpState)(ar.AsyncState)).u;
      System.Net.IPEndPoint e = (System.Net.IPEndPoint)((UdpState)(ar.AsyncState)).e;
      var receiveBytes = u.EndReceive(ar, ref e);
      var receiveString = System.Text.Encoding.ASCII.GetString(receiveBytes);

      Debug.Log(string.Format("Received: {0}", receiveString)); // ここに任意の処理を書く

      received = true;

      actions.ForEach(action =>
      {
        action(receiveString);
      });
    }

    IEnumerator receive_loop()
    {
      var e = new System.Net.IPEndPoint(System.Net.IPAddress.Any, listenPort);
      var u = new System.Net.Sockets.UdpClient(e);
      u.EnableBroadcast = true;
      var s = new UdpState();
      s.e = e;
      s.u = u;
      for (; ; )
      {
        received = false;
        u.BeginReceive(new System.AsyncCallback(ReceiveCallback), s);
        while (!received)
        {
          yield return null;
        }
      }
    }

    public void AddEventListener(Action<string> action)
    {
      actions.Add(action);
    }

    void Start()
    {
      StartCoroutine(receive_loop());
    }
  }
}


// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using UniRx;
// using System.Text;
// using System;

// namespace URS
// {
//   public class URSServer
//   {
//     private Subject<UDPMessage> messageReceivedStream = new Subject<UDPMessage>();
//     public IObservable<UDPMessage> MessageReceivedStream => messageReceivedStream;

//     void Awake()
//     {
//       ListenMessage();
//     }

//     public async void ListenMessage()
//     {
//       // 接続ソケットの準備
//       var local = new IPEndPoint(IPAddress.Any, 8000);
//       var remote = new IPEndPoint(IPAddress.Any, 8000);
//       var client = new UdpClient(local);

//       while (true)
//       {
//         // データ受信待機
//         var result = await client.ReceiveAsync();

//         // 受信したデータを変換
//         var data = Encoding.UTF8.GetString(result.Buffer);

//         // Receive イベント を実行
//         this.OnRecieve(data);
//       }
//     }

//     private void OnRecieve(string data)
//     {
//       // 受信したときの処理
//       Debug.Log("Receive Message");
//     }
//   }

//   public class UDPMessage
//   {
//     public string json;
//   }
// }


// // //UdpReceiverRx.cs
// // //UniRxを用いて高速なUdp受信を行う

// // namespace URS
// // {
// //   using UnityEngine;
// //   using System.Net;
// //   using System.Net.Sockets;
// //   using UniRx;
// //   using System;

// //   public class UdpState : System.IEquatable<UdpState>
// //   {
// //     //UDP通信の情報を収める。送受信ともに使える
// //     public IPEndPoint EndPoint { get; set; }
// //     public string UdpMsg { get; set; }

// //     public UdpState(IPEndPoint ep, string udpMsg)
// //     {
// //       this.EndPoint = ep;
// //       this.UdpMsg = udpMsg;
// //     }
// //     public override int GetHashCode()
// //     {
// //       return EndPoint.Address.GetHashCode();
// //     }

// //     public bool Equals(UdpState s)
// //     {
// //       if (s == null)
// //       {
// //         return false;
// //       }
// //       return EndPoint.Address.Equals(s.EndPoint.Address);
// //     }
// //   }

// //   public class URSServer : MonoBehaviour
// //   {
// //     private const int listenPort = 10000;
// //     private static UdpClient myClient;
// //     private bool isAppQuitting;
// //     public IObservable<UdpState> sequence;

// //     void Awake()
// //     {
// // #if UNITY_EDITOR
// //       sequence = Observable.Create<UdpState>(observer =>
// //       {
// //         Debug.Log(string.Format("_udpSequence thread: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId));
// //         try
// //         {
// //           myClient = new UdpClient(listenPort);
// //         }
// //         catch (SocketException ex)
// //         {
// //           observer.OnError(ex);
// //         }
// //         IPEndPoint remoteEP = null;
// //         myClient.EnableBroadcast = true;
// //         myClient.Client.ReceiveTimeout = 5000;
// //         while (!isAppQuitting)
// //         {
// //           try
// //           {
// //             remoteEP = null;
// //             var receivedMsg = System.Text.Encoding.ASCII.GetString(myClient.Receive(ref remoteEP));
// //             observer.OnNext(new UdpState(remoteEP, receivedMsg));
// //           }
// //           catch (SocketException)
// //           {
// //             Debug.Log("UDP::Receive timeout");
// //           }
// //         }
// //         observer.OnCompleted();
// //         return null;
// //       })
// //       .SubscribeOn(Scheduler.ThreadPool)
// //       .Publish()
// //       .RefCount();
// // #endif
// //     }

// //     void OnApplicationQuit()
// //     {
// // #if UNITY_EDITOR
// //       isAppQuitting = true;
// //       myClient.Client.Blocking = false;
// // #endif
// //     }
// //   }
// // }