using UnityEngine;
using System.Collections;

public class AIScript_JaiVang_1 : MonoBehaviour
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
        // Move towards the target button
        MoveTowardsButton();
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

    // Get the index of the closest button to a given position
    int GetClosestButtonIndex(float position)
    {
        int closestIndex = -1;
        float minDistance = float.MaxValue;

        // Iterate through button locations
        for (int i = 0; i < buttonLocations.Length; i++)
        {
            // Calculate distance to the button
            float distance = Mathf.Abs(buttonLocations[i] - position);
            // Update closest index if this button is closer
            if (distance < minDistance)
            {
                closestIndex = i;
                minDistance = distance;
            }
        }

        return closestIndex;
    }

    // Calculate time until a bomb reaches a button
    float TimeUntilBombReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        // Calculate time based on distance and bomb speed
        return mainScript.getBombDistances()[buttonIndex] / bombSpeeds[buttonIndex];
    }

    // Calculate time until player reaches a button
    float TimeUntilPlayerReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        // Calculate time based on distance and player speed
        return Mathf.Abs(mainScript.getCharacterLocation() - buttonLocations[buttonIndex]) / playerSpeed;
    }
}
