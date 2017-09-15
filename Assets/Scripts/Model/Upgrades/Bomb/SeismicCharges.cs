﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Ship;

namespace UpgradesList
{

    public class SeismicCharges : GenericTimedBomb
    {

        public SeismicCharges() : base()
        {
            Type = UpgradeType.Bomb;
            Name = ShortName = "Seismic Charges";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/upgrades/Bomb/seismic-charges.png";
            Cost = 2;

            bombPrefabPath = "Prefabs/Bombs/SeismicCharge";

            IsDiscardedAfterDropped = true;
        }

        public override void ExplosionEffect(GenericShip ship, Action callBack)
        {
            ship.AssignedDamageDiceroll.AddDice(DiceSide.Success);

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Suffer damage from bomb",
                TriggerType = TriggerTypes.OnDamageIsDealt,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = ship.SufferDamage,
                EventArgs = new DamageSourceEventArgs()
                {
                    Source = this,
                    DamageType = DamageTypes.BombDetonation
                }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnDamageIsDealt, callBack);
        }

        public override void PlayDetonationAnimSound(Action callBack)
        {
            BombObject.transform.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1, delegate { PlayDefferedSound(callBack); });
        }

        private void PlayDefferedSound(Action callBack)
        {
            Sounds.PlayBombSound("SeismicBomb");
            BombObject.transform.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(1.4f, delegate { callBack(); });
        }

    }

}