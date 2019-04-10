// Decompiled with JetBrains decompiler
// Type: Smaa.SMAA
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

namespace Smaa
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Subpixel Morphological Antialiasing")]
  public class SMAA : MonoBehaviour
  {
    public DebugPass DebugPass;
    public QualityPreset Quality;
    public EdgeDetectionMethod DetectionMethod;
    public bool UsePredication;
    public Preset CustomPreset;
    public PredicationPreset CustomPredicationPreset;
    public Shader Shader;
    public Texture2D AreaTex;
    public Texture2D SearchTex;
    protected Camera m_Camera;
    protected Preset m_LowPreset;
    protected Preset m_MediumPreset;
    protected Preset m_HighPreset;
    protected Preset m_UltraPreset;
    protected Material m_Material;

    public Material Material
    {
      get
      {
        if (Object.op_Equality((Object) this.m_Material, (Object) null))
        {
          this.m_Material = new Material(this.Shader);
          ((Object) this.m_Material).set_hideFlags((HideFlags) 61);
        }
        return this.m_Material;
      }
    }

    public SMAA()
    {
      base.\u002Ector();
    }
  }
}
