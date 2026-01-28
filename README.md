# Turnfire

## Bots

### Overview
- Singleplayer mode features bots with 3 possible difficulties: easy, medium and hard
- The bots are implemented using **goal-oriented action planning**: The **bot brain** decides the goal, and the **bot controller** handles the action to reach it.
- Bots use different strategies via **bot tunings** based on their difficulty.

### Bot Logic
- First, the bot brain receives the intent to come up with a goal when the turn manager requests for input from the bot's input source.
- Then the goal is calculated depending on the current character phase (movement or item usage). 
- For the movement phase a pre-calculated **jump graph** is used in order to determine the possible positions where the character can jump based on its current position and mobility stats. **Every point is then scored** based on the bot's strategy (tuning) and the best position is then picked using a simple **soft-max function.**
- For the item usage phase **every item is simulated** using the item's behavior. If the item is a weapon then the possible firing angles are iteratively simulated as well. The best item (with the best firing angle) is then picked by evaluating the simulation results. If the character does not have any items then this action is simply skipped.
- The bot tunings on top of strategic parameters (like offense, defense and package greed) include adjustable aim precision and decision randomness parameters to maintain differences between difficulty levels in all character phases.

###  Bot Evaluation
- Bot tuning parameters were first set to an ad-hoc value and then adjusted after continuous evaluation and re-tuning.
- During evaluation around 200 1v1 matches were fast played on all possible maps where every bot difficulty played against every other difficulty. 
- The round evaluation stats include: match outcome (win/tie/lose), suicide count, remaining team health, skipped move count, damage dealt, friendly fire damage dealt, etc.
- Based on the results the tunings were manually adjusted while logical fixes and upgrades were applied.
- As a final result bots with relatively difficult tunings against less difficult tunings tend to converge towards a win/tie/lose ratio of 60/10/30.

---

### Notes & Possible Improvements
- I might improve upon the bot evaluation later by automating the parameter tuning with a neural network to reach a standard 70/30 win/lose ratio.
