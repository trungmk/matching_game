using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GenerateNewTileState : IState
{
    private readonly GameStateMachineMono _stateMachine;

    public GenerateNewTileState(GameStateMachineMono stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}