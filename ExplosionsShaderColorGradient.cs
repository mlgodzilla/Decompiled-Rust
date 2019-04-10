// Decompiled with JetBrains decompiler
// Type: ExplosionsShaderColorGradient
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionsShaderColorGradient : MonoBehaviour
{
  public string ShaderProperty;
  public int MaterialID;
  public Gradient Color;
  public float TimeMultiplier;
  private bool canUpdate;
  private Material matInstance;
  private int propertyID;
  private float startTime;
  private UnityEngine.Color oldColor;

  private void Start()
  {
    Material[] materials = ((Renderer) ((Component) this).GetComponent<Renderer>()).get_materials();
    if (this.MaterialID >= materials.Length)
      Debug.Log((object) "ShaderColorGradient: Material ID more than shader materials count.");
    this.matInstance = materials[this.MaterialID];
    if (!this.matInstance.HasProperty(this.ShaderProperty))
      Debug.Log((object) ("ShaderColorGradient: Shader not have \"" + this.ShaderProperty + "\" property"));
    this.propertyID = Shader.PropertyToID(this.ShaderProperty);
    this.oldColor = this.matInstance.GetColor(this.propertyID);
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
      this.matInstance.SetColor(this.propertyID, UnityEngine.Color.op_Multiply(this.Color.Evaluate(num / this.TimeMultiplier), this.oldColor));
    if ((double) num < (double) this.TimeMultiplier)
      return;
    this.canUpdate = false;
  }

  public ExplosionsShaderColorGradient()
  {
    base.\u002Ector();
  }
}
