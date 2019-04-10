// Decompiled with JetBrains decompiler
// Type: TerrainPathChildObjects
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPathChildObjects : MonoBehaviour
{
  public bool Spline;
  public float Width;
  public float Offset;
  public float Fade;
  [InspectorFlags]
  public TerrainSplat.Enum Splat;
  [InspectorFlags]
  public TerrainTopology.Enum Topology;
  public InfrastructureType Type;

  protected void Awake()
  {
    List<Vector3> vector3List = new List<Vector3>();
    IEnumerator enumerator = ((Component) this).get_transform().GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
      {
        Transform current = (Transform) enumerator.Current;
        vector3List.Add(current.get_position());
      }
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
    if (vector3List.Count >= 2)
    {
      switch (this.Type)
      {
        case InfrastructureType.Road:
          TerrainMeta.Path.Roads.Add(new PathList("Road " + (object) TerrainMeta.Path.Roads.Count, vector3List.ToArray())
          {
            Width = this.Width,
            InnerFade = this.Fade * 0.5f,
            OuterFade = this.Fade * 0.5f,
            MeshOffset = this.Offset * 0.3f,
            TerrainOffset = this.Offset,
            Topology = (int) this.Topology,
            Splat = (int) this.Splat,
            Spline = this.Spline
          });
          break;
        case InfrastructureType.Power:
          TerrainMeta.Path.Powerlines.Add(new PathList("Powerline " + (object) TerrainMeta.Path.Powerlines.Count, vector3List.ToArray())
          {
            Width = this.Width,
            InnerFade = this.Fade * 0.5f,
            OuterFade = this.Fade * 0.5f,
            MeshOffset = this.Offset * 0.3f,
            TerrainOffset = this.Offset,
            Topology = (int) this.Topology,
            Splat = (int) this.Splat,
            Spline = this.Spline
          });
          break;
      }
    }
    GameManager.Destroy(((Component) this).get_gameObject(), 0.0f);
  }

  protected void OnDrawGizmos()
  {
    bool flag = false;
    Vector3 a = Vector3.get_zero();
    IEnumerator enumerator = ((Component) this).get_transform().GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
      {
        Vector3 position = ((Transform) enumerator.Current).get_position();
        if (flag)
        {
          Gizmos.set_color(new Color(0.5f, 0.5f, 0.5f, 1f));
          GizmosUtil.DrawWirePath(a, position, 0.5f * this.Width);
        }
        a = position;
        flag = true;
      }
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
  }

  public TerrainPathChildObjects()
  {
    base.\u002Ector();
  }
}
