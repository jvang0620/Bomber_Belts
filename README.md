# Bomber-Belts

This project originated from Professor Daniel Jugan's ITCS 3153 course, "Introduction to Artificial Intelligence" at the University of North Carolina at Charlotte. Everything except for the AI Script that runs the robot was created by the Instructor. Students are to create the script to move the AT.

The challenge involves scripting an AI to control an agent (robot) tasked with pressing buttons. In this interactive application, the agent competes against another robot, both pressing buttons strategically. Each button press sets off a chain reaction, moving bombs along a conveyor belt towards the opponent's side. Upon reaching the opponent's side, a bomb explodes, deducting one life from the opponent. Players start with 8 lives, and the first to reach 0 loses the game.

<div align="center">
    <img src="/Media/image_1.JPG" alt="Screenshot of Live Server Extension" width="600">
</div>

<div align="center">
    <img src="/Media/image_2.JPG" alt="Screenshot of Live Server Extension" width="600">
</div>

<div align="center">
    <img src="/Media/image_3.JPG" alt="Screenshot of Live Server Extension" width="600">
</div>

## Game Demo:

In this video, two robots are engaged in a fast-paced battle where they must strategically press buttons to control the movement of bombs on a conveyor belt. The objective is clear: prevent the bombs from reaching the opponent's side, as they will detonate upon arrival.

As the robots frantically press their respective buttons, the bombs swiftly move along the conveyor belt, inching closer to their target with each button press. The tension builds as players must balance offense and defense, timing their actions precisely to outmaneuver their opponent and avoid catastrophe.

"Bomber Belt" promises an exhilarating multiplayer experience filled with action, strategy, and adrenaline-pumping moments. Whether playing against friends or challenging AI opponents, this game is sure to provide hours of entertainment and competitive fun.

[![Bomber Belt](https://img.youtube.com/vi/luqdXuSWS50/0.jpg)](https://www.youtube.com/watch?v=luqdXuSWS50)

## Purpose:

The purpose of this project is to explore the principles of artificial intelligence in a practical setting. By scripting an AI to make strategic decisions in a simulated environment, students gain hands-on experience in problem-solving, decision-making, and algorithm implementation.

## Key Features:

- AI Scripting: Students are challenged to write scripts that control the agent's actions, including movement and button pressing.
- Button Interaction: The AI must determine which buttons to press based on their current state and strategic considerations.
- Life Management: Players must manage their lives effectively, avoiding bomb explosions while strategically pressing buttons to defeat the opponent.

## How to Use:

To use the application, students can follow these steps:

- Write a script to control the agent's actions, considering factors such as button cooldowns, belt directions, and bomb speeds.
- Execute the script to observe the agent's behavior within the simulated environment.
- Continuously refine and optimize the script to improve the agent's performance and increase the chances of winning the game.

## Developed Using:

![C#](https://img.shields.io/badge/-C%23-blue?style=for-the-badge&logo=c-sharp&logoColor=white)
![JavaScript](https://img.shields.io/badge/-JavaScript-yellow?style=for-the-badge&logo=javascript&logoColor=black)
![ShaderLab](https://img.shields.io/badge/-ShaderLab-green?style=for-the-badge&logo=unity&logoColor=white)

This script, written primarily in C#, controls the AI behavior in the game. It evaluates button priorities based on game state variables such as cooldowns, belt directions, and button locations. The AI then moves towards the target button and performs actions accordingly, such as pressing the button or adjusting its position.

Please note that this script is part of a Unity project and should be used within the Unity IDE.
