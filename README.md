# Hax

This was a personal summerproject in 2023 where I tried to Hack Magic The Gathering Arena becuase I was bored with the game and it was a bit expensive. MTGA is written in C# and not cross compiled to c++. There are I found out that there is a large modding community for C# created games, plenty of tools for injecting code into the running application, and modding/patching existing game functionality.

-- Alex 2024

Here is my hacking diary from the project uncensored:

# credentials lololol

mtga

batbingbongbat@proton.me:]2_]]Uc:C*M!Tu,
## Investigations 

- [ ] Is it possible to investigate the rng?

## Cheats to Implement

- [ ] Always start
- [ ] Supress Hover events
- [ ] Perfect info on OP Hand
- [ ] Look at the top card of your library
- [ ] stack your deck
- [ ] open all boosters in inventory
- [ ] Starting life Total

## Next Implementation

- [x] Get data from the game
- [x] Test setting localplayerlife to 20
- [x] create GUI for hack
- [ ] Investigate how to interact with frontdooraws. Look for interesting functions that could be hooked and logged.
- [ ] Look for simple way to interact with game
- [ ] Compile mono to debug the game
- [ ] run the debugger stepping through interesting functions
- [ ] Interact with the game function through GUI button like "Next Phase" for example
   
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



## 2023-08-21: TIL: Unity version 2020.3.13f1 was used to create the latest mtga

I learned this by looking at the executable and running `Application.unityVersion` in my gui. Pretty neat :) 

https://unity.com/releases/editor/whats-new/2020.3.13


I need this info to compile `mono.dll` using the correct unity version so I can attach the dnspy debugger to the process. My plan is to do this 

## 2023-08-23: Going for the lulz

Today I read some code, After trying to figure out the networking I found that MTGA uses its own networking modules instead of
unity supplied networking. Not sure why, but it sounds like it could be exploited. Lets try it out later on!

However, that sounded kind of tedious so I just searched for "rewards" and "gems", and I found some code that does just that. Lets look at it.

```csharp
namespace Core.Rewards
namespace Core.Rewards
{
	// Token: 0x0200151E RID: 5406
	[Serializable]
	public abstract class AmountReward<P> : RewardBase<P> where P : RewardDisplay
	{
		// Token: 0x060082CC RID: 33484
		public void AddIfPositive(int amount)
		{
			if (amount > 0)
			{
				this.ToAddCount += amount + 5000; // lololol
			}
		}

		// Token: 0x060082CD RID: 33485 RVA: 0x0025B559 File Offset: 0x00259759
		public override void ClearAdded()
		{
			this.ToAddCount = 0;
		}

		// Token: 0x060082CE RID: 33486 RVA: 0x0025B562 File Offset: 0x00259762
		protected IEnumerator DisplayAmountRewardPile(ContentControllerRewards ccr, int childIndex, WwiseEvents sfx, int amt, YieldInstruction andThen = null)
		{
			if (base.Instances.Count == 0)
			{
				RewardDisplay rewardDisplay = base.Instantiate(ccr, childIndex);
				rewardDisplay.SetCountText(amt);
				rewardDisplay.GetComponent<Animator>().SetTrigger(ContentControllerRewards.QuantityUpdate);
				AudioManager.PlayAudio(sfx, rewardDisplay.gameObject);
			}
			yield return andThen;
			yield break;
		}
	}
}
```

I also found that `RewardDisplay` is a MonoBehaviour object which I can search for in my hack :D Maybe I can hack it by simple adding 5000!

Moreover


I am just gonna list all the awesome code I found. hehe


- `GoldReward`
- `GemReward`
- `MatchConfigurator`
- `GameObjectExtensions`

last comment before commence hack. GameObjecExtensions extends functionality of the Unity Object. So, I think why I didnt find a lot of code before was because
I assumed that mtga gameobjects were inheriting from `GameObject` when in fact a lot of functionality from Unity has been extended into objects found in the mtga
code base.

## 2023-03-09 Harmony And patching Debug Settings


### debug key
Learned that there is a debugkey to be pressed in `Wotc.Mtga.DuelScene.DebugUI`

```csharp
		private bool IsDebugKeyPressed()
		{
			return Input.GetKeyDown(KeyCode.F2);
		}
```

### Setting DebugBuild in `UnityEngine.Debug`

didnt work

### Setting debug account in wotc

This worked! I am able to enable the debug menu using `left alt` and `f2` buttons! I am the greatest hacker : D lots of links to wizard of the coast confluence. lololol.


Debug account worked. Now I need to read the code and see what the debug functions actually do.



# 2023-09-05: HarmonyMonoInjector

The old MonoInjector stopped working. Likely related to windows defender. Started using the other HarmonyMonoInjector instead. which is working fine

```
C:\Users\frogman\source\tools\HarmonyMonoInjector>dir
 Volume in drive C has no label.
 Volume Serial Number is 3AFB-9C23

 Directory of C:\Users\frogman\source\tools\HarmonyMonoInjector

09/03/2023  10:50 AM    <DIR>          .
09/03/2023  10:50 AM    <DIR>          ..
09/04/2023  10:31 PM               198 config.json
07/24/2023  05:50 AM    <DIR>          Harmony
09/03/2023  01:01 PM               416 Injector.deps.json
07/14/2023  02:49 PM            28,160 Injector.dll
07/14/2023  02:49 PM           147,968 Injector.exe
06/30/2023  06:53 AM               253 Injector.runtimeconfig.json
06/30/2023  06:53 AM    <DIR>          ref
               5 File(s)        176,995 bytes
               4 Dir(s)  54,648,979,456 bytes free

C:\Users\frogman\source\tools\HarmonyMonoInjector>pwd
'pwd' is not recognized as an internal or external command,
operable program or batch file.

C:\Users\frogman\source\tools\HarmonyMonoInjector>
```

just execute the `Injector.exe` file. `config.json` configured run paramters. Works better than old injector as parameter configured once.

# 2023-09-08: Debugging and wotc rep comments

so I was able to open up some debuggin menus.

The cool thing about one of the menus was to be able to see the packages sent between client and server in real time. Many of the packets are BusinessIntelligence, latancy and client performance stats, which are not important for hacking the game. However, some were sending commands to the server, which are interesting.

It would be nice to have a list of commands available, Maybe find the API that is generating these commands. I think it is time to compile mono to be able to debug which functions are generating the AWSfrontdoor commands during gameplay...

# 2023-09-09: Resetting new quest

# 2023-09-09: Studying some payloads from gameplay

I found something better, `ClientToGREMessage` class seems to contain a lot of commands that the client sends to the server. Let's try to send some messages using the GUI. For example, `ForceDrawReq`, which forces a draw? This is not something that a user can do normally, haha xD Let's try it :).



# 2023-09-09: Compiling Mono on Windows 10

I posted a thread on the UnknownCheats forum about this. I also installed vs2015 to try and compile ethe dnspy-Unity-mono. We will see how this playes out.

# 2023-09-09: How the code works

Ok! So I finally get the basics of the code and how it works. The code is working through a pub-sub scheme, where events are published by one module and subscribed to by another. This acheives high decoupling of modules. For example, read the following code

```csharp
public class MainThreadDispatcher : MonoBehaviour
{
		public static void Dispatch(Action action)
		{
			MainThreadDispatcher.Instance.Add(action);
		}

		// Token: 0x04008C95 RID: 35989
		private static MainThreadDispatcher _instance;

		// Token: 0x04008C96 RID: 35990
		private ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();
		...
}

```

Along with for example,

```csharp
public class WrapperController : MonoBehaviour
{
	public void Init(object panelContext)
	{
		this.InventoryManager.Subscribe(InventoryUpdateSource.MercantilePurchase, new Action<ClientInventoryUpdateReportItem>(this.OnMercantilePurchase), null, false);
		
}
```

# 2023-09-10  FrontDoorconnectionAws (fdc)

this is where the server interaction happens. There is websockets at play so messages go in both directions. Incoming messages in the form of actions.

Anywhooo,

I was looking at the code to see what data is sent along `JoinEvent` messages. I see that I can trick the server into sending the correct ammount of money. This I will test to see if I can join events for free. I will also try send negative ammounts of money to see if the server gets fooled and subtracts negative ammount from my account, e.g giving me money > D muhohahaha.

I also saw that I can hack this function `CrackBoosterFromSetCode`. one of my goals was to crack all boosters at once as it is so tedious to click through all boosters. But lets start with trying to join events for free. To start with I will just create a static money `1500` gems and try to join the queue. I expect this to fail as there is likely some serverside verification that I have the money in my account.




# 2023-09-19 joining events for free

This didnt work due to server side validations : ( , what I want to do next is to fuzz the various parameters and also try nosql injection as I saw that
the server was using mongo in some of the documentation.


# 2023-09-19 Determining the endpoints to fuzz

What I want do know is
- [ ] Log all the communication going out from the client to the server by hooking the tcp connection functions used by the program
- [ ] Attach a debugger
- The names of the server endpoints

Maybe forgetting about tls cracking is a good idea as I

# 2023-06-10 fuzzing the api

Hello again dark diary,

i've decided that hacking the client seems cumbersome for a couple of reasons:

- I think i need to keep track of the state that the client is in to test functionality
- AFAIK, attaching a debugger requires me to compile mono from source, which I have tried and failed with.

Armed with the above knowledge, I've decided to take a new route. Today I found the classes `WizardsAccountInformation` and `WASHTTPClient`, looking att the `WASHTTPClient` we see that we have the following REST api to interact with,

```
https://api.platform.wizards.com/
```

and the following endpoints:
- POST, `accounts/forgotpassword`
- Get, `accounts/socialidentities`
- RegisterAsFullAccount, `accounts/register`
- GetPurchaseToken, `xsollaconnector/client/token`
- GetProfileToken, `xsollaconnector/client/profile`
- GetAllEntitlementsByReceiptIdAndSource, `entitlements/source/" + source + "/receipt/" + receiptId`

and much more. All we need from the game is to get the following pieces of information 

- `WASHTTPClient.ClientID`
- `WASHTTPClient.ClientSecret`

So we need to log them from the game client! hehe : ) shouldnt be too hard.

## Loggin the HTTPClient id and secret from the game client


