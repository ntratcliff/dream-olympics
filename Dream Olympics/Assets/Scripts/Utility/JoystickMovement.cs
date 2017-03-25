using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(Rigidbody))]
public class JoystickMovement : MinigameBahaviour
{
    public float SpeedMultiplier;
    public float RotationSpeed;
    public float JumpForce;
    public float InAirThreshold = 0.05f;

    private PlayerController playerController;

    private float lastJumpVal;

    private bool inAir
    {
        get
        {
            return Mathf.Abs(GetComponent<Rigidbody>().velocity.y) > InAirThreshold;
        }
    }

    private bool veloCutThisJump;

    // Use this for initialization
    public override void Init(GameManager manager)
    {
        base.Init(manager);
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameRunning)
        {
            Rigidbody rb = GetComponent<Rigidbody>();

            Transform view = Camera.main.transform.parent.parent;

            //move forward for vertical axis
            float vertical = playerController.PlayerInfo.GetAxis("Vertical") * SpeedMultiplier;
            float horizontal = playerController.PlayerInfo.GetAxis("Horizontal") * SpeedMultiplier;

            Vector3 velocity = new Vector3(horizontal, 0, vertical);
            velocity = view.TransformDirection(velocity);

            // handle jump
            float jumpVal = playerController.PlayerInfo.GetAxis("Action");
            velocity.y = rb.velocity.y;

            if(jumpVal > lastJumpVal && !inAir) // first time jump was pressed
            {
                Debug.Log("Jump pressed!");

                // add force
                rb.AddForce(Vector3.up * JumpForce);

                veloCutThisJump = false;
            }
            else if(jumpVal == lastJumpVal) // jump is held down
            {

            }
            else if(jumpVal < lastJumpVal && inAir && !veloCutThisJump) // first time jump is released
            {
                Debug.Log("Jump released!");
                // cut velocity in half
                velocity.y /= 2;

                veloCutThisJump = true;
            }

            lastJumpVal = jumpVal;

            rb.velocity = velocity;

            //point player to movement direction
            // ignore y velocity
            velocity.y = 0;
            velocity.Normalize();
            if (velocity != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(velocity),
                    Time.deltaTime * RotationSpeed);
            }
        }
    }
}
