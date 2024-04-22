using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class AIScript_JaiVang : MonoBehaviour
{
    // Reference to the main character script
    public CharacterScript mainScript;

    // Arrays to store bomb speeds, button cooldowns, button locations, belt directions, and bomb distances
    public float[] bombSpeeds;
    public float[] buttonCooldowns;
    public float playerSpeed;
    public float[] buttonLocations;
    public float currentLocation;
    public float enemyLocation;
    public int[] beltDirections = new int[8]; 
    public float[] bombDistances = new float[8];
    public const float BUFFER = 0.37f;

    // Sorted list to store belt indices and corresponding scores
    public SortedList<int, float> beltList = new SortedList<int, float>();

    // Flag to control list update
    public bool updateList = true;

    // Flags to indicate AI behavior modes
    public bool isSafeRobot = true;
    public bool isAggressiveRobot = false;
    

    // Start method is called before the first frame update
    void Start()
    {
        // Get the main character script component
        mainScript = GetComponent<CharacterScript>();

        // Disable the script if CharacterScript component is not found
        if (mainScript == null)
        {
            print("No CharacterScript found on " + gameObject.name);
            this.enabled = false;
        }

        // Initialize button locations and player speed
        buttonLocations = mainScript.getButtonLocations();
        playerSpeed = mainScript.getPlayerSpeed();
    }

    // Method to count enemy belts
    public int countEnemyBelts()
    {
        int result = 0;
        for (int i = 0; i < beltDirections.Length; i++)
        {
            if (beltDirections[i] == -1)
            {
                result++;
            }
        }
        return result;
    }

    // Method to count ally belts
    public int countFriendlyBelts()
    {
        int result = 0;
        for (int i = 0; i < beltDirections.Length; i++)
        {
            if (beltDirections[i] == 1)
            {
                result++;
            }
        }
        return result;
    }

    // Method to calculate time to explosion for a given bomb
    public float getExplosionTime(int bombNumber)
    {
        if (bombNumber < 0 || bombNumber > 7)
        {
            return Mathf.Infinity;
        }
        else if (bombSpeeds[bombNumber] != 0)
        {
            return bombDistances[bombNumber] / bombSpeeds[bombNumber];
        }
        else
        {
            if (beltList.Count != 0)
            {
                return (float)beltList.Keys[beltList.Count - 1];
            }
            else
            {
                return 10;
            }
        }
    }

    // Method to determine if a bomb is isSavable
    public bool isSavable(int bomb)
    {
        float timeToSave = Mathf.Abs(currentLocation - buttonLocations[bomb]) / playerSpeed + BUFFER;
        float explosionTime = getExplosionTime(bomb);

        return timeToSave < explosionTime && buttonCooldowns[bomb] < 0.0f;
    }

    // Method to determine if a bomb is considerable for action
    public bool isConsiderableBomb(int bomb)
    {
        return isSavable(bomb) && beltDirections[bomb] != 1;
    }

    // Method to get current belt index
    public int getCurrentBeltIndex()
    {
        float distance = Mathf.Infinity;
        int currentIndex = 0;
        for (int i = 0; i < beltDirections.Length; i++)
        {
            if (Mathf.Abs(currentLocation - buttonLocations[i]) < distance)
            {
                currentIndex = i;
                distance = Mathf.Abs(currentLocation - buttonLocations[i]);
            }
        }
        return currentIndex;
    }

    // Method to get current enemy belt index
    public int getEnemyCurBelt()
    {
        float distance = Mathf.Infinity;
        int currentIndex = 0;
        for (int i = 0; i < beltDirections.Length; i++)
        {
            if (Mathf.Abs(enemyLocation - buttonLocations[i]) < distance)
            {
                currentIndex = i;
                distance = Mathf.Abs(enemyLocation - buttonLocations[i]);
            }
        }
        return currentIndex;
    }

    // Method to get index of belt with lowest score
    public int getLowScore(SortedList<int, float> l)
    {
        int resultIndex = 0;
        float tempory_variable = Mathf.Infinity;
        for (int i = 0; i < l.Count; i++)
        {
            if (l.Values[i] < tempory_variable)
            {
                tempory_variable = l.Values[i];
                resultIndex = i;
            }
        }
        return resultIndex;
    }

    // Update method is called once per frame
    void Update()
    {
        // Update button cooldowns, belt directions, bomb distances, and character/enemy locations
        buttonCooldowns = mainScript.getButtonCooldowns();
        beltDirections = mainScript.getBeltDirections();
        bombDistances = mainScript.getBombDistances();
        currentLocation = mainScript.getCharacterLocation();
        enemyLocation = mainScript.getOpponentLocation();
        bombSpeeds = mainScript.getBombSpeeds();

        // Loop through each belt
        for (int i = 0; i < beltDirections.Length; i++)
        {
            // Calculate various parameters for decision-making
            bool isPushed = beltDirections[i] == 1;
            float distanceFromBot = Mathf.Abs(currentLocation - buttonLocations[i]);
            float timeUntilExplosion = getExplosionTime(i);
            float score = 0;
            int enemyBelts = countEnemyBelts();
            int friendlyBelts = countFriendlyBelts();

            // Switch between safe and aggressive modes based on ally belt count
            if (friendlyBelts > 3) 
            {
                SetAggressiveMode();
            }
            else
            {
                SetSafeMode();
            }

            // Calculate score based on mode and various factors
            if (isSafeRobot)
            {
                // Score calculation for safe mode
                score = CalculateSafeScore(distanceFromBot, timeUntilExplosion, i);
            }
            else
            {
                // Score calculation for aggressive mode
                score = CalculateAggressiveScore(distanceFromBot, timeUntilExplosion, i);
            }

            // Adjust score for bombs that are not considerable
            if (!isConsiderableBomb(i))
            {
                score += 1000;
            }

            // Update or add score to belt list
            if (beltList.Count < 8)
            {
                beltList.Add(i, score);
            }
            else
            {
                beltList[i] = score;
            }


        }

        // Switch logging for bot mode
        updateList = false;

        // Make decision based on lowest scored belt
        if (beltList.Count != 0)
        {
            int index, location = 0;
            float score = 0.0f;

            // Get lowest score and corresponding belt index
            index = getLowScore(beltList);
            location = (int)beltList.Keys[index];
            score = (float)beltList.Values[index];

            // Move character based on decision
            if (buttonLocations[location] < mainScript.getCharacterLocation())
            {
                mainScript.moveDown();
            }
            else if (buttonLocations[location] > mainScript.getCharacterLocation())
            {
                mainScript.moveUp();
            }

            // Push button if isSavable and on current belt
            if (isSavable(location) && location == getCurrentBeltIndex())
            {
                mainScript.push();
                updateList = true;
            }
        }
    }

    // Calculate score for safe mode
    float CalculateSafeScore(float distance, float timeToExplosion, int index)
    {
        float baseScore = distance + timeToExplosion - bombSpeeds[index] * 10;
        if (beltDirections[index] == -1)
        {
            baseScore -= 10;
        }
        return baseScore;
    }

    // Calculate score for aggressive mode
    float CalculateAggressiveScore(float distance, float timeToExplosion, int index)
    {
        float baseScore = 1 / distance + timeToExplosion - bombSpeeds[index] * 4 - Math.Abs(getEnemyCurBelt() - getCurrentBeltIndex()) * 2;
        if (beltDirections[index] == -1)
        {
            baseScore -= 6;
        }
        return baseScore;
    }


    // Set the robot mode to aggressive
    void SetAggressiveMode()
    {
        isSafeRobot = false;
        isAggressiveRobot = true;
    }

    // Set the robot mode to safe
    void SetSafeMode()
    {
        isAggressiveRobot = false;
        isSafeRobot = true;
    }

}
