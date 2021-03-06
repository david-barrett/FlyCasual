﻿using UnityEngine;

namespace RulesList
{
    public class AsteroidLandedRule
    {

        public AsteroidLandedRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Ship.GenericShip.OnTryPerformAttackGlobal += CanPerformAttack;
            Ship.GenericShip.OnPositionFinishGlobal += InformLandedOnAsteroid;
        }

        private void InformLandedOnAsteroid()
        {
            if (Selection.ThisShip.IsLandedOnObstacle)
            {
                Messages.ShowErrorToHuman("Landed on asteroid, cannot attack");
            }
        }

        public void CanPerformAttack(ref bool result)
        {
            if (Selection.ThisShip.IsLandedOnObstacle)
            {
                Messages.ShowErrorToHuman("Landed on asteroid - cannot attack");
                result = false;
            }
        }

    }
}
