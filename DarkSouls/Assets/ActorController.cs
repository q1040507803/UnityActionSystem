using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    public GameObject model;
    public CameraController camcon;
    public IUserInput pi;
    public float walkSpeed = 2.0f;
    public float runMultiplier = 2.0f;
    public float jumpVelocity = 5.0f;
    public float rollVelocity = 1.0f;

    [Space(10)]
    [Header("===== Friction Settings =====")]
    public PhysicMaterial frictionOne;
    public PhysicMaterial frictionZero;

    private Animator anim;
    private Rigidbody rigid;
    private Vector3 planarVec;
    private Vector3 thrustVec;
    private bool canAttack;
    private bool lockPlanar = false;
    private bool trackDirection = false;
    private CapsuleCollider col;
    private float lerpTarget;
    private Vector3 deltaPos;

    // Start is called before the first frame update
    void Awake()
    {
        IUserInput[] inputs = GetComponents <IUserInput>();
        foreach (var input in inputs)
        {
            if(input.enabled == true)
            {
                pi = input;
                break;
            }
        }
        anim = model.GetComponent<Animator>();
        //不能在update调用
        rigid = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()       //Time.deltaTime  1/60
    {
        if(pi.lockon)
        {
            camcon.LockUnlock();
        }

        if(camcon.lockState == false)
        {
            anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), ((pi.run) ? 2.0f : 1.0f), 0.5f));
            anim.SetFloat("right", 0);
        }
        else
        {
            Vector3 localDVec = transform.InverseTransformVector(pi.Dvec);
            anim.SetFloat("forward", localDVec.z * ((pi.run) ? 2.0f : 1.0f));
            anim.SetFloat("right", localDVec.x * ((pi.run) ? 2.0f : 1.0f));
        }

        //float targetRunMulti = ((pi.run) ? 2.0f : 1.0f);

        anim.SetBool("defense", pi.defense);

        if(pi.roll || rigid.velocity.magnitude > 7f)
        {
            anim.SetTrigger("roll");
            canAttack = false;
        }

        if (pi.jump)
        {
            anim.SetTrigger("jump");
            canAttack = false;
        }

        if(pi.attack  && CheckState("ground") && canAttack)
        {
            anim.SetTrigger("attack");
        }

        if(camcon.lockState == false)
        {
            if (pi.Dmag > 0.1f)
            {
                //Vector3 targetForward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
                model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
            }

            if (lockPlanar == false)
            {
                planarVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }
        }
        else
        {
            if(trackDirection == false)
            {
                model.transform.forward = transform.forward;
            }
            else
            {
                model.transform.forward = planarVec.normalized;
            }

            if(lockPlanar == false)
            {
                planarVec = pi.Dvec * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
            }

        }

    }

    void FixedUpdate()      //Time.fixedDeltaTime  1/50
    {
        rigid.position += deltaPos;
        //rigid.position += planarVec * Time.fixedDeltaTime;    //位置
        rigid.velocity = new Vector3(planarVec.x, rigid.velocity.y, planarVec.z) + thrustVec;     //速度
        thrustVec = Vector3.zero;
        deltaPos = Vector3.zero;
    }

    private bool CheckState(string stateName,string layerName = "Base Layer")
    {
        //int layerIndex = anim.GetLayerIndex(layerName);
        //bool result = anim.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        //return result;
        return anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex(layerName)).IsName(stateName);
    }

    ///
    /// Message processing block
    ///
    public void OnJumpEnter()
    {
        thrustVec = new Vector3(0, jumpVelocity, 0);
        pi.inputEnabled = false;
        lockPlanar = true;
        trackDirection = true;
    }

    public void IsGround()
    {
        anim.SetBool("isGround", true);
    }

    public void IsNotGround()
    {
        anim.SetBool("isGround", false);
    }

    public void OnGroundEnter()
    {
        pi.inputEnabled = true;
        lockPlanar = false;
        canAttack = true;
        col.material = frictionOne;
        trackDirection = false;
    }

    public void OnGroundExit()
    {
        col.material = frictionZero;
    }

    public void OnFallEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;
    }

    public void OnRollEnter()
    {
        thrustVec = new Vector3(0, rollVelocity, 0);
        pi.inputEnabled = false;
        lockPlanar = true;
        trackDirection = true;
    }

    public void OnJabEnter()
    {
        pi.inputEnabled = false;
        lockPlanar = true;
    }

    public void OnJabUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("jabVelocity");
    }

    public void OnAttack1hAEnter()
    {
        pi.inputEnabled = false;
        lerpTarget = 1.0f;
    }
    public void OnAttack1hAUpdate()
    {
        thrustVec = model.transform.forward * anim.GetFloat("attack1hAVelocity");
        //float currentWeight = anim.GetLayerWeight(anim.GetLayerIndex("attack"));
        //currentWeight = Mathf.Lerp(currentWeight, lerpTarget, 0.1f);
        //anim.SetLayerWeight(anim.GetLayerIndex("attack"), currentWeight);
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }

    public void OnAttackIdleEnter()
    {
        pi.inputEnabled = true;
        lerpTarget = 0f;
    }

    public void OnAttackIdleUpdate()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("attack"), Mathf.Lerp(anim.GetLayerWeight(anim.GetLayerIndex("attack")), lerpTarget, 0.4f));
    }

    public void OnUpdateRM(object _deltaPos)
    {
        if(CheckState("attack1hC","attack"))
        {
            deltaPos += (0.8f * deltaPos + 0.2f * (Vector3)_deltaPos);
        }
    }
}
