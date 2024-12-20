﻿using UnityEngine;
using CW.Common;
using PaintCore;

namespace PaintIn3D
{
	/// <summary>This component allows you to enable/disable the target component while the specified key is held down.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwToggleScript")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Toggle Script")]
	public class CwToggleScript : MonoBehaviour
	{
		/// <summary>The key that must be held for this component to activate.
		/// None = Any mouse button or finger.</summary>
		public KeyCode Key { set { key = value; } get { return key; } } [SerializeField] private KeyCode key = KeyCode.Mouse0;

		/// <summary>The component that will be enabled or disabled.</summary>
		public MonoBehaviour Target { set { target = value; } get { return target; } } [SerializeField] private MonoBehaviour target;

		/// <summary>Should painting triggered from this component be eligible for being undone?</summary>
		public bool StoreStates { set { storeStates = value; } get { return storeStates; } } [SerializeField] protected bool storeStates;

		protected virtual void Update()
		{
			if (target != null)
			{
				if (CwInput.GetKeyIsHeld(key) == true)
				{
					if (storeStates == true && target.enabled == false)
					{
						CwStateManager.PotentiallyStoreAllStates();
					}

					target.enabled = true;
				}
				else
				{
					target.enabled = false;
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	using UnityEditor;
	using TARGET = CwToggleScript;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwKeyControl_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("key", "The key that must be held for this component to activate.\n\nNone = Any mouse button or finger.");
			BeginError(Any(tgts, t => t.Target == null));
				Draw("target", "The component that will be enabled or disabled.");
			EndError();
			Draw("storeStates", "Should painting triggered from this component be eligible for being undone?");
		}
	}
}
#endif