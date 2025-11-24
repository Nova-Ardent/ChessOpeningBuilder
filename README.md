# ChessOpeningBuilder

## How to use

Start by making moves on the chess trainer, this will add moves to the board-history on the right side of the board.
<img width="1549" height="1198" alt="Making Moves" src="https://github.com/user-attachments/assets/1bca3077-9bd8-47ac-8cca-c8ab6d6a9313" />

When you're satisfied with the moves, in your variation, press the add button. This will add it to the trainer.
<img width="54" height="56" alt="Add" src="https://github.com/user-attachments/assets/a135684a-a110-4a81-955e-2530fc6fe9b6" />

In the Trainer tab, just press run, to run through all your variations. For more detailed instructions, read further.

## History Tab
### Add Variation to 
<img width="54" height="56" alt="Add" src="https://github.com/user-attachments/assets/a135684a-a110-4a81-955e-2530fc6fe9b6" />  <br>
This functionality will add all the moves played as a variation to your training module

### Clear
<img width="59" height="51" alt="Clear" src="https://github.com/user-attachments/assets/e3c846e7-e1af-4e49-a09b-5d8416864bf1" /> <br>
This will clear all the current moves in the history, and reset the board.

### Undo
<img width="54" height="55" alt="Undo" src="https://github.com/user-attachments/assets/8c58e2c7-0981-4e5f-84e1-e27d632f9508" /> <br>
This will undo one move, in the history.

## Moves History
<img width="293" height="175" alt="Moves" src="https://github.com/user-attachments/assets/bdf2b22d-6caa-4598-b416-f2f323722773" /> <br>
This will track the current moves made, for the given variation. Clicking on any of the moves will bring you to that point, in the history.<br>
note: you cannot make moves until you return to the head of the history-chain.

## Top Moves
<img width="308" height="159" alt="StockfishInfo" src="https://github.com/user-attachments/assets/161c5a5b-c41e-4d71-8d1d-3eda7ba2b2a3" /> <br>
Above the move history is stockfishes top-5 suggested moves. Left clicking any of these moves will play the move, and right-clicking will draw an arrow.

## Trainer Tab
### Trainer Variations
<img width="626" height="243" alt="Capture" src="https://github.com/user-attachments/assets/dc7e1974-4ece-451b-817a-b761c2c72a3d" />
<br>This section will show the possible moves in your training module, the current move you're looking at, and the previous move. 
<br>Click on the previous or next move will progress through the training module. 
<br>Clicking on the current mvoe will load the history up until that move.

### Options
<img width="1356" height="202" alt="Capture" src="https://github.com/user-attachments/assets/eb081562-406a-44a8-b77f-0252cc5b326e" /><br> 
- <b>Save:</b> Opens a dialogue to save your current training module
- <b>load:</b> Opens a dialogue to load a training module
- <b>Combine:</b> Opens a dialogue to load a training module, and combine it with your current training module
- <b>Run:</b> Runs your current training module
- <b>Opening Type:</b> Defines if the opening is for white, or black.
  - if it is white, it will branch on black moves while being run
  - if it is black, it will branch on white moves while being run
- <b>Stats Display:</b> How the percentage stats will be compiled
  - Stats by Move: will calculate the percentage stats of how many times a specific move was correct
  - Stats by Branch: will calculate the percentage of stats of how many times moves were correct in a given branch
  - Stats by Variation: will calculate the percentage of states based on how many times a variation was correct
- <b>Method:</b> Upon failing a move, while running your training module, it defines how the failure will be repeated, to try again
  - No Repeats: Do not repeat failed attempts  
  - Repeat: repeats the failed attempt at the very end of your run
  - Repeat Immediately: repeats the failed attempt immediately after failing
- <b>Variation Type:</b>
  - By Complete variation: The variations included in your run will be counted by unique variation
  - By Move Count: The variations included in your run will be limited by number of moves
- <b>Variations:</b>
  - The count variation type will be effected by. If -1 or 0, run all variations
