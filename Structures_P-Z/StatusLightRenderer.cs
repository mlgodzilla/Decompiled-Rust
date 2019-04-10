// Decompiled with JetBrains decompiler
// Type: StatusLightRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class StatusLightRenderer : MonoBehaviour, IClientComponent
{
  public Material offMaterial;
  public Material onMaterial;
  private MaterialPropertyBlock propertyBlock;
  private Renderer targetRenderer;
  private Color lightColor;
  private Light targetLight;
  private int colorID;
  private int emissionID;

  protected void Awake()
  {
    this.propertyBlock = new MaterialPropertyBlock();
    this.targetRenderer = (Renderer) ((Component) this).GetComponent<Renderer>();
    this.targetLight = (Light) ((Component) this).GetComponent<Light>();
    this.colorID = Shader.PropertyToID("_Color");
    this.emissionID = Shader.PropertyToID("_EmissionColor");
  }

  public void SetOff()
  {
    if (Object.op_Implicit((Object) this.targetRenderer))
    {
      this.targetRenderer.set_sharedMaterial(this.offMaterial);
      this.targetRenderer.SetPropertyBlock((MaterialPropertyBlock) null);
    }
    if (!Object.op_Implicit((Object) this.targetLight))
      return;
    this.targetLight.set_color(Color.get_clear());
  }

  public void SetOn()
  {
    if (Object.op_Implicit((Object) this.targetRenderer))
    {
      this.targetRenderer.set_sharedMaterial(this.onMaterial);
      this.targetRenderer.SetPropertyBlock(this.propertyBlock);
    }
    if (!Object.op_Implicit((Object) this.targetLight))
      return;
    this.targetLight.set_color(this.lightColor);
  }

  public void SetRed()
  {
    this.propertyBlock.Clear();
    this.propertyBlock.SetColor(this.colorID, this.GetColor((byte) 197, (byte) 46, (byte) 0, byte.MaxValue));
    this.propertyBlock.SetColor(this.emissionID, this.GetColor((byte) 191, (byte) 0, (byte) 2, byte.MaxValue, 2.916925f));
    this.lightColor = this.GetColor(byte.MaxValue, (byte) 111, (byte) 102, byte.MaxValue);
    this.SetOn();
  }

  public void SetGreen()
  {
    this.propertyBlock.Clear();
    this.propertyBlock.SetColor(this.colorID, this.GetColor((byte) 19, (byte) 191, (byte) 13, byte.MaxValue));
    this.propertyBlock.SetColor(this.emissionID, this.GetColor((byte) 19, (byte) 191, (byte) 13, byte.MaxValue, 2.5f));
    this.lightColor = this.GetColor((byte) 156, byte.MaxValue, (byte) 102, byte.MaxValue);
    this.SetOn();
  }

  private Color GetColor(byte r, byte g, byte b, byte a)
  {
    return Color32.op_Implicit(new Color32(r, g, b, a));
  }

  private Color GetColor(byte r, byte g, byte b, byte a, float intensity)
  {
    return Color.op_Multiply(Color32.op_Implicit(new Color32(r, g, b, a)), intensity);
  }

  public StatusLightRenderer()
  {
    base.\u002Ector();
  }
}
