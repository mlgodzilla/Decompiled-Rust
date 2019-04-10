// Decompiled with JetBrains decompiler
// Type: WaterInteraction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

[ExecuteInEditMode]
public class WaterInteraction : MonoBehaviour
{
  [SerializeField]
  private Texture2D texture;
  [Range(0.0f, 1f)]
  public float Displacement;
  [Range(0.0f, 1f)]
  public float Disturbance;
  private Transform cachedTransform;

  public Texture2D Texture
  {
    get
    {
      return this.texture;
    }
    set
    {
      this.texture = value;
      this.CheckRegister();
    }
  }

  public WaterDynamics.Image Image { get; private set; }

  public Vector2 Position { get; private set; }

  public Vector2 Scale { get; private set; }

  public float Rotation { get; private set; }

  protected void OnEnable()
  {
    this.CheckRegister();
    this.UpdateTransform();
  }

  protected void OnDisable()
  {
    this.Unregister();
  }

  public void CheckRegister()
  {
    if (!((Behaviour) this).get_enabled() || Object.op_Equality((Object) this.texture, (Object) null))
    {
      this.Unregister();
    }
    else
    {
      if (this.Image != null && !Object.op_Inequality((Object) this.Image.texture, (Object) this.texture))
        return;
      this.Register();
    }
  }

  private void UpdateImage()
  {
    this.Image = new WaterDynamics.Image(this.texture);
  }

  private void Register()
  {
    this.UpdateImage();
    WaterDynamics.RegisterInteraction(this);
  }

  private void Unregister()
  {
    if (this.Image == null)
      return;
    WaterDynamics.UnregisterInteraction(this);
    this.Image = (WaterDynamics.Image) null;
  }

  public void UpdateTransform()
  {
    this.cachedTransform = Object.op_Inequality((Object) this.cachedTransform, (Object) null) ? this.cachedTransform : ((Component) this).get_transform();
    if (!this.cachedTransform.get_hasChanged())
      return;
    Vector3 position = this.cachedTransform.get_position();
    Vector3 lossyScale = this.cachedTransform.get_lossyScale();
    this.Position = new Vector2((float) position.x, (float) position.z);
    this.Scale = new Vector2((float) lossyScale.x, (float) lossyScale.z);
    Quaternion rotation = this.cachedTransform.get_rotation();
    this.Rotation = (float) ((Quaternion) ref rotation).get_eulerAngles().y;
    this.cachedTransform.set_hasChanged(false);
  }

  public WaterInteraction()
  {
    base.\u002Ector();
  }
}
