using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class Herbivore : CreatureAgent
{
    protected override bool CanEat{
        get
        {
            if (FirstAdjacent("plant") != null) return true;
            else return false;
        }
    }
    
    override protected void Attack()
    {
        Defend();
    }

    override protected void Eat()
    {
        if (CanEat)
        {
            var adj = FirstAdjacent("plant");
            if (adj != null)
            {
                var creature = adj.GetComponent<Plant>();
                var consume = Mathf.Min(creature.Energy, MaxEnergy);
                creature.Energy -= consume;
                if (creature.Energy < .1)
                {
                    creature.Die();
                }
                Energy += consume;
                AddReward(.35f);
                currentAction = "Eating";
            }
        }
    }

}