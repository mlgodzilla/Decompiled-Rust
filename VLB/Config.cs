// Decompiled with JetBrains decompiler
// Type: VLB.Config
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.Serialization;

namespace VLB
{
  [HelpURL("http://saladgamer.com/vlb-doc/config/")]
  public class Config : ScriptableObject
  {
    public int geometryLayerID;
    public string geometryTag;
    public int geometryRenderQueue;
    public bool forceSinglePass;
    [SerializeField]
    [HighlightNull]
    private Shader beamShader1Pass;
    [HighlightNull]
    [SerializeField]
    [FormerlySerializedAs("beamShader")]
    [FormerlySerializedAs("BeamShader")]
    private Shader beamShader2Pass;
    public int sharedMeshSides;
    public int sharedMeshSegments;
    [Range(0.01f, 2f)]
    public float globalNoiseScale;
    public Vector3 globalNoiseVelocity;
    [HighlightNull]
    public TextAsset noise3DData;
    public int noise3DSize;
    [HighlightNull]
    public ParticleSystem dustParticlesPrefab;
    private static Config m_Instance;

    public Shader beamShader
    {
      get
      {
        if (!this.forceSinglePass)
          return this.beamShader2Pass;
        return this.beamShader1Pass;
      }
    }

    public Vector4 globalNoiseParam
    {
      get
      {
        return new Vector4((float) this.globalNoiseVelocity.x, (float) this.globalNoiseVelocity.y, (float) this.globalNoiseVelocity.z, this.globalNoiseScale);
      }
    }

    public void Reset()
    {
      this.geometryLayerID = 1;
      this.geometryTag = "Untagged";
      this.geometryRenderQueue = 3000;
      this.beamShader1Pass = Shader.Find("Hidden/VolumetricLightBeam1Pass");
      this.beamShader2Pass = Shader.Find("Hidden/VolumetricLightBeam2Pass");
      this.sharedMeshSides = 24;
      this.sharedMeshSegments = 5;
      this.globalNoiseScale = 0.5f;
      this.globalNoiseVelocity = Consts.NoiseVelocityDefault;
      this.noise3DData = Resources.Load("Noise3D_64x64x64") as TextAsset;
      this.noise3DSize = 64;
      this.dustParticlesPrefab = Resources.Load("DustParticles", typeof (ParticleSystem)) as ParticleSystem;
    }

    public ParticleSystem NewVolumetricDustParticles()
    {
      if (!Object.op_Implicit((Object) this.dustParticlesPrefab))
      {
        if (Application.get_isPlaying())
          Debug.LogError((object) "Failed to instantiate VolumetricDustParticles prefab.");
        return (ParticleSystem) null;
      }
      M0 m0 = Object.Instantiate<ParticleSystem>((M0) this.dustParticlesPrefab);
      ((ParticleSystem) m0).set_useAutoRandomSeed(false);
      ((Object) m0).set_name("Dust Particles");
      ((Object) ((Component) m0).get_gameObject()).set_hideFlags(Consts.ProceduralObjectsHideFlags);
      ((Component) m0).get_gameObject().SetActive(true);
      return (ParticleSystem) m0;
    }

    public static Config Instance
    {
      get
      {
        if (Object.op_Equality((Object) Config.m_Instance, (Object) null))
        {
          M0[] m0Array = Resources.LoadAll<Config>(nameof (Config));
          Debug.Assert((uint) m0Array.Length > 0U, string.Format("Can't find any resource of type '{0}'. Make sure you have a ScriptableObject of this type in a 'Resources' folder.", (object) typeof (Config)));
          Config.m_Instance = (Config) m0Array[0];
        }
        return Config.m_Instance;
      }
    }

    public Config()
    {
      base.\u002Ector();
    }
  }
}
