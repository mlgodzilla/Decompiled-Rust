// Decompiled with JetBrains decompiler
// Type: UnityEngine.CoroutineEx
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;

namespace UnityEngine
{
  public static class CoroutineEx
  {
    public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private static Dictionary<float, WaitForSeconds> waitForSecondsBuffer = new Dictionary<float, WaitForSeconds>();

    public static WaitForSeconds waitForSeconds(float seconds)
    {
      WaitForSeconds waitForSeconds;
      if (!CoroutineEx.waitForSecondsBuffer.TryGetValue(seconds, out waitForSeconds))
      {
        waitForSeconds = new WaitForSeconds(seconds);
        CoroutineEx.waitForSecondsBuffer.Add(seconds, waitForSeconds);
      }
      return waitForSeconds;
    }

    public static WaitForSecondsRealtime waitForSecondsRealtime(float seconds)
    {
      return new WaitForSecondsRealtime(seconds);
    }
  }
}
