// Create script: UIVertexGradient.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class UIVertexGradient : BaseMeshEffect
{
    [SerializeField] private Color topColor = new Color(0.1f, 0.12f, 0.14f, 1f);
    [SerializeField] private Color bottomColor = new Color(0.04f, 0.055f, 0.07f, 1f);
    [SerializeField] private bool useGraphicAlpha = true;
    
    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive())
            return;

        int count = vh.currentVertCount;
        if (count == 0)
            return;

        // Get vertices
        UIVertex vertex = new UIVertex();
        float bottomY = float.MaxValue;
        float topY = float.MinValue;

        // Find top and bottom Y positions
        for (int i = 0; i < count; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            if (vertex.position.y > topY)
                topY = vertex.position.y;
            if (vertex.position.y < bottomY)
                bottomY = vertex.position.y;
        }

        float height = topY - bottomY;
        
        if (height == 0f)
            return;

        // Apply gradient colors
        for (int i = 0; i < count; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            
            float normalizedY = (vertex.position.y - bottomY) / height;
            Color color = Color.Lerp(bottomColor, topColor, normalizedY);
            
            if (useGraphicAlpha)
                color.a = vertex.color.a;
            
            vertex.color = color;
            vh.SetUIVertex(vertex, i);
        }
    }
}