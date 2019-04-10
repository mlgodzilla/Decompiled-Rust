// Decompiled with JetBrains decompiler
// Type: GameAnalyticsSDK.Events.GA_SpecialEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

namespace GameAnalyticsSDK.Events
{
  public class GA_SpecialEvents : MonoBehaviour
  {
    private static int _frameCountAvg;
    private static float _lastUpdateAvg;
    private int _frameCountCrit;
    private float _lastUpdateCrit;
    private static int _criticalFpsCount;

    public void Start()
    {
      this.StartCoroutine(this.SubmitFPSRoutine());
      this.StartCoroutine(this.CheckCriticalFPSRoutine());
    }

    private IEnumerator SubmitFPSRoutine()
    {
      while (Application.get_isPlaying() && GameAnalytics.SettingsGA.SubmitFpsAverage)
      {
        yield return (object) new WaitForSeconds(30f);
        GA_SpecialEvents.SubmitFPS();
      }
    }

    private IEnumerator CheckCriticalFPSRoutine()
    {
      while (Application.get_isPlaying() && GameAnalytics.SettingsGA.SubmitFpsCritical)
      {
        yield return (object) new WaitForSeconds((float) GameAnalytics.SettingsGA.FpsCirticalSubmitInterval);
        this.CheckCriticalFPS();
      }
    }

    public void Update()
    {
      if (GameAnalytics.SettingsGA.SubmitFpsAverage)
        ++GA_SpecialEvents._frameCountAvg;
      if (!GameAnalytics.SettingsGA.SubmitFpsCritical)
        return;
      ++this._frameCountCrit;
    }

    public static void SubmitFPS()
    {
      if (GameAnalytics.SettingsGA.SubmitFpsAverage)
      {
        float num1 = Time.get_time() - GA_SpecialEvents._lastUpdateAvg;
        if ((double) num1 > 1.0)
        {
          float num2 = (float) GA_SpecialEvents._frameCountAvg / num1;
          GA_SpecialEvents._lastUpdateAvg = Time.get_time();
          GA_SpecialEvents._frameCountAvg = 0;
          if ((double) num2 > 0.0)
            GameAnalytics.NewDesignEvent("GA:AverageFPS", (float) (int) num2);
        }
      }
      if (!GameAnalytics.SettingsGA.SubmitFpsCritical || GA_SpecialEvents._criticalFpsCount <= 0)
        return;
      GameAnalytics.NewDesignEvent("GA:CriticalFPS", (float) GA_SpecialEvents._criticalFpsCount);
      GA_SpecialEvents._criticalFpsCount = 0;
    }

    public void CheckCriticalFPS()
    {
      if (!GameAnalytics.SettingsGA.SubmitFpsCritical)
        return;
      float num1 = Time.get_time() - this._lastUpdateCrit;
      if ((double) num1 < 1.0)
        return;
      double num2 = (double) this._frameCountCrit / (double) num1;
      this._lastUpdateCrit = Time.get_time();
      this._frameCountCrit = 0;
      double criticalThreshold = (double) GameAnalytics.SettingsGA.FpsCriticalThreshold;
      if (num2 > criticalThreshold)
        return;
      ++GA_SpecialEvents._criticalFpsCount;
    }

    public GA_SpecialEvents()
    {
      base.\u002Ector();
    }
  }
}
