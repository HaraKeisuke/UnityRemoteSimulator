using System.Collections.Generic;
using UnityEngine;
using System;
using URS.entity;
using URS.views;
using System.Linq;
using System.Collections;
using System.Net.Sockets;
using UnityEditor;

namespace URS
{
  public class URSManager : SingletonMonoBehaviour<URSManager>
  {
    [SerializeField] URSServer ursServer;
    List<URSView> views = new List<URSView>();

    public string viewID;
    private string host = "192.168.1.1";
    private int port = 2222;
    private Coroutine coroutine_ = null;
    public static bool sending = false;

    private List<Action> queue = new List<Action>();

    void Awake()
    {
      host = EditorPrefs.GetString("URSIPADDRESS");
    }

    void Start()
    {
#if UNITY_EDITOR
      InitializeGameObject(); // すでにインスタンス化されてるものを読み込む
      ursServer.AddEventListener((message) =>
      {
        try
        {
          var entity = JsonUtility.FromJson<BaseEntity>(message);
          switch (entity.type)
          {
            case "GameObjectEntity":
              var _entity = JsonUtility.FromJson<GameObjectEntity>(message);
              switch (_entity.eventName)
              {
                case "Instantiate":
                  queue.Add(() =>
                  {
                    CreateGameObject(_entity);
                  });
                  break;
              }
              break;
            case "TransformEntity":
              var _transform_entity = JsonUtility.FromJson<TransformEntity>(message);
              switch (_transform_entity.eventName)
              {
                case "SyncTransform":
                  queue.Add(() =>
                  {
                    SyncTransform(_transform_entity);
                  });
                  break;
              }
              break;
          }

        }
        catch (Exception e)
        {
          Debug.Log(e);
        }
      });
#endif
    }

    void Update()
    {
      if (queue.Count > 0)
      {
        queue.ForEach((action) => action());
        queue.RemoveAll((action) => true);
      }
    }

    void CreateGameObject(GameObjectEntity entity)
    {
      if (entity.isClient)
      {
        GameObject prefab = (GameObject)Resources.Load(entity.path);
        var go = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        var view = go.GetComponent<URSView>();
        view.viewID = entity.viewID;
        views.Add(view);
      }
    }

    void SyncTransform(TransformEntity entity)
    {
      var _view = views.Where((view) => view.viewID == entity.viewID).First();
      _view.transform.position = entity.position;
      _view.transform.rotation = entity.rotation;
      _view.transform.localScale = entity.scale;
    }

    public void InitializeGameObject()
    {
      var _views = (URSView[])FindObjectsOfType(typeof(URSView));
      foreach (var _view in _views)
      {
        views.Add(_view);
      }
    }

    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation)
    {
      var go = (GameObject)Resources.Load(path);
      var goI = Instantiate(go, position, rotation);
      var viewID = goI.GetInstanceID().ToString();

      var isClient = true;
#if UNITY_EDITOR
      var view = goI.GetComponent<URSView>();
      views.Add(view);
      isClient = false;
#endif

      var entity = new GameObjectEntity("Instantiate", viewID, path, isClient);
      Send(JsonUtility.ToJson(entity));
      return goI;
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