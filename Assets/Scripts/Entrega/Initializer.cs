using UnityEngine;
using ChallengeAI;

public class Initializer : FSMInitializer
{
    public override string Name => "Gabriel Beltz";

    public override void Init()
    {
        RegisterState<DecidingState>(States.StatesEnum.Decision.ToString());
        RegisterState<WalkingFlag>(States.StatesEnum.GetFlag.ToString());
        RegisterState<WalkingBase>(States.StatesEnum.ReturnToBase.ToString());
        RegisterState<ReturningFlag>(States.StatesEnum.ReturnFlag.ToString());
        RegisterState<WalkingEnergy>(States.StatesEnum.GetEnergy.ToString());
        RegisterState<WaitingEnergy>(States.StatesEnum.WaitEnergy.ToString());
        RegisterState<Aiming>(States.StatesEnum.Shoot.ToString());
        RegisterState<WalkingAmmo>(States.StatesEnum.GetAmmo.ToString());
    }
}
