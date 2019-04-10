// Decompiled with JetBrains decompiler
// Type: UIParticle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.UI;

public class UIParticle : BaseMonoBehaviour
{
  public Vector2 Gravity = new Vector2(1000f, 1000f);
  public Vector2 InitialScale = Vector2.get_one();
  public Vector2 LifeTime;
  public Vector2 InitialX;
  public Vector2 InitialY;
  public Vector2 InitialDelay;
  public Vector2 ScaleVelocity;
  public Gradient InitialColor;
  private float lifetime;
  private float gravity;
  private Vector2 velocity;
  private float scaleVelocity;

  public static void Add(
    UIParticle particleSource,
    RectTransform spawnPosition,
    RectTransform particleCanvas)
  {
    M0 m0 = Object.Instantiate<GameObject>((M0) ((Component) particleSource).get_gameObject());
    ((GameObject) m0).get_transform().SetParent((Transform) spawnPosition, false);
    Transform transform = ((GameObject) m0).get_transform();
    Rect rect1 = spawnPosition.get_rect();
    double num1 = (double) Random.Range(0.0f, ((Rect) ref rect1).get_width());
    Rect rect2 = spawnPosition.get_rect();
    double num2 = (double) ((Rect) ref rect2).get_width() * spawnPosition.get_pivot().x;
    double num3 = num1 - num2;
    Rect rect3 = spawnPosition.get_rect();
    double num4 = (double) Random.Range(0.0f, ((Rect) ref rect3).get_height());
    Rect rect4 = spawnPosition.get_rect();
    double num5 = (double) ((Rect) ref rect4).get_height() * spawnPosition.get_pivot().y;
    double num6 = num4 - num5;
    Vector3 vector3 = new Vector3((float) num3, (float) num6, 0.0f);
    transform.set_localPosition(vector3);
    ((GameObject) m0).get_transform().SetParent((Transform) particleCanvas, true);
    ((GameObject) m0).get_transform().set_localScale(Vector3.get_one());
    ((GameObject) m0).get_transform().set_localRotation(Quaternion.get_identity());
  }

  private void Start()
  {
    Transform transform = ((Component) this).get_transform();
    transform.set_localScale(Vector3.op_Multiply(transform.get_localScale(), Random.Range((float) this.InitialScale.x, (float) this.InitialScale.y)));
    this.velocity.x = (__Null) (double) Random.Range((float) this.InitialX.x, (float) this.InitialX.y);
    this.velocity.y = (__Null) (double) Random.Range((float) this.InitialY.x, (float) this.InitialY.y);
    this.gravity = Random.Range((float) this.Gravity.x, (float) this.Gravity.y);
    this.scaleVelocity = Random.Range((float) this.ScaleVelocity.x, (float) this.ScaleVelocity.y);
    Image component = (Image) ((Component) this).GetComponent<Image>();
    if (Object.op_Implicit((Object) component))
      ((Graphic) component).set_color(this.InitialColor.Evaluate(Random.Range(0.0f, 1f)));
    this.lifetime = Random.Range((float) this.InitialDelay.x, (float) this.InitialDelay.y) * -1f;
    if ((double) this.lifetime < 0.0)
      ((CanvasGroup) ((Component) this).GetComponent<CanvasGroup>()).set_alpha(0.0f);
    this.Invoke(new Action(this.Die), Random.Range((float) this.LifeTime.x, (float) this.LifeTime.y) + this.lifetime * -1f);
  }

  private void Update()
  {
    if ((double) this.lifetime < 0.0)
    {
      this.lifetime += Time.get_deltaTime();
      if ((double) this.lifetime < 0.0)
        return;
      ((CanvasGroup) ((Component) this).GetComponent<CanvasGroup>()).set_alpha(1f);
    }
    else
      this.lifetime += Time.get_deltaTime();
    Vector3 position = ((Component) this).get_transform().get_position();
    Vector3 localScale = ((Component) this).get_transform().get_localScale();
    ref __Null local1 = ref this.velocity.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = ^(float&) ref local1 - this.gravity * Time.get_deltaTime();
    ref __Null local2 = ref position.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = ^(float&) ref local2 + (float) this.velocity.x * Time.get_deltaTime();
    ref __Null local3 = ref position.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local3 = ^(float&) ref local3 + (float) this.velocity.y * Time.get_deltaTime();
    Vector3 vector3 = Vector3.op_Addition(localScale, Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_one(), this.scaleVelocity), Time.get_deltaTime()));
    if (vector3.x <= 0.0 || vector3.y <= 0.0)
    {
      this.Die();
    }
    else
    {
      ((Component) this).get_transform().set_position(position);
      ((Component) this).get_transform().set_localScale(vector3);
    }
  }

  private void Die()
  {
    Object.Destroy((Object) ((Component) this).get_gameObject());
  }
}
