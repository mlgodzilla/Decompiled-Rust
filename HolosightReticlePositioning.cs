// Decompiled with JetBrains decompiler
// Type: HolosightReticlePositioning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class HolosightReticlePositioning : MonoBehaviour
{
  public IronsightAimPoint aimPoint;

  public RectTransform rectTransform
  {
    get
    {
      return ((Component) this).get_transform() as RectTransform;
    }
  }

  private void Update()
  {
    if (!MainCamera.isValid)
      return;
    this.UpdatePosition(MainCamera.mainCamera);
  }

  private void UpdatePosition(Camera cam)
  {
    Vector3 position = ((Component) this.aimPoint.targetPoint).get_transform().get_position();
    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, position);
    RectTransformUtility.ScreenPointToLocalPointInRectangle(((Transform) this.rectTransform).get_parent() as RectTransform, screenPoint, cam, ref screenPoint);
    ref __Null local1 = ref screenPoint.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    double num1 = (double) ^(float&) ref local1;
    Rect rect1 = (((Transform) this.rectTransform).get_parent() as RectTransform).get_rect();
    double num2 = (double) ((Rect) ref rect1).get_width() * 0.5;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = (float) (num1 / num2);
    ref __Null local2 = ref screenPoint.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    double num3 = (double) ^(float&) ref local2;
    Rect rect2 = (((Transform) this.rectTransform).get_parent() as RectTransform).get_rect();
    double num4 = (double) ((Rect) ref rect2).get_height() * 0.5;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = (float) (num3 / num4);
    this.rectTransform.set_anchoredPosition(screenPoint);
  }

  public HolosightReticlePositioning()
  {
    base.\u002Ector();
  }
}
