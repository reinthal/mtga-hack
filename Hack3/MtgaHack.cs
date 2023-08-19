using UnityEngine;
using GreClient.Rules;
using Wotc.Mtgo.Gre.External.Messaging;
public class MtgaHack : MonoBehaviour
{

    private GameManager gameManager  = GameObject.FindObjectOfType<GameManager>();
    private int gameTick = 0;
    public GameManager GetGameManager()
    {
        return this.gameManager;
    }
    private void printAllObjects()
    {

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            SimpleLog.LogError(string.Format("{0},{1},{2},{3}", gameTick, go.tag, go.GetFullPath(), go.name));
        }
    }

    public void Start()
    {
        //Do stuff here once for initialization
        
    }

    public void Update()
    {
        //Do stuff here on every 200 tick tick
        if (gameTick % 200 == 0)
        {
            this.gameManager = GameObject.FindObjectOfType<GameManager>();
            MtgGameState gs = this.gameManager.GetCurrentGameState();
            MtgZone opHand = gs.OpponentHand;
            opHand.Visibility = Visibility.Visibility_Public;
            MtgPlayer activePlayer = gs.ActivePlayer;
            SimpleLog.LogError("Player Holding Priority: Local?" + activePlayer.IsLocalPlayer);
            SimpleLog.LogError("LifeTotal LocalPlayer:" + gs.LocalPlayer.LifeTotal);
            SimpleLog.LogError("LifeTotal OP:" + gs.Opponent.LifeTotal);
        }
    }
}
