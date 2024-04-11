using UnityEngine;
using System.Collections;

public class AI_5 : MonoBehaviour
{
    public CharacterScript mainScript;

    private float[] buttonSpeeds;
    private float[] buttonCooldowns;
    private float playerSpeed;
    private int[] beltDirections;
    private float[] buttonLocations;
    private float characterLocation;

    // Constants
    private const float ButtonRadius = 1f;
    private const float BufferTime = 0.1f;

    void Start()
    {
        mainScript = GetComponent<CharacterScript>();
        if (mainScript == null)
        {
            Debug.LogError("No CharacterScript found on " + gameObject.name);
            enabled = false;
        }

        // Retrieve initial game state
        UpdateGameState();
    }

    void Update()
    {
        // Update game state
        UpdateGameState();

        // Evaluate and execute actions
        EvaluateAndExecuteActions();
    }

    void UpdateGameState()
    {
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        buttonLocations = mainScript.getButtonLocations();
        playerSpeed = mainScript.getPlayerSpeed();
        buttonSpeeds = mainScript.getBombSpeeds();
        characterLocation = mainScript.getCharacterLocation();
    }

    void EvaluateAndExecuteActions()
    {
        int targetButtonIndex = GetTargetButtonIndex();
        if (targetButtonIndex != -1)
        {
            MoveTowardsButton(targetButtonIndex);
            if (IsButtonPressable(targetButtonIndex))
                PushButton(targetButtonIndex);
        }
    }

    int GetTargetButtonIndex()
    {
        int closestButtonIndex = -1;
        float minDistance = Mathf.Infinity;

        for (int i = 0; i < buttonLocations.Length; i++)
        {
            if (IsButtonPressable(i))
            {
                float distanceToButton = Mathf.Abs(buttonLocations[i] - characterLocation);
                if (distanceToButton < minDistance)
                {
                    minDistance = distanceToButton;
                    closestButtonIndex = i;
                }
            }
        }
        return closestButtonIndex;
    }

    bool IsButtonPressable(int buttonIndex)
    {
        return buttonCooldowns[buttonIndex] <= 0 && beltDirections[buttonIndex] == -1;
    }

    void MoveTowardsButton(int buttonIndex)
    {
        float buttonPosition = buttonLocations[buttonIndex];

        if (Mathf.Abs(buttonPosition - characterLocation) < ButtonRadius)
            return; // Already in range, no need to move

        if (buttonPosition < characterLocation)
            mainScript.moveDown();
        else
            mainScript.moveUp();
    }

    void PushButton(int buttonIndex)
    {
        if (Mathf.Abs(buttonLocations[buttonIndex] - characterLocation) < ButtonRadius)
            mainScript.push();
    }

    float TimeUntilBombReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        return mainScript.getBombDistances()[buttonIndex] / buttonSpeeds[buttonIndex];
    }

    float TimeUntilPlayerReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        return Mathf.Abs(characterLocation - buttonLocations[buttonIndex]) / playerSpeed;
    }
}
