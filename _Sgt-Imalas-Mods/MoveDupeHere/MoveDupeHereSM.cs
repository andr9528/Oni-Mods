﻿namespace MoveDupeHere
{
	internal class MoveDupeHereSM : StateMachineComponent<MoveDupeHereSM.StatesInstance>, ISaveLoadable
	//, IGameObjectEffectDescriptor
	{
		public static readonly HashedString PORT_ID = (HashedString)nameof(MoveDupeHereSM);
		[MyCmpReq]
		public Assignable assignable;
		[MyCmpGet]
		private Operational operational;
		public CellOffset targetCellOffset = CellOffset.none;
		public int TargetCellInt = -1;

		public bool HasDupeAssigned()
		{
			if (assignable != null)
				return assignable.IsAssigned() && GetAssignableProxy() != null;
			return true;
		}
		public MinionAssignablesProxy GetAssignableProxy()
		{
			if (!assignable.IsAssigned())
				return null;
			//Debug.Log("Ass: " + assignable.assignee.GetSoleOwner().GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
			assignable.assignee.GetSoleOwner().TryGetComponent<MinionAssignablesProxy>(out var proxy);
			if (proxy.target is StoredMinionIdentity)
				proxy = null;

			return proxy;
		}

		public override void OnSpawn()
		{
			assignable.OnAssign += new System.Action<IAssignableIdentity>(this.RedoAssignment);
			base.OnSpawn();
			TargetCellInt = Grid.OffsetCell(Grid.PosToCell(this), targetCellOffset);
			this.smi.StartSM();
			this.Subscribe(-801688580, new System.Action<object>(this.OnLogicValueChanged));

		}

		private void OnLogicValueChanged(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID != MoveDupeHereSM.PORT_ID)
				return;
			bool logic_on = LogicCircuitNetwork.IsBitActive(0, logicValueChanged.newValue);
			operational.SetActive(logic_on);
		}

		void RedoAssignment(IAssignableIdentity target)
		{
			smi.GoTo(smi.sm.Idle);
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}
		#region StateMachine

		public class StatesInstance : GameStateMachine<States, StatesInstance, MoveDupeHereSM>.GameInstance
		{
			public MinionAssignablesProxy minionProxy;
			public StatesInstance(MoveDupeHereSM master) : base(master)
			{
			}
		}

		public class States : GameStateMachine<MoveDupeHereSM.States, MoveDupeHereSM.StatesInstance, MoveDupeHereSM, object>
		{
			public class DupeAssignedStates : State
			{
				public State RedSignal;
				public GreenStates GreenSignal;
			}
			public class GreenStates : State
			{
				public State MovingDupe;
				public State DupeArrived;
			}

			public State Init;
			public State Idle;
			public DupeAssignedStates dupeAssignedStates;

			public override void InitializeStates(out BaseState defaultState)
			{

				defaultState = Init;

				Init.Enter((smi) =>
				{
					//Debug.Log("enter init");

					if (smi.master.HasDupeAssigned() && smi.GetComponent<Operational>().IsOperational)
					{
						smi.GoTo(dupeAssignedStates);
					}
					else
					{
						smi.GoTo(Idle);
					}
				});


				Idle

					//.Enter(smi => Debug.Log("enter idle"))
					//.PlayAnim("no_power")
					.Update((smi, dt) =>
					{
						var animController = smi.sm.masterTarget.Get<KAnimControllerBase>(smi);
						if (animController != null)
							animController.Play("no_power");

						if (smi.master.HasDupeAssigned() && smi.GetComponent<Operational>().IsOperational)
						{
							smi.GoTo(dupeAssignedStates);
						}
					}, UpdateRate.SIM_200ms);


				dupeAssignedStates.defaultState = dupeAssignedStates.RedSignal;

				dupeAssignedStates
					.Enter((smi) =>
					{
						//Debug.Log("enter assigned");
						smi.minionProxy = smi.master.GetAssignableProxy();//.GetTargetGameObject().GetComponent<Navigator>(); ;
					})
					.Update((smi, dt) =>
					{
						//Debug.Log("Operational: " + smi.GetComponent<Operational>().IsOperational + ", isActive: " + smi.GetComponent<Operational>().IsActive);
						if (!smi.master.HasDupeAssigned())
						{
							smi.GoTo(Idle);
						}
					}, UpdateRate.SIM_200ms)
					.EventTransition(GameHashes.OperationalChanged, Idle, smi => !smi.GetComponent<Operational>().IsOperational)
					.Exit((smi) =>
					{
						smi.minionProxy = null;
					});

				dupeAssignedStates.RedSignal
					.PlayAnim("off")
					//.Enter(smi => Debug.Log("enter red sig"))
					.EventTransition(GameHashes.ActiveChanged, dupeAssignedStates.GreenSignal, smi => smi.GetComponent<Operational>().IsActive)
					;
				dupeAssignedStates.GreenSignal.defaultState = dupeAssignedStates.GreenSignal.MovingDupe;

				dupeAssignedStates.GreenSignal
					.EventTransition(GameHashes.ActiveChanged, dupeAssignedStates.RedSignal, smi => !smi.GetComponent<Operational>().IsActive)
					.Enter((smi) =>
					{
						smi.minionProxy = smi.master.GetAssignableProxy();

						if (smi.minionProxy.target is StoredMinionIdentity)
							smi.minionProxy = null;
					}).PlayAnim("on");
				dupeAssignedStates.GreenSignal.MovingDupe
					.ToggleChore(CreateChore, dupeAssignedStates.GreenSignal.DupeArrived);
				dupeAssignedStates.GreenSignal.DupeArrived
					.GoTo(dupeAssignedStates.GreenSignal.MovingDupe);
				;
			}
			public Chore CreateChore(MoveDupeHereSM.StatesInstance smi)
			{
				if (smi == null || smi.minionProxy == null || smi.minionProxy.target is StoredMinionIdentity || smi.master.TargetCellInt == -1) return null;

				if (smi.minionProxy.GetTargetGameObject().TryGetComponent<Navigator>(out var nav))
				{
					MoveChore chore = new MoveChore(nav, Db.Get().ChoreTypes.MoveTo, (mover_smi => smi.master.TargetCellInt));
					chore.AddPrecondition(ChorePreconditions.instance.CanMoveToCell, (object)smi.master.TargetCellInt);
					return (Chore)chore;
				}
				return null;

			}
		}


		#endregion
	}
}


