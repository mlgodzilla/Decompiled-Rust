// Decompiled with JetBrains decompiler
// Type: CH47DropZone
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CH47DropZone : MonoBehaviour
{
  private static List<CH47DropZone> dropZones = new List<CH47DropZone>();
  public float lastDropTime;

  public void Awake()
  {
    if (CH47DropZone.dropZones.Contains(this))
      return;
    CH47DropZone.dropZones.Add(this);
  }

  public static CH47DropZone GetClosest(Vector3 pos)
  {
    float num1 = float.PositiveInfinity;
    CH47DropZone ch47DropZone = (CH47DropZone) null;
    foreach (CH47DropZone dropZone in CH47DropZone.dropZones)
    {
      float num2 = Vector3Ex.Distance2D(pos, ((Component) dropZone).get_transform().get_position());
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        ch47DropZone = dropZone;
      }
    }
    return ch47DropZone;
  }

  public void OnDestroy()
  {
    if (!CH47DropZone.dropZones.Contains(this))
      return;
    CH47DropZone.dropZones.Remove(this);
  }

  public float TimeSinceLastDrop()
  {
    return Time.get_time() - this.lastDropTime;
  }

  public void Used()
  {
    this.lastDropTime = Time.get_time();
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_yellow());
    Gizmos.DrawSphere(((Component) this).get_transform().get_position(), 5f);
  }

  public CH47DropZone()
  {
    base.\u002Ector();
  }
}
