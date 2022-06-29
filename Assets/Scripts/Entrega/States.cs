using UnityEngine;
using ChallengeAI;

public class States { public enum StatesEnum { Decision, WaitEnergy, GetFlag, GetEnergy, ReturnFlag, ReturnToBase, Shoot, GetAmmo} }

public class DecidingState : State
{
    public DecidingState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }

    public override void Enter() {}
    public override void Exit() { }
    public override void Update(float deltaTime) 
    {
        if(Agent.Data.Energy > 50) ChangeState(Agent.Data.HasFlag ? States.StatesEnum.ReturnToBase.ToString() : States.StatesEnum.GetFlag.ToString());
        else if(Agent.Data.Energy < 40) ChangeState(States.StatesEnum.WaitEnergy.ToString());
        else ChangeState(States.StatesEnum.GetEnergy.ToString()); 

        if(Agent.Data.FlagState == FlagState.Dropped) ChangeState(States.StatesEnum.ReturnFlag.ToString());
        if(Agent.EnemyData[0].FlagState == FlagState.Dropped) ChangeState(States.StatesEnum.GetFlag.ToString());
        if(Agent.Data.FlagState == FlagState.Catched) 
        {
            if(Agent.Data.Ammo > 0) ChangeState(States.StatesEnum.Shoot.ToString());
            else if(Agent.Data.AmmoRefill.GetValue(0) != null) ChangeState(States.StatesEnum.GetAmmo.ToString());
            else ChangeState(States.StatesEnum.WaitEnergy.ToString());
        }
    }
}

public class WalkingState : State
{
    public WalkingState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public Vector3 Destination;

    public override void Enter() => Agent.Move(Destination);
    public override void Exit() {}

    public override void Update(float deltaTime) 
    {
        if(Agent.Data.Energy < 5) ChangeState(States.StatesEnum.Decision.ToString());
        if(Agent.EnemyData[0].HasFlag) ChangeState(States.StatesEnum.Decision.ToString());

        if(Agent.Data.HasSightEnemy && Agent.EnemyData[0].HasFlag && Agent.EnemyData[0].Speed > 0) Agent.Fire();

        if(Agent.Data.RemainingDistance > 0.1f) return;
        ChangeState(States.StatesEnum.Decision.ToString());
    }
}

public class WalkingFlag : WalkingState {
    public WalkingFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter() 
    {
        Destination = (Vector3)Agent.EnemyData[0].FlagPosition;
        base.Enter();
    }
}

public class ReturningFlag : WalkingState {
    public ReturningFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter() 
    {
        Destination = (Vector3)Agent.Data.FlagPosition;
        base.Enter();
    }
}

public class WalkingEnergy : WalkingState {
    public WalkingEnergy(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter() 
    {
        Destination = Agent.Data.PowerUps[Agent.Data.PowerUps.Length - 1];
        base.Enter();
    }
}

public class WalkingAmmo : WalkingState {
    public WalkingAmmo(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter() 
    {
        Destination = Agent.Data.AmmoRefill[0];
        base.Enter();
    }
}

public class WalkingBase : WalkingState {
    public WalkingBase(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }
    public override void Enter() 
    {
        Destination = Agent.Data.StartPosition;
        base.Enter();
    }
}

public class WaitingEnergy : State {
    public WaitingEnergy(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }

    public override void Enter() { }
    public override void Exit() { }

    public override void Update(float deltaTime) 
    {
        float rotation = Mathf.Rad2Deg * Mathf.Atan2(Agent.EnemyData[0].Position.x - Agent.Data.Position.x, Agent.EnemyData[0].Position.z - Agent.Data.Position.z);
        Agent.Rotate(rotation);
        
        if(Agent.Data.HasSightEnemy && Agent.EnemyData[0].Speed > 0) Agent.Fire();

        if(Agent.Data.Energy > 75) ChangeState(States.StatesEnum.Decision.ToString());
    }
}

public class Aiming : State {
    public Aiming(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) { }


    public override void Enter() { }
    public override void Exit() { }

    public override void Update(float deltaTime) 
    {
        float rotation = Mathf.Rad2Deg * Mathf.Atan2(Agent.EnemyData[0].Position.x - Agent.Data.Position.x, Agent.EnemyData[0].Position.z - Agent.Data.Position.z);
        Agent.Rotate(rotation);

        if(Agent.Data.HasSightEnemy && Agent.EnemyData[0].Speed > 0) Agent.Fire();

        if(Agent.EnemyData[0].HasFlag) ChangeState(States.StatesEnum.Decision.ToString());
    }
}
