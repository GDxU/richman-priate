using UnityEngine;

//This class applies gravity towards a spline to rigidbodies that this script is attached to
[AddComponentMenu("SuperSplines/Animation/Gravity Animator")]
public class SplineGravitySimulator : MonoBehaviour
{
	public Spline spline;
	
	public float gravityConstant = 9.81f;
	
	public int iterations = 5;
	
	void Start( )
	{
		//Disable default gravity calculations
		rigidbody.useGravity = false;
	}
	
	void FixedUpdate( ) 
	{
		if( rigidbody == null || spline == null )
			return;
		
		Vector3 closestPointOnSpline = spline.GetPositionOnSpline( spline.GetClosestPointParam( rigidbody.position, iterations ) ); 
		Vector3 shortestConnection = closestPointOnSpline - rigidbody.position;
		
		//Calculate gravity force according to Newton's law of universal gravity
		Vector3 force = shortestConnection * Mathf.Pow( shortestConnection.magnitude, -3 ) * gravityConstant * rigidbody.mass;
		
		rigidbody.AddForce( force );
	}
}
