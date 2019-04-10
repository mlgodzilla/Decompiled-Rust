// Decompiled with JetBrains decompiler
// Type: AmbienceManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class AmbienceManager : SingletonComponent<AmbienceManager>, IClientComponent
{
  public List<AmbienceManager.EmitterTypeLimit> localEmitterLimits;
  public AmbienceManager.EmitterTypeLimit catchallEmitterLimit;
  public int maxActiveLocalEmitters;
  public int activeLocalEmitters;
  public List<AmbienceEmitter> cameraEmitters;
  public List<AmbienceEmitter> emittersInRange;
  public List<AmbienceEmitter> activeEmitters;
  public float localEmitterRange;
  public List<AmbienceZone> currentAmbienceZones;

  public AmbienceManager()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class EmitterTypeLimit
  {
    public int limit = 1;
    public List<AmbienceDefinitionList> ambience;
    public int active;
  }
}
