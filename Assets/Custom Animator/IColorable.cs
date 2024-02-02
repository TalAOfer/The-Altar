using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IColorable
{
    Color color { get; set; }
}

public class SpriteRendererWrapper : IColorable
{
    private SpriteRenderer _spriteRenderer;

    public SpriteRendererWrapper(SpriteRenderer spriteRenderer)
    {
        _spriteRenderer = spriteRenderer;
    }

    public Color color
    {
        get => _spriteRenderer.color;
        set => _spriteRenderer.color = value;
    }
}

public class ImageWrapper : IColorable
{
    private Image _image;

    public ImageWrapper(Image image)
    {
        _image = image;
    }

    public Color color
    {
        get => _image.color;
        set => _image.color = value;
    }
}

public class TextMeshProWrapper : IColorable
{
    private TextMeshProUGUI _textMeshPro;

    public TextMeshProWrapper(TextMeshProUGUI textMeshPro)
    {
        _textMeshPro = textMeshPro;
    }

    public Color color
    {
        get => _textMeshPro.color;
        set => _textMeshPro.color = value;
    }
}