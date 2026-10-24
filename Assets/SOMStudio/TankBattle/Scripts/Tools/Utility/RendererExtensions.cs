using UnityEngine;

namespace SOMStudio.TankBattle.Scripts.Tools.Utility
{
	public static class RendererExtensions
	{
		public static bool IsVisibleFrom(this Renderer renderer, UnityEngine.Camera camera)
		{
			if (camera == null)
				return true;

			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
		}
	}
}