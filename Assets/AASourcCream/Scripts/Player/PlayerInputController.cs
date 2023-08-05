using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{

    private PlayerAbilityController playerAbilityController;
    private PlayerDataController playerDataController;

    [SerializeField]
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        playerAbilityController = GetComponent<PlayerAbilityController>();
        playerDataController = GetComponent<PlayerDataController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //MOVE THIS to attack related
    public void onMbL(InputAction.CallbackContext ctx)
    {

        if (ctx.phase == InputActionPhase.Started) {
            playerAbilityController.CallAbility(0);
        }
        if (ctx.phase == InputActionPhase.Canceled) {
            Debug.Log("MbLEFT_RELEASE");
        }
    }


    //MOVE THIS to attack related
    public void onMbR(InputAction.CallbackContext ctx)
    {

        if (ctx.phase == InputActionPhase.Started) {
            playerAbilityController.CallAbility(1);
        }
        if (ctx.phase == InputActionPhase.Canceled) {
            Debug.Log("MbRIGHT_RELEASE");
        }
    }

    public void OnInput1(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) {
            Debug.Log("1_CLICK");
            playerAbilityController.CallAbility(2);
        }
        if (ctx.phase == InputActionPhase.Canceled) {
            Debug.Log("1_RELEASE");
        }
    }

    public void OnInput2(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) {
            playerAbilityController.CallAbility(3);
            Debug.Log("2_CLICK");
        }
        if (ctx.phase == InputActionPhase.Canceled) {
            Debug.Log("2_RELEASE");
        }

    }

    public void OnInput3(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) {
            Debug.Log("3_CLICK");
        }
        if (ctx.phase == InputActionPhase.Canceled) {
            Debug.Log("3_RELEASE");
        }
    }

    public void OnInput4(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) {
            Debug.Log("4_CLICK");
        }
        if (ctx.phase == InputActionPhase.Canceled) {
            Debug.Log("4_RELEASE");
        }
    }
}