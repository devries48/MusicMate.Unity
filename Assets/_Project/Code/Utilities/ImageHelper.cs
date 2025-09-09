using UnityEngine;
using UnityEngine.UI;

public static class ImageHelper
{
    public static Sprite LoadSpriteFromBytes(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            return null;

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (texture.LoadImage(imageData))
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f), // pivot in center
                100f // pixels per unit, tweak if needed
            );
        
        Debug.LogWarning("Invalid image data, cannot create texture.");
        return null;
    }
    
    public static void SetImageAlpha(Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
