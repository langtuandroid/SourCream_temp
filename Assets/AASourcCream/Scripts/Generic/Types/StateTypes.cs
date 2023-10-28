using System;
using System.Collections.Generic;
using UnityEngine;

public class StateGraph
{
    public State currentState { get; private set; }

    /// <summary>
    /// DO NOT USE TO SET STATE AFTER INITIALIZATION THAT HAS TO BE DONE THROUHG THE STATE CLASS ITSELF 
    /// VIA State.setConnectedStates()
    /// </summary>
    /// <param name="state"></param>
    public void setCurrentState(State state)
    {
        currentState = state;
    }
}

public class State
{
    public StateGraph stateGraph;
    public string id;

    private Action exitMethod;

    private Action enterMethod;

    public List<State> connectedStates = new List<State>();

    public State(string paramId, Action enterEvent, Action exitEvent, StateGraph graph)
    {
        id = paramId;
        exitMethod = exitEvent;
        enterMethod = enterEvent;
        stateGraph = graph;
    }

    public void setConnectedStates(params State[] states)
    {
        connectedStates.Add(this);
        for (int i = 0; i < states.Length; i++) {
            connectedStates.Add(states[i]);
        }
    }


    public void enterConnectedState(string stateId)
    {

        var state = connectedStates.Find(state => stateId == state.id);
        Debug.Log(state?.id);
        if (state?.id != null) {
            exitMethod();
            stateGraph.setCurrentState(state);
            state.enterMethod();
        } else {
            Debug.Log(stateGraph.currentState.id);
            Debug.Log(stateId);
            Debug.Log("-----------------------------------");
            for (int i = 0; i < connectedStates.Count; i++) {
                Debug.Log(connectedStates[i].id);
            }
        }
    }

    public void enterInitialState()
    {
        enterMethod();
        stateGraph.setCurrentState(this);
    }
}