// Decompiled with JetBrains decompiler
// Type: UIStyle_Menu_Panel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIStyle_Menu_Panel : MonoBehaviour, IClientComponent
{
  public bool toggle;

  private void OnValidate()
  {
    ((Graphic) ((Component) this).GetComponent<Image>()).set_color(Color32.op_Implicit(new Color32((byte) 29, (byte) 32, (byte) 31, byte.MaxValue)));
  }

  public UIStyle_Menu_Panel()
  {
    base.\u002Ector();
  }
}
