using UnityEngine;

public class AIScript_JaiVang_2 : MonoBehaviour
{
    // Reference to the main character script
    public CharacterScript mainScript;

    // Radius within which the AI considers a button pressable
    private float buttonRadius = 1f;

    // Index of the target button the AI is currently focusing on
    private int targetButtonIndex = -1;

    // Arrays to store cooldowns, directions, locations, and speeds
    private float[] buttonCooldowns;
    private int[] beltDirections;
    private float[] buttonLocations;
    private float playerSpeed;
    private float[] bombSpeeds;

    // Start is called before the first frame update
    void Start()
    {
        // Get the CharacterScript component attached to this GameObject
        mainScript = GetComponent<CharacterScript>();

        // Check if CharacterScript is assigned
        ValidateMainScript();

        // Start moving towards the target button immediately
        MoveTowardsButton();
    }

    // Validate if CharacterScript is assigned
    void ValidateMainScript()
    {
        if (mainScript == null)
        {
            Debug.LogError("No CharacterScript found on " + gameObject.name);
            // Disable this script if CharacterScript is not assigned
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the game state by getting current values
        UpdateGameState();
        // Evaluate button priorities based on current game state
        EvaluateButtonPriorities();
    }

    // Update game state variables
    void UpdateGameState()
    {
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        buttonLocations = mainScript.getButtonLocations();
        playerSpeed = mainScript.getPlayerSpeed();
        bombSpeeds = mainScript.getBombSpeeds();
    }

    // Evaluate priorities for pressing buttons
    void EvaluateButtonPriorities()
    {
        // Initialize minimum distance to infinity
        float minDistance = Mathf.Infinity;

        // Iterate through button locations
        for (int i = 0; i < buttonLocations.Length; i++)
        {
            // Check if the button is pressable
            if (IsButtonPressable(i))
            {
                // Calculate distance to the button
                float distanceToButton = Mathf.Abs(buttonLocations[i] - mainScript.getCharacterLocation());

                // Update the target button if it's closer than the current minimum distance
                if (distanceToButton < minDistance)
                {
                    minDistance = distanceToButton;
                    targetButtonIndex = i;
                }
            }
        }

        // Move towards the target button
        MoveTowardsButton();
    }

    // Check if a button is pressable
    bool IsButtonPressable(int buttonIndex)
    {
        // Button is pressable if its cooldown is zero and belt direction is not towards the button
        return buttonCooldowns[buttonIndex] <= 0 && beltDirections[buttonIndex] == -1;
    }

    // Move towards the target button
    void MoveTowardsButton()
    {
        // Check if there's a target button
        if (targetButtonIndex != -1)
        {
            // Get the position of the target button and current player position
            float buttonPosition = buttonLocations[targetButtonIndex];
            float playerLocation = mainScript.getCharacterLocation();

            // Press the button if player is within the button radius
            if (Mathf.Abs(buttonPosition - playerLocation) < buttonRadius)
                mainScript.push();
            // Move down if button is below the player
            else if (buttonPosition < playerLocation)
                mainScript.moveDown();
            // Move up if button is above the player
            else
                mainScript.moveUp();
        }
    }
}
