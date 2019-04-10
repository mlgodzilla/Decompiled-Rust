// Decompiled with JetBrains decompiler
// Type: MainCamera
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Kino;
using Smaa;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.CinematicEffects;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class MainCamera : SingletonComponent<MainCamera>
{
  public static Camera mainCamera;
  public DepthOfField dof;
  public AmplifyOcclusionEffect ssao;
  public Motion motionBlur;
  public TOD_Rays shafts;
  public TonemappingColorGrading tonemappingColorGrading;
  public FXAA fxaa;
  public SMAA smaa;
  public PostProcessLayer post;
  public CC_SharpenAndVignette sharpenAndVignette;
  public SEScreenSpaceShadows contactShadows;
  public VisualizeTexelDensity visualizeTexelDensity;
  public EnvironmentVolumePropertiesCollection environmentVolumeProperties;

  public static bool isValid
  {
    get
    {
      if (Object.op_Inequality((Object) MainCamera.mainCamera, (Object) null))
        return ((Behaviour) MainCamera.mainCamera).get_enabled();
      return false;
    }
  }

  public static Vector3 position
  {
    get
    {
      return ((Component) MainCamera.mainCamera).get_transform().get_position();
    }
    set
    {
      ((Component) MainCamera.mainCamera).get_transform().set_position(value);
    }
  }

  public static Vector3 forward
  {
    get
    {
      return ((Component) MainCamera.mainCamera).get_transform().get_forward();
    }
    set
    {
      if ((double) ((Vector3) ref value).get_sqrMagnitude() <= 0.0)
        return;
      ((Component) MainCamera.mainCamera).get_transform().set_forward(value);
    }
  }

  public static Vector3 right
  {
    get
    {
      return ((Component) MainCamera.mainCamera).get_transform().get_right();
    }
    set
    {
      if ((double) ((Vector3) ref value).get_sqrMagnitude() <= 0.0)
        return;
      ((Component) MainCamera.mainCamera).get_transform().set_right(value);
    }
  }

  public static Vector3 up
  {
    get
    {
      return ((Component) MainCamera.mainCamera).get_transform().get_up();
    }
    set
    {
      if ((double) ((Vector3) ref value).get_sqrMagnitude() <= 0.0)
        return;
      ((Component) MainCamera.mainCamera).get_transform().set_up(value);
    }
  }

  public static Quaternion rotation
  {
    get
    {
      return ((Component) MainCamera.mainCamera).get_transform().get_rotation();
    }
  }

  public static Ray Ray
  {
    get
    {
      return new Ray(MainCamera.position, MainCamera.forward);
    }
  }

  public static RaycastHit Raycast
  {
    get
    {
      RaycastHit raycastHit;
      Physics.Raycast(MainCamera.Ray, ref raycastHit, 1024f, 229731073);
      return raycastHit;
    }
  }

  public MainCamera()
  {
    base.\u002Ector();
  }
}
