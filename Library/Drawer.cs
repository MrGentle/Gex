using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Gex.Library;
using UnityEngine.Experimental.Rendering;

namespace Gex.Library {
	public static class Drawer {
        static Material lineMaterial;

        public static void InitializeDrawer() {
            lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", 5);
            lineMaterial.SetInt("_DstBlend", 10);
            lineMaterial.SetInt("_Cull", 0);
            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }


		public static void DrawCube(Vector2 center, Vector2 size, Color color, Alignment align = Alignment.CENTERED) {
            Camera camera = ObjectFinder.mainCam;
            if (!camera) {
                return;
            }

            var lineWidth = 1 * (0.01f * (camera.orthographicSize/3));

            #region Matrix

            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(color);
            GL.LoadProjectionMatrix(camera.projectionMatrix);

            Vector2 bottomLeft = new Vector2(center.x - size.x / 2, center.y - size.y/2);     // Bottom Left
            Vector2 topLeft = new Vector2(center.x - size.x/2, center.y + size.y/2);        // Top Left
            Vector2 topRight = new Vector2(center.x + size.x/2, center.y + size.y/2);       // Top Right
            Vector2 bottomRight = new Vector2(center.x + size.x/2, center.y - size.y/2);    // Bottom Righ

            Vector2 offsetX = new Vector2(lineWidth/2, 0);
            Vector2 offsetY = new Vector2(0, lineWidth/2);

            switch (align) {
                case Alignment.CENTERED:
                    GL.Vertex(bottomLeft - offsetX);
                    GL.Vertex(topLeft - offsetX);
                    GL.Vertex(topLeft + offsetX);
                    GL.Vertex(bottomLeft + offsetX);
            
                    GL.Vertex(topLeft - offsetY);
                    GL.Vertex(topRight - offsetY);
                    GL.Vertex(topRight + offsetY);
                    GL.Vertex(topLeft + offsetY);

                    GL.Vertex(topRight + offsetX);
                    GL.Vertex(bottomRight + offsetX);
                    GL.Vertex(bottomRight - offsetX);
                    GL.Vertex(topRight - offsetX);

                    GL.Vertex(bottomRight + offsetY);
                    GL.Vertex(bottomLeft + offsetY);
                    GL.Vertex(bottomLeft - offsetY);
                    GL.Vertex(bottomRight - offsetY);
                    break;
                case Alignment.INSIDE:
                    GL.Vertex(bottomLeft);
                    GL.Vertex(topLeft);
                    GL.Vertex(topLeft + offsetX*2);
                    GL.Vertex(bottomLeft + offsetX*2);
            
                    GL.Vertex(topLeft - offsetY*2);
                    GL.Vertex(topRight - offsetY*2);
                    GL.Vertex(topRight);
                    GL.Vertex(topLeft);

                    GL.Vertex(topRight);
                    GL.Vertex(bottomRight);
                    GL.Vertex(bottomRight - offsetX*2);
                    GL.Vertex(topRight - offsetX*2);

                    GL.Vertex(bottomRight + offsetY*2);
                    GL.Vertex(bottomLeft + offsetY*2);
                    GL.Vertex(bottomLeft);
                    GL.Vertex(bottomRight);
                    break;
            }
            


            GL.End();
            GL.PopMatrix();
            #endregion
        }

        public enum Alignment : byte
        {
            INSIDE,
            CENTERED,
        }

	}
}
