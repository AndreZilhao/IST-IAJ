using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public interface IWorldModel
    {
        object GetProperty(string propertyName);
        void SetProperty(string propertyName, object value);
        float GetGoalValue(string goalName);
        void SetGoalValue(string goalName, float value);
        IWorldModel GenerateChildWorldModel();
        float CalculateDiscontentment(List<Goal> goals);
        Action GetNextAction();
        Action[] GetExecutableActions();
        void DumpState();
        bool IsTerminal();
        float GetScore();
        int GetNextPlayer();
        void CalculateNextPlayer();
    }
}
