﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CW.Common;

namespace PaintCore
{
	/// <summary>This component fills the attached UI Image based on the total amount of pixels that have been painted in the specified <b>CwChangeCounterFill</b> components.</summary>
	[RequireComponent(typeof(Image))]
	[HelpURL(CwCommon.HelpUrlPrefix + "CwChangeCounterFill")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Change Counter Fill")]
	public class CwChangeCounterFill : MonoBehaviour
	{
		/// <summary>This allows you to specify the counters that will be used.
		/// Zero = All active and enabled counters in the scene.</summary>
		public List<CwChangeCounter> Counters { get { if (counters == null) counters = new List<CwChangeCounter>(); return counters; } } [SerializeField] private List<CwChangeCounter> counters;

		/// <summary>Inverse the fill?</summary>
		public bool Inverse { set { inverse = value; } get { return inverse; } } [SerializeField] private bool inverse;

		[System.NonSerialized]
		private Image cachedImage;

		protected virtual void OnEnable()
		{
			cachedImage = GetComponent<Image>();
		}

		protected virtual void Update()
		{
			var finalCounters = counters.Count > 0 ? counters : null;
			var ratio         = CwChangeCounter.GetRatio(finalCounters);

			if (inverse == true)
			{
				ratio = 1.0f - ratio;
			}

			cachedImage.fillAmount = Mathf.Clamp01(ratio);
		}
	}
}

#if UNITY_EDITOR
namespace PaintCore
{
	using UnityEditor;
	using TARGET = CwChangeCounterFill;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwChangeCounterFill_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("counters", "This allows you to specify the counters that will be used.\n\nZero = All active and enabled counters in the scene.");

			Separator();

			Draw("inverse", "Inverse the fill?");
		}
	}
}
#endif