using UnityEngine;
using System.Collections;

public class AI_4 : MonoBehaviour
{
    public CharacterScript mainScript;

    private float playerSpeed;
    private float[] buttonLocations;
    private float[] buttonCooldowns;
    private int[] beltDirections;
    private float[] bombSpeeds;

    private const float buttonRadius = 1f;

    void Start()
    {
        mainScript = GetComponent<CharacterScript>();
        if (mainScript == null)
        {
            Debug.LogError("No CharacterScript found on " + gameObject.name);
            enabled = false;
        }

        InitializeVariables();
    }

    void InitializeVariables()
    {
        buttonLocations = mainScript.getButtonLocations();
        playerSpeed = mainScript.getPlayerSpeed();
    }

    void Update()
    {
        UpdateGameState();
        EvaluateAndMove();
    }

    void UpdateGameState()
    {
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        bombSpeeds = mainScript.getBombSpeeds();
    }

    void EvaluateAndMove()
    {
        int closestButtonIndex = FindClosestPressableButton();
        if (closestButtonIndex == -1)
            return;

        float buttonPosition = buttonLocations[closestButtonIndex];
        float playerLocation = mainScript.getCharacterLocation();
        float bombTime = TimeUntilBombReachesButton(closestButtonIndex);
        float playerTime = TimeUntilPlayerReachesButton(closestButtonIndex);

        bool canPressButton = playerTime + 0.35f < bombTime;
        bool onTarget = closestButtonIndex == FindClosestButton(mainScript.getOpponentLocation());

        if (canPressButton || onTarget)
        {
            MoveTowards(buttonPosition, playerLocation);
            mainScript.push();
        }
        else
        {
            MoveAwayFromBomb(playerLocation);
        }
    }

    int FindClosestButton(float position)
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

    int FindClosestPressableButton()
    {
        int closestIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < buttonLocations.Length; i++)
        {
            if (IsButtonPressable(i))
            {
                float distance = Mathf.Abs(buttonLocations[i] - mainScript.getCharacterLocation());
                if (distance < minDistance)
                {
                    closestIndex = i;
                    minDistance = distance;
                }
            }
        }

        return closestIndex;
    }

    bool IsButtonPressable(int buttonIndex)
    {
        return buttonCooldowns[buttonIndex] <= 0 && (beltDirections[buttonIndex] == -1 || beltDirections[buttonIndex] == 0);
    }

    float TimeUntilBombReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        return mainScript.getBombDistances()[buttonIndex] / bombSpeeds[buttonIndex];
    }

    float TimeUntilPlayerReachesButton(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= buttonLocations.Length)
            return Mathf.Infinity;

        return Mathf.Abs(mainScript.getCharacterLocation() - buttonLocations[buttonIndex]) / playerSpeed;
    }

    void MoveTowards(float targetPosition, float currentPosition)
    {
        if (targetPosition < currentPosition)
            mainScript.moveDown();
        else
            mainScript.moveUp();
    }

    void MoveAwayFromBomb(float playerLocation)
    {
        int closestBombIndex = FindClosestButton(playerLocation);
        if (closestBombIndex != -1)
        {
            if (bombSpeeds[closestBombIndex] > 0)
                mainScript.moveUp();
            else
                mainScript.moveDown();
        }
    }
}
