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


		public static void DrawCube(Vector2 center, Vector2 size, Color color, float LineWidth = 1f, Alignment align = Alignment.INSIDE) {
            Camera camera = ObjectFinder.mainCam;
            if (!camera) {
                return;
            }

            LineWidth *= (0.01f * (camera.orthographicSize/3));

            #region Matrix

            lineMaterial.SetPass(0);
            GL.PushMatrix();
            GL.Begin(GL.QUADS);
            GL.Color(color);
            GL.LoadProjectionMatrix(camera.projectionMatrix);

            Vector3 bottomLeft = new Vector2(center.x - size.x / 2, center.y - size.y/2);     // Bottom Left
            Vector3 topLeft = new Vector2(center.x - size.x/2, center.y + size.y/2);        // Top Left
            Vector3 topRight = new Vector2(center.x + size.x/2, center.y + size.y/2);       // Top Right
            Vector3 bottomRight = new Vector2(center.x + size.x/2, center.y - size.y/2);    // Bottom Righ

            Vector3 offsetX = new Vector3(LineWidth, 0, 0);
            Vector3 offsetY = new Vector3(0, LineWidth * (byte)align, 0);
            Vector3 offsetXY = new Vector3(LineWidth, LineWidth, 0);
            Vector3 offsetXY2 = new Vector3(LineWidth, -LineWidth, 0);

            GL.Vertex(bottomLeft - offsetY);
            GL.Vertex(topLeft + offsetY);
            if (align == Alignment.INSIDE) {
                GL.Vertex(topLeft + offsetX);
                GL.Vertex(bottomLeft + offsetX);
            } else {
                GL.Vertex(topLeft - offsetXY2);
                GL.Vertex(bottomLeft - offsetXY);
            }

            GL.Vertex(topLeft);
            GL.Vertex(topRight);
            if (align == Alignment.INSIDE) {
                GL.Vertex3(topRight.x, topRight.y - LineWidth, topRight.z);
                GL.Vertex3(topLeft.x, topLeft.y - LineWidth, topLeft.z);
            } else {
                GL.Vertex3(topRight.x, topRight.y + LineWidth, topRight.z);
                GL.Vertex3(topLeft.x, topLeft.y + LineWidth, topLeft.z);
            }

            GL.Vertex(topRight + offsetY);
            GL.Vertex(bottomRight - offsetY);
            if (align == Alignment.INSIDE) {
                GL.Vertex(bottomRight - offsetX);
                GL.Vertex(topRight - offsetX);
            } else {
                GL.Vertex(bottomRight + offsetXY2);
                GL.Vertex(topRight + offsetXY);
            }

            GL.Vertex(bottomRight);
            GL.Vertex(bottomLeft);
            if (align == Alignment.INSIDE) {
                GL.Vertex3(bottomLeft.x, bottomLeft.y + LineWidth, bottomLeft.z);
                GL.Vertex3(bottomRight.x, bottomRight.y + LineWidth, bottomRight.z);
            } else {
                GL.Vertex3(bottomLeft.x, bottomLeft.y - LineWidth, bottomLeft.z);
                GL.Vertex3(bottomRight.x, bottomRight.y - LineWidth, bottomRight.z);
            }
            

            GL.End();
            GL.PopMatrix();
            #endregion
        }

        public enum Alignment : byte
        {
            INSIDE,
            OUTSIDE,
        }

	}
}
