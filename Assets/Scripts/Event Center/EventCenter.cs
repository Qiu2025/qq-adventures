using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventCenter
{
    private static EventCenter instance;
    private Dictionary<string, UnityAction> eventDict = new Dictionary<string, UnityAction>();

    public static EventCenter GetInstance()
    {
        if (instance == null)
            instance = new EventCenter();
        return instance;
    }

    public void AddEventListener(string _name, UnityAction _action)
    {
        if(eventDict.ContainsKey(_name))
        {
            eventDict[_name] += _action;
        }
        else
        {
            eventDict.Add(_name, _action);
        }
    }

    public void RemoveEventListener(string _name, UnityAction _action)
    {
        if(eventDict.ContainsKey(_name))
        {
            eventDict[_name] -= _action;
        }
    }

    public void EventTrigger(string _name)
    {
        if(eventDict.ContainsKey(_name))
            eventDict[_name]();
    }

    public void Clear()
    {
        eventDict.Clear();
    }

}
