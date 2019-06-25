using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace URS.entity
{
  [Serializable]
  public class TransformEntity : BaseEntity
  {
    public string viewID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformEntity(string _eventName, string _viewID, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
      type = "TransformEntity";
      eventName = _eventName;
      viewID = _viewID;
      position = _position;
      rotation = _rotation;
      scale = _scale;
    }
  }
}