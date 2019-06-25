using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace URS.Sample
{
  public class SyncCameraTransform : MonoBehaviour
  {
    private Camera camera;

    void Start()
    {
      camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
#else
      transform.position = camera.transform.position;
      transform.rotation = camera.transform.rotation;
      transform.localScale = camera.transform.localScale;
#endif
    }
  }
}