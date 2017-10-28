﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipMovementScript : MonoBehaviour {

    //TODO: Refactor old
    public Collider CollidedWith;
    public Collider ObstacleEnter;
    public Collider ObstacleExit;
    public Collider ObstacleHitEnter;
    public Collider ObstacleHitExit;

    public List<System.Func<bool>> FuncsToUpdate = new List<System.Func<bool>>();

    public bool isMoving;

    void Update ()
    {
        Selection.UpdateSelection();
        UpdateMovement();
        UpdateSubscribedFuncs();
        ClearCollision();
    }

    private void UpdateMovement()
    {
        if (isMoving)
        {
            Selection.ThisShip.AssignedManeuver.UpdateMovementExecution();
        }
    }

    private void UpdateSubscribedFuncs()
    {
        List<System.Func<bool>> subscribedFuncs = new List<System.Func<bool>>();
        subscribedFuncs.AddRange(FuncsToUpdate);

        foreach (var func in subscribedFuncs)
        {
            bool isFinished = func();
            if (isFinished) FuncsToUpdate.Remove(func);
        }        
    }

    private void ClearCollision()
    {
        CollidedWith = null;
    }

    //Assignment and launch of execution of meneuver

    public void AssignManeuverButtonPressed()
    {
        UI.HideDirectionMenu();

        string maneuverCode = EventSystem.current.currentSelectedGameObject.name;

        if (!Network.IsNetworkGame)
        {
            AssignManeuver(Selection.ThisShip.ShipId, maneuverCode);
        }
        else
        {
            Network.AssignManeuver(Selection.ThisShip.ShipId, maneuverCode);
        }
        
    }

    public void AssignManeuver(int shipId, string maneuverCode)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);
        UI.HideContextMenu();

        Selection.ThisShip.SetAssignedManeuver(MovementFromString(maneuverCode));

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase))
        {
            Roster.HighlightShipOff(Selection.ThisShip);

            if (Roster.AllManuersAreAssigned(Phases.CurrentPhasePlayer))
            {
                UI.ShowNextButton();
                UI.HighlightNextButton();
            }
        }
        else
        {
            Triggers.FinishTrigger();
        }
    }

    public Movement.GenericMovement MovementFromStruct(Movement.MovementStruct movementStruct)
    {
        Movement.GenericMovement result = null;

        if (movementStruct.Bearing == Movement.ManeuverBearing.Straight)
        {
            result = new Movement.StraightMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.KoiogranTurn)
        {
            result = new Movement.KoiogranTurnMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Turn)
        {
            result = new Movement.TurnMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Bank)
        {
            result = new Movement.BankMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.SegnorsLoop)
        {
            result = new Movement.SegnorsLoopMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.TallonRoll)
        {
            result = new Movement.TallonRollMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Stationary)
        {
            result = new Movement.StationaryMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }

        return result;
    }

    public Movement.GenericMovement MovementFromString(string parameters)
    {
        Movement.MovementStruct movementStruct = new Movement.MovementStruct(parameters);
        return MovementFromStruct(movementStruct);
    }

    public void PerformStoredManeuverButtonIsPressed()
    {
        if (!Network.IsNetworkGame)
        {
            PerformStoredManeuver(Selection.ThisShip.ShipId);
        }
        else
        {
            Network.PerformStoredManeuver(Selection.ThisShip.ShipId);
        }
    }

    public void PerformStoredManeuver(int shipId)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);
        Phases.StartTemporarySubPhase("Movement", typeof(SubPhases.MovementExecutionSubPhase));
    }

}
