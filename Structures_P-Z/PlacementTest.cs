// Decompiled with JetBrains decompiler
// Type: PlacementTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class PlacementTest : MonoBehaviour
{
  public MeshCollider myMeshCollider;
  public Transform testTransform;
  public Transform visualTest;
  public float hemisphere;
  public float clampTest;
  public float testDist;
  private float nextTest;

  public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
  {
    degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
    Vector2 insideUnitCircle = Random.get_insideUnitCircle();
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) insideUnitCircle.x * degreesOffset, Random.Range(-1f, 1f) * degreesOffset, (float) insideUnitCircle.y * degreesOffset);
    Vector3 vector3_2 = Vector3.op_Addition(input, vector3_1);
    return ((Vector3) ref vector3_2).get_normalized();
  }

  public Vector3 RandomCylinderPointAroundVector(
    Vector3 input,
    float distance,
    float minHeight = 0.0f,
    float maxHeight = 0.0f)
  {
    Vector2 insideUnitCircle = Random.get_insideUnitCircle();
    Vector3 vector3 = new Vector3((float) insideUnitCircle.x, 0.0f, (float) insideUnitCircle.y);
    Vector3 normalized = ((Vector3) ref vector3).get_normalized();
    return new Vector3((float) normalized.x * distance, Random.Range(minHeight, maxHeight), (float) normalized.z * distance);
  }

  public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
  {
    degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
    Vector3 vector3_1 = Vector3.op_Addition(hemiInput, Vector3.op_Multiply(Vector3.get_one(), degreesOffset));
    Vector3 normalized1 = ((Vector3) ref vector3_1).get_normalized();
    Vector3 vector3_2 = Vector3.op_Addition(hemiInput, Vector3.op_Multiply(Vector3.get_one(), -degreesOffset));
    Vector3 normalized2 = ((Vector3) ref vector3_2).get_normalized();
    for (int index = 0; index < 3; ++index)
      ((Vector3) ref inputVec).set_Item(index, Mathf.Clamp(((Vector3) ref inputVec).get_Item(index), ((Vector3) ref normalized2).get_Item(index), ((Vector3) ref normalized1).get_Item(index)));
    return ((Vector3) ref inputVec).get_normalized();
  }

  private void Update()
  {
    if ((double) Time.get_realtimeSinceStartup() < (double) this.nextTest)
      return;
    this.nextTest = Time.get_realtimeSinceStartup() + 0.0f;
    ((Component) this.testTransform).get_transform().set_position(((Component) this).get_transform().TransformPoint(this.RandomCylinderPointAroundVector(Vector3.get_up(), 0.5f, 0.25f, 0.5f)));
    if (!Object.op_Inequality((Object) this.testTransform, (Object) null) || !Object.op_Inequality((Object) this.visualTest, (Object) null))
      return;
    Vector3 vector3_1 = ((Component) this).get_transform().get_position();
    MeshCollider meshCollider = this.myMeshCollider;
    Vector3 position = this.testTransform.get_position();
    Vector3 vector3_2 = Vector3.op_Subtraction(((Component) this).get_transform().get_position(), this.testTransform.get_position());
    Vector3 normalized = ((Vector3) ref vector3_2).get_normalized();
    Ray ray = new Ray(position, normalized);
    RaycastHit raycastHit;
    ref RaycastHit local = ref raycastHit;
    if (((Collider) meshCollider).Raycast(ray, ref local, 5f))
      vector3_1 = ((RaycastHit) ref raycastHit).get_point();
    else
      Debug.LogError((object) "Missed");
    ((Component) this.visualTest).get_transform().set_position(vector3_1);
  }

  public void OnDrawGizmos()
  {
  }

  public PlacementTest()
  {
    base.\u002Ector();
  }
}
