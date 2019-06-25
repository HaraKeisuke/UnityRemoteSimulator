using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URS.entity;
using System.Text;
using System.Threading.Tasks;

namespace URS.views
{
  public class TransformView : URSView
  {
    [SerializeField] bool EnablePosition;
    [SerializeField] bool EnableRotation;
    [SerializeField] bool EnableScale;
    public float span = 0.1f;
    private float currentTime = 0f;

    void Awake()
    {
      base.Initialize();
    }

    void Update()
    {
      currentTime += Time.deltaTime;
      if (currentTime > span)
      {
        currentTime = 0f;
        var entity = new TransformEntity("SyncTransform", this.viewID, transform.position, transform.rotation, transform.localScale);
        Send(JsonUtility.ToJson(entity));
      }
    }
  }
}