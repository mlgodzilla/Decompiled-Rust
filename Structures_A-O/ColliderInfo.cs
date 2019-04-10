// Decompiled with JetBrains decompiler
// Type: ColliderInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ColliderInfo : MonoBehaviour
{
  public const ColliderInfo.Flags FlagsNone = (ColliderInfo.Flags) 0;
  public const ColliderInfo.Flags FlagsEverything = (ColliderInfo.Flags) -1;
  public const ColliderInfo.Flags FlagsDefault = ColliderInfo.Flags.Usable | ColliderInfo.Flags.Shootable | ColliderInfo.Flags.Melee | ColliderInfo.Flags.Opaque;
  [InspectorFlags]
  public ColliderInfo.Flags flags;

  public bool HasFlag(ColliderInfo.Flags f)
  {
    return (this.flags & f) == f;
  }

  public void SetFlag(ColliderInfo.Flags f, bool b)
  {
    if (b)
      this.flags |= f;
    else
      this.flags &= ~f;
  }

  public bool Filter(HitTest info)
  {
    switch (info.type)
    {
      case HitTest.Type.ProjectileEffect:
      case HitTest.Type.Projectile:
        if ((this.flags & ColliderInfo.Flags.Shootable) == (ColliderInfo.Flags) 0)
          return false;
        break;
      case HitTest.Type.MeleeAttack:
        if ((this.flags & ColliderInfo.Flags.Melee) == (ColliderInfo.Flags) 0)
          return false;
        break;
      case HitTest.Type.Use:
        if ((this.flags & ColliderInfo.Flags.Usable) == (ColliderInfo.Flags) 0)
          return false;
        break;
    }
    return true;
  }

  public ColliderInfo()
  {
    base.\u002Ector();
  }

  [System.Flags]
  public enum Flags
  {
    Usable = 1,
    Shootable = 2,
    Melee = 4,
    Opaque = 8,
    Airflow = 16, // 0x00000010
  }
}
