using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[RequireComponent (typeof (Animator))]
public class PlayerAction : MonoBehaviour {

	private Animator animator;

	public Transform relativeTransform;

	public Rigidbody rb;

	void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

	const int countOfDamageAnimations = 3;
	int lastDamageAnimation = -1;

	public int Restarts;


	public Vector3[] checkPoints; 

	private int currentPoint = 0; 

	public float moveSpeed = 2;
    public float rotationSpeed;

	public GameObject[] points; 

	public TMP_Text won;

	void FixedUpdate() {
		AnimationControl();	
	}
	private float jumpSpeed = 100f * 60;

	private bool canJump;
    private List<Collider> m_collisions = new List<Collider>();
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider))
                {
                    m_collisions.Add(collision.collider);
                }
                canJump = true;
            }
        }
    }



	private void OnCollisionExit(Collision collision)
    {
        if (m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { canJump = false; }
    }


	private void OnTriggerEnter(Collider other){
		
		if(other.gameObject.tag == "Floor")
		{
			transform.position = checkPoints[currentPoint];
			return;
		}	

		if(other.gameObject.tag == "Trophy"){
			won.text = "You Won!!!";
			Destroy(other.gameObject);
		}

		for(int i = 0; i < points.Length; i++) {
			if(other.gameObject == points[i]) {
				currentPoint = i;
			}
		}
		
	}

	

	void AnimationControl(){
		Vector3 moveDirection = Vector3.zero;

		string mth = "";
		
		bool notjumps = true;

		if (Input.GetKey(KeyCode.Space) & canJump)
        {
            rb.AddForce(0f, jumpSpeed * Time.deltaTime, 0f);
			// Jump();
			mth = "Jump";
        }


		// if (mth == "Jump"){
		// 	Jump();
		// 	notjumps = false;
		// }
		
        if(Input.GetKey(KeyCode.W)) 
		{
			moveDirection += relativeTransform.forward;
			// Walk();
			mth = "Walk";
		}
        if(Input.GetKey(KeyCode.S)) 
		{
			moveDirection += -relativeTransform.forward;
			// Walk();
			mth = "Walk";
		}
        if(Input.GetKey(KeyCode.A)) 
		{
			moveDirection += -relativeTransform.right;
			// Walk();
			mth = "Walk";
		}
        if(Input.GetKey(KeyCode.D)) 
		{
			moveDirection += relativeTransform.right;
			// Walk();
			mth = "Walk";
		} 

		if(Input.GetKey(KeyCode.LeftShift))
		{
			// Run();
			mth = "Run";
			moveSpeed = 3;
		} else {
			moveSpeed = 2;
		}


		moveDirection.y = 0f;
		
		if(mth == "Walk" && notjumps) {
			Walk();
		}
		else if (mth == "Jump" && notjumps){
			Jump();
		}
		else if (mth == "Run" && notjumps){
			Run();
		}

		transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
 
        if(moveDirection != Vector3.zero) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);
		} else {
			Stay();
		}
		


	}


	void Awake () {
		animator = GetComponent<Animator> ();
	}

	public void Stay () {

		animator.SetBool("Aiming", false);
		animator.SetFloat ("Speed", 0f);
	}

	public void Walk () {

		animator.SetBool("Aiming", false);
		animator.SetFloat ("Speed", 0.5f);
	}

	public void Run () {

		animator.SetBool("Aiming", false);
		animator.SetFloat ("Speed", 1f);
	}

	public void Attack () {
		Aiming ();
		animator.SetTrigger ("Attack");
	}

	public void Death () {
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Death"))
			animator.Play("Idle", 0);
		else
			animator.SetTrigger ("Death");
	}

	public void Damage () {
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Death")) return;
		int id = Random.Range(0, countOfDamageAnimations);
		if (countOfDamageAnimations > 1)
			while (id == lastDamageAnimation)
				id = Random.Range(0, countOfDamageAnimations);
		lastDamageAnimation = id;
		animator.SetInteger ("DamageID", id);
		animator.SetTrigger ("Damage");
	}

	public void Jump () { 
		animator.Play("Jump");
	}

	public void Aiming () {
		animator.SetBool ("Squat", false);
		animator.SetFloat ("Speed", 0f);
		animator.SetBool("Aiming", true);
	}

	public void Sitting () {
		animator.SetBool ("Squat", !animator.GetBool("Squat"));
		animator.SetBool("Aiming", false);
	}
}
