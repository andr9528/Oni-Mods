﻿/*
 * Copyright 2023 Peter Han
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
 * BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using HarmonyLib;
using PeterHan.PLib.Core;
using System;
#if DEBUG
using System.Collections.Generic;
using System.Reflection;
#endif
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ModProfileManager_Addon.UnityUI.FastTrack_VirtualScroll
{
    /// <summary>
    /// Extension methods make life easier!
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// The shared stopwatch used to avoid allocations when timing handles.
        /// </summary>
        private static readonly Stopwatch WAIT_HANDLE_CLOCK = new Stopwatch();

        /// <summary>
        /// Appends two string builders with no intermediate ToString allocation.
        /// </summary>
        /// <param name="dest">The destination string.</param>
        /// <param name="src">The source string.</param>
        /// <returns>The modified destination string with src appended.</returns>
        public static StringBuilder Append(this StringBuilder dest, StringBuilder src)
        {
            int n = src.Length;
            dest.EnsureCapacity(dest.Length + n);
            for (int i = 0; i < n; i++)
                dest.Append(src[i]);
            return dest;
        }

        /// <summary>
        /// Copies layout information to a fixed layout element. Useful for freezing a UI
        /// object.
        /// </summary>
        /// <param name="dest">The fixed layout component that will replace it.</param>
        /// <param name="src">The current layout component.</param>
        public static void CopyFrom(this LayoutElement dest, ILayoutElement src)
        {
            dest.flexibleHeight = src.flexibleHeight;
            dest.flexibleWidth = src.flexibleWidth;
            dest.preferredHeight = src.preferredHeight;
            dest.preferredWidth = src.preferredWidth;
            dest.minHeight = src.minHeight;
            dest.minWidth = src.minWidth;
        }

        /// <summary>
        /// Creates a GameObject to render meshes using a MeshRenderer.
        /// </summary>
        /// <param name="targetMesh">The mesh to be rendered.</param>
        /// <param name="name">The object's name.</param>
        /// <param name="layer">The layer on which the mesh will be rendered.</param>
        /// <param name="shader">The material to use, or null to leave unassigned.</param>
        /// <returns>The game object to use for rendering.</returns>
        public static GameObject CreateMeshRenderer(this Mesh targetMesh, string name,
                int layer, Material shader = null)
        {
            if (targetMesh == null)
                throw new ArgumentNullException(nameof(targetMesh));
            var go = new GameObject(name ?? "Mesh Renderer", typeof(MeshRenderer), typeof(
                    MeshFilter))
            {
                layer = layer
            };
            // Set up the mesh with the right material
            if (go.TryGetComponent(out MeshRenderer renderer))
            {
                renderer.allowOcclusionWhenDynamic = false;
                renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                renderer.receiveShadows = false;
                renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                if (shader != null)
                    renderer.material = shader;
            }
            // Set the mesh to render
            if (go.TryGetComponent(out MeshFilter filter))
                filter.sharedMesh = targetMesh;
            return go;
        }

        /// <summary>
        /// A faster version of string.Format with one string argument.
        /// </summary>
        /// <param name="str">The LocString to format.</param>
        /// <param name="value">The value to substitute for "{0}".</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this LocString str, string value)
        {
            return str.text.Replace("{0}", value);
        }

        /// <summary>
        /// A faster version of string.Format with one string argument.
        /// </summary>
        /// <param name="str">The StringEntry to format.</param>
        /// <param name="value">The value to substitute for "{0}".</param>
        /// <returns>The formatted string.</returns>
        public static string Format(this StringEntry str, string value)
        {
            return str.String.Replace("{0}", value);
        }


    }
}
