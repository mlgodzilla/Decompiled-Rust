// Decompiled with JetBrains decompiler
// Type: BoxStorage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class BoxStorage : StorageContainer
{
  public override Vector3 GetDropPosition()
  {
    return this.ClosestPoint(Vector3.op_Addition(base.GetDropPosition(), Vector3.op_Multiply(this.LastAttackedDir, 10f)));
  }
}
