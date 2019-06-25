using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace URS.entity
{
  [Serializable]
  public class GameObjectEntity : BaseEntity
  {
    public string viewID;
    public string path;
    public bool isClient;

    public GameObjectEntity(string _eventName, string _viewID, string _path, bool _isClient = true)
    {
      type = "GameObjectEntity";
      eventName = _eventName;
      viewID = _viewID;
      path = _path;
      isClient = _isClient;
    }
  }
}