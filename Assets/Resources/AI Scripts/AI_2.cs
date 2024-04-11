using UnityEngine;
using System.Collections;

public class AI_2 : MonoBehaviour
{
    public CharacterScript mainScript;

    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public int[] beltDirections;
    public float[] buttonLocations;

    private int targetButtonIndex = -1;

    void Start()
    {
        mainScript = GetComponent<CharacterScript>();

        if (mainScript == null)
        {
            Debug.LogError("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        buttonLocations = mainScript.getButtonLocations();
        playerSpeed = mainScript.getPlayerSpeed();
    }

    void Update()
    {
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        bombSpeeds = mainScript.getBombSpeeds();

        int playerLocation = Mathf.RoundToInt(mainScript.getCharacterLocation());
        int opponentLocation = Mathf.RoundToInt(mainScript.getOpponentLocation());

        // Print debug information
        Debug.Log("Player Location: " + playerLocation);
        Debug.Log("Opponent Location: " + opponentLocation);

        // Evaluate button priorities based on current situation
        EvaluateButtonPriorities(playerLocation, opponentLocation);

        // Move towards the selected button
        MoveTowardsButton(playerLocation);
    }

    void EvaluateButtonPriorities(int playerLocation, int opponentLocation)
    {
        float minPriority = float.MaxValue;

        for (int i = 0; i < buttonLocations.Length; i++)
        {
            if (buttonCooldowns[i] <= 0 && beltDirections[i] <= 0)
            {
                // Calculate priority based on distance to the button, opponent, and bomb speed
                float buttonDistance = Mathf.Abs(buttonLocations[i] - playerLocation);
                float opponentDistance = Mathf.Abs(buttonLocations[i] - opponentLocation);
                float priority = buttonDistance / playerSpeed + opponentDistance / playerSpeed - buttonLocations[i] / bombSpeeds[i];

                Debug.Log("Button " + i + " Priority: " + priority);

                if (priority < minPriority)
                {
                    minPriority = priority;
                    targetButtonIndex = i;
                }
            }
        }

        Debug.Log("Target Button Index: " + targetButtonIndex);
    }

    void MoveTowardsButton(int playerLocation)
    {
        if (targetButtonIndex != -1)
        {
            float buttonPosition = buttonLocations[targetButtonIndex];

            Debug.Log("Button Position: " + buttonPosition);

            if (buttonPosition < playerLocation)
            {
                mainScript.moveDown();
                if (Mathf.Abs(playerLocation - buttonPosition) < 1)
                {
                    mainScript.push();
                }
            }
            else if (buttonPosition > playerLocation)
            {
                mainScript.moveUp();
                if (Mathf.Abs(playerLocation - buttonPosition) < 1)
                {
                    mainScript.push();
                }
            }
        }
    }
}
