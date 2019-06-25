using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace URS.entity
{
  [Serializable]
  public class BaseEntity
  {
    public string type;
    public string eventName;
  }

  enum EventName {
    SyncTransform
  }
}
