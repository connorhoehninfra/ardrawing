﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using CW.Common;

namespace PaintCore
{
	/// <summary>This component allows you to perform an event when the specified <b>CwColorCounter</b> instances are painted a specific amount.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwColorCounterEvent")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Color Counter Event")]
	public class CwColorCounterEvent : MonoBehaviour
	{
		/// <summary>This allows you to specify the counters that will be used.
		/// <b>None</b> = All active and enabled counters in the scene.</summary>
		public List<CwColorCounter> Counters { get { if (counters == null) counters = new List<CwColorCounter>(); return counters; } } [SerializeField] private List<CwColorCounter> counters;

		/// <summary>This allows you to set which color will be handled by this component.</summary>
		public CwColor Color { set { color = value; } get { return color; } } [SerializeField] private CwColor color;

		/// <summary>This paint ratio must be inside this range to be considered inside.</summary>
		public Vector2 Range { set { range = value; } get { return range; } } [SerializeField] private Vector2 range = new Vector2(0.0f, 1.0f);

		/// <summary>This tells you if the paint ratio is within the current <b>Range</b>.</summary>
		public bool Inside { set { inside = value; } get { return inside; } } [SerializeField] private bool inside;

		/// <summary>This event will be called on the first frame <b>Inside</b> becomes true.</summary>
		public UnityEvent OnInside { get { if (onInside == null) onInside = new UnityEvent(); return onInside; } } [SerializeField] private UnityEvent onInside;

		/// <summary>This event will be called on the first frame <b>Inside</b> becomes false.</summary>
		public UnityEvent OnOutside { get { if (onOutside == null) onOutside = new UnityEvent(); return onOutside; } } [SerializeField] private UnityEvent onOutside;

		/// <summary>This tells you the current paint ratio of the specified <b>Color</b>, where 0 is no paint, and 1 is fully painted.</summary>
		public float Ratio
		{
			get
			{
				var finalCounters = counters != null && counters.Count > 0 ? counters : null;

				return CwColorCounter.GetRatio(color, finalCounters);
			}
		}

		public bool AllCountersReady
		{
			get
			{
				var finalCounters = counters != null && counters.Count > 0 ? counters : null;

				return CwColorCounter.GetReady(finalCounters);
			}
		}

		protected virtual void Update()
		{
			if (AllCountersReady == true)
			{
				UpdateInside(Ratio);
			}
		}

		private void UpdateInside(float ratio)
		{
			var newInside = default(bool);

			// Change comparison to prevent overlap when using multiple ranges that begin and end at the same value
			if (range.y == 1.0f)
			{
				newInside = ratio >= range.x && ratio <= range.y;
			}
			else
			{
				newInside = ratio >= range.x && ratio < range.y;
			}

			if (inside == true && newInside == false)
			{
				inside = false;

				if (onOutside != null)
				{
					onOutside.Invoke();
				}
			}
			else if (inside == false && newInside == true)
			{
				inside = true;

				if (onInside != null)
				{
					onInside.Invoke();
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace PaintCore
{
	using UnityEditor;
	using TARGET = CwColorCounterEvent;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwColorCounterEvent_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("counters", "This allows you to specify the counters that will be used.\n\nNone = All active and enabled counters in the scene.");

			Separator();

			BeginError(Any(tgts, t => t.Color == null));
				Draw("color", "This allows you to set which color will be handled by this component.");
			EndError();
			DrawMinMax("range", 0.0f, 1.0f, "This paint ratio must be inside this range to be considered inside.");

			BeginDisabled(true);
				var ratio = tgt.Ratio;
				EditorGUILayout.MinMaxSlider(new GUIContent("Ratio", "This tells you the current paint ratio of the specified Color, where 0 is no paint, and 1 is fully painted."), ref ratio, ref ratio, 0.0f, 1.0f);
			EndDisabled();

			Separator();

			Draw("inside", "This tells you if the paint ratio is within the current Range.");
			Draw("onInside", "This event will be called on the first frame Inside becomes true.");
			Draw("onOutside", "This event will be called on the first frame Inside becomes false.");
		}
	}
}
#endif