// Decompiled with JetBrains decompiler
// Type: VitalNote
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class VitalNote : MonoBehaviour, IClientComponent
{
  public VitalNote.Vital VitalType;
  public FloatConditions showIf;
  public Text valueText;
  public Animator animator;

  public VitalNote()
  {
    base.\u002Ector();
  }

  public enum Vital
  {
    Comfort,
    Radiation,
    Poison,
    Cold,
    Bleeding,
    Hot,
    Drowning,
    Wet,
    Hygiene,
    Starving,
    Dehydration,
  }
}
