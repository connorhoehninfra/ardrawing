﻿using UnityEngine;
using CW.Common;
using PaintCore;

namespace PaintIn3D
{
	/// <summary>This component allows you to rotate the current <b>Transform</b>.</summary>
	[HelpURL(CwCommon.HelpUrlPrefix + "CwRotate")]
	[AddComponentMenu(CwCommon.ComponentMenuPrefix + "Rotate")]
	public class CwRotate : MonoBehaviour
	{
		/// <summary>This allows you to set the coordinate space the movement will use.</summary>
		public Space Space { set { space = value; } get { return space; } } [SerializeField] private Space space = Space.Self;

		/// <summary>The position will be incremented by this each second.</summary>
		public Vector3 PerSecond { set { perSecond = value; } get { return perSecond; } } [SerializeField] private Vector3 perSecond;

		protected virtual void Update()
		{
			transform.Rotate(perSecond * Time.deltaTime, space);
		}
	}
}

#if UNITY_EDITOR
namespace PaintIn3D
{
	using UnityEditor;
	using TARGET = CwRotate;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(TARGET))]
	public class CwRotate_Editor : CwEditor
	{
		protected override void OnInspector()
		{
			TARGET tgt; TARGET[] tgts; GetTargets(out tgt, out tgts);

			Draw("space", "This allows you to set the coordinate space the movement will use.");
			Draw("perSecond", "The position will be incremented by this each second.");
		}
	}
}
#endif