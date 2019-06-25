using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URS;

namespace URS.Sample
{
  public class SampleController : MonoBehaviour
  {
    void Start()
    {
      URSManager.Instance.Instantiate("Player", new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}