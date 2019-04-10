// Decompiled with JetBrains decompiler
// Type: LifeInfographicStat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class LifeInfographicStat : MonoBehaviour
{
  public LifeInfographicStat.DataType dataSource;

  public LifeInfographicStat()
  {
    base.\u002Ector();
  }

  public enum DataType
  {
    None,
    AliveTime_Short,
    SleepingTime_Short,
    KillerName,
    KillerWeapon,
    AliveTime_Long,
  }
}
