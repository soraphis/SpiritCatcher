using UnityEngine;

namespace Assets.Soraphis.Lib {
    public static class RectExtensions {

        /// <summary>
        /// Splits a Rect in vertical ordered, even, pieces and returns the piece at the given index
        /// </summary>
        public static Rect SplitRectV(this Rect rect, int many, int start, int length = 1) {
            var height = rect.height/many;
            return new Rect(rect.x, rect.y + height*start, rect.width, height * length);
        }

        /// <summary>
        /// Splits a Rect in horizontal ordered, even, pieces and returns the piece at the given index
        /// </summary>
        public static Rect SplitRectH(this Rect rect, int many, int start, int length = 1) {
            var width = rect.width/many;
            return new Rect(rect.x + width*start, rect.y, width * length, rect.height);
        }
    }
}