// Decompiled with JetBrains decompiler
// Type: LookatHealth
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class LookatHealth : MonoBehaviour
{
  public static bool Enabled = true;
  public GameObject container;
  public Text textHealth;
  public Text textStability;
  public Image healthBar;
  public Image healthBarBG;
  public Color barBGColorNormal;
  public Color barBGColorUnstable;

  public LookatHealth()
  {
    base.\u002Ector();
  }
}
