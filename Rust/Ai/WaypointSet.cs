// Decompiled with JetBrains decompiler
// Type: Rust.Ai.WaypointSet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
  public class WaypointSet : MonoBehaviour, IServerComponent
  {
    [SerializeField]
    private List<WaypointSet.Waypoint> _points;
    [SerializeField]
    private WaypointSet.NavModes navMode;

    public List<WaypointSet.Waypoint> Points
    {
      get
      {
        return this._points;
      }
      set
      {
        this._points = value;
      }
    }

    public WaypointSet.NavModes NavMode
    {
      get
      {
        return this.navMode;
      }
    }

    private void OnDrawGizmos()
    {
      for (int index = 0; index < this.Points.Count; ++index)
      {
        Transform transform = this.Points[index].Transform;
        if (Object.op_Inequality((Object) transform, (Object) null))
        {
          if (this.Points[index].IsOccupied)
            Gizmos.set_color(Color.get_red());
          else
            Gizmos.set_color(Color.get_cyan());
          Gizmos.DrawSphere(transform.get_position(), 0.25f);
          Gizmos.set_color(Color.get_cyan());
          if (index + 1 < this.Points.Count)
            Gizmos.DrawLine(transform.get_position(), this.Points[index + 1].Transform.get_position());
          else if (this.NavMode == WaypointSet.NavModes.Loop)
            Gizmos.DrawLine(transform.get_position(), this.Points[0].Transform.get_position());
          Gizmos.set_color(Color.op_Subtraction(Color.get_magenta(), new Color(0.0f, 0.0f, 0.0f, 0.5f)));
          foreach (Transform lookatPoint in this.Points[index].LookatPoints)
          {
            Gizmos.DrawSphere(lookatPoint.get_position(), 0.1f);
            Gizmos.DrawLine(transform.get_position(), lookatPoint.get_position());
          }
        }
      }
    }

    public WaypointSet()
    {
      base.\u002Ector();
    }

    public enum NavModes
    {
      Loop,
      PingPong,
    }

    [Serializable]
    public struct Waypoint
    {
      public Transform Transform;
      public float WaitTime;
      public Transform[] LookatPoints;
      [NonSerialized]
      public bool IsOccupied;
    }
  }
}
