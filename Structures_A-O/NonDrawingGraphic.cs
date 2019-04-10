// Decompiled with JetBrains decompiler
// Type: NonDrawingGraphic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine.UI;

public class NonDrawingGraphic : Graphic
{
  public virtual void SetMaterialDirty()
  {
  }

  public virtual void SetVerticesDirty()
  {
  }

  protected virtual void OnPopulateMesh(VertexHelper vh)
  {
    vh.Clear();
  }

  public NonDrawingGraphic()
  {
    base.\u002Ector();
  }
}
