using System;
using System.Collections.Generic;

namespace Gamelogic
{
	/**
		A lightweight state machine.

		To use it:
			-#	Define your own label. Enums are probably the best choice.
			-#	Construct a new state machine, typically in a MonoBehaviour's Start method.
			-#	Add the various states with the appropriate delegates. 
			-#	Call the state machine's Update method from the MonoBehaviour's Update method.
			-#	Call the ChangeState method on the state machine to transition. (You can eitther call this method 
				from one of the state delegates, or from anywhere else. 
			-#	When a state is changed, the OnStop on exisiting state is called, then the OnStart of the new state, 
				nd from there on OnUpdate of the new state each time the update is called.

		@version1_0
	*/
	public class StateMachine<TLabel>
	{
		private class State
		{
			public readonly Action onStart;
			public readonly Action onUpdate;
			public readonly Action onStop;
			public readonly TLabel label;

			public State(TLabel label, Action onStart, Action onUpdate, Action onStop)
			{
				this.onStart = onStart;
				this.onUpdate = onUpdate;
				this.onStop = onStop;
				this.label = label;
			}
		}

		private readonly Dictionary<TLabel, State> stateDictionary;
		private State currentState;

		/**
			Returns the label of the current state.
		*/

		public TLabel CurrentState
		{
			get { return currentState.label; }

			/**@version1_2*/
			set { ChangeState(value); }
		}

		/**
			Constructs a new StateMachine.
		*/

		public StateMachine()
		{
			stateDictionary = new Dictionary<TLabel, State>();
		}

		/**
			Adds a state, and the delegates that should run 
			when the state starts, stops, 
			and when the state machine is updated.

			Any delegate can be null, and wont be executed.
		*/
		public void AddState(TLabel label, Action onStart, Action onUpdate, Action onStop)
		{
			stateDictionary[label] = new State(label, onStart, onUpdate, onStop);
		}

		public void AddState(TLabel label, Action onStart, Action onUpdate)
		{
			AddState(label, onStart, onUpdate, null);
		}

		public void AddState(TLabel label, Action onStart)
		{
			AddState(label, onStart, null);
		}

		public void AddState(TLabel label)
		{
			AddState(label, null);
		}
		
		/**
			Adds a sub state machine for the given state.
			
			The sub state machine need not be updated, as long as this state machine
			is being updated.
		*/
		public void AddState<TSubstateLabel>(TLabel label, StateMachine<TSubstateLabel> substateMachine,
			TSubstateLabel substate)
		{
			AddState(
				label,
				() => substateMachine.ChangeState(substate),
				substateMachine.Update);
		}

		/**
			Changes the state from the existing one to the state with the given label.

			It is legal (and useful) to transition to the same state, in which case the 
			current state's onStop action is called, the onstart ation is called, and the
			state keeps on updating as before. The behviour is exactly the same as switching to
			a new state.
		*/
		private void ChangeState(TLabel newState)
		{
			if (currentState != null && currentState.onStop != null)
			{
				currentState.onStop();
			}

			currentState = stateDictionary[newState];

			if (currentState.onStart != null)
			{
				currentState.onStart();
			}
		}

		/**
			This method should be called every frame. 
		*/
		public void Update()
		{
			if (currentState != null && currentState.onUpdate != null)
			{
				currentState.onUpdate();
			}
		}

		public override string ToString()
		{
			return CurrentState.ToString();
		}
	}
}