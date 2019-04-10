// Decompiled with JetBrains decompiler
// Type: ExplosionsShaderFloatCurves
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsShaderFloatCurves : MonoBehaviour
{
  public string ShaderProperty;
  public int MaterialID;
  public AnimationCurve FloatPropertyCurve;
  public float GraphTimeMultiplier;
  public float GraphScaleMultiplier;
  private bool canUpdate;
  private Material matInstance;
  private int propertyID;
  private float startTime;

  private void Start()
  {
    Material[] materials = ((Renderer) ((Component) this).GetComponent<Renderer>()).get_materials();
    if (this.MaterialID >= materials.Length)
      Debug.Log((object) "ShaderColorGradient: Material ID more than shader materials count.");
    this.matInstance = materials[this.MaterialID];
    if (!this.matInstance.HasProperty(this.ShaderProperty))
      Debug.Log((object) ("ShaderColorGradient: Shader not have \"" + this.ShaderProperty + "\" property"));
    this.propertyID = Shader.PropertyToID(this.ShaderProperty);
  }

  private void OnEnable()
  {
    this.startTime = Time.get_time();
    this.canUpdate = true;
  }

  private void Update()
  {
    float num = Time.get_time() - this.startTime;
    if (this.canUpdate)
      this.matInstance.SetFloat(this.propertyID, this.FloatPropertyCurve.Evaluate(num / this.GraphTimeMultiplier) * this.GraphScaleMultiplier);
    if ((double) num < (double) this.GraphTimeMultiplier)
      return;
    this.canUpdate = false;
  }

  public ExplosionsShaderFloatCurves()
  {
    base.\u002Ector();
  }
}
