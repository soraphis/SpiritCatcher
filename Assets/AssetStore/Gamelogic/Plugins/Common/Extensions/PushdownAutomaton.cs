using System.Collections.Generic;

namespace Gamelogic
{
	/**
		This class is a state machine that has the ability to remember previous states
		and transition back to them (FIFO).
	*/
	public class PushdownAutomaton<TLabel> : StateMachine<TLabel>
	{
		private readonly Stack<TLabel> stateHistory;

		public PushdownAutomaton()
		{
			stateHistory = new Stack<TLabel>();
		}

		public void Push(TLabel nextState)
		{
			stateHistory.Push(CurrentState);
			
			CurrentState = nextState;
		}

		public void Pop()
		{
			if (stateHistory.Count > 0)
			{
				CurrentState = stateHistory.Pop();
			}
		}
	}
}