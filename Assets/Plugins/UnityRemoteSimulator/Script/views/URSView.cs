using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEditor;

namespace URS.views
{
  public class URSView : MonoBehaviour
  {
    public string viewID;
    private string host = "192.168.1.1";
    private int port = 2222;
    private Coroutine coroutine_ = null;
    public static bool sending = false;

    void Awake()
    {
      host = EditorPrefs.GetString("URSIPADDRESS");
    }

    public void Initialize()
    {
      viewID = gameObject.GetInstanceID().ToString();
    }


    public void Send(string message)
    {
      StartCoroutine(send(message));
    }

    public static void SendCallback(System.IAsyncResult ar)
    {
      // System.Net.Sockets.UdpClient u = (System.Net.Sockets.UdpClient)ar.AsyncState;
      sending = true;
    }

    IEnumerator send(string message)
    {
      var u = new UdpClient();
      u.EnableBroadcast = true;
      u.Connect(host, port);
      var sendBytes = System.Text.Encoding.ASCII.GetBytes(message);
      sending = false;
      u.BeginSend(sendBytes, sendBytes.Length,
            new System.AsyncCallback(SendCallback), u);
      while (!sending)
      {
        yield return null;
      }
      u.Close();
      coroutine_ = null;
    }
  }
}