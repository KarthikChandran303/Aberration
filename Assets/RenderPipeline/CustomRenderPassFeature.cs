using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings {
        [Header("Draw Renderers Settings")]
        public LayerMask layerMask = 1;
        public Material overrideMaterial;
        public int overrideMaterialPass;
        public string colorTargetDestinationID = "";

        [Header("Blit Settings")]
        public Material blitMaterial;
    }
    
    // exposed values
    public Settings settings;
    public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;
    
    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(settings, name);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = _event;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera every frame.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
    
    class CustomRenderPass : ScriptableRenderPass
    {
        private Settings _settings;
        private ProfilingSampler _profilingSampler;

        public CustomRenderPass(Settings settings, string name)
        {
            this._settings = settings;
            this._profilingSampler = new ProfilingSampler(name);
        }
        
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }
}




