// Decompiled with JetBrains decompiler
// Type: ConVar.Graphics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust.Workshop;
using UnityEngine;

namespace ConVar
{
  [ConsoleSystem.Factory("graphics")]
  public class Graphics : ConsoleSystem
  {
    private static float _shadowdistance = 800f;
    [ClientVar(Saved = true)]
    public static int shadowmode = 2;
    [ClientVar(Saved = true)]
    public static int shadowlights = 1;
    private static int _shadowquality = 1;
    [ClientVar(Saved = true)]
    public static bool grassshadows = false;
    [ClientVar(Saved = true)]
    public static bool contactshadows = false;
    [ClientVar(Saved = true)]
    public static float drawdistance = 2500f;
    private static float _fov = 75f;
    [ClientVar]
    public static bool hud = true;
    [ClientVar(Saved = true)]
    public static bool chat = true;
    [ClientVar(Saved = true)]
    public static bool branding = true;
    [ClientVar(Saved = true)]
    public static int compass = 1;
    [ClientVar(Saved = true)]
    public static bool dof = false;
    [ClientVar(Saved = true)]
    public static float dof_aper = 12f;
    [ClientVar(Saved = true)]
    public static float dof_blur = 1f;
    private static float _uiscale = 1f;
    private static int _anisotropic = 1;
    private static int _parallax = 0;
    private const float MinShadowDistance = 40f;
    private const float MaxShadowDistance2Split = 180f;
    private const float MaxShadowDistance4Split = 800f;

    [ClientVar(Help = "The currently selected quality level")]
    public static int quality
    {
      get
      {
        return QualitySettings.GetQualityLevel();
      }
      set
      {
        int shadowcascades = Graphics.shadowcascades;
        QualitySettings.SetQualityLevel(value, true);
        Graphics.shadowcascades = shadowcascades;
      }
    }

    public static float EnforceShadowDistanceBounds(float distance)
    {
      distance = QualitySettings.get_shadowCascades() != 1 ? (QualitySettings.get_shadowCascades() != 2 ? Mathf.Clamp(distance, 40f, 800f) : Mathf.Clamp(distance, 40f, 180f)) : Mathf.Clamp(distance, 40f, 40f);
      return distance;
    }

    [ClientVar(Saved = true)]
    public static float shadowdistance
    {
      get
      {
        return Graphics._shadowdistance;
      }
      set
      {
        Graphics._shadowdistance = value;
        QualitySettings.set_shadowDistance(Graphics.EnforceShadowDistanceBounds(Graphics._shadowdistance));
      }
    }

    [ClientVar(Saved = true)]
    public static int shadowcascades
    {
      get
      {
        return QualitySettings.get_shadowCascades();
      }
      set
      {
        QualitySettings.set_shadowCascades(value);
        QualitySettings.set_shadowDistance(Graphics.EnforceShadowDistanceBounds(Graphics.shadowdistance));
      }
    }

    [ClientVar(Saved = true)]
    public static int shadowquality
    {
      get
      {
        return Graphics._shadowquality;
      }
      set
      {
        Graphics._shadowquality = Mathf.Clamp(value, 0, 2);
        Graphics.shadowmode = Graphics._shadowquality + 1;
        KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_HIGH", SystemInfo.get_graphicsDeviceType() != 17 && Graphics._shadowquality >= 2);
      }
    }

    [ClientVar(Saved = true)]
    public static float fov
    {
      get
      {
        return Graphics._fov;
      }
      set
      {
        Graphics._fov = Mathf.Clamp(value, 70f, 90f);
      }
    }

    [ClientVar]
    public static float lodbias
    {
      get
      {
        return QualitySettings.get_lodBias();
      }
      set
      {
        QualitySettings.set_lodBias(Mathf.Clamp(value, 0.25f, 5f));
      }
    }

    [ClientVar(Saved = true)]
    public static int shaderlod
    {
      get
      {
        return Shader.get_globalMaximumLOD();
      }
      set
      {
        Shader.set_globalMaximumLOD(Mathf.Clamp(value, 100, 600));
      }
    }

    [ClientVar(Saved = true)]
    public static float uiscale
    {
      get
      {
        return Graphics._uiscale;
      }
      set
      {
        Graphics._uiscale = Mathf.Clamp(value, 0.5f, 1f);
      }
    }

    [ClientVar(Saved = true)]
    public static int af
    {
      get
      {
        return Graphics._anisotropic;
      }
      set
      {
        value = Mathf.Clamp(value, 1, 16);
        Texture.SetGlobalAnisotropicFilteringLimits(1, value);
        if (value <= 1)
          Texture.set_anisotropicFiltering((AnisotropicFiltering) 0);
        if (value > 1)
          Texture.set_anisotropicFiltering((AnisotropicFiltering) 1);
        Graphics._anisotropic = value;
      }
    }

    [ClientVar(Saved = true)]
    public static int parallax
    {
      get
      {
        return Graphics._parallax;
      }
      set
      {
        switch (value)
        {
          case 1:
            Shader.EnableKeyword("TERRAIN_PARALLAX_OFFSET");
            Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
            break;
          case 2:
            Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
            Shader.EnableKeyword("TERRAIN_PARALLAX_OCCLUSION");
            break;
          default:
            Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
            Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
            break;
        }
        Graphics._parallax = value;
      }
    }

    [ClientVar]
    public static bool itemskins
    {
      get
      {
        return (bool) WorkshopSkin.AllowApply;
      }
      set
      {
        WorkshopSkin.AllowApply = (__Null) (value ? 1 : 0);
      }
    }

    [ClientVar]
    public static float itemskintimeout
    {
      get
      {
        return (float) WorkshopSkin.DownloadTimeout;
      }
      set
      {
        WorkshopSkin.DownloadTimeout = (__Null) (double) value;
      }
    }

    public Graphics()
    {
      base.\u002Ector();
    }
  }
}
