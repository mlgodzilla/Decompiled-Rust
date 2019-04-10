// Decompiled with JetBrains decompiler
// Type: UIScale
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;
using UnityEngine.UI;

public class UIScale : MonoBehaviour
{
  public CanvasScaler scaler;

  private void Update()
  {
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector(1280f / Graphics.uiscale, 720f / Graphics.uiscale);
    if (!Vector2.op_Inequality(this.scaler.get_referenceResolution(), vector2))
      return;
    this.scaler.set_referenceResolution(vector2);
  }

  public UIScale()
  {
    base.\u002Ector();
  }
}
