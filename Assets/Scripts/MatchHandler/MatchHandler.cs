﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = EventSystem.Model.Event;

public class MatchHandler : EventBehaviour
{
    public PlayerInfo PlayerInfo;
    
    private MainTCPConnection _mainTcp;
    private MatchTCPConnection _matchTcp;
    private UDPConnection _matchUdp;
    private MessageHandler _messageHandler;

    new void Start()
    {
        base.Start();
        GameObject connectionManager = GameObject.Find("ConnectionManager");
        if (connectionManager != null)
        {
            _messageHandler = connectionManager.GetComponentInChildren<MessageHandler>();
        }

        _matchTcp = GetComponent<MatchTCPConnection>();
        _matchUdp = GetComponent<UDPConnection>();
    }

    private float lastT;

    protected override void OnEvent(Event e)
    {
        switch (e.Type)
        {
            case "MatchPlace":
                Debug.Log("We Have Match Port");
                _matchTcp.ConnectMatchServer(int.Parse(e.Info["matchPort"]));
                _matchUdp.Connect(int.Parse(e.Info["matchPort"]));
                PlayerInfo.MatchID = e.Info["matchId"];
                PlayerInfo.MatchPlayerID = e.Info["fakeId"];
                _messageHandler.TCPHandshake();
                _messageHandler.UDPHandshake();

                break;
            case "MatchUpdate":
                GameObject o = GameObject.Find(e.Info["Id"]);
                if (o == null)
                {
                    Debug.Log("Object " + e.Info["Id"] + " Not Found");
                    return;
                }

                o.GetComponent<NetworkDriven>().SetTarget(float.Parse(e.Info["X"]), float.Parse(e.Info["Y"]),
                    Mathf.Rad2Deg * float.Parse(e.Info["Angle"]));
                break;
            case "MatchStart":
                
                break;
            case "HeroID":
                if (PlayerInfo.ID == e.Info["ID"])
                {
                    PlayerInfo.HeroName = e.Info["HeroID"];
                }
                break;
        }
    }
}