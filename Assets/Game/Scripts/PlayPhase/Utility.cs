using UnityEngine;

namespace Game.PlayPhase
{
    public static class Utility
    {
        /// <summary>
        /// Returns a random element of an array
        /// </summary>
        public static T GetOneOf<T>(ref T[] variations)
        {
            return variations[(variations.Length > 1 ? Random.Range(0, variations.Length) : 0)];
        }

        /// <summary>
        /// Returns a random cardinal direction (forward, right, back, or left).
        /// </summary>
        public static Quaternion GetRandomDirection()
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    return Quaternion.LookRotation(Vector3.forward);
                case 1:
                    return Quaternion.LookRotation(Vector3.right);
                case 2:
                    return Quaternion.LookRotation(Vector3.back);
                default:
                    return Quaternion.LookRotation(Vector3.left);
            }
        }

        /// <summary>
        /// Converts the given grid coordinates to Vector3 world coordinates.
        /// </summary>
        public static Vector3 TranslateGridPosition(Vector2 gridPos)
        {
            int tileSize = 20;
            return new Vector3(gridPos.x * tileSize, 0f, gridPos.y * tileSize);
        }
    }
}