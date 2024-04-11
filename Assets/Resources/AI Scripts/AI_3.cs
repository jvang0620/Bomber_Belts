using UnityEngine;
using System.Collections;

public class AI_3 : MonoBehaviour
{
    public CharacterScript mainScript;

    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public int[] beltDirections;
    public float[] buttonLocations;

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

    // Finds the closest belt/bomb to the given position
    int GetClosestButtonIndex(float position)
    {
        int closestIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < buttonLocations.Length; i++)
        {
            float distance = Mathf.Abs(buttonLocations[i] - position);
            if (distance < minDistance)
            {
                closestIndex = i;
                minDistance = distance;
            }
        }

        return closestIndex;
    }

    // Calculates time until the bomb reaches the target button
    float TimeUntilBombReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        return mainScript.getBombDistances()[buttonIndex] / mainScript.getBombSpeeds()[buttonIndex];
    }

    // Calculates time until the player reaches the target button
    float TimeUntilPlayerReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        return Mathf.Abs(mainScript.getCharacterLocation() - buttonLocations[buttonIndex]) / playerSpeed;
    }

    void Update()
    {
        // Get current game state information
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        bombSpeeds = mainScript.getBombSpeeds();

        // Find the closest button that is not on cooldown and not moving away
        int closestButtonIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < buttonLocations.Length; i++)
        {
            float distance = Mathf.Abs(buttonLocations[i] - mainScript.getCharacterLocation());
            if (buttonCooldowns[i] <= 0 && (beltDirections[i] == -1 || beltDirections[i] == 0))
            {
                if (distance < minDistance)
                {
                    closestButtonIndex = i;
                    minDistance = distance;
                }
            }
        }

        if (closestButtonIndex == -1)
            return;

        // Calculate time until the bomb reaches a button and time until a player reaches a button
        float bombTime = TimeUntilBombReachesButton(closestButtonIndex);
        float playerTime = TimeUntilPlayerReachesButton(closestButtonIndex);

        // Decide whether to press the button or avoid the bomb
        bool canPressButton = playerTime + 0.35f < bombTime;
        bool onTarget = closestButtonIndex == GetClosestButtonIndex(mainScript.getOpponentLocation());

        if (canPressButton || onTarget)
        {
            // Move towards the target button to push it
            if (buttonLocations[closestButtonIndex] < mainScript.getCharacterLocation())
                mainScript.moveDown();
            else
                mainScript.moveUp();

            mainScript.push();
        }
        else
        {
            // Move away from the closest bomb
            int closestBombIndex = GetClosestButtonIndex(mainScript.getCharacterLocation());
            if (closestBombIndex != -1)
            {
                if (bombSpeeds[closestBombIndex] > 0)
                    mainScript.moveUp();
                else
                    mainScript.moveDown();
            }
        }
    }
}
