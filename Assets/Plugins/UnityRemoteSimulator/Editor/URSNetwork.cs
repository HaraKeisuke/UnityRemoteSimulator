using UnityEngine;
using UnityEditor;
using System.Net;
using System.Linq;


namespace URS
{
  [InitializeOnLoad]
  public static class URSNetwork
  {
    public static string IPAddress = "";
    static URSNetwork()
    {
      string hostname = Dns.GetHostName();
      System.Collections.Generic.List<IPAddress> adrList = Dns.GetHostAddresses(hostname).ToList();

      IPAddress = adrList.Where(adr => adr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToArray().First().ToString();
      EditorPrefs.SetString("URSIPADDRESS", IPAddress);
    }
  }
}