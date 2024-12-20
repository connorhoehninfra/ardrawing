﻿using UnityEngine;
using System.Collections.Generic;

namespace PaintCore
{
	/// <summary>This is the base class for all paint commands. These commands (e.g. paint decal) are added to the command list for each CwPaintableTexture, and are executed at the end of the frame to optimize state changes.</summary>
	[System.Serializable]
	public abstract class CwCommand
	{
		/// <summary>This is the original array index, used to stable sort between two commands if they have the same priority.</summary>
		public int Index;

		/// <summary>Is this preview painting, or real painting?</summary>
		public bool Preview;

		/// <summary>The draw order priority of this command for this frame.</summary>
		public int Priority;

		/// <summary>The hash of the Material used to apply this paint command.</summary>
		public CwHashedMaterial Material;

		/// <summary>The material pass that will be used.</summary>
		public int Pass;

		/// <summary>The hash of the  Model used to apply this paint command.</summary>
		public CwHashedModel Model;

		/// <summary>The mesh submesh that will be painted.</summary>
		public int Submesh;

		/// <summary>The LocalMask that will be used when painting.</summary>
		public CwHashedTexture LocalMaskTexture;

		/// <summary>The channel of the LocalMaskTexture that will be used.</summary>
		public Vector4 LocalMaskChannel;

		private static int _LocalMaskTexture = Shader.PropertyToID("_LocalMaskTexture");
		private static int _LocalMaskChannel = Shader.PropertyToID("_LocalMaskChannel");

		public static void BuildMaterial(ref Material material, ref int materialHash, string path, string keyword = null)
		{
			material     = CwCommon.BuildMaterial(path, keyword);
			materialHash = CwSerialization.TryRegister(material);
		}

		public static int Compare(CwCommand a, CwCommand b)
		{
			var delta = a.Priority.CompareTo(b.Priority);

			if (delta > 0)
			{
				return 1;
			}
			else if (delta < 0)
			{
				return -1;
			}

			return a.Index.CompareTo(b.Index);
		}

		public abstract bool RequireMesh
		{
			get;
		}

		public void SetState(bool preview, int priority)
		{
			Preview  = preview;
			Priority = priority;
			Index    = 0;
		}

		public virtual void Apply(Material material)
		{
			material.SetTexture(_LocalMaskTexture, LocalMaskTexture);
			material.SetVector(_LocalMaskChannel, LocalMaskChannel);
		}

		public abstract void Pool();
		public abstract void Transform(Matrix4x4 posMatrix, Matrix4x4 rotMatrix, Matrix4x4 rotMatrix2);
		public abstract CwCommand SpawnCopy();

		public CwCommand SpawnCopyLocal(Transform transform)
		{
			var copy   = SpawnCopy();
			var matrix = transform.worldToLocalMatrix;

			copy.Transform(matrix, Matrix4x4.Rotate(matrix.rotation), Matrix4x4.identity);

			return copy;
		}

		public CwCommand SpawnCopyWorld(Transform transform)
		{
			var copy   = SpawnCopy();
			var matrix = transform.localToWorldMatrix;

			copy.Transform(matrix, Matrix4x4.Rotate(matrix.rotation), Matrix4x4.identity);

			return copy;
		}

		protected T SpawnCopy<T>(Stack<T> pool)
			where T : CwCommand, new()
		{
			var command = pool.Count > 0 ? pool.Pop() : new T();
			
			command.Index            = Index;
			command.Preview          = Preview;
			command.Priority         = Priority;
			command.Material         = Material;
			command.Pass             = Pass;
			command.Model            = Model;
			command.Submesh          = Submesh;
			command.LocalMaskTexture = LocalMaskTexture;
			command.LocalMaskChannel = LocalMaskChannel;

			return command;
		}

		public virtual void Apply(CwPaintableTexture paintableTexture)
		{
			LocalMaskTexture = paintableTexture.LocalMaskTexture;
			LocalMaskChannel = CwCommon.IndexToVector((int)paintableTexture.LocalMaskChannel);
		}
	}
}