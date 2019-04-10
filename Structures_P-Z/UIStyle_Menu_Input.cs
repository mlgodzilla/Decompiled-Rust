// Decompiled with JetBrains decompiler
// Type: UIStyle_Menu_Input
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIStyle_Menu_Input : MonoBehaviour, IClientComponent
{
  public bool apply;

  private void OnValidate()
  {
    ((Graphic) ((Component) this).GetComponent<Image>()).set_color(Color.get_white());
    ColorBlock colors = ((Selectable) ((Component) this).GetComponent<InputField>()).get_colors();
    ((ColorBlock) ref colors).set_normalColor(Color32.op_Implicit(new Color32((byte) 43, (byte) 41, (byte) 36, byte.MaxValue)));
    ((ColorBlock) ref colors).set_highlightedColor(Color32.op_Implicit(new Color32((byte) 72, (byte) 86, (byte) 46, byte.MaxValue)));
    ((ColorBlock) ref colors).set_pressedColor(Color32.op_Implicit(new Color32((byte) 37, (byte) 86, (byte) 122, byte.MaxValue)));
    ((ColorBlock) ref colors).set_disabledColor(Color32.op_Implicit(new Color32((byte) 33, (byte) 31, (byte) 26, byte.MaxValue)));
    ((Selectable) ((Component) this).GetComponent<InputField>()).set_colors(colors);
  }

  public UIStyle_Menu_Input()
  {
    base.\u002Ector();
  }
}
