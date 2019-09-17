using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]

public class ThirdPersonCharacter : MonoBehaviour {


    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;
    [SerializeField] float m_JumpPower = 12f;
    [Range(1f, 4f)] [SerializeField] float m_GravityMultiplier = 2f;
    [SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
    [SerializeField] float m_MoveSpeedMultiplier = 1f;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    [SerializeField] float m_GroundCheckDistance = 0.1f;



    public bool MovementLocked = false;
    public bool RotationLocked = false;
    public int MaxSpeed;
    private bool paused;
    private Vector3 lastMove;
    Rigidbody m_Rigidbody;
    Animator m_Animator;
    public bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_LateralAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    CapsuleCollider m_Capsule;
    bool m_Crouching;
    Vector3 lastV;
    Vector3 curMove;
    Vector3 animMove;



    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;

        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        m_OrigGroundCheckDistance = m_GroundCheckDistance;
    }

    void OnPauseGame()
    {
        paused = true;
    }

    void OnResumeGame()
    {
        paused = false;
    }



    public void Move(Vector3 move, bool crouch, bool jump)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        curMove = move;
        animMove = transform.InverseTransformDirection(move);
        //So forward is still going to be animMove.z since we made it local.
        //Direction is going to be the local x, so animMove.x

        move = transform.InverseTransformDirection(move);
        CheckGroundStatus();
        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_ForwardAmount = move.z;
        m_LateralAmount = move.x;


        //If we aren't rotating (stunned, dead, etc) this doesn't matter.
        if (!RotationLocked)
        {
            //Shoot a ray from cam to mouse pos for a distance of 100
            RaycastHit[] hits;
            RaycastHit hit;
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 100.0f);

            //Iterate through all collisions, when obj tagged "Ground" is found, turn char to look at it
            //and stop iterating
            for (int i = 0; i < hits.Length; i++)
            {
                hit = hits[i];
                if (hit.collider.gameObject.tag == "Ground")
                {
                    //So he doesn't look down when we put it at his feet we displace by our pos y
                    transform.LookAt(new Vector3(hit.point.x, this.transform.position.y, hit.point.z));
                    i = hits.Length;
                }
            }
        }

        // control and velocity handling is different when grounded and airborne:
        if (!m_IsGrounded)
        {
            HandleAirborneMovement();
        }

        ScaleCapsuleForCrouching(crouch);
        PreventStandingInLowHeadroom();

        //If we are locked or if we aren't on the ground don't worry about anim or moving
        if (MovementLocked || !m_IsGrounded)
        {
            return;
        }

        curMove = (curMove * 50f);
        Vector3 storeMove = curMove;
        Vector3 negativeImpulse = Vector3.zero; 

        //If they're both positive, but new is strictly lesser we want to stop abruptly to remove that floaty deceleration
        //Will and Ben talked about. 

        if (curMove.magnitude < lastMove.magnitude)
        {
            m_Rigidbody.velocity.Set(0f, 0f, 0f);
            m_Rigidbody.angularVelocity.Set(0f, 0f, 0f);
            negativeImpulse = -((m_Rigidbody.mass * (m_Rigidbody.velocity * .1f)) / Time.deltaTime);
            m_Rigidbody.AddForce(negativeImpulse);
        }

        if (m_IsGrounded)
        {
            curMove.y = 0;
        }
        else
        {
            curMove.y -= m_GroundCheckDistance;
        }

        lastMove = storeMove;

        curMove.y = 0;

        m_Rigidbody.AddForce(curMove);

        m_Rigidbody.velocity = Vector3.ClampMagnitude(m_Rigidbody.velocity, MaxSpeed);

        //send input and other state parameters to the animator
        UpdateAnimator(move);
    }


    void ScaleCapsuleForCrouching(bool crouch)
    {
        if (m_IsGrounded && crouch)
        {
            if (m_Crouching) return;
            m_Capsule.height = m_Capsule.height / 2f;
            m_Capsule.center = m_Capsule.center / 2f;
            m_Crouching = true;
        }
        else
        {
            Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
                return;
            }
            m_Capsule.height = m_CapsuleHeight;
            m_Capsule.center = m_CapsuleCenter;
            m_Crouching = false;
        }
    }


    void PreventStandingInLowHeadroom()
    {
        // prevent standing up in crouch-only zones
        if (!m_Crouching)
        {
            Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
            float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
            if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_Crouching = true;
            }
        }
    }


    void UpdateAnimator(Vector3 move)
    {
        if (MovementLocked)
        {
            m_Animator.SetFloat("xMovement", 0, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("zMovement", 0, 0.1f, Time.deltaTime);
            return;
        } else
        {
            m_Animator.SetFloat("xMovement", m_LateralAmount, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("zMovement", m_ForwardAmount, 0.1f, Time.deltaTime);
        }

        // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
        // which affects the movement speed because of the root motion.
        if (m_IsGrounded && move.magnitude > 0)
        {
            m_Animator.speed = m_AnimSpeedMultiplier;
        }
        else
        {
            // don't use that while airborne
            m_Animator.speed = 1;
        }
    }


    //Applies gravity, checks ground.
    void HandleAirborneMovement()
    {
        // apply extra gravity from multiplier:
        Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
        m_Rigidbody.AddForce(extraGravityForce);

        m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
    }


    //Does not handle grounded movement, only checks if we can jump and if we are jumping.
    void HandleGroundedMovement(bool crouch, bool jump)
    {
        // check whether conditions are right to allow a jump:
        if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
        {
            // jump!
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
            m_IsGrounded = false;
            m_Animator.applyRootMotion = false;
            m_GroundCheckDistance = 0.1f;
        }
    }
    

    //Depreciated but saved in case team doesn't want it.
    public void OnAnimatorMove()
    {
        return;

        if (MovementLocked)
        {
            return;
        }
        //Vector3 moving = m_Rigidbody.transform.position + (curMove * m_MoveSpeedMultiplier);
        Vector3 moving = (m_Rigidbody.transform.position + (curMove * .3f));
        if (m_IsGrounded)
        {
            moving.y = 0;
        }
        else
        {
            moving.y -= m_GroundCheckDistance;
        }
        moving.y = 0;
        m_Rigidbody.transform.position = moving;

    }


    public void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
        {
            m_GroundNormal = hitInfo.normal;
            m_IsGrounded = true;
            m_Animator.applyRootMotion = true;
        }
        else
        {
            m_IsGrounded = false;
            m_GroundNormal = Vector3.up;
            m_Animator.applyRootMotion = false;
        }
    }


}
