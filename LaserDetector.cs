// Decompiled with JetBrains decompiler
// Type: LaserDetector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LaserDetector : BaseDetector
{
  public override void OnObjects()
  {
    foreach (BaseEntity entityContent in this.myTrigger.entityContents)
    {
      if (entityContent.IsVisible(Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(((Component) this).get_transform().get_forward(), 0.1f)), 4f))
      {
        base.OnObjects();
        break;
      }
    }
  }
}
