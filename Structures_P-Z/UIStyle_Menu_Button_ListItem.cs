// Decompiled with JetBrains decompiler
// Type: UIStyle_Menu_Button_ListItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class UIStyle_Menu_Button_ListItem : MonoBehaviour, IClientComponent
{
  public bool apply;

  private void OnValidate()
  {
    if (Object.op_Equality((Object) ((Component) this).GetComponent<Image>(), (Object) null) || Object.op_Equality((Object) ((Component) this).GetComponent<Button>(), (Object) null))
      return;
    ((Graphic) ((Component) this).GetComponent<Image>()).set_color(Color.get_white());
    ColorBlock colors = ((Selectable) ((Component) this).GetComponent<Button>()).get_colors();
    ((ColorBlock) ref colors).set_normalColor(Color32.op_Implicit(new Color32((byte) 43, (byte) 41, (byte) 36, byte.MaxValue)));
    ((ColorBlock) ref colors).set_highlightedColor(Color32.op_Implicit(new Color32((byte) 72, (byte) 86, (byte) 46, byte.MaxValue)));
    ((ColorBlock) ref colors).set_pressedColor(Color32.op_Implicit(new Color32((byte) 37, (byte) 86, (byte) 122, byte.MaxValue)));
    ((ColorBlock) ref colors).set_disabledColor(Color32.op_Implicit(new Color32((byte) 72, (byte) 86, (byte) 46, byte.MaxValue)));
    ((ColorBlock) ref colors).set_colorMultiplier(1f);
    ((ColorBlock) ref colors).set_fadeDuration(0.1f);
    ((Selectable) ((Component) this).GetComponent<Button>()).set_colors(colors);
  }

  public UIStyle_Menu_Button_ListItem()
  {
    base.\u002Ector();
  }
}
