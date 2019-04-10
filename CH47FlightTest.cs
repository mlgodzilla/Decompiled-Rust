// Decompiled with JetBrains decompiler
// Type: CH47FlightTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class CH47FlightTest : MonoBehaviour
{
  private static float altitudeTolerance = 1f;
  public Rigidbody rigidBody;
  public float engineThrustMax;
  public Vector3 torqueScale;
  public Transform com;
  public Transform[] GroundPoints;
  public Transform[] GroundEffects;
  public Transform AIMoveTarget;
  public float currentThrottle;
  public float avgThrust;
  public float liftDotMax;

  public void Awake()
  {
    this.rigidBody.set_centerOfMass(this.com.get_localPosition());
  }

  public CH47FlightTest.HelicopterInputState_t GetHelicopterInputState()
  {
    CH47FlightTest.HelicopterInputState_t helicopterInputStateT = new CH47FlightTest.HelicopterInputState_t();
    helicopterInputStateT.throttle = Input.GetKey((KeyCode) 119) ? 1f : 0.0f;
    helicopterInputStateT.throttle -= Input.GetKey((KeyCode) 115) ? 1f : 0.0f;
    helicopterInputStateT.pitch = Input.GetAxis("Mouse Y");
    helicopterInputStateT.roll = -Input.GetAxis("Mouse X");
    helicopterInputStateT.yaw = Input.GetKey((KeyCode) 100) ? 1f : 0.0f;
    helicopterInputStateT.yaw -= Input.GetKey((KeyCode) 97) ? 1f : 0.0f;
    helicopterInputStateT.pitch = (float) Mathf.RoundToInt(helicopterInputStateT.pitch);
    helicopterInputStateT.roll = (float) Mathf.RoundToInt(helicopterInputStateT.roll);
    return helicopterInputStateT;
  }

  public CH47FlightTest.HelicopterInputState_t GetAIInputState()
  {
    CH47FlightTest.HelicopterInputState_t helicopterInputStateT = new CH47FlightTest.HelicopterInputState_t();
    Vector3 vector3 = Vector3.Cross(Vector3.get_up(), ((Component) this).get_transform().get_right());
    float num1 = Vector3.Dot(Vector3.Cross(Vector3.get_up(), vector3), Vector3Ex.Direction2D(this.AIMoveTarget.get_position(), ((Component) this).get_transform().get_position()));
    helicopterInputStateT.yaw = (double) num1 < 0.0 ? 1f : 0.0f;
    helicopterInputStateT.yaw -= (double) num1 > 0.0 ? 1f : 0.0f;
    float num2 = Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_right());
    helicopterInputStateT.roll = (double) num2 < 0.0 ? 1f : 0.0f;
    helicopterInputStateT.roll -= (double) num2 > 0.0 ? 1f : 0.0f;
    float num3 = Vector3Ex.Distance2D(((Component) this).get_transform().get_position(), this.AIMoveTarget.get_position());
    float num4 = Vector3.Dot(vector3, Vector3Ex.Direction2D(this.AIMoveTarget.get_position(), ((Component) this).get_transform().get_position()));
    float num5 = Vector3.Dot(Vector3.get_up(), ((Component) this).get_transform().get_forward());
    if ((double) num3 > 10.0)
    {
      helicopterInputStateT.pitch = (double) num4 > 0.800000011920929 ? -0.25f : 0.0f;
      helicopterInputStateT.pitch -= (double) num4 < -0.800000011920929 ? -0.25f : 0.0f;
      if ((double) num5 < -0.349999994039536)
        helicopterInputStateT.pitch = -1f;
      else if ((double) num5 > 0.349999994039536)
        helicopterInputStateT.pitch = 1f;
    }
    else if ((double) num5 < -0.0)
      helicopterInputStateT.pitch = -1f;
    else if ((double) num5 > 0.0)
      helicopterInputStateT.pitch = 1f;
    float idealAltitude = this.GetIdealAltitude();
    float y = (float) ((Component) this).get_transform().get_position().y;
    float num6 = (double) y <= (double) idealAltitude + (double) CH47FlightTest.altitudeTolerance ? ((double) y >= (double) idealAltitude - (double) CH47FlightTest.altitudeTolerance ? ((double) num3 <= 20.0 ? 0.0f : Mathf.Lerp(0.0f, 1f, num3 / 20f)) : 1f) : -1f;
    Debug.Log((object) ("desiredThrottle : " + (object) num6));
    helicopterInputStateT.throttle = num6 * 1f;
    return helicopterInputStateT;
  }

  public float GetIdealAltitude()
  {
    return (float) ((Component) this.AIMoveTarget).get_transform().get_position().y;
  }

  public void FixedUpdate()
  {
    CH47FlightTest.HelicopterInputState_t aiInputState = this.GetAIInputState();
    this.currentThrottle = Mathf.Lerp(this.currentThrottle, aiInputState.throttle, 2f * Time.get_fixedDeltaTime());
    this.currentThrottle = Mathf.Clamp(this.currentThrottle, -0.2f, 1f);
    this.rigidBody.AddRelativeTorque(Vector3.op_Multiply(new Vector3(aiInputState.pitch * (float) this.torqueScale.x, aiInputState.yaw * (float) this.torqueScale.y, aiInputState.roll * (float) this.torqueScale.z), Time.get_fixedDeltaTime()), (ForceMode) 0);
    this.avgThrust = Mathf.Lerp(this.avgThrust, this.engineThrustMax * this.currentThrottle, Time.get_fixedDeltaTime());
    float num = Mathf.InverseLerp(this.liftDotMax, 1f, Mathf.Clamp01(Vector3.Dot(((Component) this).get_transform().get_up(), Vector3.get_up())));
    Vector3 vector3_1 = Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.get_up(), this.engineThrustMax), 0.5f), this.currentThrottle), num);
    Vector3 vector3_2 = Vector3.op_Subtraction(((Component) this).get_transform().get_up(), Vector3.get_up());
    Vector3 vector3_3 = Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Vector3) ref vector3_2).get_normalized(), this.engineThrustMax), this.currentThrottle), 1f - num);
    this.rigidBody.AddForce(Vector3.op_Multiply(Vector3.op_Multiply(Vector3.op_Multiply(((Component) this).get_transform().get_up(), this.rigidBody.get_mass() * (float) -Physics.get_gravity().y), num), 0.99f), (ForceMode) 0);
    this.rigidBody.AddForce(vector3_1, (ForceMode) 0);
    this.rigidBody.AddForce(vector3_3, (ForceMode) 0);
    for (int index = 0; index < this.GroundEffects.Length; ++index)
    {
      Transform groundPoint = this.GroundPoints[index];
      Transform groundEffect = this.GroundEffects[index];
      RaycastHit raycastHit;
      if (Physics.Raycast(((Component) groundPoint).get_transform().get_position(), Vector3.get_down(), ref raycastHit, 50f, 8388608))
      {
        ((Component) groundEffect).get_gameObject().SetActive(true);
        ((Component) groundEffect).get_transform().set_position(Vector3.op_Addition(((RaycastHit) ref raycastHit).get_point(), new Vector3(0.0f, 1f, 0.0f)));
      }
      else
        ((Component) groundEffect).get_gameObject().SetActive(false);
    }
  }

  public void OnDrawGizmos()
  {
    Gizmos.set_color(Color.get_yellow());
    Gizmos.DrawSphere(((Component) this.AIMoveTarget).get_transform().get_position(), 1f);
    Vector3 vector3_1 = Vector3.Cross(((Component) this).get_transform().get_right(), Vector3.get_up());
    Vector3 vector3_2 = Vector3.Cross(vector3_1, Vector3.get_up());
    Gizmos.set_color(Color.get_blue());
    Gizmos.DrawLine(((Component) this).get_transform().get_position(), Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(vector3_1, 10f)));
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawLine(((Component) this).get_transform().get_position(), Vector3.op_Addition(((Component) this).get_transform().get_position(), Vector3.op_Multiply(vector3_2, 10f)));
  }

  public CH47FlightTest()
  {
    base.\u002Ector();
  }

  public struct HelicopterInputState_t
  {
    public float throttle;
    public float roll;
    public float yaw;
    public float pitch;
  }
}
