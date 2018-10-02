using Assets.Scripts.IAJ.Unity.Movement.KinematicMovement;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;
using UnityEngine.UI;

public class CharacterKinematicMovementController : MonoBehaviour {

    public const float X_WORLD_SIZE = 55;
    public const float Z_WORLD_SIZE = 32.5f;
    private const float MAX_SPEED = 10.0f;
    private const float TIME_TO_TARGET = 2.0f;
    private const float RADIUS = 1.0f;
    private const float MAX_ROTATION = 8 * MathConstants.MATH_PI;

    public KeyCode stopKey = KeyCode.A;
    public KeyCode seekKey = KeyCode.S;
    public KeyCode fleeKey = KeyCode.D;
    public KeyCode arriveKey = KeyCode.F;
    public KeyCode wanderKey = KeyCode.G;

    // public KeyValuePair<float,float>[,] teste;

    public GameObject targetGameObject;
    public GameObject movementText;

    private KinematicCharacter targetCharacter;
    private KinematicCharacter character;
    private Text movementTextText;


    //early initialization
    void Awake()
    {
        this.character = new KinematicCharacter(this.gameObject);
        this.movementTextText = this.movementText.GetComponent<Text>();
    }

    // Use this for initialization
    void Start()
    {
        //gets the target DynamicCharacter's properties
        this.targetCharacter = this.targetGameObject.GetComponent<CharacterKinematicMovementController>().character;
    }

	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(stopKey)) character.Movement = null;

        else if (Input.GetKeyDown(seekKey))
        {
            character.Movement = new KinematicSeek
            {
                Target = targetCharacter.StaticData,
                MaxSpeed = MAX_SPEED
            };
        }
        else if (Input.GetKeyDown(fleeKey))
        {
            character.Movement = new KinematicFlee
            {
                Target = targetCharacter.StaticData,
                MaxSpeed = MAX_SPEED
            };
        }
        else if (Input.GetKeyDown(arriveKey))
        {
            character.Movement = new KinematicArrive
            {
                Target = targetCharacter.StaticData,
                MaxSpeed = MAX_SPEED,
                TimeToTarget = TIME_TO_TARGET,
                Radius = RADIUS
            };
        }
        else if (Input.GetKeyDown(wanderKey))
        {
            character.Movement = new KinematicWander
            {
                MaxRotation = MAX_ROTATION,
                MaxSpeed = MAX_SPEED
            };
        }

        UpdateMovingGameObject();
        UpdateMovementText();

   
	}

    private void UpdateMovingGameObject()
    {
        if (character.Movement != null)
        {
            character.Update();
            character.StaticData.ApplyWorldLimit(X_WORLD_SIZE, Z_WORLD_SIZE);
        }
    }

    private void UpdateMovementText()
    {
        if (this.character.Movement == null)
        {
            this.movementTextText.text = this.name  + " Movement: Stationary";
        }
        else
        {
            this.movementTextText.text = this.name + " Movement: " + this.character.Movement.Name;
        }

    } 

}
