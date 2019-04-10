// Decompiled with JetBrains decompiler
// Type: FlasherLight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class FlasherLight : IOEntity
{
  public float flashSpacing = 0.2f;
  public float flashBurstSpacing = 0.5f;
  public float flashOnTime = 0.1f;
  public int numFlashesPerBurst = 5;
  public EmissionToggle toggler;
  public Light myLight;
}
