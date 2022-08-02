using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace CFC
{
	[RequireComponent(typeof(TPFightingController))]
	public class CFCInputs : StarterAssetsInputs
	{
		public bool attack;
		public bool toggleAttack;
		public bool action;
		public bool block;

		private EventSystem _currentEventSys = EventSystem.current;

		void Start()
		{
			_currentEventSys = EventSystem.current;
		}

		private void OnAttack(InputValue value)
		{
			if (CheckCanMove())
			{
				AttackInput(value.isPressed);
			}
		}
		
		private void OnToggleAttack(InputValue value)
		{
			if (CheckCanMove())
			{
				ToggleAttackInput(value.isPressed);
			}
		}
		
		private void OnAction(InputValue value)
		{
			if (CheckCanMove())
			{
				ActionInput(value.isPressed);
			}
		}
		
		private void OnBlock(InputValue value)
		{
		    if(CheckCanMove())
		    {
		    	BlockInput(value.isPressed);
		    }
		}
		
		private void AttackInput(bool newAttackState)
		{
			if (CheckCanMove())
			{
				attack = newAttackState;
			}
		}
		
		private void ToggleAttackInput(bool newToggleAttackState)
		{
			if (CheckCanMove())
			{
				toggleAttack = newToggleAttackState;
			}
		}
		
		private void ActionInput(bool newActionState)
		{
			if (CheckCanMove())
			{
				action = newActionState;
			}
		}
		
		private void BlockInput(bool newBlockState)
		{
			if (CheckCanMove())
			{
				block = newBlockState;
			}
		}

		private bool CheckCanMove()
		{
			MenuManager.Instance.ShowTutorial(false);
			return CheckState() && CheckInputField();
		}

		private bool CheckState()
		{
			return GameManager.Instance.currentMatchState != 1 &&
			       GameManager.Instance.currentMatchState != 3;
		}

		private bool CheckInputField()
		{
			return _currentEventSys?.currentSelectedGameObject?.GetComponent<TMP_InputField>() == null;
		}
	}
	
}