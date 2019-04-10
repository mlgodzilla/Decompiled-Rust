// Decompiled with JetBrains decompiler
// Type: EnvironmentVolumePropertiesCollection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Environment Volume Properties Collection")]
public class EnvironmentVolumePropertiesCollection : ScriptableObject
{
  public float TransitionSpeed;
  public EnvironmentVolumeProperties[] Properties;

  public EnvironmentVolumeProperties FindQuality(int quality)
  {
    foreach (EnvironmentVolumeProperties property in this.Properties)
    {
      if (property.ReflectionQuality == quality)
        return property;
    }
    return (EnvironmentVolumeProperties) null;
  }

  public EnvironmentVolumePropertiesCollection()
  {
    base.\u002Ector();
  }
}
