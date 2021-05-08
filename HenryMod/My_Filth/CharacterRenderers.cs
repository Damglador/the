﻿using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

namespace FacelessJoe {

    public class CharacterRenderers : MonoBehaviour {

        [SerializeField]
        private SkinnedMeshRenderer mainSkinnedMeshRenderer;
        public SkinnedMeshRenderer MainSkinnedMeshRenderer { get => mainSkinnedMeshRenderer; }

        [SerializeField]
        private List<Renderer> renderers;
        public List<Renderer> Renderers { get => renderers; }

        [SerializeField]
        private List<Material> materials;
        public List<Material> Materials { get => materials; }

        public void setHoopooShaders() {

            Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/hgstandard");

            for (int i = 0; i < renderers.Count; i++) {
                renderers[i].material.shader = hotpoo;
            }
        }
    }
}