using UnityEngine;

namespace Player_System.Extensions
{
    internal static class TryGetComponentExtensions
    {
        internal static bool TryGetComponentInParent<T>(this Component obj, out T component) where T : Object =>
            obj.gameObject.TryGetComponentInParent(out component);
        
        internal static bool TryGetComponentInParent<T>(this GameObject obj, out T component) where T : Object {
            component = obj.GetComponentInParent<T>();

            return component != null;
        }
    }
}