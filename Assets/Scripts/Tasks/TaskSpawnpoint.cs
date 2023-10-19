using System.Collections.Generic;
using UnityEngine;

namespace WS20.P3.Overcrowded
{
	public class TaskSpawnpoint : MonoBehaviour
	{
		#region Public Fields

		public List<GameObject> prefabs;
		public GameObject mapPointer;

		public TasktypesEnum.OPTIONS option;

		#endregion
		
		//https://github.com/PeterDekkers/unity3d-gizmo-mesh-preview/blob/master/gizmoMeshPreview.cs
		#region Code By Peter Dekkers


		public bool showGizmoAlways;
		// The prefab that we want to draw gizmo meshes for
		public GameObject prefab;

		//#if UNITY_EDITOR
		// You can turn gizmo mesh rendering off via the Inspector, if you want
		public bool showGizmoMesh = true;

		// This gets set to 'true' once there are meshes cached.
		// If you need to redraw the gizmo meshes (e.g. when your prefab changes)
		// you can simply toggle this checkbox in the inspector and they
		// will instantly update.
		public bool gizmoMeshesCached = false;

		// We'll cache the meshes that we want to draw gizmos for.
		public MeshFilter[] gizmoMeshes = new MeshFilter[0];

		// Cache transforms of all the meshes to draw gizmos for
		private Transform[] gizmoMeshTransforms;

		[SerializeField]
		private bool showWireFrame;

		//#endif



		void OnDrawGizmos()
		{

			if (this.showGizmoMesh == false || Application.isPlaying)
			{
				return;
			}

			if (this.prefab != prefabs[(int)option])
			{
				this.prefab = prefabs[(int)option];
				gizmoMeshesCached = false;
			}




			Gizmos.DrawSphere(this.transform.position, 0.1f);

			// Fetch meshes inside the prefab once and cache them
			// and their transforms.
			if (!this.gizmoMeshesCached)
			{
				this.gizmoMeshes = this.prefab.GetComponentsInChildren<MeshFilter>(true);
				this.gizmoMeshTransforms = new Transform[this.gizmoMeshes.Length];
				for (int i = 0; i < this.gizmoMeshes.Length; i++)
				{
					this.gizmoMeshTransforms[i] = this.gizmoMeshes[i].GetComponent<Transform>();
				}
				if (this.gizmoMeshes.Length > 0)
				{
					this.gizmoMeshesCached = true;
				}
			}

			// If there are meshes in the array, draw a gizmo mesh for each
			if (this.gizmoMeshesCached)
			{

				for (int i = 0; i < this.gizmoMeshes.Length; i++)
				{

					// Attempt to get a vertex color for the gizmo
					if (this.gizmoMeshes[i].sharedMesh.colors.Length >= 1)
					{
						Gizmos.color = this.gizmoMeshes[i].sharedMesh.colors[0];
					}
					else
					{
						// Default to cyan
						Gizmos.color = Color.cyan;
					}

					// Adjust the position and rotation of the gizmo mesh
					if (gizmoMeshTransforms == null)
					{
						this.gizmoMeshesCached = false;
						return;
					}
					//Debug.LogError("My name is: " + this.gizmoMeshTransforms[0]);
					Vector3 pos = transform.TransformPoint(this.gizmoMeshTransforms[i].position);
					Quaternion rot = transform.rotation * this.gizmoMeshTransforms[i].rotation;
					Vector3 scale = this.gizmoMeshTransforms[i].localScale;

					// Display the gizmo mesh
					if (showWireFrame)
					{
						Gizmos.DrawWireMesh(this.gizmoMeshes[i].sharedMesh, pos, rot, scale);
					}
					else
					{
						Gizmos.DrawMesh(this.gizmoMeshes[i].sharedMesh, pos, rot, scale);
					}
					//

				}

			}
			else
			{

				// As a fallback just display a yellow gizmo sphere
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(transform.position, 1);

			}


		}
#endregion
	}
}

