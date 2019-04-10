// Decompiled with JetBrains decompiler
// Type: DecorTransform
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class DecorTransform : DecorComponent
{
  public Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);
  public Vector3 Rotation = new Vector3(0.0f, 0.0f, 0.0f);
  public Vector3 Scale = new Vector3(1f, 1f, 1f);

  public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
  {
    pos = Vector3.op_Addition(pos, Quaternion.op_Multiply(rot, Vector3.Scale(scale, this.Position)));
    rot = Quaternion.op_Multiply(Quaternion.Euler(this.Rotation), rot);
    scale = Vector3.Scale(scale, this.Scale);
  }
}
