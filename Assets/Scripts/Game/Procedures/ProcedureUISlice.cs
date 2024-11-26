using Engine;
using UnityEngine;

internal class ProcedureUISlice : MonoBehaviour
{
    [SerializeField] private SpriteRenderer progressBar;
    [SerializeField] private SpriteRenderer iconRenderer;
    private ProcedureBase procedure;
    private Material material;

    public void Init(ProcedureBase procedure)
    {
        this.procedure = procedure;
        if (procedure.Input != null)
        {
            iconRenderer.sprite = procedure.Input.GetIcon();
            iconRenderer.gameObject.SetActive(true);
        }

        material = progressBar.CreateMaterialInstance();
        procedure.OnProcedureProgressionChangedEvent.AddListener(OnProgressionChanged);
    }

    private void OnDestroy()
    {
        if (procedure != null)
            procedure.OnProcedureProgressionChangedEvent.RemoveListener(OnProgressionChanged);
    }

    private void OnProgressionChanged(float progression)
    {
        material.SetFloat("_progress", progression);
    }
}
