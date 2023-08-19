# Hax


## Investigations 

- [] Is it possible to investigate the rng?

## Cheats to Implement

- [] Always start
- [] Supress Hover events
- [] Perfect info on OP Hand
- [] Look at the top card of your library
- [] stack your deck
- [] open all boosters in inventory
- [] Starting life Total

## Next Implementation

- [x] Get data from the game
- [x] Test setting localplayerlife to 20
- [] create GUI for hack using mvc 
- [] Interact with the game function through GUI button like "Next Phase" for example
   
## tailing logs for mtga
Using Powershell

```
Get-Content " %userprofile%\appdata\locallow\Wizards Of The Coast\MTGA\Player.log" -wait | where { $_ -match PATTERN }
```

## 2023-08-17: Current Major Goal [Finished 2023-08-18]

Find a way to get game state data into your application. Prove that you have the data using print statements to the log file:

```csharp
                SimpleLog.LogError("PUT THE DATA HERE");
```

tail the log like this

```
Get-Content "C:\Users\kog\AppData\LocalLow\Wizards Of The Coast\MTGA\Player.log" -wait
```



## TODO

- [x] Setup project
- [x] Create Trivial injection code
- [x] Test injecting using injector
- [x] Find a way to output data, (SimpleLog.LogError)
- [x] debug why injection does not work
- [x] Find candidate classes, `public interface IBlackboard`
- [ ] Convert game to debug build https://github.com/dnSpyEx/dnSpy/wiki/Debugging-Unity-Games

## debug why injection gets error

Done! Injection works!


Read the following link: [sharpmonoinjector-fixed-updated-readme](https://www.unknowncheats.me/forum/unity/408878-sharpmonoinjector-fixed-updated-3.html)

- [x] Read the forum posts
- [x] check if code is using il2cpp
- [x] Check if changing project to .NET 4 works
- [x] Make sure a larger minor version of .NET is installed
- [x] Make sure all dlls are in references, make sure imports are imported.



## Why did injection get error. What did I learn?

Because I had the wrong .NET version targeted in project settings and I had not loaded the references correctly into the new project. The error I was getting was from trying to import DLLs which are imported by default, System* etc.


## Issue trying to get info from the game

currently I am running the following code

```csharp
    public void Update()
    {
        //Do stuff here on every tick
        PlayerLifeTotal p = new PlayerLifeTotal();
        SimpleLog.LogError("#### Total Life: " + p.LifeThreshold);
    }
```

And i get the following error from the logs I am loggin to:
```text
#### Total Life: 0
PlayerLifeTotal must be instantiated using the ScriptableObject.CreateInstance method instead of new PlayerLifeTotal.   
```

## Taking Notes on reading Code.

I found some interesting code while trying to figure out what type of objects I can look for using the . I found `ChooseStartingPlayerWorkflow` then I found `MtgGameState` which contained `MtgPlayer`. These classes contain a lot of game logic and initialization of the game which should be fun to tamper with. However, They are not `ScriptableObjects` in Unity.

This means that, in order to use the search function `GameObject.FindObjectsOfType<BLA>()`, I need to find an object which is scriptable that has access to the board state. Where can that be?


Looking further into the code I also found that the object `PlayerLifeTotal` inherits from `BoardstateHeuristic` which inherits from `ScriptableObject`. I am sure that there are more `BoardstateHeuristic` to be found in the game using dnSpy.

note: taking a brake now with CSGO before Ruphert wakes up. This is good enough for today.



## How to find derived classes in dnSpy?

From chat gpt
```
dnSpy is a powerful tool for reverse engineering and analyzing .NET applications. If you want to view all methods that inherit from a particular class using dnSpy, you can follow these steps:

Open the Assembly in dnSpy:
Launch dnSpy and open the .NET assembly (DLL or EXE) that you want to analyze.

Navigate to the Inherited Class:
In the Assembly Explorer pane on the left, locate the namespace that contains the class you are interested in. Expand the nodes to find the class itself.

View Inherited Methods:
Right-click on the inherited class, and select "Go To" -> "Derived Types" from the context menu. This will show you a list of all classes that inherit from the selected class.

View Methods of Derived Classes:
For each derived class listed, you can expand the nodes to reveal the methods and other members of that class. Double-clicking on a method will open the code in the main panel, allowing you to view and analyze the code.

Use Search and Filters:
You can also use the search functionality in dnSpy to quickly find methods within the derived classes. The search bar is located at the top of the Assembly Explorer. You can search by method name, signature, or any text that you want to find within the code.
```

## More dnSpy tool tips

There is a function in dnSpy where I can look for how code is used elsewhere in the code base. Simply right-click the class or method of interest and choose "Analyze". This results in the bottom pane showing "Analyzer" with the following options:

- Exposed by
- Extension methods
- Instantiated by
- Used by

which can give some context around how something is used. This is a lot more effective than trying to manually navigate the code. Also, I am looking at dotPeek which is jetbrains version of the dnSpy tool, this tool has better search functionality, but does not allow hacking the code.

## Found some interesting classes

- `BoardstateHeuristic`
- `DeckHeuristic`
- `CardHeuristic`


## AssetLoader

Today I read the asset loader. It imports the following 
```chsarp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AssetLookupTree;
using UnityEngine;
using Wizards.Mtga;
using Wizards.Mtga.Assets;
using Wizards.Mtga.Platforms;
using Wotc.Mtga;
using Wotc.Mtga.Extensions;
```

what caught my eye was the following boolean

```csharp
	private static IAssetLoader Instance
	{
		get
		{
			if (AssetLoader._instance == null)
			{
				bool isPlaying = Application.isPlaying;
			}
			return AssetLoader._instance;
		}
	}
```

Which is getting data from the application about the playing state. I will try and see if this state changes when I start a match today! I will also try to write this state to the GUI window instead of outputing to the logs as this will be faster than writing to logs and I will learn something new doing this!

## Hypothesis: The Application.playing is a variable that shows if the game is running or is paused from the Unity UI.

This likely does not have to do with the state of the playing state that I am interested in.


Hypothesis was confirmed while tring to output the data to the screen. The GUI creation was successful! :D


##  2023-08-18: Hit the motherload

I think I had been going at this problem the wrong way. Online tutorials were talking about the UnityEngine and GameObject oriented programming. during the day I have been thinking about object oriented programming, interfaces and static variables. I was searching around in the code today for "GameState" when I found the class `GameState_ActivePlayer`. This file was not importing anything from `UnityEngine`, However, it had the following imports:

```csharp
using System;
using AssetLookupTree.Blackboard;
using GreClient.Rules;

namespace AssetLookupTree.Extractors.GameState.GameInfo
{
	// Token: 0x0200127A RID: 4730
	public class GameState_ActivePlayer : IExtractor<int>
...
```

This class is inheriting from `IExtractor`, what could that be?


```csharp
using System;
using AssetLookupTree.Blackboard;

namespace AssetLookupTree.Extractors
{
	// Token: 0x02001206 RID: 4614
	public interface IExtractor<T>
	{
		// Token: 0x06007AE1 RID: 31457
		bool Execute(IBlackboard bb, out T value);
	}
}
```


Not Really sure about the syntax here. But semantically, it looks like It executes a `IBlackboard` and outputs a generic type `T` with `value`. Lets look at this interface `IBlackboard`

```csharp
...
using AssetLookupTree.Extractors.DRS;
using AssetLookupTree.Extractors.UI;
using AssetLookupTree.Payloads.Ability.Metadata;
using GreClient.CardData;
using GreClient.Rules;
using TMPro;
using UnityEngine;
using Wizards.MDN;
using Wizards.Mtga.FrontDoorModels;
using Wotc.Mtga.Cards;
using Wotc.Mtga.Cards.Database;
using Wotc.Mtga.Cards.Text;
using Wotc.Mtga.Duel;
using Wotc.Mtga.DuelScene.AvatarView;
using Wotc.Mtga.DuelScene.Interactions;
using Wotc.Mtga.Hangers;
using Wotc.Mtga.Wrapper;
using Wotc.Mtgo.Gre.External.Messaging;
```

Looking at the above imports we see lots of `Wotc` code. We also see `UnityEngine` imported. This gave me a hunch that this interface could be interesting. Scrolling down in the code I see the following get/set methods

```csharp
		// Token: 0x1700136D RID: 4973
		// (get) Token: 0x060078EF RID: 30959
		// (set) Token: 0x060078F0 RID: 30960
		PlayerInfo PlayerInfoGame { get; set; }

		// Token: 0x1700136E RID: 4974
		// (get) Token: 0x060078F1 RID: 30961
		// (set) Token: 0x060078F2 RID: 30962
		MtgPlayer Player { get; set; }

		// Token: 0x1700136F RID: 4975
		// (get) Token: 0x060078F3 RID: 30963
		// (set) Token: 0x060078F4 RID: 30964
		MtgGameState GameState { get; set; }
```


Bingo! I think I can use this interface to get `PlayerInfo` and `MtgGameState` which should contain juicy info about the game!


##  2023-08-18 30 min later: ... but not quite.


Looking further into the code it looks like I need a reference to a `IBlackboard` implementing object to be able to get the info I need anyway. Kind of circular if you ask me. SOOOO I poked around a bit longer in the code and found `GameManager` which sounds interesting. 


`GameManger` is indeed an interesting object. Looking at the `Start` function for example we see the following code

```csharp
	NPEControllerPrefab payload = assetLookupTree.GetPayload(this._assetLookupSystem.Blackboard);
```

As we see its using `assetLookupTree.GetPayload` get some payload of blackboard interfaces. This is sort of the game engine I guess which dynamically gets a payload from the assetlookupsystem based on what type of object is passed to `GetPayload`. I wonder if I can get hold of the GameManager. This should be the way to successfully retreiving data from the game and acting on it accordingly.


## Next Step: Getting some data from the GameManager

 ```csharp
      GameManager thisGameManger = GetGameManager();
        MtgGameState gs = thisGameManger.GetCurrentGameState();
        MtgPlayer activePlayer = gs.ActivePlayer;
        if (activePlayer == null)
        {
            UIHelper.Label("Active Player was null. Enter game?");
        }
        else
        {
            if(activePlayer.IsLocalPlayer)
                UIHelper.Label("Active Player is: Local");
            else
                UIHelper.Label("Active Player is : OP");
            if (activePlayer.LifeTotal < 10)
                activePlayer.LifeTotal = 1337;
            UIHelper.Label("LifeTotal: " + activePlayer.LifeTotal);
            
            
        }
```

I also learned that `activePlayer` is the player holding priority and not the player whose turn it is.

## 2023-08-18 21:53 Conclusion of day

Interesting day. No cheating yet but I do have access to the game data. While perusing the `MtgGameState` class, I discovered that several class variables that suggests that the local client does not have more information about the gamestate than the player should know. For example:

```csharp

		// Token: 0x04003BCE RID: 15310
		public ZoneType Type;

		// Token: 0x04003BCF RID: 15311
		public uint Id;

		// Token: 0x04003BD0 RID: 15312
		public MtgPlayer Owner;

		// Token: 0x04003BD1 RID: 15313
		public Visibility Visibility;

		// Token: 0x04003BD2 RID: 15314
		public List<uint> CardIds = new List<uint>();

		// Token: 0x04003BD3 RID: 15315
		public List<MtgCardInstance> VisibleCards = new List<MtgCardInstance>();
```	 


Here we see that there is a list of `CardIds`, a list of ints, and a list of Visiblecards, which is a list of `MtgCardInstance`. My guess is that the CardIds are known serverside and that they become VisibleCards when the player knows what they are, by sending the CardIds that should be revealed to the server. Perhaps we can forge these requests!? 

Let the hacking begin.


hehehe xD * Evil Laugh *



