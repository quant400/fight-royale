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
		public bool punch;
		public bool kick;
		public bool action;
		public bool block;

		private EventSystem _currentEventSys = EventSystem.current;

		void Start()
		{
			_currentEventSys = EventSystem.current;
		}

		private void OnPunch(InputValue value)
		{
			if (CheckInputField())
			{
				PunchInput(value.isPressed);
			}
		}
		
		private void OnKick(InputValue value)
		{
			if (CheckInputField())
			{
				KickInput(value.isPressed);
			}
		}
		
		private void OnAction(InputValue value)
		{
			if (CheckInputField())
			{
				ActionInput(value.isPressed);
			}
		}
		
		private void OnBlock(InputValue value)
		{
		    if(CheckInputField())
		    {
		    	BlockInput(value.isPressed);
		    }
		}
		
		private void PunchInput(bool newAttackState)
		{
			if (CheckInputField())
			{
				punch = newAttackState;
			}
		}
		
		private void KickInput(bool newAttackState)
		{
			if (CheckInputField())
			{
				kick = newAttackState;
			}
		}
		
		private void ActionInput(bool newActionState)
		{
			if (CheckInputField())
			{
				action = newActionState;
			}
		}
		
		private void BlockInput(bool newBlockState)
		{
			if (CheckInputField())
			{
				block = newBlockState;
			}
		}

		private bool CheckInputField()
		{
			MenuManager.Instance.ShowTutorial(false);
			return _currentEventSys?.currentSelectedGameObject?.GetComponent<TMP_InputField>() == null;
		}
	}
	
}