// Decompiled with JetBrains decompiler
// Type: Tooltip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class Tooltip : BaseMonoBehaviour, IClientComponent
{
  public string token = "";
  public static GameObject Current;
  [TextArea]
  public string Text;
  public GameObject TooltipObject;

  public string english
  {
    get
    {
      return this.Text;
    }
  }
}
