# Time Mine

You can play the game at this link:

https://eldonelectronic.itch.io/time-mine-final

Here is a video demo of our game:

https://youtu.be/QFxDrEqKDkw


## Description

This game is (to the best of our knowledge) the first fluid, mostly unrestricted, time-travel game. There are two teams: miners must collect crystals whilst guardians try to catch the miners, forcing them to drop their crystals. If the miners collect more than ten crystals by the end of the game, they win; otherwise the guardians win. Collectable crystals are generated at random times and places throughout the arena and they only exist for a short window of time. Large blocker crystals grow and break throughout the game, preventing access to some regions of the map. All players should time travel to get around the blockers and miners should time travel to collect crystals in the time windows where they exist.


## My Contributions

My top contributions to this project were:

#### Conceptualisation
I proposed the idea for a time travel game and outlined a lot of the core concepts. These include a tutorial and a tracker device to guide the user; crystal objects whose behaviour is linked to time, providing an incentive to time-travel; and a timeline on the HUD to visualise time-travel.

#### Time Travel System
I reworked our original time-travel backend from the ground up. This meant creating the data structures for storing state and tracking players' positions in time as well as the connections between time-travelling components.

#### Test Framework
I wrote the unit tests for our backend. To help with this, I also created a simulator which could simulate a game of time-travelling exclusively in the backend, allowing for broader testing. I also worked on visualisation and diagnostics tools to help debug the time-travel features of our game.

#### Code Maintenance
I wrote documentation, structure diagrams and algorithm guides. I was the driving force behind the refactoring which iteratively improved our codebase's maintainability (introducing a consistent style convention and adding an event system for example).


## Deployment Instructions

1. Follow the instructions at the following link to download the Unity Hub and install Unity version 2020.3.27f1.
https://unity.com/download

2. Download Autodesk Maya (this step is likely no longer needed since we made efforts to convert final asset files to OBJ).
https://www.autodesk.co.uk/products/maya/overview?term=1-YEAR&tab=subscription

3. With Unity Hub running, select "open" and then navigate to the "time-arena" folder. Select "time-arena-game".

4. To build the project for desktop, go to "file->Build Settings". Make sure "PC, Mac & Linux Standalone" is selected as the platform. Click the "Switch platform" button if it appears at the bottom. Finally press "Build and Run" to compile and run the game.

5. To deploy the project as a web game, select "WebGL" as the platform, press "Switch Platform" if necessary and finally press "Build". Compress the generated folder to a ZIP file. Create an itch.io account. Go to "Dashboard", then select "Create new project". Fill out the fields and importantly, under uploads, upload the generated zip file. Finally, press "Save & view page" to access the finished project.
https://itch.io/register


## Contributors

Angus Robertson

Anton Wallsdedt

Henry Hartnoll

Nisa Bayraktar

Paolo Mura

Samuel Balson

Zac Woodford
