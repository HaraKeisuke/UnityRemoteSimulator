using UnityEngine;
using System.Collections;
using UnityEditor;

public class TestView : MonoBehaviour
{
  public int listenPort = 2222; // ポートはサーバ・クライアントで合わせる必要がある
  private static bool sent = false;
  private Coroutine coroutine_ = null;

  public static void SendCallback(System.IAsyncResult ar)
  {
    // System.Net.Sockets.UdpClient u = (System.Net.Sockets.UdpClient)ar.AsyncState;
    sent = true;
  }

  IEnumerator send(string server, string message)
  {
    Debug.Log("sending..");
    var u = new System.Net.Sockets.UdpClient();
    u.EnableBroadcast = true;
    u.Connect(server, listenPort);
    var sendBytes = System.Text.Encoding.ASCII.GetBytes(message);
    sent = false;
    u.BeginSend(sendBytes, sendBytes.Length,
          new System.AsyncCallback(SendCallback), u);
    while (!sent)
    {
      yield return null;
    }
    u.Close();
    coroutine_ = null;
    Debug.Log("done.");
  }

  void Update()
  {
    if (coroutine_ == null && Input.GetKeyDown(KeyCode.Space))
    {
      string sendMsg = "space key!";
      string server = EditorPrefs.GetString("URSIPADDRESS");
      coroutine_ = StartCoroutine(send(server, sendMsg));
    }
  }
}